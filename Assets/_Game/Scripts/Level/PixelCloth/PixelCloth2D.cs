using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Sprites;

namespace Sling.Level.PixelCloth
{
  [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
  public sealed class PixelCloth2D : MonoBehaviour
  {
    private static readonly int _sMainTextureId = Shader.PropertyToID("_MainTex");
    private const float _kMinDistanceSqr = 0.000001f;

    [Header("Grid")]
    [Tooltip("Horizontal point count. More columns = finer left-right folds and stretch, higher CPU.")]
    [FormerlySerializedAs("_columns")]
    [Range(2, 24)] public int Columns = 5;
    [Tooltip("Vertical point count. Row 0 is pinned to the anchor; more rows = longer, smoother drape, higher CPU.")]
    [FormerlySerializedAs("_rows")]
    [Range(2, 48)] public int Rows = 13;
    [Tooltip("Rest size in world units (X = width, Y = hang length downward from the anchor). Mismatch with the sprite aspect stretches the texture.")]
    [FormerlySerializedAs("_size")]
    public Vector2 Size = new(4f, 12f);

    [Header("Simulation")]
    [Tooltip("How many times per second the cloth pose updates. Lower values look like stop-motion pixel animation. Mesh is not interpolated between ticks.")]
    [FormerlySerializedAs("_simulationFps")]
    [Range(1, 60)] public int SimulationFps = 12;
    [Tooltip("How much Verlet velocity is kept each simulation tick. 1 = no energy loss (jittery); lower = cloth settles faster. Lower this further if simulation FPS is low.")]
    [FormerlySerializedAs("_damping")]
    [Range(0f, 1f)] public float Damping = 0.8f;
    [Tooltip("How strongly fibers hold their rest length. 1 = taut (less stretch under gravity); 0 = limp. If it still stretches at 1, raise Constraint Iterations.")]
    [FormerlySerializedAs("_fiberStrength")]
    [Range(0f, 1f)] public float FiberStrength = 1f;
    [Tooltip("Distance-constraint solver passes per simulation tick. Higher = less stretch under gravity, stiffer cloth, more CPU.")]
    [FormerlySerializedAs("_constraintIterations")]
    [Range(1, 16)] public int ConstraintIterations = 6;
    [Tooltip("If the anchor jumps farther than this (world units) in one simulation tick, the cloth teleports/resets instead of stretching across the gap. 0 disables the check.")]
    [FormerlySerializedAs("_teleportThreshold")]
    [Range(0f, 40f)] public float TeleportThreshold = 12f;

    [Header("Rendering")]
    [Tooltip("Snap simulated points to a pixel grid after each tick so the cape jumps pixel-to-pixel.")]
    [FormerlySerializedAs("_pixelSnapEnabled")]
    public bool PixelSnapEnabled = true;
    [Tooltip("Pixel grid used for snapping: world positions round to 1 / this. Match the sprite Pixels Per Unit so texels stay stable.")]
    [FormerlySerializedAs("_pixelsPerUnit")]
    [Range(0.25f, 32f)] public float PixelsPerUnit = 4f;
    [Tooltip("2D sorting layer for the MeshRenderer. Must be a layer the camera and Light2D include, or the cloth is culled/unlit.")]
    [FormerlySerializedAs("_sortingLayerId")]
    [SortingLayer] public int SortingLayerId;
    [Tooltip("Draw order inside the sorting layer. Higher draws in front of other renderers on the same layer.")]
    [FormerlySerializedAs("_orderInLayer")]
    [Range(-100, 100)] public int OrderInLayer;

    [Header("Strategies")]
    [Tooltip("How the pinned top row follows the anchor. Translation Only moves the top edge with the anchor and ignores rotation.")]
    [FormerlySerializedAs("_anchorMotionStrategy")]
    [SerializeReference, SubclassSelector]
    public PixelClothAnchorMotionStrategy AnchorMotionStrategy = new TranslationOnlyAnchorMotionStrategy();
    [Tooltip("Extra accelerations on free points (not the pinned row). Add Gravity Mass or the cloth will not sag — it only keeps inertia and constraints.")]
    [FormerlySerializedAs("_forceModifiers")]
    [SerializeReference] public List<PixelClothForceModifier> ForceModifiers = new();

    [Header("References")]
    [Tooltip("Transform the top row follows. Leave empty to use this object's transform.")]
    [FormerlySerializedAs("_anchor")]
    public Transform Anchor;
    [Tooltip("Sprite sampled onto the cloth mesh. UVs map the sprite's outer rect across the grid (sprite top = pinned row).")]
    [FormerlySerializedAs("_sprite")]
    [Required] public Sprite Sprite;
    [Tooltip("Receives the runtime cloth mesh. Must be on this GameObject.")]
    [FormerlySerializedAs("_meshFilter")]
    [Required] public MeshFilter MeshFilter;
    [Tooltip("Draws the cloth mesh. Sorting and the sprite texture are applied here at runtime.")]
    [FormerlySerializedAs("_meshRenderer")]
    [Required] public MeshRenderer MeshRenderer;

    private readonly List<RuntimeModifierRegistration> _runtimeModifiers = new();

    private Vector2[] _positions;
    private Vector2[] _previousPositions;
    private Vector2[] _restPinnedWorldPositions;
    private Vector2[] _pinnedWorldPositions;
    private Vector3[] _renderVertices;
    private Vector2[] _uvs;
    private Constraint[] _constraints;
    private int[] _triangles;

    private MaterialPropertyBlock _propertyBlock;
    private Mesh _mesh;
    private Quaternion _initialAnchorRotation;
    private Vector2 _initialAnchorPosition;
    private Vector2 _previousAnchorPosition;
    private float _simulationAccumulator;
    private int _builtColumns;
    private int _builtRows;
    private Vector2 _builtSize;
    private bool _isInitialized;
    private bool _requiresReset = true;

    public IDisposable RegisterForceModifier(IPixelClothForceModifier modifier)
    {
      if (modifier == null)
        throw new ArgumentNullException(nameof(modifier));

      var registration = new RuntimeModifierRegistration(this, modifier);
      _runtimeModifiers.Add(registration);
      return registration;
    }

    private void OnEnable()
    {
      if (!ValidateConfiguration())
      {
        enabled = false;
        return;
      }

      Initialize();
    }

    private void OnValidate()
    {
      ApplySorting();

      if (!Application.isPlaying || !_isInitialized)
        return;

      ApplySpriteTexture();

      if (Columns != _builtColumns || Rows != _builtRows || Size != _builtSize)
        Initialize();
    }

    private void Update()
    {
      if (!_isInitialized)
        return;

      Vector2 currentAnchorPosition = GetAnchorPosition();
      float teleportThreshold = TeleportThreshold;

      if (_requiresReset || teleportThreshold > 0f &&
          (currentAnchorPosition - _previousAnchorPosition).sqrMagnitude > teleportThreshold * teleportThreshold)
      {
        ResetSimulation(currentAnchorPosition);
        SnapSimulatedPositions();
        _simulationAccumulator = 0f;
        return;
      }

      float step = 1f / SimulationFps;
      _simulationAccumulator += Time.deltaTime;

      if (_simulationAccumulator < step)
        return;

      StepSimulation(currentAnchorPosition, step);
      _simulationAccumulator -= step;

      if (_simulationAccumulator >= step)
        _simulationAccumulator %= step;
    }

    private void LateUpdate()
    {
      if (!_isInitialized)
        return;

      UploadMesh();
    }

    private void OnDisable() =>
      _requiresReset = true;

    private void OnDestroy()
    {
      for (int i = _runtimeModifiers.Count - 1; i >= 0; i--)
        _runtimeModifiers[i].Detach();

      _runtimeModifiers.Clear();

      if (_mesh == null)
        return;

      if (MeshFilter != null && MeshFilter.sharedMesh == _mesh)
        MeshFilter.sharedMesh = null;

      Destroy(_mesh);
      _mesh = null;
    }

    private bool ValidateConfiguration()
    {
      if (Sprite == null || MeshFilter == null || MeshRenderer == null)
      {
        Debug.LogError($"{nameof(PixelCloth2D)} on '{name}' is missing a required reference.", this);
        return false;
      }

      if (Columns < 2 || Rows < 2 || Size.x <= 0f || Size.y <= 0f ||
          SimulationFps < 1 || ConstraintIterations < 1 ||
          PixelSnapEnabled && PixelsPerUnit <= 0f || AnchorMotionStrategy == null ||
          ForceModifiers == null)
      {
        Debug.LogError($"{nameof(PixelCloth2D)} on '{name}' has invalid settings.", this);
        return false;
      }

      return true;
    }

    private void Initialize()
    {
      if (Anchor == null)
        Anchor = transform;

      EnsureMesh();
      BuildBuffers();
      BuildMeshTopology();

      _builtColumns = Columns;
      _builtRows = Rows;
      _builtSize = Size;
      _initialAnchorRotation = Anchor.rotation;
      ResetSimulation(GetAnchorPosition());
      SnapSimulatedPositions();
      _isInitialized = true;
      _simulationAccumulator = 0f;
      UploadMesh();
    }

    private void EnsureMesh()
    {
      if (_mesh == null)
      {
        _mesh = new Mesh { name = $"{name} Pixel Cloth" };
        _mesh.MarkDynamic();
      }
      else
      {
        _mesh.Clear();
      }

      MeshFilter.sharedMesh = _mesh;
      ApplySorting();
      ApplySpriteTexture();
    }

    private void ApplySpriteTexture()
    {
      if (MeshRenderer == null || Sprite == null)
        return;

      _propertyBlock ??= new MaterialPropertyBlock();
      MeshRenderer.GetPropertyBlock(_propertyBlock);
      _propertyBlock.SetTexture(_sMainTextureId, Sprite.texture);
      MeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void ApplySorting()
    {
      if (MeshRenderer == null)
        return;

      MeshRenderer.sortingLayerID = SortingLayerId;
      MeshRenderer.sortingOrder = OrderInLayer;
    }

    private void BuildBuffers()
    {
      int columns = Columns;
      int rows = Rows;
      int pointCount = columns * rows;
      int constraintCount = rows * (columns - 1) +
                            (rows - 1) * columns +
                            (rows - 1) * (columns - 1);

      _positions = new Vector2[pointCount];
      _previousPositions = new Vector2[pointCount];
      _restPinnedWorldPositions = new Vector2[columns];
      _pinnedWorldPositions = new Vector2[columns];
      _renderVertices = new Vector3[pointCount];
      _uvs = new Vector2[pointCount];
      _constraints = new Constraint[constraintCount];
      _triangles = new int[(columns - 1) * (rows - 1) * 6];
    }

    private void BuildMeshTopology()
    {
      int columns = Columns;
      int rows = Rows;
      Vector4 outerUv = DataUtility.GetOuterUV(Sprite);

      for (int row = 0; row < rows; row++)
      {
        float row01 = row / (float)(rows - 1);

        for (int column = 0; column < columns; column++)
        {
          float column01 = column / (float)(columns - 1);
          int index = GetPointIndex(column, row);
          _uvs[index] = new Vector2(
            Mathf.Lerp(outerUv.x, outerUv.z, column01),
            Mathf.Lerp(outerUv.w, outerUv.y, row01));
        }
      }

      int triangleIndex = 0;

      for (int row = 0; row < rows - 1; row++)
      {
        for (int column = 0; column < columns - 1; column++)
        {
          int topLeft = GetPointIndex(column, row);
          int topRight = GetPointIndex(column + 1, row);
          int bottomLeft = GetPointIndex(column, row + 1);
          int bottomRight = GetPointIndex(column + 1, row + 1);

          // 2D camera looks along +Z; this winding gives -Z normals so Cull Back keeps the cloth visible.
          _triangles[triangleIndex++] = topLeft;
          _triangles[triangleIndex++] = topRight;
          _triangles[triangleIndex++] = bottomLeft;
          _triangles[triangleIndex++] = topRight;
          _triangles[triangleIndex++] = bottomRight;
          _triangles[triangleIndex++] = bottomLeft;
        }
      }

      BuildConstraints();
      _mesh.SetVertices(_renderVertices);
      _mesh.SetUVs(0, _uvs);
      _mesh.SetTriangles(_triangles, 0, false);

      Vector3[] normals = new Vector3[_uvs.Length];
      for (int i = 0; i < normals.Length; i++)
        normals[i] = Vector3.back;

      _mesh.SetNormals(normals);
    }

    private void BuildConstraints()
    {
      int columns = Columns;
      int rows = Rows;
      int constraintIndex = 0;

      for (int row = 0; row < rows; row++)
      {
        for (int column = 0; column < columns - 1; column++)
          AddConstraint(ref constraintIndex, column, row, column + 1, row);
      }

      for (int row = 0; row < rows - 1; row++)
      {
        for (int column = 0; column < columns; column++)
          AddConstraint(ref constraintIndex, column, row, column, row + 1);
      }

      for (int row = 0; row < rows - 1; row++)
      {
        for (int column = 0; column < columns - 1; column++)
          AddConstraint(ref constraintIndex, column, row, column + 1, row + 1);
      }
    }

    private void AddConstraint(ref int constraintIndex, int columnA, int rowA, int columnB, int rowB)
    {
      float horizontalStep = Size.x / (Columns - 1);
      float verticalStep = Size.y / (Rows - 1);
      float deltaX = (columnB - columnA) * horizontalStep;
      float deltaY = (rowB - rowA) * verticalStep;

      _constraints[constraintIndex++] = new Constraint(
        GetPointIndex(columnA, rowA),
        GetPointIndex(columnB, rowB),
        Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY));
    }

    private void ResetSimulation(Vector2 anchorPosition)
    {
      int columns = Columns;
      int rows = Rows;
      _initialAnchorPosition = anchorPosition;
      _previousAnchorPosition = anchorPosition;

      for (int row = 0; row < rows; row++)
      {
        float row01 = row / (float)(rows - 1);

        for (int column = 0; column < columns; column++)
        {
          float column01 = column / (float)(columns - 1);
          Vector2 localOffset = new(
            Mathf.Lerp(-Size.x * 0.5f, Size.x * 0.5f, column01),
            -Size.y * row01);
          Vector2 worldOffset = _initialAnchorRotation * (Vector3)localOffset;
          int index = GetPointIndex(column, row);
          Vector2 worldPosition = anchorPosition + worldOffset;

          _positions[index] = worldPosition;
          _previousPositions[index] = worldPosition;

          if (row == 0)
          {
            _restPinnedWorldPositions[column] = worldPosition;
            _pinnedWorldPositions[column] = worldPosition;
          }
        }
      }

      _requiresReset = false;
    }

    private void StepSimulation(Vector2 currentAnchorPosition, float deltaTime)
    {
      AnchorMotionStrategy.FillPinnedPositions(
        _initialAnchorPosition,
        currentAnchorPosition,
        _restPinnedWorldPositions,
        _pinnedWorldPositions);

      ApplyPinnedPositions();
      SimulateFreePoints(deltaTime);

      for (int i = 0; i < ConstraintIterations; i++)
      {
        SolveConstraints();
        ApplyPinnedPositions();
      }

      SnapSimulatedPositions();
      _previousAnchorPosition = currentAnchorPosition;
    }

    private void SimulateFreePoints(float deltaTime)
    {
      float damping = Damping;
      float deltaTimeSqr = deltaTime * deltaTime;
      int columns = Columns;
      int rows = Rows;
      IReadOnlyList<PixelClothForceModifier> localModifiers = ForceModifiers;
      bool hasForces = _runtimeModifiers.Count > 0;

      if (!hasForces)
      {
        for (int i = 0; i < localModifiers.Count; i++)
        {
          if (localModifiers[i] != null)
          {
            hasForces = true;
            break;
          }
        }
      }

      for (int row = 1; row < rows; row++)
      {
        float row01 = hasForces ? row / (float)(rows - 1) : 0f;

        for (int column = 0; column < columns; column++)
        {
          int index = GetPointIndex(column, row);
          Vector2 position = _positions[index];
          Vector2 velocity = position - _previousPositions[index];
          Vector2 acceleration = Vector2.zero;

          if (hasForces)
          {
            Vector2 worldVelocity = velocity / deltaTime;
            var context = new PixelClothForceContext(
              index,
              new Vector2Int(column, row),
              new Vector2(column / (float)(columns - 1), row01),
              position,
              worldVelocity,
              Time.time,
              deltaTime);

            for (int i = 0; i < localModifiers.Count; i++)
            {
              PixelClothForceModifier modifier = localModifiers[i];

              if (modifier != null)
                acceleration += modifier.GetAcceleration(in context);
            }

            for (int i = 0; i < _runtimeModifiers.Count; i++)
              acceleration += _runtimeModifiers[i].Modifier.GetAcceleration(in context);
          }

          _previousPositions[index] = position;
          _positions[index] = position + velocity * damping + acceleration * deltaTimeSqr;
        }
      }
    }

    private void SolveConstraints()
    {
      int pinnedCount = Columns;

      for (int i = 0; i < _constraints.Length; i++)
      {
        Constraint constraint = _constraints[i];
        Vector2 delta = _positions[constraint.IndexB] - _positions[constraint.IndexA];
        float distanceSqr = delta.sqrMagnitude;

        if (distanceSqr <= _kMinDistanceSqr)
          continue;

        float distance = Mathf.Sqrt(distanceSqr);
        Vector2 correction = delta * ((distance - constraint.RestDistance) / distance * FiberStrength);
        bool aPinned = constraint.IndexA < pinnedCount;
        bool bPinned = constraint.IndexB < pinnedCount;

        if (aPinned && bPinned)
          continue;

        if (aPinned)
        {
          _positions[constraint.IndexB] -= correction;
        }
        else if (bPinned)
        {
          _positions[constraint.IndexA] += correction;
        }
        else
        {
          Vector2 halfCorrection = correction * 0.5f;
          _positions[constraint.IndexA] += halfCorrection;
          _positions[constraint.IndexB] -= halfCorrection;
        }
      }
    }

    private void ApplyPinnedPositions()
    {
      for (int i = 0; i < _pinnedWorldPositions.Length; i++)
      {
        _positions[i] = _pinnedWorldPositions[i];
        _previousPositions[i] = _pinnedWorldPositions[i];
      }
    }

    private void SnapSimulatedPositions()
    {
      if (!PixelSnapEnabled)
        return;

      float pixelsPerUnit = PixelsPerUnit;

      for (int i = 0; i < _positions.Length; i++)
      {
        Vector2 position = _positions[i];
        Vector2 velocity = position - _previousPositions[i];
        Vector2 snapped = new(
          Mathf.Round(position.x * pixelsPerUnit) / pixelsPerUnit,
          Mathf.Round(position.y * pixelsPerUnit) / pixelsPerUnit);
        _positions[i] = snapped;
        _previousPositions[i] = snapped - velocity;
      }
    }

    private void UploadMesh()
    {
      Bounds bounds = new(Vector3.zero, Vector3.zero);

      for (int i = 0; i < _positions.Length; i++)
      {
        Vector3 localPosition = MeshFilter.transform.InverseTransformPoint(_positions[i]);
        _renderVertices[i] = localPosition;

        if (i == 0)
          bounds = new Bounds(localPosition, Vector3.zero);
        else
          bounds.Encapsulate(localPosition);
      }

      _mesh.SetVertices(
        _renderVertices,
        0,
        _renderVertices.Length,
        MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);
      _mesh.bounds = bounds;
    }

    private int GetPointIndex(int column, int row) =>
      row * Columns + column;

    private Vector2 GetAnchorPosition() =>
      Anchor.position;

    private void Unregister(RuntimeModifierRegistration registration) =>
      _runtimeModifiers.Remove(registration);

    private readonly struct Constraint
    {
      public Constraint(int indexA, int indexB, float restDistance)
      {
        IndexA = indexA;
        IndexB = indexB;
        RestDistance = restDistance;
      }

      public int IndexA { get; }
      public int IndexB { get; }
      public float RestDistance { get; }
    }

    private sealed class RuntimeModifierRegistration : IDisposable
    {
      private PixelCloth2D _owner;

      public RuntimeModifierRegistration(PixelCloth2D owner, IPixelClothForceModifier modifier)
      {
        _owner = owner;
        Modifier = modifier;
      }

      public IPixelClothForceModifier Modifier { get; }

      public void Dispose()
      {
        if (_owner == null)
          return;

        PixelCloth2D owner = _owner;
        _owner = null;
        owner.Unregister(this);
      }

      public void Detach() =>
        _owner = null;
    }
  }
}
