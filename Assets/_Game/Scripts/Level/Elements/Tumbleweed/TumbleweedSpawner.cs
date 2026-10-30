using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sling.Level.Elements.Tumbleweed
{
  public class TumbleweedSpawner : MonoBehaviour
  {
    [SerializeField] private TumbleweedConfig _config;
    [SerializeField] private Camera _camera;
    [SerializeField] private Tumbleweed _template;

    private const int InitialSpawnAttempts = 20;

    private readonly List<Tumbleweed> _pool = new();
    private readonly List<TumbleweedSurface> _candidates = new();
    private List<TumbleweedSurface> _surfaces = new();
    private float _timer;

    private IEnumerator Start()
    {
      // let physics pick up the level colliders before querying them
      yield return new WaitForFixedUpdate();

      Rect area = GetCameraRect(_config.CameraPadding);
      _surfaces = TumbleweedSurfaceScanner.Scan(area, _config);

      for (int i = 0; i < _config.MaxAlive; i++)
      {
        Tumbleweed instance = Instantiate(_template, transform);
        instance.gameObject.SetActive(false);
        _pool.Add(instance);
      }

      int spawned = 0;
      for (int attempt = 0; attempt < InitialSpawnAttempts && spawned < _config.InitialCount; attempt++)
      {
        if (TrySpawn())
          spawned++;
      }

      ResetTimer();
    }

    private void Update()
    {
      if (_pool.Count == 0)
        return;

      _timer -= Time.deltaTime;
      if (_timer > 0f)
        return;

      ResetTimer();
      TrySpawn();
    }

    private bool TrySpawn()
    {
      Tumbleweed free = _pool.Find(t => !t.gameObject.activeSelf);
      if (!free)
        return false;

      Rect view = GetCameraRect(0f);
      _candidates.Clear();
      float totalLength = 0f;
      foreach (TumbleweedSurface surface in _surfaces)
      {
        if (surface.Left < view.xMax && surface.Right > view.xMin
                                     && surface.TopY > view.yMin && surface.TopY < view.yMax)
        {
          _candidates.Add(surface);
          totalLength += surface.Right - surface.Left;
        }
      }

      if (_candidates.Count == 0)
        return false;

      TumbleweedSurface chosen = PickByLength(totalLength);
      Sprite sprite = _config.Sprites[Random.Range(0, _config.Sprites.Length)];
      float radius = sprite.bounds.extents.y;

      float minLifetime = Mathf.Max(_config.MinLifetime, _config.FadeInDuration + _config.FadeOutDuration);
      float maxLifetime = Mathf.Max(_config.MaxLifetime, minLifetime);

      float leftX = chosen.Left + radius;
      float rightX = chosen.Right - radius;
      if (leftX >= rightX)
        return false;

      // flight length comes from speed and lifetime; on a short surface it is cut to fit,
      // then the start is picked anywhere it still won't overshoot the right edge
      float speed = Random.Range(_config.SpeedMin, _config.SpeedMax);
      float distance = Mathf.Min(speed * Random.Range(minLifetime, maxLifetime), rightX - leftX);
      float startX = Random.Range(leftX, rightX - distance);
      float endX = startX + distance;

      float hop = Random.Range(_config.HopHeightMin, _config.HopHeightMax);
      free.Launch(_config, chosen, startX, endX, sprite, speed, hop);
      return true;
    }

    // weighted by length so spawn density per unit is the same on short and long surfaces
    private TumbleweedSurface PickByLength(float totalLength)
    {
      float pick = Random.Range(0f, totalLength);
      foreach (TumbleweedSurface surface in _candidates)
      {
        pick -= surface.Right - surface.Left;
        if (pick <= 0f)
          return surface;
      }

      return _candidates[^1];
    }

    private void ResetTimer() =>
      _timer = Random.Range(_config.SpawnIntervalMin, _config.SpawnIntervalMax);

    private Rect GetCameraRect(float padding)
    {
      float height = _camera.orthographicSize * 2f;
      float width = height * _camera.aspect;
      Vector3 center = _camera.transform.position;
      return new Rect(center.x - width * 0.5f - padding, center.y - height * 0.5f - padding,
        width + padding * 2f, height + padding * 2f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.yellow;
      foreach (TumbleweedSurface surface in _surfaces)
      {
        Gizmos.DrawLine(new Vector3(surface.Left, surface.TopY), new Vector3(surface.Right, surface.TopY));
        Gizmos.DrawSphere(new Vector3(surface.Left, surface.TopY), 0.15f);
        Gizmos.DrawSphere(new Vector3(surface.Right, surface.TopY), 0.15f);
      }
    }
#endif
  }
}
