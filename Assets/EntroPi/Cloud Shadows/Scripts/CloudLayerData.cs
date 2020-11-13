using System;
using UnityEngine;

namespace EntroPi
{
    /// <summary>
    /// Plain Old Data container for storing Cloud Layer properties
    /// </summary>
    [Serializable]
    public class CloudLayerData
    {
        #region Public Constants

        public const string PATH_IS_VISIBLE = "m_IsVisible";
        public const string PATH_LAYER_NAME = "m_LayerName";
        public const string PATH_BLEND_MODE = "m_BlendMode";
        public const string PATH_OPACITY = "m_Opacity";
        public const string PATH_COVERAGE = "m_Coverage";
        public const string PATH_SOFTNESS = "m_Softness";
        public const string PATH_SPEED = "m_Speed";
        public const string PATH_DIRECTION = "m_Direction";
        public const string PATH_TEXTURE = "m_Texture";
        public const string PATH_TEXTURE_TILING = "m_TextureTiling";
        public const string PATH_TEXTURE_OFFSET = "m_TextureOffset";

        #endregion Public Constants

        #region Exposed Data Members

        [SerializeField]
        private bool m_IsVisible = true;

        [SerializeField]
        private string m_LayerName = "Layer Name";

        [Tooltip("Mode used for blending this layer")]
        [SerializeField]
        private CloudLayer.BlendMode m_BlendMode = CloudLayer.BlendMode.Subtract;

        [Tooltip("Opacity of this layer")]
        [Range(0f, 1f)]
        [SerializeField]
        private float m_Opacity = 1f;

        [Tooltip("Cloud coverage (0 = no clouds, 1 = overcast)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float m_Coverage = 0.5f;

        [Tooltip("The softness of the clouds outline")]
        [Range(0f, 1f)]
        [SerializeField]
        private float m_Softness = 0.5f;

        [Tooltip("Movement speed of the clouds")]
        [SerializeField]
        private float m_Speed = 1.0f;

        [Tooltip("Movement direction of the clouds in degrees")]
        [Range(0f, 360f)]
        [SerializeField]
        private float m_Direction = 0f;

        [Tooltip("Cloud texture used by this layer")]
        [SerializeField]
        private Texture2D m_Texture = null;

        [Tooltip("Texture tiling (Values are rounded to nearest int value)")]
        [SerializeField]
        private Vector2 m_TextureTiling = Vector2.one;

        [Tooltip("Initial texture offset")]
        [SerializeField]
        private Vector2 m_TextureOffset = Vector2.zero;

        #endregion Exposed Data Members

        #region Public Properties

        public bool IsVisible
        {
            get { return m_IsVisible; }
            set { m_IsVisible = value; }
        }

        public string LayerName
        {
            get { return m_LayerName; }
            set { m_LayerName = value; }
        }

        public CloudLayer.BlendMode BlendMode
        {
            get { return m_BlendMode; }
            set { m_BlendMode = value; }
        }

        public float Opacity
        {
            get { return m_Opacity; }
            set { m_Opacity = Mathf.Clamp01(value); }
        }

        public float Coverage
        {
            get { return m_Coverage; }
            set { m_Coverage = Mathf.Clamp01(value); }
        }

        public float Softness
        {
            get { return m_Softness; }
            set { m_Softness = Mathf.Clamp01(value); }
        }

        public float Speed
        {
            get { return m_Speed; }
            set { m_Speed = Mathf.Max(0, value); }
        }

        public float DirectionAngle
        {
            get { return m_Direction; }
            set { m_Direction = Mathf.Clamp(value, 0, 360); }
        }

        public Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        public Vector2 TextureTiling
        {
            get { return m_TextureTiling; }
            set
            {
                m_TextureTiling = LimitTextureTiling(value);
            }
        }

        public Vector2 TextureOffset
        {
            get { return m_TextureOffset; }
            set { m_TextureOffset = value; }
        }

        public Vector4 TextureTransform
        {
            get { return new Vector4(m_TextureTiling.x, m_TextureTiling.y, m_TextureOffset.x, m_TextureOffset.y); }
        }

        public Vector2 AnimationOffset
        {
            get { return m_AnimationOffset; }
            set { m_AnimationOffset = value; }
        }

        #endregion Public Properties

        #region Private Data Members

        private Vector2 m_AnimationOffset = Vector2.zero;

        #endregion Private Data Members

        #region Public Static Functions

        /// <summary>
        /// Ensure that Texture tiling is always larger than 1 and rounded to int.
        /// </summary>
        public static Vector2 LimitTextureTiling(Vector2 tiling)
        {
            tiling.x = Mathf.Max(tiling.x, 1);
            tiling.y = Mathf.Max(tiling.y, 1);

            tiling.x = Mathf.Round(tiling.x);
            tiling.y = Mathf.Round(tiling.y);

            return tiling;
        }

        #endregion Public Static Functions
    }
}