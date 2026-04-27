using System;
using UnityEngine;

namespace Sling.Player
{
    public class PlayerInputEvents
    {
        public Action<Vector2> OnPointerDown;
        public Action<Vector2> OnPointerUp;
        public Action<Vector2> OnPointerDragged;
    }
}
