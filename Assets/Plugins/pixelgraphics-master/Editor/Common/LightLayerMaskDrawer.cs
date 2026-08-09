using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Aarthificial.PixelGraphics.Editor.Common
{
    [CustomPropertyDrawer(typeof(uint))]
    public class LightLayerMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.name.Contains("renderingLayerMask") || property.name.Contains("lightLayerMask"))
            {
                string[] naming = GraphicsSettings.currentRenderPipeline.renderingLayerMaskNames;

                property.intValue = EditorGUI.MaskField(position, label, property.intValue, naming);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
