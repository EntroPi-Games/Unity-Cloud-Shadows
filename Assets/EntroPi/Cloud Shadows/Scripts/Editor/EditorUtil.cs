using UnityEditor;
using UnityEngine;

namespace EntroPi
{
    public static class EditorUtil
    {
        public const string PATH_RELATIVE_LOGO = "/Cloud Shadows/Editor/InternalResources/EntroPiLogo.png";
        public const string PATH_RELATIVE_BLEND_ARROW = "/Cloud Shadows/Editor/InternalResources/CloudLayerBlendArrow.png";

        public static string GetEntroPiFolderPath(ScriptableObject scriptableObject)
        {
            string scriptFilePath = GetScriptFilePath(scriptableObject);
            int index = scriptFilePath.IndexOf("/Cloud Shadows");

            return scriptFilePath.Substring(0, index);
        }

        private static string GetScriptFilePath(ScriptableObject scriptableObject)
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(scriptableObject);
            string scriptFilePath = AssetDatabase.GetAssetPath(monoScript);

            return scriptFilePath;
        }
    }
}