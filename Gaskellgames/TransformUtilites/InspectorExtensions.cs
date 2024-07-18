#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Code created by Gaskellgames: https://github.com/Gaskellgames
/// </summary>

namespace Gaskellgames.EditorOnly
{
    public static class InspectorExtensions
    {
        #region Static Variables

        // blank
        public static readonly Color32 blankColor = new Color32(000, 000, 000, 000);
        public static readonly Color32 cyanColor = new Color32(000, 179, 223, 255);
        public static readonly Color32 yellowColor = new Color32(223, 179, 000, 255);
        
        // background
        public static readonly Color32 backgroundNormalColorLight = new Color32(056, 056, 056, 255);
        public static readonly Color32 backgroundNormalColor = new Color32(051, 051, 051, 255);
        public static readonly Color32 backgroundNormalColorDark = new Color32(045, 045, 045, 255);
        public static readonly Color32 backgroundActiveColor = new Color32(044, 093, 135, 255);
        public static readonly Color32 backgroundHoverColor = new Color32(056, 056, 056, 255);
        
        public static readonly Color32 backgroundShadowColor = new Color32(042, 042, 042, 255);
        
        public static readonly Color32 backgroundSeperatorColor = new Color32(079, 079, 079, 255);
        public static readonly Color32 backgroundSeperatorColorDark = new Color32(035, 035, 035, 255);
        
        // selected
        public static readonly Color32 buttonSelectedColor = new Color32(128, 179, 223, 255);
        
        // button - normal, hover, active
        public static readonly Color32 buttonHoverColor = new Color32(099, 099, 099, 255);
        public static readonly Color32 buttonHoverBorderColor = new Color32(028, 028, 028, 255);
        public static readonly Color32 buttonActiveColor = new Color32(000, 128, 223, 255);
        public static readonly Color32 buttonActiveBorderColor = new Color32(010, 010, 010, 255);
        
        // text
        public static readonly Color32 textNormalColor = new Color32(179, 179, 179, 255);
        public static readonly Color32 textDisabledColor = new Color32(113, 113, 113, 255);

        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region Helper Functions

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
            EditorUtility.SetDirty(property.serializedObject.targetObject); // Repaint
        }
        
