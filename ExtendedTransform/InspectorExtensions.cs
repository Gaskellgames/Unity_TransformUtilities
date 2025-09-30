using UnityEngine;

namespace Gaskellgames
{
    /// <remarks>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
    /// </remarks>
    
    public static class InspectorExtensions
    {
        #region Static Variables

        // blank
        public static readonly Color32 blankColor = new Color32(000, 000, 000, 000);
        
        // background
        public static readonly Color32 backgroundNormalColor = new Color32(051, 051, 051, 255);
        public static readonly Color32 backgroundSeperatorColor = new Color32(079, 079, 079, 255);
        
        // button - normal, hover, active
        public static readonly Color32 buttonHoverColor = new Color32(099, 099, 099, 255);
        public static readonly Color32 buttonActiveColor = new Color32(000, 128, 223, 255);
        public static readonly Color32 buttonActiveBorderColor = new Color32(010, 010, 010, 255);
        
        // text
        public static readonly Color32 textNormalColor = new Color32(179, 179, 179, 255);
        public static readonly Color32 textDisabledColor = new Color32(113, 113, 113, 255);

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Helper Functions
        
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
        
    } // class end
}
