using UnityEditor;
using UnityEngine;

namespace EntroPi
{
    [CustomEditor(typeof(CloudLayers))]
    public class CloudLayersEditor : Editor
    {
        private static GUIContent s_ExpandOn = new GUIContent("\u25BC", "Hide layer properties");
        private static GUIContent s_ExpandOff = new GUIContent("\u25B6", "Show layer properties");
        private static GUIContent s_VisibilityOn = new GUIContent("\u2714", "Toggle layer visibility");
        private static GUIContent s_VisibilityOff = new GUIContent("\u2718", "Toggle layer visibility");
        private static GUIContent s_DuplicateLayer = new GUIContent("\u274F", "Duplicate layer");
        private static GUIContent s_MoveLayerUp = new GUIContent("\u25B2", "Move layer up");
        private static GUIContent s_MoveLayerDown = new GUIContent("\u25BC", "Move layer down");
        private static GUIContent s_DeleteLayer = new GUIContent("\u2716", "Delete layer");
        private static GUILayoutOption s_MiniButtonWidth = GUILayout.Width(22f);

        private GUIContent m_AddLayer = new GUIContent("+", "Add Cloud Shadow layer");
        private GUIContent m_ConvertPrefabButton = new GUIContent("Convert Prefab", "Converts prefabs created with older versions of this asset into the new format.");
        private GameObject m_CloudLayerPrefab;
        private Texture2D m_Logo;
        private Texture2D m_BlendArrow;

        private void OnEnable()
        {
            string pathEntroPiFolder = EditorUtil.GetEntroPiFolderPath(this);
            m_Logo = AssetDatabase.LoadAssetAtPath(pathEntroPiFolder + EditorUtil.PATH_RELATIVE_LOGO, typeof(Texture2D)) as Texture2D;
            m_BlendArrow = AssetDatabase.LoadAssetAtPath(pathEntroPiFolder + EditorUtil.PATH_RELATIVE_BLEND_ARROW, typeof(Texture2D)) as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(m_Logo);

            serializedObject.Update();

            SerializedProperty layerList = serializedObject.FindProperty("m_LayerData");

            DrawCloudLayers(layerList, m_BlendArrow);
            DrawFooter(layerList);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawCloudLayers(SerializedProperty layerList, Texture2D blendArrow)
        {
            for (int i = 0; i < layerList.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                bool isListDirty = DrawCloudLayerHeader(layerList, i);
                if (isListDirty)
                {
                    break;
                }

                SerializedProperty cloudLayerProperty = layerList.GetArrayElementAtIndex(i);
                if (cloudLayerProperty.isExpanded)
                {
                    bool isVisible = cloudLayerProperty.FindPropertyRelative(CloudLayerData.PATH_IS_VISIBLE).boolValue;
                    EditorGUI.BeginDisabledGroup(!isVisible);

                    DrawCloudLayer(cloudLayerProperty);

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndVertical();

                if (cloudLayerProperty.isExpanded && i < layerList.arraySize - 1)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(blendArrow);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
        }

        private static bool DrawCloudLayerHeader(SerializedProperty layerList, int index)
        {
            bool isListDirty = false;
            SerializedProperty cloudLayerProperty = layerList.GetArrayElementAtIndex(index);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(cloudLayerProperty.isExpanded ? s_ExpandOn : s_ExpandOff, EditorStyles.miniButtonLeft, s_MiniButtonWidth))
            {
                cloudLayerProperty.isExpanded = !cloudLayerProperty.isExpanded;
            }

            SerializedProperty isVisible = cloudLayerProperty.FindPropertyRelative(CloudLayerData.PATH_IS_VISIBLE);
            if (GUILayout.Button(isVisible.boolValue ? s_VisibilityOn : s_VisibilityOff, EditorStyles.miniButtonRight, s_MiniButtonWidth))
            {
                isVisible.boolValue = !isVisible.boolValue;
            }

            EditorGUILayout.PropertyField(cloudLayerProperty.FindPropertyRelative(CloudLayerData.PATH_LAYER_NAME), GUIContent.none);

            if (GUILayout.Button(s_DuplicateLayer, EditorStyles.miniButtonMid, s_MiniButtonWidth))
            {
                layerList.InsertArrayElementAtIndex(index);
                isListDirty = true;
            }

            if (GUILayout.Button(s_MoveLayerUp, EditorStyles.miniButtonMid, s_MiniButtonWidth))
            {
                SwapCloudLayers(layerList, index, index - 1);
                isListDirty = true;
            }

            if (GUILayout.Button(s_MoveLayerDown, EditorStyles.miniButtonMid, s_MiniButtonWidth))
            {
                SwapCloudLayers(layerList, index, index + 1);
                isListDirty = true;
            }

            if (GUILayout.Button(s_DeleteLayer, EditorStyles.miniButtonRight, s_MiniButtonWidth))
            {
                int initialSize = layerList.arraySize;
                int lastIndex = initialSize - 1;
                // Swap index with last index
                if (index != lastIndex)
                {
                    SwapCloudLayers(layerList, index, lastIndex);
                }

                // Clear reference in list.
                layerList.DeleteArrayElementAtIndex(lastIndex);

                // Remove element from list.
                if (layerList.arraySize == initialSize)
                {
                    layerList.DeleteArrayElementAtIndex(lastIndex);
                }

                isListDirty = true;
            }

            EditorGUILayout.EndHorizontal();

            return isListDirty;
        }

        private static void DrawCloudLayer(SerializedProperty cloudLayerElement)
        {
            EditorGUILayout.Space();
            DrawBlendingOptions(cloudLayerElement);
            DrawCloudOptions(cloudLayerElement);
            DrawTextureOptions(cloudLayerElement);
        }

        private static void DrawBlendingOptions(SerializedProperty cloudLayerElement)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Blend Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_BLEND_MODE));
            EditorGUILayout.PropertyField(cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_OPACITY));
            EditorGUI.indentLevel -= 1;

