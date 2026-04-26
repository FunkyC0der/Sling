using UnityEngine;

public class LevelSceneContext : MonoBehaviour
{
    [SerializeField] public PlayerView PlayerView;
    [SerializeField] public FinishView FinishView;
    [SerializeField] public HazardView[] Hazards;

    // Set by LoadLevelController after scene load
    [HideInInspector] public LevelEvents LevelEvents;
}
