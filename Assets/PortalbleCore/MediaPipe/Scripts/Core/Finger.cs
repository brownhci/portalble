using UnityEngine;

namespace Mediapipe.HandTracking {
    public class Finger : MonoBehaviour {

        public int index;

        private void FixedUpdate() {
            if (null != Process.INSTANCE) {
                transform.localPosition = Process.INSTANCE.GetFingerLandmark(index);

            }
        }
    }
}