using UnityEngine;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// <para>Screenshot manager class. </para>
    /// <para>Take some awesome screenies with this!</para>
    /// </summary>
    public static class ScreenshotManager
    {
        /// <summary>
        /// This returns the raw pixels byte array of a screenshot taken from a specific <a href="https://docs.unity3d.com/ScriptReference/Camera.html">Camera</a>.<para> </para>
        ///
        /// You can then use the <c>Texture2D.Load(byte[] image)</c> extension method to load the byte array into a usable texture image.
        /// </summary>
        /// <param name="screenshotCamera">The camera through which the screenshot shall be taken.</param>
        /// <param name="screenshotWidth">The width of the screenshot (in pixels).</param>
        /// <param name="screenshotHeight">The height of the screenshot (in pixels).</param>
        /// <returns>The resulting <c>byte[]</c> array of the screenshot image.</returns>
        public static byte[] GetScreenshotFromCamera(Camera screenshotCamera, int screenshotWidth = 800, int screenshotHeight = 450)
        {
            // Initialize the screenshot texture and render texture.
            var screenshotTexture = new Texture2D(Mathf.Abs(screenshotWidth), Mathf.Abs(screenshotHeight), TextureFormat.RGB24, false);
            var renderTexture = new RenderTexture(screenshotTexture.width, screenshotTexture.height, 24);

            // Create the screenshot rectangle.
            var screenshotRect = new Rect(0.0f, 0.0f, screenshotTexture.width, screenshotTexture.height);

            // Keep track of the old render texture references.
            RenderTexture oldCameraRenderTexture = screenshotCamera.targetTexture;
            RenderTexture oldActiveRenderTexture = RenderTexture.active;

            // Take a screenshot from the specified camera.
            screenshotCamera.targetTexture = renderTexture;
            screenshotCamera.Render();
            RenderTexture.active = renderTexture;
            screenshotTexture.ReadPixels(screenshotRect, 0, 0);
            screenshotTexture.Apply();

            // Release the resources.
            screenshotCamera.targetTexture = oldCameraRenderTexture;
            RenderTexture.active = oldActiveRenderTexture;
            renderTexture.Release();

            // Return the JPG-encoded texture's raw byte[] array.
            return screenshotTexture.EncodeToJPG();
        }
    }
}

// Copyright (C) Raphael Beck, 2017 | https://glitchedpolygons.com