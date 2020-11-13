using System.Collections.Generic;
using UnityEngine;

namespace EntroPi
{
    [CreateAssetMenu(fileName = "CloudLayers", menuName = "EntroPi/Cloud Shadows/Cloud Layers", order = 1)]
    public class CloudLayers : ScriptableObject
    {
        #region Exposed Data Members

        [SerializeField]
        private List<CloudLayerData> m_LayerData = new List<CloudLayerData>();

        #endregion Exposed Data Members

        #region Public Properties

        public IEnumerable<CloudLayerData> Layers
        {
            get
            {
                foreach (CloudLayerData cloudLayer in m_LayerData)
                {
                    yield return cloudLayer;
                }
            }
        }

        public CloudLayerData this[int index]
        {
            get
            {
                CloudLayerData cloudLayerData = null;

                if (index >= 0 && index < m_LayerData.Count)
                {
                    cloudLayerData = m_LayerData[index];
                }

                UnityEngine.Debug.Assert(cloudLayerData != null, "Failed to get CloudLayerData. Index out of bounds");

                return cloudLayerData;
            }
        }

        public int LayerCount { get { return m_LayerData.Count; } }

        #endregion Public Properties
    }
}