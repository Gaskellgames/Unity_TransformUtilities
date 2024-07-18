#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
/// </summary>

namespace Gaskellgames.EditorHelper
{
    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class TransformEditor : Editor
    {
        #region Serialized Properties & Variables
        
        private Editor transformEditor;
        List<Transform> transformTargets = new List<Transform>();
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
            AssignTransformEditor();
        }

        private void OnDisable()
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
                repaintPositions = new Rect[4];
                CreateButtons();
                
                // draw inspector
                AssignSelectedTargets();
                OnInspectorGUI_WrappedTransform(transformTargets);
                OnInspectorGUI_TransformUtilities(transformTargets);
            
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

        private void OnInspectorGUI_WrappedTransform(List<Transform> transformTargets)
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
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localPosition == Vector3.zero) { continue;}
                    transform.localPosition = Vector3.zero;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local position reset for '{names}'", this);
                }
            }
            repaintPositions[0] = GUILayoutUtility.GetLastRect();
            GUILayout.Space(1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local rotation to Vector3.zero"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localEulerAngles == Vector3.zero) { continue;}
                    transform.localEulerAngles = Vector3.zero;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local position reset for '{names}'", this);
                }
            }
            repaintPositions[1] = GUILayoutUtility.GetLastRect();
            GUILayout.Space(1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local scale to Vector3.one"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localScale == Vector3.one) { continue;}
                    transform.localScale = Vector3.one;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local position reset for '{names}'", this);
                }
            }
            repaintPositions[2] = GUILayoutUtility.GetLastRect();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void OnInspectorGUI_TransformUtilities(List<Transform> transformTargets)
        {
            // get & update references
            Color defaultBackground = GUI.backgroundColor;
            
            // scale warning: top
            EditorGUILayout.Space();
            DrawScaleWarning(transformTargets, 1, -2);
            
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
                if (1 == transformTargets.Count && NonUniformScaleInObject(transformTargets[0]) || NonUniformScaleInParent(transformTargets))
                {
                    // non-uniform scale
                    GUI.enabled = true;
                    EditorGUILayout.HelpBox("Non-uniform scale detected. It is recommended to keep scale at '1, 1, 1' where possible.", MessageType.Warning);
                    GUI.enabled = false;
                }
                GUILayout.Space(2);
                if (1 == targets.Length)
                {
                    EditorGUILayout.Vector3Field("Global Position", transformTargets[0].position);
                    EditorGUILayout.Vector3Field("Global Rotation", transformTargets[0].eulerAngles);
                    EditorGUILayout.Vector3Field("Lossy Scale", transformTargets[0].lossyScale);
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Global properties only available when a single object is selected.");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(2);
                GUI.enabled = true;
            }
            GUILayout.Space(2);
            EndCustomInspectorBackground();
            
            // scale warning: bottom
            DrawScaleWarning(transformTargets, 2, -3);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Private Functions

        private void AssignTransformEditor()
        {
            DestroyImmediate(transformEditor);
            
            Transform thisTarget = (Transform)target;
            Type type = typeof(EditorApplication).Assembly.GetType("UnityEditor.TransformInspector");
            transformEditor = CreateEditorWithContext(targets, thisTarget, type);
        }

        private void AssignSelectedTargets()
        {
            transformTargets.Clear();
            foreach (var targetObject in targets)
            {
                Transform transform = (Transform)targetObject;
                if (!transformTargets.Contains(transform))
                {
                    transformTargets.Add(transform);
                }
            }
        }
        
        private void DrawScaleWarning(List<Transform> transformTargets, float paddingTop = -4F, float paddingBottom = -15F, float paddingLeft = -18F, float paddingRight = -4F)
        {
            if (1 == transformTargets.Count && NonUniformScaleInObject(transformTargets[0]))
            {
                BeginCustomInspectorBackground(new Color32(223, 050, 050, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EndCustomInspectorBackground();
            }
            else if (NonUniformScaleInParent(transformTargets))
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

        private bool NonUniformScaleInParent(List<Transform> transformTargets)
        {
            foreach (var transformTarget in transformTargets)
            {
                if (transformTarget.lossyScale != Vector3.one) { return true; }
            }
            
            return false;
        }

        private bool NonUniformScaleInObject(Transform transformTarget)
        {
            return transformTarget.localScale != Vector3.one;
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

#endif
