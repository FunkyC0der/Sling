using Sling.Common;
using UnityEngine;

namespace Sling.Level.Session
{
  public class LevelModel
  {
    public Vector2 PlayerStartPos;
    public bool PlayerIsFacingLeft;
    
    public readonly Observable<int> PlayerDeathCount = new();
    public float ElapsedTimeInSeconds;
    public bool IsNewBestScore;
  }
}
