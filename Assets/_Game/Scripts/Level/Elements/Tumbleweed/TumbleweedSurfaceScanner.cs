using System.Collections.Generic;
using Sling.Level.Elements.CrumblingObject;
using Sling.Level.Hazards;
using UnityEngine;

namespace Sling.Level.Elements.Tumbleweed
{
  public static class TumbleweedSurfaceScanner
  {
    private const float Epsilon = 0.01f;
    private const float ProbeOffset = 0.05f;
    private const float BlockerBoxScale = 0.9f;
    private const float HazardSink = 1f;

    public static List<TumbleweedSurface> Scan(Rect area, TumbleweedConfig config)
    {
      var edges = new List<TumbleweedSurface>();
      var visited = new HashSet<Collider2D>();

      foreach (Collider2D collider in Physics2D.OverlapAreaAll(area.min, area.max, config.GroundMask))
      {
        if (!visited.Add(collider) || !IsStaticGround(collider))
          continue;

        CollectTopEdges(collider, config, edges);
      }

      return Merge(edges, config);
    }

    private static bool IsStaticGround(Collider2D collider)
    {
      Rigidbody2D body = collider.attachedRigidbody;
      if (body && body.bodyType != RigidbodyType2D.Static)
        return false;

      return !collider.GetComponentInParent<CrumblingObject.CrumblingObject>();
    }

    private static void CollectTopEdges(Collider2D collider, TumbleweedConfig config, List<TumbleweedSurface> result)
    {
      int pathCount = collider switch
      {
        PolygonCollider2D polygon => polygon.pathCount,
        CompositeCollider2D composite => composite.pathCount,
        _ => 0
      };

      var points = new List<Vector2>();
      for (int i = 0; i < pathCount; i++)
      {
        points.Clear();
        if (collider is PolygonCollider2D p)
          p.GetPath(i, points);
        else
          ((CompositeCollider2D)collider).GetPath(i, points);

        for (int j = 0; j < points.Count; j++)
        {
          Vector2 a = collider.transform.TransformPoint(points[j] + collider.offset);
          Vector2 b = collider.transform.TransformPoint(points[(j + 1) % points.Count] + collider.offset);
          if (Mathf.Abs(a.y - b.y) > Epsilon || Mathf.Abs(a.x - b.x) < Epsilon)
            continue;

          float left = Mathf.Min(a.x, b.x);
          float right = Mathf.Max(a.x, b.x);
          float y = (a.y + b.y) * 0.5f;
          Vector2 mid = new((left + right) * 0.5f, y);

          // top edge: ground below, free space above
          if (collider.OverlapPoint(mid + Vector2.up * ProbeOffset) || !collider.OverlapPoint(mid + Vector2.down * ProbeOffset))
            continue;

          SplitByBlockers(left, right, y, config, result);
        }
      }
    }

    private static void SplitByBlockers(float left, float right, float y, TumbleweedConfig config,
      List<TumbleweedSurface> result)
    {
      float cell = config.CellSize;
      float runStart = left;
      bool inRun = false;
      float x = left;

      while (x < right - Epsilon)
      {
        float next = Mathf.Min(x + cell, right);
        float width = next - x;
        Vector2 center = new((x + next) * 0.5f, y + cell * 0.5f);
        bool blocked = Physics2D.OverlapBox(center, new Vector2(width, cell) * BlockerBoxScale, 0f, config.BlockerMask)
                       || HasHazard(x, next, y, cell);

        if (blocked && inRun)
        {
          result.Add(new TumbleweedSurface(runStart, x, y));
          inRun = false;
        }
        else if (!blocked && !inRun)
        {
          runStart = x;
          inRun = true;
        }

        x = next;
      }

      if (inRun)
        result.Add(new TumbleweedSurface(runStart, right, y));
    }

    // hazards may sit slightly sunk into the ground or on any layer, so probe a full box from just below the surface
    private static bool HasHazard(float left, float right, float y, float cell)
    {
      var center = new Vector2((left + right) * 0.5f, y + (cell - HazardSink) * 0.5f);
      var size = new Vector2(right - left, cell + HazardSink);
      foreach (Collider2D collider in Physics2D.OverlapBoxAll(center, size, 0f))
      {
        if (collider.GetComponentInParent<HazardZoneView>())
          return true;
      }

      return false;
    }

    private static List<TumbleweedSurface> Merge(List<TumbleweedSurface> edges, TumbleweedConfig config)
    {
      edges.Sort((a, b) => Mathf.Abs(a.TopY - b.TopY) > Epsilon ? a.TopY.CompareTo(b.TopY) : a.Left.CompareTo(b.Left));

      var merged = new List<TumbleweedSurface>();
      foreach (TumbleweedSurface edge in edges)
      {
        if (merged.Count > 0)
        {
          TumbleweedSurface last = merged[^1];
          if (Mathf.Abs(last.TopY - edge.TopY) < Epsilon && edge.Left <= last.Right + Epsilon)
          {
            merged[^1] = new TumbleweedSurface(last.Left, Mathf.Max(last.Right, edge.Right), last.TopY);
            continue;
          }
        }

        merged.Add(edge);
      }

      merged.RemoveAll(s => s.Right - s.Left < config.MinSurfaceCells * config.CellSize - Epsilon);
      return merged;
    }
  }
}
