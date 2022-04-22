using System;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This component tests for depth functionality and enables/disables
    /// a text message on the screen reporting that depth is not suppoted.
    /// </summary>
    public class CheckRuntimeDepth : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The AROcclusionManager which will manage depth functionality.")]
        AROcclusionManager m_OcclusionManager;

        /// <summary>
        /// Get or set the <c>AROcclusionManager</c>.
        /// </summary>
        public AROcclusionManager occlusionManager
        {
            get => m_OcclusionManager;
            set => m_OcclusionManager = value;
        }

        [SerializeField]
        Text m_DepthAvailabilityInfo;

        /// <summary>
        /// The UI Text used to display information about the availability of depth functionality.
        /// </summary>
        public Text depthAvailabilityInfo
        {
            get => m_DepthAvailabilityInfo;
            set => m_DepthAvailabilityInfo = value;
        }

        void Start()
        {
            Debug.Assert(m_OcclusionManager != null, "no occlusion manager");
            Debug.Assert(m_DepthAvailabilityInfo != null, "no text box");
            var descriptor = m_OcclusionManager.descriptor;
            m_DepthAvailabilityInfo.enabled =
                descriptor == null ||
                (descriptor.humanSegmentationStencilImageSupported == Supported.Unsupported &&
                descriptor.humanSegmentationDepthImageSupported == Supported.Unsupported &&
                descriptor.environmentDepthImageSupported == Supported.Unsupported);
        }
    }
}
