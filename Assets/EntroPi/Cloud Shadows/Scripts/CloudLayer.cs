using UnityEngine;

namespace EntroPi
{
    public class CloudLayer : MonoBehaviour
    {
        public enum BlendMode { Subtract, MultiplyInverse, ColorBurn, VividLight, PinLight }

        #region Exposed Data Members

        [Header("Texture Settings", order = 0)]
        [Tooltip("Cloud texture used by this layer.")]
        [SerializeField]
        private Texture2D m_Texture;

        [Tooltip("Texture tiling. Values will be rounded to nearest int value.")]
        [SerializeField]
        private Vector2 m_Tiling = Vector2.one;

        [Tooltip("Initial texture offset.")]
        [SerializeField]
        private Vector2 m_Offset = Vector2.zero;

        [Header("Blend Settings", order = 1)]
        [Tooltip("Mode used for blending this layer.")]
        [SerializeField]
        private BlendMode m_BlendMode = BlendMode.Subtract;

        [Tooltip("Opacity of this layer.")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Opacity = 1.0f;

        [Header("Cloud Settings", order = 2)]
        [Tooltip("Cloud coverage. (0 = no clouds, 1 = overcast)")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Coverage = 0.5f;

        [Tooltip("The softness of the clouds outline.")]
        [SerializeField]
        [Range(0.1f, 1)]
        private float m_Softness = 0.25f;

        [Tooltip("The velocity of the clouds in world space.")]
        [SerializeField]
        private Vector2 m_Velocity = Vector2.zero;

        [Header("Advanced Settings", order = 2)]
        [Tooltip("The angle from the horizon at which the cloud shadows fade out completely.")]
        [SerializeField]
        [Range(0, 90)]
        private float m_HorizonAngleThreshold = 10.0f;

        [Tooltip("The angle from the horizon over which the cloud shadows fade out.")]
        [SerializeField]
        [Range(0, 90)]
        private float m_HorizonAngleFade = 10.0f;

        #endregion Exposed Data Members

        #region Public Properties

        public Texture2D Texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        public Vector4 TextureTransform
        {
            get { return new Vector4(m_Tiling.x, m_Tiling.y, m_Offset.x, m_Offset.y); }
        }

        public Vector2 TextureTiling
        {
            get { return m_Tiling; }
            set { m_Tiling = value; }
        }

        public Vector2 TextureOffset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        public BlendMode TextureBlendMode
        {
            get { return m_BlendMode; }
            set { m_BlendMode = value; }
        }

        public float Opacity
        {
            get { return m_Opacity; }
            set { m_Opacity = value; }
        }

        public float Coverage
        {
            get { return m_Coverage; }
            set { m_Coverage = value; }
        }

        public float Softness
        {
            get { return m_Softness; }
            set { m_Softness = value; }
        }

        public Vector2 Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public float HorizonAngleThreshold
        {
            get { return m_HorizonAngleThreshold; }
            set { m_HorizonAngleThreshold = value; }
        }

        public float HorizonAngleFade
        {
            get { return m_HorizonAngleFade; }
            set { m_HorizonAngleFade = value; }
        }

        #endregion Public Properties

        #region MonoBehaviour Functions

        private void OnValidate()
        {
            // Validate that Texture scale is always larger than 1 and rounded to int
            m_Tiling.x = Mathf.Max(m_Tiling.x, 1);
            m_Tiling.y = Mathf.Max(m_Tiling.y, 1);

            m_Tiling.x = Mathf.Round(m_Tiling.x);
            m_Tiling.y = Mathf.Round(m_Tiling.y);
        }

        private void OnEnable()
        {
            // Verify that this component can be enabled.
            enabled &= Debug.Verify(m_Texture != null, "Cloud Layer texture not assigned!");
        }

        #endregion MonoBehaviour Functions
    }
}