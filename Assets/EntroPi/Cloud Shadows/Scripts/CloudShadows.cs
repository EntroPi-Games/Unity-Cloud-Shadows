using UnityEngine;

namespace EntroPi
{
    [AddComponentMenu("EntroPi/Cloud Shadows/Cloud Shadows")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public class CloudShadows : MonoBehaviour
    {
        #region Type Declarations

        public enum ProjectMode { Mode3D = 0, Mode2D = 1 }

        #endregion Type Declarations

        #region Public Constants

        public const string PATH_PREVIEW_IN_EDITOR = "m_PreviewInEditor";
        public const string PATH_PROJECT_MODE = "m_ProjectMode";
        public const string PATH_CLOUD_LAYERS = "m_CloudLayers";
        public const string PATH_WORLD_SIZE = "m_WorldSize";
        public const string PATH_RENDER_TEXTURE_RESOLUTION = "m_RenderTextureResolution";
        public const string PATH_OPACITY_MULTIPLIER = "m_OpacityMultiplier";
        public const string PATH_COVERAGE_MODIFIER = "m_CoverageModifier";
        public const string PATH_SOFTNESS_MODIFIER = "m_SoftnessModifier";
        public const string PATH_SPEED_MULTIPLIER = "m_SpeedMultiplier";
        public const string PATH_DIRECTION_MODIFIER = "m_DirectionModifier";
        public const string PATH_HORIZON_ANGLE_THRESHOLD = "m_HorizonAngleThreshold";
        public const string PATH_HORIZON_ANGLE_FADE = "m_HorizonAngleFade";

        #endregion Public Constants

        #region Private Constants

        private const string SHADER_PATH_CLOUD_SHADOWS = "Shaders/CloudShadows";

        private const float WORLD_SIZE_MIN = 1f;
        private const float ROTATION_MODIFIER_LIMIT = 180f;
        private const float HORIZON_ANGLE_MAX = 90f;
        private const float HORIZON_ANGLE_FADE_MIN = 0.1f;

        #endregion Private Constants

        #region Exposed Data Members

#if UNITY_EDITOR

        [Tooltip("Toggle the visibility of the effect while in Edit Mode")]
        [SerializeField]
        private bool m_PreviewInEditor = true;

#endif

        [Tooltip("Project Mode")]
        [SerializeField]
        private ProjectMode m_ProjectMode = ProjectMode.Mode3D;

        [Tooltip("Asset containing Cloud Layers")]
        [SerializeField]
        private CloudLayers m_CloudLayers;

        [Tooltip("Size in world units that the cloud layer textures will be projected on")]
        [SerializeField]
        private float m_WorldSize = 100;

        [Tooltip("Resolution of texture used to render layers into")]
        [SerializeField]
        private int m_RenderTextureResolution = 1024;

        [Tooltip("Multiplier which influences the opacity of all cloud layers")]
        [SerializeField]
        [Range(0, 1)]
        private float m_OpacityMultiplier = 1.0f;

        [Tooltip("Modifies the coverage of all cloud layers")]
        [SerializeField]
        [Range(-1, 1)]
        private float m_CoverageModifier = 0.0f;

        [Tooltip("Modifies the softness of all cloud layers")]
        [SerializeField]
        [Range(-1, 1)]
        private float m_SoftnessModifier = 0.0f;

        [Tooltip("Multiplier which influences the speed of all cloud layers")]
        [SerializeField]
        private float m_SpeedMultiplier = 1.0f;

        [Tooltip("Modifies the direction of all cloud layers")]
        [Range(-ROTATION_MODIFIER_LIMIT, ROTATION_MODIFIER_LIMIT)]
        [SerializeField]
        private float m_DirectionModifier = 0.0f;

        [Tooltip("The angle from the horizon at which the cloud shadows fade out completely")]
        [SerializeField]
        [Range(0, HORIZON_ANGLE_MAX)]
        private float m_HorizonAngleThreshold = 10.0f;

        [Tooltip("The angle from the horizon over which the cloud shadows fade out")]
        [SerializeField]
        [Range(HORIZON_ANGLE_FADE_MIN, HORIZON_ANGLE_MAX)]
        private float m_HorizonAngleFade = 10.0f;

        #endregion Exposed Data Members

        #region Public Properties

        public ProjectMode Mode
        {
            get { return m_ProjectMode; }
            set { m_ProjectMode = value; }
        }

        public CloudLayers CloudLayers
        {
            get { return m_CloudLayers; }
            set { m_CloudLayers = value; }
        }

        public float WorldSize
        {
            get { return m_WorldSize; }
            set { m_WorldSize = Mathf.Max(value, WORLD_SIZE_MIN); }
        }

        public int RenderTextureResolution
        {
            get { return m_RenderTextureResolution; }
            set
            {
                m_RenderTextureResolution = value;
                UpdateRenderTextureResolution();
            }
        }

        public float OpacityMultiplier
        {
            get { return m_OpacityMultiplier; }
            set { m_OpacityMultiplier = Mathf.Clamp01(value); }
        }

        public float CoverageModifier
        {
            get { return m_CoverageModifier; }
            set { m_CoverageModifier = Mathf.Clamp(value, -1, 1); }
        }

        public float SoftnessModifier
        {
            get { return m_SoftnessModifier; }
            set { m_SoftnessModifier = Mathf.Clamp(value, -1, 1); }
        }

        public float SpeedMultiplier
        {
            get { return m_SpeedMultiplier; }
            set { m_SpeedMultiplier = value; }
        }

        public float DirectionModifier
        {
            get { return m_DirectionModifier; }
            set { m_DirectionModifier = Mathf.Clamp(value, -ROTATION_MODIFIER_LIMIT, ROTATION_MODIFIER_LIMIT); }
        }

        public float HorizonAngleThreshold
        {
            get { return m_HorizonAngleThreshold; }
            set { m_HorizonAngleThreshold = Mathf.Clamp(value, 0, HORIZON_ANGLE_MAX); }
        }

        public float HorizonAngleFade
        {
            get { return m_HorizonAngleFade; }
            set { m_HorizonAngleFade = Mathf.Clamp(value, HORIZON_ANGLE_FADE_MIN, HORIZON_ANGLE_MAX); }
        }

        public RenderTexture RenderTexture
        {
            get { return m_RenderTexture1; }
        }

        #endregion Public Properties

        #region Private Data Members

        private Light m_Light;
        private Material m_CloudShadowMaterial;
        private RenderTexture m_RenderTexture1;
        private RenderTexture m_RenderTexture2;

        private int m_LayerTextureID;
        private int m_LayerTextureTransformID;
        private int m_LayerParamsID;
        private int m_LayerOpacityID;

        #endregion Private Data Members

        #region MonoBehaviour Functions

        private void OnValidate()
        {
            // Validate values changed in inspector.
            m_WorldSize = Mathf.Max(m_WorldSize, WORLD_SIZE_MIN);
            m_RenderTextureResolution = Mathf.Clamp(m_RenderTextureResolution, 2, 4096);
        }

        private void OnEnable()
        {
            m_Light = GetComponent<Light>();

            // Verify that this component can be enabled.
            enabled &= Debug.Verify(m_Light.type == LightType.Directional, "Light type needs to be directional");
        }

        private void OnDisable()
        {
            // Hide effect.
            DisableEffect();
        }

        private void Start()
        {
            // Create Shader IDs.
            m_LayerTextureID = Shader.PropertyToID("_LayerTex");
            m_LayerTextureTransformID = Shader.PropertyToID("_LayerTex_ST");
            m_LayerParamsID = Shader.PropertyToID("_LayerParams");
            m_LayerOpacityID = Shader.PropertyToID("_LayerOpacity");

            // Create render texture.
            m_RenderTexture1 = RenderTextureUtil.CreateRenderTexture(m_RenderTextureResolution);
            m_RenderTexture2 = RenderTextureUtil.CreateRenderTexture(m_RenderTextureResolution);

            // Create materials.
            m_CloudShadowMaterial = ResourceUtil.CreateMaterial(SHADER_PATH_CLOUD_SHADOWS);

            // Initial Update
            UpdateLightProperties();
        }

        private void Update()
        {
#if UNITY_EDITOR
            // In the editor, only update when the "Preview in editor" setting is enabled or while playing the game.
            if (m_PreviewInEditor || UnityEditor.EditorApplication.isPlaying)
            {
                UpdateRenderTextureResolution();
                RenderCloudShadows();
            }
            else
            {
                DisableEffect();
            }
#else
        RenderCloudShadows();
#endif
        }

        #endregion MonoBehaviour Functions

        #region Private Functions

        /// <summary>
        /// Recreates render textures if their resolution differs from the set resolution.
        /// </summary>
        private void UpdateRenderTextureResolution()
        {
            if (m_RenderTexture1 != null && m_RenderTexture1.width != m_RenderTextureResolution)
            {
                RenderTextureUtil.RecreateRenderTexture(ref m_RenderTexture1, m_RenderTextureResolution);
                RenderTextureUtil.RecreateRenderTexture(ref m_RenderTexture2, m_RenderTextureResolution);
            }
        }

        /// <summary>
        /// Renders all of the layers to a single render texture and assigns it to the light.
        /// </summary>
        private void RenderCloudShadows()
        {
            // Clear alpha channel of first render texture to white.
            RenderTextureUtil.ClearRenderTexture(m_RenderTexture1, new Color(0, 0, 0, 1));

            // Don't render if Cloud Layers are not assigned or opacity is 0
            if (m_CloudLayers != null && m_OpacityMultiplier > 0.0f)
            {
                // Calculate the angle between the lights direction and the horizon.
                float angleToHorizon = Vector3.Angle(Vector3.up, transform.forward) - 90;

                // Render layers
                for (int i = 0; i < m_CloudLayers.LayerCount; ++i)
                {
                    CloudLayerData cloudLayerData = m_CloudLayers[i];

                    // Update cloud layer animation offsets.
                    UpdateCloudLayerDataAnimationOffset(cloudLayerData, m_WorldSize, m_SpeedMultiplier, m_DirectionModifier);

                    if (cloudLayerData.IsVisible)
                    {
                        // Set material texture properties.
                        m_CloudShadowMaterial.SetTexture(m_LayerTextureID, cloudLayerData.Texture);
                        m_CloudShadowMaterial.SetVector(m_LayerTextureTransformID, cloudLayerData.TextureTransform);

                        // Set remaining material properties.
                        m_CloudShadowMaterial.SetVector(m_LayerParamsID, ExtractCloudLayerParameters(cloudLayerData));
                        m_CloudShadowMaterial.SetFloat(m_LayerOpacityID, CalculateCloudLayerOpacity(cloudLayerData, angleToHorizon));

                        // Blit using material.
                        Graphics.Blit(m_RenderTexture1, m_RenderTexture2, m_CloudShadowMaterial, (int)cloudLayerData.BlendMode);

                        // Swap render texture references.
                        RenderTextureUtil.SwapRenderTextures(ref m_RenderTexture1, ref m_RenderTexture2);
                    }
                }
            }

            UpdateLightProperties();
        }

        /// <summary>
        /// Updates cloud layer animation offsets
        /// </summary>
        private static void UpdateCloudLayerDataAnimationOffset(CloudLayerData cloudLayerData, float worldSize, float globalSpeedMultiplier, float globalDirectionModifier)
        {
            Vector2 cloudLayerDirection = CalculateLayerDirection(cloudLayerData.DirectionAngle, globalDirectionModifier);
            Vector2 translation = cloudLayerDirection * cloudLayerData.Speed * globalSpeedMultiplier * Time.deltaTime;

            // Make sure cloud translation is independent from world size and texture tiling.
            translation /= worldSize;
            translation.x *= cloudLayerData.TextureTiling.x;
            translation.y *= cloudLayerData.TextureTiling.y;

            Vector2 animationOffset = cloudLayerData.AnimationOffset;
            animationOffset += translation;

            // Repeat floats to avoid precision loss problems.
            animationOffset.x = Mathf.Repeat(animationOffset.x, worldSize);
            animationOffset.y = Mathf.Repeat(animationOffset.y, worldSize);

            cloudLayerData.AnimationOffset = animationOffset;
        }

        /// <summary>
        /// Adds global direction modifier to cloud layer angle direction and converts it into a Vector2D
        /// </summary>
        private static Vector2 CalculateLayerDirection(float cloudLayerDirectionAngle, float globalDirectionModifier)
        {
            float angle = (cloudLayerDirectionAngle + globalDirectionModifier) * Mathf.Deg2Rad;
            Vector2 direction = Vector3.zero;

            direction.x = Mathf.Cos(angle);
            direction.y = Mathf.Sin(angle);

            return direction;
        }

        /// <summary>
        /// Combines layer properties into a single Vector4 to be used as a shader property.
        /// </summary>
        private Vector4 ExtractCloudLayerParameters(CloudLayerData cloudLayerData)
        {
            float coverage = Mathf.Clamp01(cloudLayerData.Coverage + m_CoverageModifier);
            float softness = Mathf.Clamp01(cloudLayerData.Softness + m_SoftnessModifier);

            return new Vector4(cloudLayerData.AnimationOffset.x, cloudLayerData.AnimationOffset.y, coverage, softness);
        }

        /// <summary>
        /// Calculates the final opacity for the passed in layer.
        /// </summary>
        private float CalculateCloudLayerOpacity(CloudLayerData cloudLayerData, float angleToHorizon)
        {
            float opacity = 1;

            // In 3D Mode calculate the opacity based on the angle between the lights direction and horizon.
            if (m_ProjectMode == ProjectMode.Mode3D)
            {
                opacity = Mathf.Clamp01((angleToHorizon - m_HorizonAngleThreshold) / m_HorizonAngleFade);
            }

            opacity *= cloudLayerData.Opacity;
            opacity *= m_OpacityMultiplier;

            return opacity;
        }

        /// <summary>
        /// Updates the light component properties.
        /// </summary>
        private void UpdateLightProperties()
        {
            m_Light.cookie = m_RenderTexture1;
            m_Light.cookieSize = m_WorldSize;
        }

        /// <summary>
        /// Disables the effect.
        /// </summary>
        private void DisableEffect()
        {
            // Release render texture resources
            if (m_RenderTexture1 != null && m_RenderTexture2 != null)
            {
                m_RenderTexture1.Release();
                m_RenderTexture2.Release();
            }

            // Remove render texture from light cookie.
            m_Light.cookie = null;
        }

        #endregion Private Functions
    }
}