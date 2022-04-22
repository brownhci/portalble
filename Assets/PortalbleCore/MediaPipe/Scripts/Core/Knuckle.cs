using UnityEngine;

namespace Mediapipe.HandTracking {
    public class Knuckle : MonoBehaviour {

        public Transform first, last;

        private void FixedUpdate() {

            Vector3 local_position_temp = last.localPosition - first.localPosition;
            Vector3 local_scale_temp = transform.localScale;

            // position
            transform.localPosition = (last.localPosition + first.localPosition) / 2;
            // scale
            transform.localScale = new Vector3(local_scale_temp.x, local_position_temp.magnitude / 2.0f, local_scale_temp.z);
            // rotation
            transform.up = local_position_temp;
        }
    }
}