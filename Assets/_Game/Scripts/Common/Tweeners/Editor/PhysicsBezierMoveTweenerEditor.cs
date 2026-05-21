using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sling.Common.Tweeners.Editor
{
  [CustomEditor(typeof(PhysicsBezierMoveTweener))]
  public class PhysicsBezierMoveTweenerEditor : UnityEditor.Editor
  {
    private float _radius = 2f;
    private int _multiplier = 1;
    private bool _clockwise = false;

    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Circle Fill", EditorStyles.boldLabel);
      _radius = EditorGUILayout.FloatField("Radius", _radius);
      _multiplier = EditorGUILayout.IntSlider("Segments (×4)", _multiplier, 1, 4);
      _clockwise = EditorGUILayout.Toggle("Clockwise", _clockwise);

      if (GUILayout.Button("Fill Circle"))
        FillCircle();
    }

    private void OnSceneGUI()
    {
      var tweener = (PhysicsBezierMoveTweener)target;
      if (tweener.Segments == null || tweener.Segments.Count == 0)
        return;

      Transform t = tweener.transform;
      Vector3 initialLocalPos = t.localPosition;
      Vector3 currentOffset = Vector3.zero;

      for (int i = 0; i < tweener.Segments.Count; i++)
      {
        BezierSegment seg = tweener.Segments[i];
        Vector3 worldStart   = OffsetToWorld(t, initialLocalPos, currentOffset);
        Vector3 worldPoint   = OffsetToWorld(t, initialLocalPos, seg.Point);
        Vector3 worldControl = OffsetToWorld(t, initialLocalPos, seg.Control);

        Handles.color = Color.yellow;
        Vector3 prev = worldStart;
        for (int s = 1; s <= 16; s++)
        {
          float tv = s / 16f, mt = 1f - tv;
          Vector3 sample = OffsetToWorld(t, initialLocalPos,
            mt * mt * currentOffset + 2f * mt * tv * seg.Control + tv * tv * seg.Point);
          Handles.DrawLine(prev, sample);
          prev = sample;
        }

        Handles.color = new Color(0.6f, 0.6f, 0.6f);
        Handles.DrawLine(worldStart, worldControl);
        Handles.DrawLine(worldControl, worldPoint);

        Handles.color = Color.yellow;
        EditorGUI.BeginChangeCheck();
        Vector3 newWorldPoint = Handles.FreeMoveHandle(
          worldPoint, HandleUtility.GetHandleSize(worldPoint) * 0.15f,
          Vector3.zero, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject(tweener, "Move Bezier Point");
          seg.Point = WorldToOffset(t, initialLocalPos, newWorldPoint);
          tweener.Segments[i] = seg;
          EditorUtility.SetDirty(tweener);
        }

        Handles.color = Color.red;
        EditorGUI.BeginChangeCheck();
        Vector3 newWorldControl = Handles.FreeMoveHandle(
          worldControl, HandleUtility.GetHandleSize(worldControl) * 0.1f,
          Vector3.zero, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject(tweener, "Move Bezier Control");
          seg.Control = WorldToOffset(t, initialLocalPos, newWorldControl);
          tweener.Segments[i] = seg;
          EditorUtility.SetDirty(tweener);
        }

        currentOffset = seg.Point;
      }
    }

    private void FillCircle()
    {
      var tweener = (PhysicsBezierMoveTweener)target;
      Undo.RecordObject(tweener, "Fill Circle Bezier Segments");
      tweener.Segments = GenerateCircleSegments(_radius, _multiplier, _clockwise);
      EditorUtility.SetDirty(tweener);
    }

    private static List<BezierSegment> GenerateCircleSegments(float r, int multiplier, bool clockwise)
    {
      int n = 4 * multiplier;
      float deltaTheta = 2f * Mathf.PI / n;
      float sign = clockwise ? -1f : 1f;
      float cosHalf = Mathf.Cos(deltaTheta / 2f);
      var segments = new List<BezierSegment>(n);

      for (int i = 0; i < n; i++)
      {
        float alpha = -Mathf.PI / 2f + sign * (i + 1) * deltaTheta;
        float alphaMid = -Mathf.PI / 2f + sign * (i + 0.5f) * deltaTheta;

        float px = r * Mathf.Cos(alpha);
        float py = r + r * Mathf.Sin(alpha);
        float cx = r / cosHalf * Mathf.Cos(alphaMid);
        float cy = r + r / cosHalf * Mathf.Sin(alphaMid);

        segments.Add(new BezierSegment
        {
          Point   = new Vector3(px, py, 0f),
          Control = new Vector3(cx, cy, 0f),
        });
      }

      return segments;
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
