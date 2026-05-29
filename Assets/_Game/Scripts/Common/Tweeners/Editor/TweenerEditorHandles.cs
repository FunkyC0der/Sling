using UnityEditor;
using UnityEngine;

namespace Sling.Common.Tweeners.Editor
{
  internal static class TweenerEditorHandles
  {
    public static Vector3 MoveHandleSnap => EditorSnapSettings.gridSnapEnabled
      ? EditorSnapSettings.move
      : Vector3.zero;

    public static bool MovePointHandle(
      UnityEngine.Object undoTarget,
      string undoName,
      Transform transform,
      Vector3 initialLocalPosition,
      ref Vector3 point)
    {
      Vector3 worldPoint = OffsetToWorld(transform, initialLocalPosition, point);

      EditorGUI.BeginChangeCheck();
      Vector3 newWorldPoint = Handles.FreeMoveHandle(
        worldPoint, HandleUtility.GetHandleSize(worldPoint) * 0.15f,
        MoveHandleSnap, Handles.SphereHandleCap);
      if (!EditorGUI.EndChangeCheck())
        return false;

      Undo.RecordObject(undoTarget, undoName);
      newWorldPoint = SnapToGridIfEnabled(newWorldPoint);
      point = WorldToOffset(transform, initialLocalPosition, newWorldPoint);
      EditorUtility.SetDirty(undoTarget);
      return true;
    }

    public static Vector3 OffsetToWorld(Transform transform, Vector3 initialLocalPosition, Vector3 offset)
    {
      Vector3 localPosition = initialLocalPosition + transform.localRotation * offset;
      return transform.parent != null ? transform.parent.TransformPoint(localPosition) : localPosition;
    }

    public static Vector3 WorldToOffset(Transform transform, Vector3 initialLocalPosition, Vector3 worldPosition)
    {
      Vector3 localPosition = transform.parent != null ? transform.parent.InverseTransformPoint(worldPosition) : worldPosition;
      return Quaternion.Inverse(transform.localRotation) * (localPosition - initialLocalPosition);
    }

    public static Vector3 SnapToGridIfEnabled(Vector3 worldPosition)
    {
      if (!EditorSnapSettings.gridSnapEnabled)
        return worldPosition;

      Vector3[] positions = { worldPosition };
      Handles.SnapToGrid(positions, SnapAxis.All);
      return positions[0];
    }
  }
}
