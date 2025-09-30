#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <remarks>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
    /// </remarks>
    
    public static class EditorExtensions
    {
        #region Inspector Functions
        
        /// <summary>
        /// start custom inspector background with a defined color and padding.
        /// Default values of padding are set to account for inspector rect offsets:
        /// paddingTop = -4, paddingBottom = -15, paddingLeft = -18, paddingRight = -4
        /// </summary>
        /// <param name="backgroundColor"></param>
        public static void BeginCustomInspectorBackground(Color32 backgroundColor, float paddingTop = -4, float paddingBottom = -15, float paddingLeft = -18, float paddingRight = -4)
        {
            // cache variables
            Rect screenRect = GUILayoutUtility.GetRect(1, 1);
            Rect verticalRect = EditorGUILayout.BeginVertical();
            
            // calculate rect size
            float xMin = screenRect.x + paddingLeft;
            float yMin = screenRect.y + paddingTop;
            float width = screenRect.width - (paddingLeft + paddingRight);
            float height = verticalRect.height - (paddingTop + paddingBottom);
            
            // draw background rect
            EditorGUI.DrawRect(new Rect(xMin, yMin, width, height), backgroundColor);
        }

        /// <summary>
        /// end custom inspector background
        /// </summary>
        public static void EndCustomInspectorBackground()
        {
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw a horizontal line across the whole of the inspector
        /// </summary>
        /// <param name="lineColor"></param>
        /// <param name="spaceBefore"></param>
        /// <param name="spaceAfter"></param>
        public static void DrawInspectorLineFull(Color32 lineColor, int spaceBefore = 0, int spaceAfter = 0)
        {
            GUILayout.Space(spaceBefore);
            BeginCustomInspectorBackground(lineColor, 2, -3);
            EndCustomInspectorBackground();
            GUILayout.Space(spaceAfter);
        }
        
        #endregion

    } // class end
}

#endif