using UnityEngine;
using UnityEditor;

/// <summary>
/// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
/// </summary>

namespace Gaskellgames
{
    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class TransformEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty m_LocalPosition;
        SerializedProperty m_LocalRotation;
        SerializedProperty m_LocalScale;
        //SerializedProperty m_ConstrainProportionsScale;
        
        private static bool uniformScale;
        private static bool open;
        private static string label;
        private static string icon = "\u2E26\\\u2E27";
        private GUIStyle buttonStyle1 = new GUIStyle();
        private GUIStyle buttonStyle2 = new GUIStyle();
        private Color32 textColor = new Color32(179, 179, 179, 255);
        private Color32 blankColor = new Color32(000, 000, 000, 000);
        
        private void OnEnable()
        {
            m_LocalPosition = serializedObject.FindProperty("m_LocalPosition");
            m_LocalRotation = serializedObject.FindProperty("m_LocalRotation");
            m_LocalScale = serializedObject.FindProperty("m_LocalScale");
            //m_ConstrainProportionsScale = serializedObject.FindProperty("m_ConstrainProportionsScale");
        }
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI
        
        public override void OnInspectorGUI()
        {
            // get & update references
            Transform transformTarget = (Transform)target;
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            Color defaultBackground = GUI.backgroundColor;
            CreateButtons();
            serializedObject.Update();
            
            // position
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Position", "Local Position"));
            EditorGUILayout.PropertyField(m_LocalPosition, GUIContent.none);
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Position to Vector3.zero"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_LocalPosition.vector3Value = Vector3.zero;
            }
            GUILayout.EndHorizontal();
            
            // rotation
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Rotation", "Local Rotation"));
            EditorGUILayout.PropertyField(m_LocalRotation, GUIContent.none);
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Rotation to Vector3.zero"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_LocalRotation.quaternionValue = new Quaternion();
            }
            GUILayout.EndHorizontal();
            
            // scale
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = defaultLabelWidth - 28;
            EditorGUILayout.PrefixLabel(new GUIContent("Scale", "Local Scale"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            if (GUILayout.Button(new GUIContent(icon, "Enable constrained proportions:\n⸦⸧ True, ⸦/⸧ False"), buttonStyle1, GUILayout.Width(25), GUILayout.Height(20)))
            {
                uniformScale = !uniformScale;
            }
            if(uniformScale) { GUI.backgroundColor = new Color32(255, 179, 000, 255); }
            else { GUI.backgroundColor = defaultBackground; }
            ScaleGUI(transformTarget);
            GUI.backgroundColor = defaultBackground;
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Scale to Vector3.one"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_LocalScale.vector3Value = Vector3.one;
            }
            GUILayout.EndHorizontal();
            
            // utilities
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            if(open) { label = "\u25cb Transform Utilities \u25cb "; } else { label = "\u25cf Transform Utilities \u25cf"; }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(label, "View global properties"), buttonStyle2, GUILayout.Width(100), GUILayout.Height(buttonStyle2.fontSize)))
            {
                open = !open;
            }
            GUI.backgroundColor = defaultBackground;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (open)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUI.enabled = false;
                EditorGUILayout.Space();
                EditorGUILayout.Vector3Field("Global Position", transformTarget.position);
                EditorGUILayout.Vector3Field("Global Rotation", transformTarget.eulerAngles);
                EditorGUILayout.Vector3Field("Lossy Scale", transformTarget.lossyScale);
                GUI.enabled = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            
            // force update window (to have snappy hover on buttons)
            Repaint();
            
            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private void CreateButtons()
        {
            Color32 hoverColor = new Color32(099, 099, 099, 255);
            Color32 hoverBorderColor = new Color32(028, 028, 028, 255);
            Color32 activeColor = new Color32(000, 128, 223, 255);
            Color32 activeBorderColor = new Color32(010, 010, 010, 255);
        
            // button 1
            buttonStyle1.fontSize = 9;
            buttonStyle1.alignment = TextAnchor.MiddleCenter;
            buttonStyle1.normal.textColor = textColor;
            buttonStyle1.hover.textColor = textColor;
            buttonStyle1.active.textColor = textColor;
            buttonStyle1.normal.background = CreateTexture(20, 20, 1, true, blankColor, blankColor);
            buttonStyle1.hover.background = CreateTexture(20, 20, 1, true, hoverColor, hoverBorderColor);
            buttonStyle1.active.background = CreateTexture(20, 20, 1, true, activeColor, activeBorderColor);
            
            // button 2
            buttonStyle2.fontSize = 10;
            buttonStyle2.normal.textColor = Color.grey;
        }
        
        private Texture2D CreateTexture(int width, int height, int border, bool isRounded, Color32 backgroundColor, Color32 borderColor)
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
        
        private void ScaleGUI(Transform transformTarget)
        {
            Undo.RecordObject(transformTarget, "transform scale updated");
            if (uniformScale)
            {
                icon = "\u2E26\u2E27";
                Vector3 originalScale = transformTarget.localScale;
                Vector3 newScale = originalScale;
                
                EditorGUI.BeginChangeCheck();
                transformTarget.localScale = EditorGUILayout.Vector3Field("", transformTarget.localScale);
                if (EditorGUI.EndChangeCheck())
                {
                    newScale.x = transformTarget.localScale.x;
                    float differenceX = newScale.x / originalScale.x;
                    
                    newScale.y = transformTarget.localScale.y;
                    float differenceY = newScale.y / originalScale.y;
                    
                    newScale.z = transformTarget.localScale.z;
                    float differenceZ = newScale.z / originalScale.z;

                    if (differenceX != 1)
                    {
                        newScale.y = MathUtility.RoundFloat(originalScale.y * differenceX, 2);
                        newScale.z = MathUtility.RoundFloat(originalScale.z * differenceX, 2);
                    }
                    else if (differenceY != 1)
                    {
                        newScale.x = MathUtility.RoundFloat(originalScale.x * differenceY, 2);
                        newScale.z = MathUtility.RoundFloat(originalScale.z * differenceY, 2);
                    }
                    else if (differenceZ != 1)
                    {
                        newScale.x = MathUtility.RoundFloat(originalScale.x * differenceZ, 2);
                        newScale.y = MathUtility.RoundFloat(originalScale.y * differenceZ, 2);
                    }
                    
                    // set scale
                    m_LocalScale.vector3Value = newScale;
                }
            }
            else
            {
                icon = "\u2E26\\\u2E27";
                EditorGUILayout.PropertyField(m_LocalScale, GUIContent.none);
            }
        }

        #endregion

    } // class end
}
