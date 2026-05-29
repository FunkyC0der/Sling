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
      Vector3 prevWorld = TweenerEditorHandles.OffsetToWorld(t, initialLocalPos, Vector3.zero);

      GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel)
      {
        normal = { textColor = Color.yellow },
      };

      for (int i = 0; i < tweener.Points.Count; i++)
      {
        Vector3 point = tweener.Points[i];
        Vector3 worldPoint = TweenerEditorHandles.OffsetToWorld(t, initialLocalPos, point);

        Handles.color = Color.yellow;
        Handles.DrawLine(prevWorld, worldPoint);

        if (TweenerEditorHandles.MovePointHandle(tweener, "Move Tweener Point", t, initialLocalPos, ref point))
          tweener.Points[i] = point;

        Vector3 labelOffset = Vector3.up * HandleUtility.GetHandleSize(worldPoint) * 0.2f;
        Handles.Label(worldPoint + labelOffset, (i + 1).ToString(), labelStyle);

        prevWorld = worldPoint;
      }
    }
  }
}
