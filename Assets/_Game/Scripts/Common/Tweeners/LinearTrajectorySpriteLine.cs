using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sling.Common.Tweeners
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof(PhysicsMoveTweener))]
  public class LinearTrajectorySpriteLine : MonoBehaviour
  {
    private const string _kTrajectoryPrefabPath =
      "Assets/_Game/Prefabs/Gameplay/Zones/SawTrajectory.prefab";

    [SerializeField] private PhysicsMoveTweener _tweener;
    [SerializeField] private GameObject _trajectoryPrefab;
    [SerializeField, HideInInspector] private GameObject _renderObject;

    private void Reset()
    {
      _tweener = GetComponent<PhysicsMoveTweener>();

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
      Undo.SetCurrentGroupName("Generate Linear Trajectory");
      Undo.RecordObject(this, "Generate Linear Trajectory");

      ClearGeneratedObject();
      CreateTrajectoryObject();

      EditorUtility.SetDirty(this);
      Undo.CollapseUndoOperations(undoGroup);
    }

    public void ClearLine()
    {
      int undoGroup = Undo.GetCurrentGroup();
      Undo.SetCurrentGroupName("Clear Linear Trajectory");
      Undo.RecordObject(this, "Clear Linear Trajectory");
      ClearGeneratedObject();
      EditorUtility.SetDirty(this);
      Undo.CollapseUndoOperations(undoGroup);
    }

    public string GetValidationError()
    {
      ResolveReferences();

      if (_tweener == null)
        return "Linear Trajectory requires a Physics Move Tweener.";

      if (_trajectoryPrefab == null)
        return $"Linear Trajectory requires a prefab at {_kTrajectoryPrefabPath}.";

      if (_trajectoryPrefab.GetComponent<SpriteShapeController>() == null)
        return "Saw Trajectory prefab must contain a Sprite Shape Controller on its root.";

      if (_tweener.Points == null || _tweener.Points.Count == 0)
        return "Physics Move Tweener must contain at least one point.";

      Vector3 currentOffset = Vector3.zero;
      for (int i = 0; i < _tweener.Points.Count; i++)
      {
        Vector3 point = _tweener.Points[i];
        if (Vector3.Distance(currentOffset, point) < 0.01f)
          return $"Linear segment {i + 1} ends too close to its start.";

        currentOffset = point;
      }

      return string.Empty;
    }

    private void ResolveReferences()
    {
      if (_tweener == null)
        _tweener = GetComponent<PhysicsMoveTweener>();
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
      Undo.RegisterCreatedObjectUndo(_renderObject, "Generate Linear Trajectory");
    }

    private void PopulateSpline(SpriteShapeController controller)
    {
      Spline spline = controller.spline;
      spline.Clear();
      spline.isOpenEnded = true;

      Transform tweenerTransform = _tweener.transform;
      Vector3 initialLocalPosition = tweenerTransform.localPosition;

      spline.InsertPointAt(
        0,
        WorldToTrajectoryLocal(
          controller.transform,
          OffsetToWorld(
            tweenerTransform,
            initialLocalPosition,
            Vector3.zero)));

      for (int i = 0; i < _tweener.Points.Count; i++)
      {
        spline.InsertPointAt(
          i + 1,
          WorldToTrajectoryLocal(
            controller.transform,
            OffsetToWorld(
              tweenerTransform,
              initialLocalPosition,
              _tweener.Points[i])));
      }

      for (int i = 0; i < spline.GetPointCount(); i++)
      {
        spline.SetTangentMode(i, ShapeTangentMode.Linear);
        spline.SetHeight(i, 1f);
        spline.SetSpriteIndex(i, 0);
        spline.SetCorner(i, true);
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
