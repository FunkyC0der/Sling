using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
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
    [Min(2)] public int _columns = 5;
    [Tooltip("Vertical point count. Row 0 is pinned to the anchor; more rows = longer, smoother drape, higher CPU.")]
    [Min(2)] public int _rows = 13;
    [Tooltip("Rest size in world units (X = width, Y = hang length downward from the anchor). Mismatch with the sprite aspect stretches the texture.")]
    public Vector2 _size = new(4f, 12f);

    [Header("Simulation")]
    [Tooltip("How much Verlet velocity is kept each physics step. 1 = no energy loss (jittery); lower = cloth settles faster.")]
    [Range(0f, 1f)] public float _damping = 0.96f;
    [Tooltip("Fixed timestep this damping value is calibrated for. Damping is scaled as Pow(damping, deltaTime / this), so feel stays similar if the project timestep changes.")]
    [Min(0.0001f)] public float _referenceTimeStep = 0.02f;
    [Tooltip("Distance-constraint solver passes per FixedUpdate. Higher = less stretch, stiffer cloth, more CPU.")]
    [Min(1)] public int _constraintIterations = 4;
    [Tooltip("If the anchor jumps farther than this (world units) in one physics step, the cloth teleports/resets instead of stretching across the gap. 0 disables the check.")]
    [Min(0f)] public float _teleportThreshold = 12f;

    [Header("Rendering")]
    [Tooltip("Snap rendered vertices to a pixel grid. Physics still runs in continuous world space; this only affects how the mesh looks.")]
    public bool _pixelSnapEnabled = true;
    [Tooltip("Pixel grid used for snapping: world positions round to 1 / this. Match the sprite Pixels Per Unit so texels stay stable.")]
    [Min(0.0001f)] public float _pixelsPerUnit = 4f;
    [Tooltip("2D sorting layer for the MeshRenderer. Must be a layer the camera and Light2D include, or the cloth is culled/unlit.")]
    [SortingLayer] public int _sortingLayerId;
    [Tooltip("Draw order inside the sorting layer. Higher draws in front of other renderers on the same layer.")]
    public int _orderInLayer;

    [Header("Strategies")]
    [Tooltip("How the pinned top row follows the anchor. Translation Only moves the top edge with the anchor and ignores rotation.")]
    [SerializeReference, SubclassSelector]
    public PixelClothAnchorMotionStrategy _anchorMotionStrategy = new TranslationOnlyAnchorMotionStrategy();
    [Tooltip("Extra accelerations on free points (not the pinned row). Add Unity Gravity or the cloth will not sag — it only keeps inertia and constraints.")]
    [SerializeReference, SubclassSelector] public List<PixelClothForceModifier> _forceModifiers = new();

    [Header("References")]
    [Tooltip("Transform the top row follows. Leave empty to use this object's transform.")]
    public Transform _anchor;
    [Tooltip("Sprite sampled onto the cloth mesh. UVs map the sprite's outer rect across the grid (sprite top = pinned row).")]
    [Required] public Sprite _sprite;
    [Tooltip("Receives the runtime cloth mesh. Must be on this GameObject.")]
    [Required] public MeshFilter _meshFilter;
    [Tooltip("Draws the cloth mesh. Sorting and the sprite texture are applied here at runtime.")]
    [Required] public MeshRenderer _meshRenderer;

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

    private void OnValidate() =>
      ApplySorting();

    private void FixedUpdate()
    {
      if (!_isInitialized)
        return;

      Vector2 currentAnchorPosition = GetAnchorPosition();
      float teleportThreshold = _teleportThreshold;

      if (_requiresReset || teleportThreshold > 0f &&
          (currentAnchorPosition - _previousAnchorPosition).sqrMagnitude > teleportThreshold * teleportThreshold)
      {
        ResetSimulation(currentAnchorPosition);
        return;
      }

      float deltaTime = Time.fixedDeltaTime;
      _anchorMotionStrategy.FillPinnedPositions(
        _initialAnchorPosition,
        currentAnchorPosition,
        _restPinnedWorldPositions,
        _pinnedWorldPositions);

      ApplyPinnedPositions();
      SimulateFreePoints(deltaTime);

      for (int i = 0; i < _constraintIterations; i++)
      {
        SolveConstraints();
        ApplyPinnedPositions();
      }

      _previousAnchorPosition = currentAnchorPosition;
    }

    private void LateUpdate()
    {
      if (!_isInitialized)
        return;

      float pixelsPerUnit = _pixelsPerUnit;
      bool snap = _pixelSnapEnabled;
      Bounds bounds = new(Vector3.zero, Vector3.zero);

      for (int i = 0; i < _positions.Length; i++)
      {
        Vector2 worldPosition = _positions[i];

        if (snap)
        {
          worldPosition.x = Mathf.Round(worldPosition.x * pixelsPerUnit) / pixelsPerUnit;
          worldPosition.y = Mathf.Round(worldPosition.y * pixelsPerUnit) / pixelsPerUnit;
        }

        Vector3 localPosition = _meshFilter.transform.InverseTransformPoint(worldPosition);
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

    private void OnDisable() =>
      _requiresReset = true;

    private void OnDestroy()
    {
      for (int i = _runtimeModifiers.Count - 1; i >= 0; i--)
        _runtimeModifiers[i].Detach();

      _runtimeModifiers.Clear();

      if (_mesh == null)
        return;

      if (_meshFilter != null && _meshFilter.sharedMesh == _mesh)
        _meshFilter.sharedMesh = null;

      Destroy(_mesh);
      _mesh = null;
    }

    private bool ValidateConfiguration()
    {
      if (_sprite == null || _meshFilter == null || _meshRenderer == null)
      {
        Debug.LogError($"{nameof(PixelCloth2D)} on '{name}' is missing a required reference.", this);
        return false;
      }

      if (_columns < 2 || _rows < 2 || _size.x <= 0f || _size.y <= 0f ||
          _referenceTimeStep <= 0f || _constraintIterations < 1 ||
          _pixelSnapEnabled && _pixelsPerUnit <= 0f || _anchorMotionStrategy == null ||
          _forceModifiers == null)
      {
        Debug.LogError($"{nameof(PixelCloth2D)} on '{name}' has invalid settings.", this);
        return false;
      }

      return true;
    }

    private void Initialize()
    {
      if (_anchor == null)
        _anchor = transform;

      EnsureMesh();
      BuildBuffers();
      BuildMeshTopology();

      _initialAnchorRotation = _anchor.rotation;
      ResetSimulation(GetAnchorPosition());
      _isInitialized = true;
      RenderImmediately();
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

      _meshFilter.sharedMesh = _mesh;
      ApplySorting();

      _propertyBlock ??= new MaterialPropertyBlock();
      _meshRenderer.GetPropertyBlock(_propertyBlock);
      _propertyBlock.SetTexture(_sMainTextureId, _sprite.texture);
      _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void ApplySorting()
    {
      if (_meshRenderer == null)
        return;

      _meshRenderer.sortingLayerID = _sortingLayerId;
      _meshRenderer.sortingOrder = _orderInLayer;
    }

    private void BuildBuffers()
    {
      int columns = _columns;
      int rows = _rows;
      int pointCount = columns * rows;
      int constraintCount = rows * (columns - 1) +
                            (rows - 1) * columns +
                            (rows - 1) * (columns - 1) * 2;

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
      int columns = _columns;
      int rows = _rows;
      Vector4 outerUv = DataUtility.GetOuterUV(_sprite);

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
      int columns = _columns;
      int rows = _rows;
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
        {
          AddConstraint(ref constraintIndex, column, row, column + 1, row + 1);
          AddConstraint(ref constraintIndex, column + 1, row, column, row + 1);
        }
      }
    }

    private void AddConstraint(ref int constraintIndex, int columnA, int rowA, int columnB, int rowB)
    {
      float horizontalStep = _size.x / (_columns - 1);
      float verticalStep = _size.y / (_rows - 1);
      float deltaX = (columnB - columnA) * horizontalStep;
      float deltaY = (rowB - rowA) * verticalStep;

      _constraints[constraintIndex++] = new Constraint(
        GetPointIndex(columnA, rowA),
        GetPointIndex(columnB, rowB),
        Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY));
    }

    private void ResetSimulation(Vector2 anchorPosition)
    {
      int columns = _columns;
      int rows = _rows;
      _initialAnchorPosition = anchorPosition;
      _previousAnchorPosition = anchorPosition;

      for (int row = 0; row < rows; row++)
      {
        float row01 = row / (float)(rows - 1);

        for (int column = 0; column < columns; column++)
        {
          float column01 = column / (float)(columns - 1);
          Vector2 localOffset = new(
            Mathf.Lerp(-_size.x * 0.5f, _size.x * 0.5f, column01),
            -_size.y * row01);
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

    private void SimulateFreePoints(float deltaTime)
    {
      float damping = Mathf.Pow(_damping, deltaTime / _referenceTimeStep);
      float deltaTimeSqr = deltaTime * deltaTime;
      int columns = _columns;
      int rows = _rows;
      IReadOnlyList<PixelClothForceModifier> localModifiers = _forceModifiers;

      for (int row = 1; row < rows; row++)
      {
        float row01 = row / (float)(rows - 1);

        for (int column = 0; column < columns; column++)
        {
          int index = GetPointIndex(column, row);
          Vector2 position = _positions[index];
          Vector2 velocity = position - _previousPositions[index];
          Vector2 worldVelocity = velocity / deltaTime;
          var context = new PixelClothForceContext(
            index,
            new Vector2Int(column, row),
            new Vector2(column / (float)(columns - 1), row01),
            position,
            worldVelocity,
            Time.fixedTime,
            deltaTime);
          Vector2 acceleration = Vector2.zero;

          for (int i = 0; i < localModifiers.Count; i++)
          {
            PixelClothForceModifier modifier = localModifiers[i];

            if (modifier != null)
              acceleration += modifier.GetAcceleration(in context);
          }

          for (int i = 0; i < _runtimeModifiers.Count; i++)
            acceleration += _runtimeModifiers[i].Modifier.GetAcceleration(in context);

          _previousPositions[index] = position;
          _positions[index] = position + velocity * damping + acceleration * deltaTimeSqr;
        }
      }
    }

    private void SolveConstraints()
    {
      int pinnedCount = _columns;

      for (int i = 0; i < _constraints.Length; i++)
      {
        Constraint constraint = _constraints[i];
        Vector2 delta = _positions[constraint.IndexB] - _positions[constraint.IndexA];
        float distanceSqr = delta.sqrMagnitude;

        if (distanceSqr <= _kMinDistanceSqr)
          continue;

        float distance = Mathf.Sqrt(distanceSqr);
        Vector2 correction = delta * ((distance - constraint.RestDistance) / distance);
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

    private void RenderImmediately()
    {
      for (int i = 0; i < _positions.Length; i++)
        _renderVertices[i] = _meshFilter.transform.InverseTransformPoint(_positions[i]);

      _mesh.SetVertices(_renderVertices);
      _mesh.RecalculateBounds();
    }

    private int GetPointIndex(int column, int row) =>
      row * _columns + column;

    private Vector2 GetAnchorPosition() =>
      _anchor.position;

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
