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
        private static string iconReset = "UnLinked";
        private static string iconScale;
        private static string iconLinked = "Linked";
        private static string iconUnlinked = "UnLinked";
        private GUIStyle iconButtonStyle = new GUIStyle();
        private GUIStyle buttonStyle2 = new GUIStyle();
        
        private void OnEnable()
        {
            iconScale = iconLinked;
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
            EditorGUILayout.PrefixLabel(new GUIContent("Position", "The local position of this gameObject, relative to it's parent gameObject"));
            EditorGUILayout.PropertyField(m_LocalPosition, GUIContent.none);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local position to Vector3.zero"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_LocalPosition.vector3Value = Vector3.zero;
            }
            GUILayout.EndHorizontal();
            
            // rotation
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Rotation", "The local rotation of this gameObject, relative to it's parent gameObject"));
            EditorGUILayout.PropertyField(m_LocalRotation, GUIContent.none);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local rotation to Vector3.zero"), iconButtonStyle,  GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_LocalRotation.quaternionValue = new Quaternion();
            }
            GUILayout.EndHorizontal();
            
            // scale
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = defaultLabelWidth - 23;
            EditorGUILayout.PrefixLabel(new GUIContent("Scale", "The local scaling of this gameObject, relative to it's parent gameObject"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            if (GUILayout.Button(EditorGUIUtility.IconContent(iconScale, "Enable constrained proportions"), iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                uniformScale = !uniformScale;
            }
            if(uniformScale) { GUI.backgroundColor = new Color32(000, 179, 223, 255); }
            else { GUI.backgroundColor = defaultBackground; }
            ScaleGUI(transformTarget);
            GUI.backgroundColor = defaultBackground;
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reset local scale to Vector3.one"), iconButtonStyle,GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_LocalScale.vector3Value = Vector3.one;
            }
            GUILayout.EndHorizontal();
            
            // utilities
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Transform Utilities", "View global properties"), buttonStyle2, GUILayout.Width(100), GUILayout.Height(buttonStyle2.fontSize)))
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
            // button style 1
            iconButtonStyle.fontSize = 9;
            iconButtonStyle.alignment = TextAnchor.MiddleCenter;
            iconButtonStyle.normal.textColor = InspectorUtility.textNormalColor;
            iconButtonStyle.hover.textColor = InspectorUtility.textNormalColor;
            iconButtonStyle.active.textColor = InspectorUtility.textNormalColor;
            iconButtonStyle.normal.background = InspectorUtility.CreateTexture(20, 20, 1, true, InspectorUtility.blankColor, InspectorUtility.blankColor);
            iconButtonStyle.hover.background = InspectorUtility.CreateTexture(20, 20, 1, true, InspectorUtility.buttonHoverColor, InspectorUtility.buttonHoverBorderColor);
            iconButtonStyle.active.background = InspectorUtility.CreateTexture(20, 20, 1, true, InspectorUtility.buttonActiveColor, InspectorUtility.buttonActiveBorderColor);
            
            // button style 2
            buttonStyle2.fontSize = 10;
            buttonStyle2.normal.textColor = InspectorUtility.textDisabledColor;
        }
        
        private void ScaleGUI(Transform transformTarget)
        {
            Undo.RecordObject(transformTarget, "transform scale updated");
            if (uniformScale)
            {
                iconScale = iconLinked;
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
                        newScale.y = RoundFloat(originalScale.y * differenceX, 2);
                        newScale.z = RoundFloat(originalScale.z * differenceX, 2);
                    }
                    else if (differenceY != 1)
                    {
                        newScale.x = RoundFloat(originalScale.x * differenceY, 2);
                        newScale.z = RoundFloat(originalScale.z * differenceY, 2);
                    }
                    else if (differenceZ != 1)
                    {
                        newScale.x = RoundFloat(originalScale.x * differenceZ, 2);
                        newScale.y = RoundFloat(originalScale.y * differenceZ, 2);
                    }
                    
                    // set scale
                    m_LocalScale.vector3Value = newScale;
                }
            }
            else
            {
                iconScale = iconUnlinked;
                EditorGUILayout.PropertyField(m_LocalScale, GUIContent.none);
            }
        }

        private float RoundFloat(float value, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10f, decimalPlaces);
            float roundedValue = Mathf.Round(value * multiplier) / multiplier;

            return roundedValue;
        }

        #endregion

    } // class end
}
