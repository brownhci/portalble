using UnityEngine;

namespace Mediapipe.HandTracking {
    public abstract class DepthSetting : MonoBehaviour {

        [SerializeField]
        private GameObject
            input = null;

        protected static DepthEstimate depth_estimate = null;

        public static DepthEstimate GetDepthEstimate() => depth_estimate;

        protected void Awake() {
            input.SetActive(false);
        }

        protected void EnableDepthInput() {
            input.SetActive(true);
        }
    }
}