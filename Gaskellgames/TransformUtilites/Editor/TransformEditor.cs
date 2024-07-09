using System;
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
        #region Serialized Properties & Variables
        
        private Editor transformEditor;
        private Rect[] repaintPositions = new Rect[4];
        private GUIStyle iconButtonStyle = new GUIStyle();
        private GUIStyle buttonStyle2 = new GUIStyle();
        
        private static bool utilitiesOpen;
        private static bool useDefaultInspector = false;

        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region OnEnable / OnDisable

        private void OnEnable()
        {
            // transform wrapper
            Transform transform = target as Transform;
            Type t = typeof(EditorApplication).Assembly.GetType("UnityEditor.TransformInspector");
            transformEditor = CreateEditor(transform, t);
        }

        void OnDisable()
        {
            DestroyImmediate(transformEditor);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            if (useDefaultInspector)
            {
                transformEditor.OnInspectorGUI();
            }
            else
            {
                serializedObject.Update();
                Transform transformTarget = (Transform)target;
                repaintPositions = new Rect[4];
                CreateButtons();
            
                // draw inspector
                OnInspectorGUI_WrappedTransform(transformTarget);
                OnInspectorGUI_TransformUtilities(transformTarget);
            
                // force update window (to have snappy hover on buttons) if mouse over buttons
                foreach (var repaintPosition in repaintPositions)
                {
                    if (repaintPosition.Contains(Event.current.mousePosition))
                    {
                        Repaint();
                    }
                }
            
                // apply reference changes
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnInspectorGUI_WrappedTransform(Transform transformTarget)
        {
            GUILayout.BeginHorizontal();
            
            // default transform
            GUILayout.BeginVertical();
            transformEditor.OnInspectorGUI();
            GUILayout.EndVertical();
            
            // reset buttons
            GUILayout.BeginVertical(GUILayout.Width(20));
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local position to Vector3.zero"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localPosition = Vector3.zero;
                Debug.Log($"Local position reset for '{serializedObject.targetObject.name}'", this);
            }
            repaintPositions[0] = GUILayoutUtility.GetLastRect();
            GUILayout.Space(1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local rotation to Vector3.zero"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localEulerAngles = Vector3.zero;
                Debug.Log($"Local Rotation reset for '{serializedObject.targetObject.name}'", this);
            }
            repaintPositions[1] = GUILayoutUtility.GetLastRect();
            GUILayout.Space(1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local scale to Vector3.one"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localScale = Vector3.one;
                Debug.Log($"Local Scale reset for '{serializedObject.targetObject.name}'", this);
            }
            repaintPositions[2] = GUILayoutUtility.GetLastRect();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void OnInspectorGUI_TransformUtilities(Transform transformTarget)
        {
            // get & update references
            Color defaultBackground = GUI.backgroundColor;
            
            // scale warning: top
            EditorGUILayout.Space();
            DrawScaleWarning(transformTarget, 1, -2);
            
            // utilities
            BeginCustomInspectorBackground(InspectorExtensions.backgroundNormalColor, 1, -3);
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            if (GUILayout.Button(new GUIContent("Transform Utilities", "View global properties"), buttonStyle2, GUILayout.Width(100), GUILayout.Height(buttonStyle2.fontSize)))
            {
                utilitiesOpen = !utilitiesOpen;
            }
            repaintPositions[3] = GUILayoutUtility.GetLastRect();
            GUI.backgroundColor = defaultBackground;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            if (utilitiesOpen)
            {
                GUI.enabled = false;
                GUILayout.Space(2);
                EditorGUILayout.Vector3Field("Global Position", transformTarget.position);
                EditorGUILayout.Vector3Field("Global Rotation", transformTarget.eulerAngles);
                EditorGUILayout.Vector3Field("Lossy Scale", transformTarget.lossyScale);
                GUILayout.Space(2);
                if (transformTarget.localScale != Vector3.one || transformTarget.lossyScale != Vector3.one)
                {
                    // non-uniform scale
                    GUI.enabled = true;
                    EditorGUILayout.HelpBox("Non-uniform scale detected. It is recommended to keep scale at '1, 1, 1' where possible.", MessageType.Warning);
                    GUI.enabled = false;
                }
                GUILayout.Space(2);
                GUI.enabled = true;
            }
            GUILayout.Space(2);
            EndCustomInspectorBackground();
            
            // scale warning: bottom
            DrawScaleWarning(transformTarget, 2, -3);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Private Functions

        private void DrawScaleWarning(Transform transformTarget, float paddingTop = -4F, float paddingBottom = -15F, float paddingLeft = -18F, float paddingRight = -4F)
        {
            if (transformTarget.localScale != Vector3.one)
            {
                BeginCustomInspectorBackground(new Color32(223, 050, 050, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EndCustomInspectorBackground();
            }
            else if (transformTarget.lossyScale != Vector3.one)
            {
                BeginCustomInspectorBackground(new Color32(179, 128, 000, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EndCustomInspectorBackground();
            }
            else
            {
                BeginCustomInspectorBackground(new Color32(028, 128, 028, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EndCustomInspectorBackground();
            }
        }
        
        private void CreateButtons()
        {
            // button style 1 (reset button)
            iconButtonStyle.fontSize = 9;
            iconButtonStyle.alignment = TextAnchor.MiddleCenter;
            iconButtonStyle.normal.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.hover.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.active.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.normal.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.blankColor, InspectorExtensions.blankColor);
            iconButtonStyle.hover.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.buttonHoverColor, InspectorExtensions.blankColor);
            iconButtonStyle.active.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.buttonActiveColor, InspectorExtensions.buttonActiveBorderColor);

            // button style 2 (utilities)
            buttonStyle2.fontSize = 10;
            buttonStyle2.normal.textColor = InspectorExtensions.textDisabledColor;
        }

        private void BeginCustomInspectorBackground(Color32 backgroundColor, float paddingTop = -4, float paddingBottom = -15, float paddingLeft = -18, float paddingRight = -4)
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

        private void EndCustomInspectorBackground()
        {
            EditorGUILayout.EndVertical();
        }

        #endregion

    } // class end
}
