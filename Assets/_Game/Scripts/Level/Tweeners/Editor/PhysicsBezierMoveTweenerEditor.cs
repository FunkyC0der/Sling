using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sling.Level.Tweeners;

[CustomEditor(typeof(PhysicsBezierMoveTweener))]
public class PhysicsBezierMoveTweenerEditor : Editor
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
}
