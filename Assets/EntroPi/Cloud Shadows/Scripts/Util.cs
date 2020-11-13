using UnityEngine;

namespace EntroPi
{
    /// <summary>
    /// Verify functions
    /// </summary>
    public class Debug
    {
        /// <summary>
        /// Verifies and returns a condition
        /// Raises an assert exception with message if condition is false
        /// </summary>
        public static bool Verify(bool condition, string message)
        {
            UnityEngine.Debug.Assert(condition, "Verify Failed: " + message);

            return condition;
        }

        /// <summary>
        /// Verifies and returns a condition
        /// Raises an assert exception if condition is false
        /// </summary>
        public static bool Verify(bool condition)
        {
            UnityEngine.Debug.Assert(condition, "Verify Failed!");

            return condition;
        }
    }

    /// <summary>
    /// Util class for creating and loading resources
    /// </summary>
    public class ResourceUtil
    {
        /// <summary>
        /// Creates and returns material from shader.
        /// </summary>
        public static Material CreateMaterial(string shaderResource)
        {
            Material material = null;

            Shader shader = LoadShader(shaderResource);

            if (Debug.Verify(CheckShader(shader)))
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.HideAndDontSave;
            }

            UnityEngine.Debug.Assert(material != null, "Failed to created material from shader: " + shaderResource);

            return material;
        }

        /// <summary>
        /// Creates and returns shader.
        /// </summary>
        public static Shader LoadShader(string shaderResource)
        {
            Shader shader = Resources.Load(shaderResource, typeof(Shader)) as Shader;

            UnityEngine.Debug.Assert(CheckShader(shader), "Shader not supported: " + shaderResource);

            return shader;
        }

        /// <summary>
        /// Checks if shader exists and is supported.
        /// </summary>
        public static bool CheckShader(Shader shader)
        {
            return (shader != null && shader.isSupported);
        }
    }

    /// <summary>
    /// Util class for managing Render Textures
    /// </summary>
    public class RenderTextureUtil
    {
        /// <summary>
        /// Creates and returns new render texture.
        /// </summary>
        public static RenderTexture CreateRenderTexture(int resolution, int depth = 0, TextureWrapMode wrapMode = TextureWrapMode.Repeat, FilterMode filterMode = FilterMode.Bilinear)
        {
            RenderTexture renderTexture = new RenderTexture(resolution, resolution, depth);
            renderTexture.wrapMode = wrapMode;
            renderTexture.filterMode = filterMode;

            return renderTexture;
        }

        /// <summary>
        /// Recreates and returns new render texture.
        /// </summary>
        public static void RecreateRenderTexture(ref RenderTexture renderTexture, int resolution)
        {
            renderTexture.Release();
            renderTexture = CreateRenderTexture(resolution);
        }

        /// <summary>
        /// Clears render texture.
        /// </summary>
        public static void ClearRenderTexture(RenderTexture renderTexture, Color clearColor)
        {
            // Store currently active render texture
            RenderTexture lastActive = RenderTexture.active;

            // Set render texture parameter active and clear
            RenderTexture.active = renderTexture;
            GL.Clear(false, true, clearColor);

            // Revert active render texture
            RenderTexture.active = lastActive;
        }

        /// <summary>
        /// Swaps the references of 2 render textures
        /// </summary>
        public static void SwapRenderTextures(ref RenderTexture renderTexture1, ref RenderTexture renderTexture2)
        {
            RenderTexture tempRenderTexture = renderTexture1;
            renderTexture1 = renderTexture2;
            renderTexture2 = tempRenderTexture;
        }
    }
}