using UnityEngine;

namespace Sling.Common.Scenes
{
  public class DontDestroyOnLoadView : MonoBehaviour
  {
    private void Awake() => DontDestroyOnLoad(gameObject);
  }
}