            EditorGUILayout.EndVertical();
        }

        private static void DrawCloudOptions(SerializedProperty cloudLayerElement)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Cloud Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel += 1;

            EditorGUILayout.PropertyField(cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_COVERAGE));
            EditorGUILayout.PropertyField(cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_SOFTNESS));

            SerializedProperty speedProperty = cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_SPEED);
            EditorGUILayout.PropertyField(speedProperty);
            speedProperty.floatValue = Mathf.Max(speedProperty.floatValue, 0);

            EditorGUILayout.PropertyField(cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_DIRECTION));

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.EndVertical();
        }

        private static void DrawTextureOptions(SerializedProperty cloudLayerElement)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Texture Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel += 1;

            SerializedProperty textureProperty = cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_TEXTURE);
            EditorGUILayout.PropertyField(textureProperty);

            if (textureProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("No Texture assigned.", MessageType.Warning);
            }

            SerializedProperty tilingProperty = cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_TEXTURE_TILING);
            EditorGUILayout.PropertyField(tilingProperty);
            tilingProperty.vector2Value = CloudLayerData.LimitTextureTiling(tilingProperty.vector2Value);

            EditorGUILayout.PropertyField(cloudLayerElement.FindPropertyRelative(CloudLayerData.PATH_TEXTURE_OFFSET));

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.EndVertical();
        }

        private void DrawFooter(SerializedProperty layerList)
        {
            if (GUILayout.Button(m_AddLayer))
            {
                SerializedProperty element = AddCloudLayer(layerList);
                ResetCloudLayerDataElement(element);
            }

            if (layerList.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Press the \"+\" button to add a new Cloud Layer.", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(m_CloudLayerPrefab == null || !PrefabContainsCloudLayerComponents(m_CloudLayerPrefab));
                if (GUILayout.Button(m_ConvertPrefabButton))
                {
                    ConvertPrefabToCloudLayerData(m_CloudLayerPrefab, layerList);
                }

                EditorGUI.EndDisabledGroup();

                m_CloudLayerPrefab = EditorGUILayout.ObjectField(m_CloudLayerPrefab, typeof(GameObject), false) as GameObject;

                EditorGUILayout.EndHorizontal();

                if (m_CloudLayerPrefab == null)
                {
                    EditorGUILayout.HelpBox("Convert Cloud Layer Prefabs created with older version of this asset to the new format.", MessageType.Info);
                }
                else
                {
                    if (PrefabContainsCloudLayerComponents(m_CloudLayerPrefab))
                    {
                        EditorGUILayout.HelpBox("Press the \"Convert\" button to convert the prefab into the new format.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Invalid Prefab!\nIt does not contain any Cloud Layer components.", MessageType.Error);
                    }
                }
            }
        }

        private static void ConvertPrefabToCloudLayerData(GameObject prefab, SerializedProperty layerList)
        {
            CloudLayer[] cloudLayers = prefab.GetComponents<CloudLayer>();

            for (int i = 0; i < cloudLayers.Length; ++i)
            {
                SerializedProperty element = AddCloudLayer(layerList);
                ConvertCloudLayerToCloudLayerData(cloudLayers[i], element, i);
            }
        }

        private static SerializedProperty AddCloudLayer(SerializedProperty layerList)
        {
            int index = layerList.arraySize;
            layerList.arraySize++;

            SerializedProperty element = layerList.GetArrayElementAtIndex(index);

            return element;
        }

        private static void SwapCloudLayers(SerializedProperty layerList, int index1, int index2)
        {
            int listSize = layerList.arraySize;

            if (index1 >= 0 && index2 >= 0 && index1 < listSize && index2 < listSize)
            {
                bool isIndex1Expanded = layerList.GetArrayElementAtIndex(index1).isExpanded;
                layerList.GetArrayElementAtIndex(index1).isExpanded = layerList.GetArrayElementAtIndex(index2).isExpanded;
                layerList.GetArrayElementAtIndex(index2).isExpanded = isIndex1Expanded;

                layerList.MoveArrayElement(index1, index2);
            }
        }

        private static void ResetCloudLayerDataElement(SerializedProperty element)
        {
            element.isExpanded = true;

            element.FindPropertyRelative(CloudLayerData.PATH_IS_VISIBLE).boolValue = true;
            element.FindPropertyRelative(CloudLayerData.PATH_LAYER_NAME).stringValue = "New Cloud Layer";
            element.FindPropertyRelative(CloudLayerData.PATH_BLEND_MODE).enumValueIndex = (int)CloudLayer.BlendMode.Subtract;
            element.FindPropertyRelative(CloudLayerData.PATH_OPACITY).floatValue = 1f;
            element.FindPropertyRelative(CloudLayerData.PATH_COVERAGE).floatValue = 0.5f;
            element.FindPropertyRelative(CloudLayerData.PATH_SOFTNESS).floatValue = 0.25f;
            element.FindPropertyRelative(CloudLayerData.PATH_SPEED).floatValue = 1f;
            element.FindPropertyRelative(CloudLayerData.PATH_DIRECTION).floatValue = 0f;
            element.FindPropertyRelative(CloudLayerData.PATH_TEXTURE).objectReferenceValue = null;
            element.FindPropertyRelative(CloudLayerData.PATH_TEXTURE_TILING).vector2Value = Vector2.one;
            element.FindPropertyRelative(CloudLayerData.PATH_TEXTURE_OFFSET).vector2Value = Vector2.zero;
        }

        private static void ConvertCloudLayerToCloudLayerData(CloudLayer cloudLayer, SerializedProperty element, int index)
        {
            element.isExpanded = true;

            element.FindPropertyRelative(CloudLayerData.PATH_IS_VISIBLE).boolValue = cloudLayer.enabled;
            element.FindPropertyRelative(CloudLayerData.PATH_LAYER_NAME).stringValue = "Cloud Layer " + (index + 1);
            element.FindPropertyRelative(CloudLayerData.PATH_BLEND_MODE).enumValueIndex = (int)cloudLayer.TextureBlendMode;
            element.FindPropertyRelative(CloudLayerData.PATH_OPACITY).floatValue = cloudLayer.Opacity;
            element.FindPropertyRelative(CloudLayerData.PATH_COVERAGE).floatValue = cloudLayer.Coverage;
            element.FindPropertyRelative(CloudLayerData.PATH_SOFTNESS).floatValue = cloudLayer.Softness;
            element.FindPropertyRelative(CloudLayerData.PATH_SPEED).floatValue = cloudLayer.Velocity.magnitude;
            element.FindPropertyRelative(CloudLayerData.PATH_DIRECTION).floatValue = ConvertVelocityToDirectionAngle(cloudLayer.Velocity);
            element.FindPropertyRelative(CloudLayerData.PATH_TEXTURE).objectReferenceValue = cloudLayer.Texture;
            element.FindPropertyRelative(CloudLayerData.PATH_TEXTURE_TILING).vector2Value = cloudLayer.TextureTiling;
            element.FindPropertyRelative(CloudLayerData.PATH_TEXTURE_OFFSET).vector2Value = cloudLayer.TextureOffset;
        }

        private static bool PrefabContainsCloudLayerComponents(GameObject prefab)
        {
            return prefab.GetComponent<CloudLayer>() != null;
        }

        private static float ConvertVelocityToDirectionAngle(Vector2 velocity)
        {
            float directionAngle = Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x);

            if (directionAngle < 0)
            {
                directionAngle = 360 + directionAngle;
            }

            return directionAngle;
        }
    }
}