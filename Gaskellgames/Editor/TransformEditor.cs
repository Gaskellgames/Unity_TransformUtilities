using UnityEngine;
using UnityEditor;

namespace Gaskellgames
{
    [CustomEditor(typeof(Transform))] [CanEditMultipleObjects]
    public class TransformEditor : Editor
    {
        #region Variables

        private static bool uniformScale;
        private static bool open;
        private static string label;
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            Transform transformTarget = (Transform)target;
            
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
            ScaleGUI(transformTarget);
            if (GUILayout.Button(new GUIContent("\u21BA", "Reset Scale to Vector3.one"), GUILayout.Width(20), GUILayout.Height(20)))
            {
                transformTarget.localScale = Vector3.one;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Color defaultColor = GUI.backgroundColor;
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 10;
            myStyle.normal.textColor = Color.grey;
            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.25f);
            if(open) { label = "\u25cb Transform Utilities \u25cb "; } else { label = "\u25cf Transform Utilities \u25cf"; }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(label, "Reset Scale to Vector3.one"), myStyle, GUILayout.Width(100), GUILayout.Height(myStyle.fontSize)))
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
                GUI.backgroundColor = new Color32(179, 179, 179, 128);
                if (GUILayout.Button(new GUIContent("Lock Scale Aspect Ratio", "Set scale uniformly"), GUILayout.Width(150), GUILayout.Height(20)))
                {
                    uniformScale = !uniformScale;
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUI.enabled = false;
                EditorGUILayout.Space();
                ScaleGUI(transformTarget, true);
                EditorGUILayout.Space();
                EditorGUILayout.Vector3Field("Global Position", transformTarget.position);
                EditorGUILayout.Vector3Field("Global Rotation", transformTarget.eulerAngles);
                EditorGUILayout.Vector3Field("Lossy Scale", transformTarget.lossyScale);
                GUI.enabled = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private void ScaleGUI(Transform transformTarget, bool invert = false)
        {
            bool value = uniformScale;
            if (invert) { value = !value; }
            
            if (!value)
            {
                transformTarget.localScale = EditorGUILayout.Vector3Field("Scale", transformTarget.localScale);
            }
            else
            {
                Vector3 originalScale = transformTarget.localScale;
                Vector3 newScale = originalScale;
                EditorGUI.BeginChangeCheck();
                newScale.x = EditorGUILayout.FloatField("Scale Ratio", newScale.x);
                if (EditorGUI.EndChangeCheck())
                {
                    float difference = newScale.x / originalScale.x;
                    newScale.y = MathUtility.RoundFloat(originalScale.y * difference, 2);
                    newScale.z = MathUtility.RoundFloat(originalScale.z * difference, 2);
                }
                transformTarget.localScale = newScale;
            }
        }

        #endregion

    } // class end
}