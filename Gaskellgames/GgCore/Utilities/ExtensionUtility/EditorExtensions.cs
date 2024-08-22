#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.EditorOnly
{
    public static class EditorExtensions
    {
        #region Get Assets

        /// <summary>
        /// Get all assets of a set type from the project files
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Return all objects of a set type from the project files</returns>
        public static List<T> GetAllAssetsByType<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------
        
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
        /// Force repaint the inspector for a targetObject via a SerializedProperty reference
        /// </summary>
        /// <param name="property"></param>
        public static void RepaintInspector(SerializedProperty property)
        {
            // EditorUtility.SetDirty(property.serializedObject.targetObject);
            
            foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (item.serializedObject != property.serializedObject) { continue; }
                
                item.Repaint();
                return;
            }
        }
        
        /// <summary>
        /// Start a foldout group (nestable)
        /// </summary>
        /// <param name="label">Label used for label text and tooltip</param>
        /// <param name="isOpen">Reference to a bool to be used to store if the foldout group is open</param>
        /// <param name="style">The style to be used for the vertical group (i.e "Box")</param>
        /// <returns></returns>
        public static bool BeginFoldoutGroupNestable(GUIContent label, bool isOpen, GUIStyle style = null, int paddingTop = 0, int paddingBottom = 0)
        {
            // default style
            if (style == null) { style = EditorStyles.helpBox; }
            
            EditorGUILayout.BeginVertical(style);
            GUILayout.Space(paddingTop);
            
            string icon = "d_IN_foldout";
            if (isOpen) { icon = "d_IN_foldout_on"; }
            Texture iconTexture = EditorGUIUtility.IconContent(icon).image;
            
            bool defaultState = GUI.enabled;
            GUI.enabled = true;
            if (GUILayout.Button(new GUIContent(label.text, iconTexture, label.tooltip), InspectorExtensions.Style_DropdownButton(), GUILayout.ExpandWidth(true)))
            {
                isOpen = !isOpen;
            }
            GUI.enabled = defaultState;
            
            GUILayout.Space(paddingBottom);
            
            return isOpen;
        }

        /// <summary>
        /// End a foldout group (nestable)
        /// </summary>
        public static void EndFoldoutGroupNestable()
        {
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw a horizontal line across the inspector
        /// </summary>
        /// <param name="lineColor"></param>
        /// <param name="spaceBefore"></param>
        /// <param name="spaceAfter"></param>
        public static void DrawInspectorLine(Color32 lineColor, int spaceBefore = 0, int spaceAfter = 0)
        {
            // add space before?
            GUILayout.Space(spaceBefore);
            
            // draw line and reset gui color
            Color defaultGUIColor = GUI.color;
            GUI.color = lineColor;
            GUIStyle horizontalLine = new GUIStyle
            {
                normal = { background = EditorGUIUtility.whiteTexture },
                margin = new RectOffset( 0, 0, 4, 4 ),
                fixedHeight = 1
            };
            
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = defaultGUIColor;
            
            // add space after?
            GUILayout.Space(spaceAfter);
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

        /// <summary>
        /// Get all editor labels for an object
        /// </summary>
        /// <param name="objectReference"></param>
        /// <returns></returns>
        public static string[] GetAllObjectLabels(Object objectReference)
        {
            return AssetDatabase.GetLabels(objectReference);
        }
        
        #endregion

    } // class end
}

#endif