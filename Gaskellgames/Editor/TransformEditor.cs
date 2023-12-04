using UnityEngine;
using UnityEditor;

/// <summary>
/// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
/// </summary>

namespace Gaskellgames
{
    [CustomEditor(typeof(Transform))] [CanEditMultipleObjects]
    public class TransformEditor : Editor
    {
        #region Variables

        private static bool uniformScale;
        private static bool open;
        private static string label;
        private static string icon = "⸦/⸧";
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            Transform transformTarget = (Transform)target;
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            Color defaultColor = GUI.backgroundColor;
            GUIStyle myStyle = new GUIStyle();
            
            // position
            GUILayout.BeginHorizontal();
            transformTarget.localPosition = EditorGUILayout.Vector3Field("Position", transformTarget.localPosition);
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Position to Vector3.zero"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localPosition = Vector3.zero;
            }
            GUILayout.EndHorizontal();

            // rotation (eulerAngles)
            GUILayout.BeginHorizontal();
            transformTarget.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", transformTarget.localEulerAngles);
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Rotation to Vector3.zero"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localEulerAngles = Vector3.zero;
            }
            GUILayout.EndHorizontal();

            // scale
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = defaultLabelWidth - 35;
            EditorGUILayout.PrefixLabel("Scale");
            myStyle.normal.textColor = new Color32(179, 179, 179, 255);
            myStyle.alignment = TextAnchor.MiddleCenter;
            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.25f);
            if (GUILayout.Button(new GUIContent(icon, "Enable constrained proportions:\n⸦⸧ True, ⸦/⸧ False"), myStyle, GUILayout.Width(35), GUILayout.Height(20)))
            {
                uniformScale = !uniformScale;
            }
            GUI.backgroundColor = defaultColor;
            EditorGUIUtility.labelWidth = 0;
            ScaleGUI(transformTarget);
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Scale to Vector3.one"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localScale = Vector3.one;
            }
            GUILayout.EndHorizontal();

            // utilities
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            myStyle.fontSize = 10;
            myStyle.normal.textColor = Color.grey;
            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.25f);
            if(open) { label = "\u25cb Transform Utilities \u25cb "; } else { label = "\u25cf Transform Utilities \u25cf"; }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(label, "View global properties"), myStyle, GUILayout.Width(100), GUILayout.Height(myStyle.fontSize)))
            {
                open = !open;
            }
            GUI.backgroundColor = defaultColor;
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
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions
        
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
                    
                    transformTarget.localScale = newScale;
                }
            }
            else
            {
                icon = "\u2E26/\u2E27";
                transformTarget.localScale = EditorGUILayout.Vector3Field("", transformTarget.localScale);
            }
        }

        #endregion

    } // class end
}
