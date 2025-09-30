#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
    /// </summary>

    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class TransformEditor : Editor
    {
        #region Serialized Properties & Variables
        
        private Type transformInspectorType;
        private Editor transformEditor;
        private List<Transform> transformTargets = new List<Transform>();
        private Texture iconTexture;
        private GUIStyle stealthButton = new GUIStyle();
        
        private readonly bool enableTransformInspector = true;
        private static bool utilitiesOpen;
        
        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region OnEnable / OnDisable

        private void OnEnable()
        {
            // transform wrapper
            transformInspectorType = typeof(EditorApplication).Assembly.GetType("UnityEditor.TransformInspector");
            AssignTransformEditor();
        }

        private void OnDisable()
        {
            if (transformEditor == null) { return; }
            DestroyImmediate(transformEditor);
            transformEditor = null;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // null check
            if (!transformEditor)
            {
                base.OnInspectorGUI();
                return;
            }
            
            serializedObject.Update();
            
            GUI.changed = false;
            if (!enableTransformInspector)
            {
                transformEditor.OnInspectorGUI();
            }
            else
            {
                CreateButtons();
            
                // draw inspector
                OnInspectorGUI_WrappedTransform();
                OnInspectorGUI_TransformUtilities();
            }
            
            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUI_WrappedTransform()
        {
            GUILayout.BeginHorizontal();
            
            // default transform
            GUILayout.BeginVertical();
            transformEditor.OnInspectorGUI();
            GUILayout.EndVertical();
            
            // reset buttons
            GUILayout.BeginVertical(GUILayout.Width(20));
            iconTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(iconTexture, EditorStyles.iconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localPosition == Vector3.zero) { continue;}
                    
                    Undo.RecordObject(transform, $"Local position reset for {transform.gameObject.name}");
                    transform.localPosition = Vector3.zero;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local position reset for '{names}'", this);
                }
            }
            GUILayout.Space(1);
            iconTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(iconTexture, EditorStyles.iconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localEulerAngles == Vector3.zero) { continue;}
                    Undo.RecordObject(transform, $"Local rotation reset for {transform.gameObject.name}");
                    transform.localEulerAngles = Vector3.zero;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local rotation reset for '{names}'", this);
                }
            }
            GUILayout.Space(1);
            iconTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(iconTexture, EditorStyles.iconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localScale == Vector3.one) { continue;}
                    Undo.RecordObject(transform, $"Local scale reset for {transform.gameObject.name}");
                    transform.localScale = Vector3.one;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local scale reset for '{names}'", this);
                }
            }
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void OnInspectorGUI_TransformUtilities()
        {
            // get & update references
            Color defaultBackground = GUI.backgroundColor;
            
            // scale warning: top
            EditorGUILayout.Space();
            DrawScaleWarning(transformTargets, 1, -2);
            
            // utilities
            EditorExtensions.BeginCustomInspectorBackground(InspectorExtensions.backgroundNormalColor, 1, -3);
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            if (GUILayout.Button(new GUIContent("Transform Utilities", "View global properties"), stealthButton, GUILayout.Width(100), GUILayout.Height(stealthButton.fontSize)))
            {
                utilitiesOpen = !utilitiesOpen;
            }
            GUI.backgroundColor = defaultBackground;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            if (utilitiesOpen)
            {
                GUILayout.Space(-1);
                EditorExtensions.DrawInspectorLineFull(InspectorExtensions.backgroundSeperatorColor, 2, 2);
                GUILayout.Space(2);
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
                OnInspectorGUI_GlobalPositions();
                GUI.enabled = true;
            }
            GUILayout.Space(2);
            EditorExtensions.EndCustomInspectorBackground();
            
            // scale warning: bottom
            DrawScaleWarning(transformTargets, 2, -3);
        }
        
        private void OnInspectorGUI_GlobalPositions()
        {
            if (1 == targets.Length)
            {
                // global transform properties
                EditorGUILayout.Vector3Field("Global Position", transformTargets[0].position);
                EditorGUILayout.Vector3Field("Global Rotation", transformTargets[0].eulerAngles);
                EditorGUILayout.Vector3Field("Lossy Scale", transformTargets[0].lossyScale);
            }
            else
            {
                EditorGUILayout.LabelField("Global properties not available when multiple objects are selected.");
            }
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Private Functions

        private void AssignTransformEditor()
        {
            // assign selected targets
            transformTargets.Clear();
            foreach (var targetObject in targets)
            {
                Transform transform = targetObject as Transform;
                if (transform && !transformTargets.Contains(transform))
                {
                    transformTargets.Add(transform);
                }
            }
            
            // assign transform editor
            if (target)
            {
                transformEditor = 1 < transformTargets.Count
                    ? CreateEditorWithContext(targets, target, transformInspectorType)
                    : CreateEditor(target, transformInspectorType);
            }
        }
        
        private void DrawScaleWarning(List<Transform> transformTargets, float paddingTop = -4F, float paddingBottom = -15F, float paddingLeft = -18F, float paddingRight = -4F)
        {
            if (1 == transformTargets.Count && NonUniformScaleInObject(transformTargets[0]))
            {
                EditorExtensions.BeginCustomInspectorBackground(new Color32(223, 050, 050, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EditorExtensions.EndCustomInspectorBackground();
            }
            else if (NonUniformScaleInParent(transformTargets))
            {
                EditorExtensions.BeginCustomInspectorBackground(new Color32(179, 128, 000, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EditorExtensions.EndCustomInspectorBackground();
            }
            else
            {
                EditorExtensions.BeginCustomInspectorBackground(new Color32(028, 128, 028, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EditorExtensions.EndCustomInspectorBackground();
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
            // stealth button
            stealthButton.fontSize = 10;
            stealthButton.normal.textColor = InspectorExtensions.textDisabledColor;
        }

        #endregion

    } // class end
}
#endif
