using Sling.Common.Collission;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Elements.MovingLevelPart
{
  public class MovingLevelPartView : MonoBehaviour, IViewListItem
  {
    public MovingLevelPartConfig Config;
    public TriggerZone TriggerZone;
    public Transform Target;
    public Rigidbody2D Rigidbody;
  }
}
