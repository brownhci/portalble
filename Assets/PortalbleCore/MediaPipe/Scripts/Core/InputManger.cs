using UnityEngine;

namespace Mediapipe.HandTracking {
    [System.Serializable]
    public abstract class InputManager : MonoBehaviour {
        public abstract FrameInput GetFrameInput();
    }

    public class FrameInput {
        public float width;
        public float height;
        public sbyte[] sbyte_array;

        public FrameInput(float with, float height, sbyte[] byte_array) {
            this.width = with;
            this.height = height;
            this.sbyte_array = byte_array;
        }
    }
}
