using UnityEditor;
using UnityEngine;

namespace Sling.Common.Tweeners.Editor
{
  [CustomEditor(typeof(PhysicsMoveTweener))]
  public class PhysicsMoveTweenerEditor : UnityEditor.Editor
  {
    private void OnSceneGUI()
    {
      var tweener = (PhysicsMoveTweener)target;
      if (tweener.Points == null || tweener.Points.Count == 0)
        return;

      Transform t = tweener.transform;
      Vector3 initialLocalPos = t.localPosition;
      Vector3 prevWorld = OffsetToWorld(t, initialLocalPos, Vector3.zero);

      for (int i = 0; i < tweener.Points.Count; i++)
      {
        Vector3 worldPoint = OffsetToWorld(t, initialLocalPos, tweener.Points[i]);

        Handles.color = Color.yellow;
        Handles.DrawLine(prevWorld, worldPoint);

        EditorGUI.BeginChangeCheck();
        Vector3 newWorld = Handles.FreeMoveHandle(
          worldPoint, HandleUtility.GetHandleSize(worldPoint) * 0.15f,
          Vector3.zero, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject(tweener, "Move Tweener Point");
          tweener.Points[i] = WorldToOffset(t, initialLocalPos, newWorld);
          EditorUtility.SetDirty(tweener);
        }

        prevWorld = worldPoint;
      }
    }

    private static Vector3 OffsetToWorld(Transform t, Vector3 initialLocalPos, Vector3 offset)
    {
      Vector3 localPos = initialLocalPos + t.localRotation * offset;
      return t.parent != null ? t.parent.TransformPoint(localPos) : localPos;
    }

    private static Vector3 WorldToOffset(Transform t, Vector3 initialLocalPos, Vector3 worldPos)
    {
      Vector3 localPos = t.parent != null ? t.parent.InverseTransformPoint(worldPos) : worldPos;
      return Quaternion.Inverse(t.localRotation) * (localPos - initialLocalPos);
    }
  }
}
