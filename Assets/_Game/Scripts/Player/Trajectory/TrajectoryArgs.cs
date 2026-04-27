using UnityEngine;

namespace Sling.Player.Trajectory
{
    public readonly struct TrajectoryArgs
    {
        public readonly Vector2 DragStartPos;
        public readonly float Mass;
        public readonly PlayerInputEvents Events;

        public TrajectoryArgs(Vector2 dragStartPos, float mass, PlayerInputEvents events)
        {
            DragStartPos = dragStartPos;
            Mass = mass;
            Events = events;
        }
    }
}
