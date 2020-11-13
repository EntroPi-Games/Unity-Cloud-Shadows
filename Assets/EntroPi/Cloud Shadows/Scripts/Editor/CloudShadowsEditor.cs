using UnityEditor;
using UnityEngine;

namespace EntroPi
{
    [CustomEditor(typeof(CloudShadows))]
    public class CloudShadowsEditor : Editor
    {
        private Texture2D m_Logo;
        private bool m_ShowGlobalMultipliers = true;
        private bool m_ShowAdvancedSettings = true;

        private void OnEnable()
        {
            string logoAssetPath = EditorUtil.GetEntroPiFolderPath(this) + EditorUtil.PATH_RELATIVE_LOGO;
            m_Logo = AssetDatabase.LoadAssetAtPath(logoAssetPath, typeof(Texture2D)) as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            GUILayout.Label(m_Logo);

            if (ContainsObsoleteCloudLayerComponents())
            {
                EditorGUILayout.HelpBox("There are obsolete Cloud Layer Script components attached to this GameObject.\nYou can safely remove these components.", MessageType.Warning);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_PREVIEW_IN_EDITOR));

            SerializedProperty projectModeProperty = serializedObject.FindProperty(CloudShadows.PATH_PROJECT_MODE);
            EditorGUILayout.PropertyField(projectModeProperty);

            EditorGUILayout.Space();

            SerializedProperty cloudLayersProperty = serializedObject.FindProperty(CloudShadows.PATH_CLOUD_LAYERS);
            EditorGUILayout.PropertyField(cloudLayersProperty);

            if (cloudLayersProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("No Cloud Layers asset assigned.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_WORLD_SIZE));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_RENDER_TEXTURE_RESOLUTION));

            EditorGUILayout.Space();

            m_ShowGlobalMultipliers = EditorGUILayout.Foldout(m_ShowGlobalMultipliers, "Global Mutators");

            if (m_ShowGlobalMultipliers)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_OPACITY_MULTIPLIER));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_COVERAGE_MODIFIER));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_SOFTNESS_MODIFIER));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_SPEED_MULTIPLIER));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_DIRECTION_MODIFIER));

                EditorGUI.indentLevel -= 1;
            }

            if (projectModeProperty.enumValueIndex == (int)CloudShadows.ProjectMode.Mode3D)
            {
                m_ShowAdvancedSettings = EditorGUILayout.Foldout(m_ShowAdvancedSettings, "Advanced 3D Settings");

                if (m_ShowAdvancedSettings)
                {
                    EditorGUI.indentLevel += 1;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_HORIZON_ANGLE_THRESHOLD));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(CloudShadows.PATH_HORIZON_ANGLE_FADE));

                    EditorGUI.indentLevel -= 1;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool ContainsObsoleteCloudLayerComponents()
        {
            MonoBehaviour targetScript = target as MonoBehaviour;

            CloudLayer cloudLayerScript = targetScript.GetComponent<CloudLayer>();

            bool containsObsoleteComponents = cloudLayerScript != null;

            return containsObsoleteComponents;
        }
    }
}