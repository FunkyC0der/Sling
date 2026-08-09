using System;
using UnityEngine;

namespace Aarthificial.PixelGraphics.Common
{
    [Serializable]
    public class SimulationSettings
    {
        [Header("Spring Physics")]
        [SerializeField]
        [Tooltip("The spring constant. Controls the 'pull' back to the origin. Higher values make the effect snap back quickly; lower values make it feel loose and heavy.")]
        private float stiffness = 70;

        [SerializeField]
        [Tooltip("The linear damping coefficient. Simulates air resistance or friction. Higher values prevent the simulation from oscillating (wobbling) forever.")]
        private float damping = 3;

        [Header("Visuals")]
        [SerializeField]
        [Tooltip(
            "The strength of the blur.\nOther factors, like the velocity texture scale, can also affect blurring. Determines how much velocity 'bleeds' into neighboring pixels. High values create a blurry, fluid-like smoke effect."
        )]
        private float blurStrength = 0.5f;
        
        [SerializeField]
        [Range(0.95f, 1f)] 
        [Tooltip("The rate at which the trail fades over time. 1.0 stays forever, 0.95 fades slowly, and lower values create a short, ghostly trail.")]
        private float trailFade = 0.95f;
        [Tooltip("Scales the impact of the object's actual movement. Increase this to make the trail 'stretch' further when the object moves fast.")]
        [Range(0, 5)] public float velocityInfluence = 1.0f; 
        
        [Header("Stability & Cleanup")]
        [SerializeField]
        [Tooltip(
            "The maximum delta time in seconds.\nUsed to keep the simulation stable in case of FPS drops. The default values is 1/30 (30 FPS). Safety cap for the physics calculation. Prevents the simulation from 'exploding' if the frame rate (FPS) drops suddenly."
        )]
        private float maxDeltaTime = 0.034f;
        [Tooltip("Values smaller than this are forced to zero. This prevents 'ghosting'—faint, invisible data that lingers in the texture and wastes performance.")]
        [Range(0.001f, 0.05f)]  public float minThreshold = 0.001f;
        public Vector4 Value =>
            new Vector4(
                stiffness,
                damping,
                blurStrength,
                maxDeltaTime
            );
        
        public Vector4 ExtraParams => new Vector4(trailFade, velocityInfluence, minThreshold, 0);
        
    }
}