        /// <summary>
        /// Start a foldout group (nestable)
        /// </summary>
        /// <param name="label">Label used for label text and tooltip</param>
        /// <param name="isOpen">Reference to a bool to be used to store if the foldout group is open</param>
        /// <param name="style">The style to be used for the vertical group (i.e "Box")</param>
        /// <returns></returns>
        public static bool BeginFoldoutGroupNestable(GUIContent label, ref bool isOpen, GUIStyle style, int paddingTop = 0, int paddingBottom = 0)
        {
            EditorGUILayout.BeginVertical(style);
            GUILayout.Space(paddingTop);
            
            string icon = "d_IN_foldout";
            if (isOpen) { icon = "d_IN_foldout_on"; }
            Texture iconTexture = EditorGUIUtility.IconContent(icon).image;
            
            bool defaultState = GUI.enabled;
            GUI.enabled = true;
            if (GUILayout.Button(new GUIContent(label.text, iconTexture, label.tooltip), Style_DropdownButton(), GUILayout.ExpandWidth(true)))
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
        /// Draw a horizontal line across the whole of the inspector
        /// </summary>
        /// <param name="lineColor"></param>
        /// <param name="addSpaceBefore"></param>
        /// <param name="addSpaceAfter"></param>
        public static void DrawInspectorLine(Color32 lineColor, bool addSpaceBefore = false, bool addSpaceAfter = false)
        {
            // add space before?
            if (addSpaceBefore) { EditorGUILayout.Space(); }
            
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
            if (addSpaceAfter) { EditorGUILayout.Space(); }
        }
        
        public static Texture2D CreateTexture(int width, int height, int border, bool isRounded, Color32 backgroundColor, Color32 borderColor)
        {
            Color[] pixels = new Color[width * height];
            int pixelIndex = 0;

            if (isRounded)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        // if at corner add corner color
                        if ((i < border || i >= width - border) && (j < border || j >= width - border))
                        {
                            pixels[pixelIndex] = blankColor;
                        }
                        // otherwise if on border... 
                        else if ((i < border || i >= width - border || j < border || j >= width - border)
                                 || ((i < border*2 || i >= width - border*2) && (j < border*2 || j >= width - border*2)))
                        {
                            // ... add border color
                            pixels[pixelIndex] = borderColor;
                        }
                        else
                        {
                            // ... otherwise add background color
                            pixels[pixelIndex] = backgroundColor;
                        }

                        pixelIndex++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        // if on border... 
                        if (i < border || i >= width - border || j < border || j >= width - border)
                        {
                            // ... add border color
                            pixels[pixelIndex] = borderColor;
                        }
                        else
                        {
                            // ... otherwise add background color
                            pixels[pixelIndex] = backgroundColor;
                        }

                        pixelIndex++;
                    }
                }
            }
   
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
        
        public static Texture2D TintTexture(Texture2D texture2D, Color tint)
        {
            // null check
            if (!texture2D) { return null; }
            
            int width = texture2D.width;
            int height = texture2D.height;
            Color[] pixels = new Color[width * height];
            int pixelIndex = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pixels[pixelIndex] = texture2D.GetPixel(j, i) * tint;
                    pixelIndex++;
                }
            }
   
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
        
        public static Texture2D AddBorderToTexture(Texture2D texture2D, Color borderColor, int borderThickness)
        {
            // null check
            if (!texture2D) { return null; }
            
            int width = texture2D.width;
            int height = texture2D.height;
            Color[] pixels = new Color[width * height];
            int pixelIndex = 0;
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // if on border... 
                    if (i < borderThickness || i >= width - borderThickness || j < borderThickness || j >= width - borderThickness)
                    {
                        // ... add border color
                        pixels[pixelIndex] = borderColor;
                    }
                    else
                    {
                        // ... otherwise get pixel color
                        pixels[pixelIndex] = pixels[pixelIndex] = texture2D.GetPixel(j, i);
                    }
                    
                    pixelIndex++;
                }
            }
   
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Custom Button Styles

        public static GUIStyle Style_DefaultButton()
        {
            return new GUIStyle(GUI.skin.button);
        }
        
        public static GUIStyle Style_Button()
        {
            GUIStyle buttonStyle = new GUIStyle();
        
            buttonStyle.fontSize = 9;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = textNormalColor;
            buttonStyle.hover.textColor = textNormalColor;
            buttonStyle.active.textColor = textNormalColor;
            buttonStyle.normal.background = CreateTexture(20, 20, 1, true, blankColor, blankColor);
            buttonStyle.hover.background = CreateTexture(20, 20, 1, true, buttonHoverColor, buttonHoverBorderColor);
            buttonStyle.active.background = CreateTexture(20, 20, 1, true, buttonActiveColor, buttonActiveBorderColor);
            
            return buttonStyle;
        }
        
        public static GUIStyle Style_ImageButton(Texture2D normalTexture, int outline = 0)
        {
            Color32 hover = new Color32(223, 223, 223, 255);
            Color32 active = new Color32(079, 128, 179, 255);

            if (0 < outline)
            {
                return Style_ImageButton(normalTexture, active, outline);
            }
            else
            {
                return Style_ImageButton(normalTexture, hover, active);
            }
        }
        
        public static GUIStyle Style_ImageButton(Texture2D normalTexture, Color activeTint, int thickness)
        {
            GUIStyle buttonStyle = new GUIStyle();
            Texture2D hoverTexture = AddBorderToTexture(normalTexture, activeTint, thickness);
            Texture2D activeTexture = TintTexture(normalTexture, activeTint);
            
            buttonStyle.normal.background = normalTexture;
            buttonStyle.hover.background = hoverTexture;
            buttonStyle.active.background = activeTexture;
            
            return buttonStyle;
        }
        
        public static GUIStyle Style_ImageButton(Texture2D normalTexture, Color hoverTint, Color activeTint)
        {
            GUIStyle buttonStyle = new GUIStyle();
            Texture2D hoverTexture = TintTexture(normalTexture, hoverTint);
            Texture2D activeTexture = TintTexture(normalTexture, activeTint);
            
            buttonStyle.normal.background = normalTexture;
            buttonStyle.hover.background = hoverTexture;
            buttonStyle.active.background = activeTexture;
            
            return buttonStyle;
        }
        
        public static GUIStyle Style_StealthButton(TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            GUIStyle buttonStyle = new GUIStyle();
            buttonStyle.alignment = alignment;
            buttonStyle.fontSize = 10;
            buttonStyle.normal.textColor = Color.grey;
            
            return buttonStyle;
        }
        
        private static GUIStyle Style_DropdownButton()
        {
            GUIStyle dropdownStyle = new GUIStyle();
            dropdownStyle.fontSize = 12;
            dropdownStyle.fontStyle = FontStyle.Bold;
            dropdownStyle.normal.textColor = textNormalColor;
            dropdownStyle.hover.textColor = textNormalColor;
            dropdownStyle.active.textColor = textNormalColor;

            return dropdownStyle;
        }

        #endregion
        
    } // class end
}
#endif
