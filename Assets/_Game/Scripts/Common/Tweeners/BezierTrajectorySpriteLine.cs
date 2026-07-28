using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.Tweeners
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof(PhysicsBezierMoveTweener))]
  public class BezierTrajectorySpriteLine : MonoBehaviour
  {
    private const string _kTrajectoryPrefabPath =
      "Assets/_Game/Prefabs/Gameplay/Zones/SawTrajectory.prefab";

    [SerializeField] private PhysicsBezierMoveTweener _tweener;
    [SerializeField] private GameObject _trajectoryPrefab;
    [SerializeField, HideInInspector] private GameObject _renderObject;

    private void Reset()
    {
      _tweener = GetComponent<PhysicsBezierMoveTweener>();

#if UNITY_EDITOR
      _trajectoryPrefab =
        AssetDatabase.LoadAssetAtPath<GameObject>(_kTrajectoryPrefabPath);
#endif
    }

#if UNITY_EDITOR
    public void GenerateLine()
    {
      ResolveReferences();

      string validationError = GetValidationError();
      if (!string.IsNullOrEmpty(validationError))
      {
        Debug.LogError(validationError, this);
        return;
      }

      int undoGroup = Undo.GetCurrentGroup();
      Undo.SetCurrentGroupName("Generate Bezier Trajectory");
      Undo.RecordObject(this, "Generate Bezier Trajectory");

      ClearGeneratedObject();
      CreateTrajectoryObject();

      EditorUtility.SetDirty(this);
      Undo.CollapseUndoOperations(undoGroup);
    }

    public void ClearLine()
    {
      int undoGroup = Undo.GetCurrentGroup();
      Undo.SetCurrentGroupName("Clear Bezier Trajectory");
      Undo.RecordObject(this, "Clear Bezier Trajectory");
      ClearGeneratedObject();
      EditorUtility.SetDirty(this);
      Undo.CollapseUndoOperations(undoGroup);
    }

    public string GetValidationError()
    {
      ResolveReferences();

      if (_tweener == null)
        return "Bezier Trajectory requires a Physics Bezier Move Tweener.";

      if (_trajectoryPrefab == null)
        return $"Bezier Trajectory requires a prefab at {_kTrajectoryPrefabPath}.";

      if (_trajectoryPrefab.GetComponent<SpriteShapeController>() == null)
        return "Saw Trajectory prefab must contain a Sprite Shape Controller on its root.";

      if (_tweener.Segments == null || _tweener.Segments.Count == 0)
        return "Physics Bezier Move Tweener must contain at least one segment.";

      Vector3 currentOffset = Vector3.zero;
      for (int i = 0; i < _tweener.Segments.Count; i++)
      {
        Vector3 point = _tweener.Segments[i].Point;
        if (Vector3.Distance(currentOffset, point) < 0.01f)
          return $"Bezier segment {i + 1} ends too close to its start.";

        currentOffset = point;
      }

      return string.Empty;
    }

    private void ResolveReferences()
    {
      if (_tweener == null)
        _tweener = GetComponent<PhysicsBezierMoveTweener>();
      if (_trajectoryPrefab == null)
        _trajectoryPrefab =
          AssetDatabase.LoadAssetAtPath<GameObject>(_kTrajectoryPrefabPath);
    }

    private void CreateTrajectoryObject()
    {
      _renderObject = PrefabUtility.InstantiatePrefab(
        _trajectoryPrefab,
        transform.parent) as GameObject;
      if (_renderObject == null)
        return;

      _renderObject.name = $"{name} Saw Trajectory";
      _renderObject.transform.localPosition = Vector3.zero;
      _renderObject.transform.localRotation = Quaternion.identity;
      _renderObject.transform.localScale = Vector3.one;

      SpriteShapeController controller =
        _renderObject.GetComponent<SpriteShapeController>();
      PopulateSpline(controller);
      controller.RefreshSpriteShape();

      EditorUtility.SetDirty(controller);
      PrefabUtility.RecordPrefabInstancePropertyModifications(
        _renderObject.transform);
      PrefabUtility.RecordPrefabInstancePropertyModifications(controller);
      Undo.RegisterCreatedObjectUndo(_renderObject, "Generate Bezier Trajectory");
    }

    private void PopulateSpline(SpriteShapeController controller)
    {
      Spline spline = controller.spline;
      spline.Clear();
      spline.isOpenEnded = true;

      Transform tweenerTransform = _tweener.transform;
      Vector3 initialLocalPosition = tweenerTransform.localPosition;
      Vector3 currentOffset = Vector3.zero;

      spline.InsertPointAt(
        0,
        WorldToTrajectoryLocal(
          controller.transform,
          OffsetToWorld(tweenerTransform, initialLocalPosition, currentOffset)));

      for (int i = 0; i < _tweener.Segments.Count; i++)
      {
        BezierSegment segment = _tweener.Segments[i];
        spline.InsertPointAt(
          i + 1,
          WorldToTrajectoryLocal(
            controller.transform,
            OffsetToWorld(
              tweenerTransform,
              initialLocalPosition,
              segment.Point)));
      }

      for (int i = 0; i < spline.GetPointCount(); i++)
      {
        spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        spline.SetHeight(i, 1f);
        spline.SetSpriteIndex(i, 0);
        spline.SetCorner(i, false);
      }

      currentOffset = Vector3.zero;
      for (int i = 0; i < _tweener.Segments.Count; i++)
      {
        BezierSegment segment = _tweener.Segments[i];
        Vector3 localStart = WorldToTrajectoryLocal(
          controller.transform,
          OffsetToWorld(
            tweenerTransform,
            initialLocalPosition,
            currentOffset));
        Vector3 localControl = WorldToTrajectoryLocal(
          controller.transform,
          OffsetToWorld(
            tweenerTransform,
            initialLocalPosition,
            segment.Control));
        Vector3 localEnd = WorldToTrajectoryLocal(
          controller.transform,
          OffsetToWorld(
            tweenerTransform,
            initialLocalPosition,
            segment.Point));

        spline.SetRightTangent(
          i,
          2f / 3f * (localControl - localStart));
        spline.SetLeftTangent(
          i + 1,
          2f / 3f * (localControl - localEnd));

        currentOffset = segment.Point;
      }
    }

    private void ClearGeneratedObject()
    {
      if (_renderObject == null)
        return;

      Undo.DestroyObjectImmediate(_renderObject);
      _renderObject = null;
    }

    private static Vector3 WorldToTrajectoryLocal(
      Transform trajectoryTransform,
      Vector3 worldPosition) =>
      trajectoryTransform.InverseTransformPoint(worldPosition);

    private static Vector3 OffsetToWorld(
      Transform tweenerTransform,
      Vector3 initialLocalPosition,
      Vector3 offset)
    {
      Vector3 localPosition =
        initialLocalPosition + tweenerTransform.localRotation * offset;
      Transform parent = tweenerTransform.parent;
      return parent != null ? parent.TransformPoint(localPosition) : localPosition;
    }
#endif
  }
}
