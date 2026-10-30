using UnityEngine;

namespace Sling.Level.Elements.Tumbleweed
{
  public class Tumbleweed : MonoBehaviour
  {
    [SerializeField] private SpriteRenderer _renderer;

    private TumbleweedConfig _config;
    private float _topY;
    private float _startX;
    private float _endX;
    private float _speed;
    private float _hopHeight;
    private float _radius;
    private float _pixelSize;
    private float _x;
    private float _time;

    public void Launch(TumbleweedConfig config, TumbleweedSurface surface, float startX, float endX,
      Sprite sprite, float speed, float hopHeight)
    {
      _config = config;
      _topY = surface.TopY;
      _startX = startX;
      _endX = endX;
      _speed = speed;
      _hopHeight = hopHeight;
      _radius = sprite.bounds.extents.y;
      _pixelSize = 1f / sprite.pixelsPerUnit;
      _x = startX;
      _time = 0f;

      _renderer.sprite = sprite;
      gameObject.SetActive(true);
      Apply();
    }

    private void Update()
    {
      _time += Time.deltaTime;
      _x += _speed * Time.deltaTime;

      if (_x >= _endX)
      {
        gameObject.SetActive(false);
        return;
      }

      Apply();
    }

    private void Apply()
    {
      float fadeIn = _config.FadeInDuration > 0f ? _time / _config.FadeInDuration : 1f;
      float fadeOut = _config.FadeOutDuration > 0f ? (_endX - _x) / (_speed * _config.FadeOutDuration) : 1f;
      float k = Mathf.Clamp01(Mathf.Min(fadeIn, fadeOut));

      float traveled = _x - _startX;
      float phase = traveled / _config.HopLength * Mathf.PI;
      float y = _topY + _radius * k + Mathf.Abs(Mathf.Sin(phase)) * _hopHeight * k;

      Vector3 position = new(_x, y, transform.position.z);
      if (_config.PixelSnap)
      {
        position.x = Mathf.Round(position.x / _pixelSize) * _pixelSize;
        position.y = Mathf.Round(position.y / _pixelSize) * _pixelSize;
      }

      transform.position = position;

      Transform visual = _renderer.transform;
      visual.localRotation = Quaternion.Euler(0f, 0f, -traveled / _radius * Mathf.Rad2Deg);
      visual.localScale = Vector3.one * k;

      Color color = _renderer.color;
      color.a = k;
      _renderer.color = color;
    }
  }
}
