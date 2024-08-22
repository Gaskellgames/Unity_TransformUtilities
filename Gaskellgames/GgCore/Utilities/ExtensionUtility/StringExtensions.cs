using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public static class StringExtensions
    {
        /// <summary>
        /// Get the width and height of a string using 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static Vector2 GetStringBounds(string stringValue, int fontSize = 0)
        {
            GUIStyle guiStyle = GUI.skin.GetStyle("Box");
            guiStyle.fontSize = fontSize;
    
            return guiStyle.CalcSize(new GUIContent(stringValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static float GetStringWidth(string stringValue, int fontSize = 0)
        {
            return GetStringBounds(stringValue, fontSize).x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static float GetStringHeight(string stringValue, int fontSize = 0)
        {
            return GetStringBounds(stringValue, fontSize).y;
        }
        
        /// <summary>
        /// Return a three digit int as a 'nicified' string value for the number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>0-9 will be returned as 000-009, 10-99 will be returned as 010-099</returns>
        public static string NicifyNumberAsString(int value)
        {
            // negatives
            if (value < 0) { return value.ToString(); }

            // 000-009
            if (value < 10) { return $"00{value}"; }

            // 010-099
            if (value < 100) { return $"0{value}"; }

            // 100+
            return value.ToString();
        }

    } // class end
}
