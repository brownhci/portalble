namespace Mediapipe.HandTracking {
    public class DepthEstimate {

        public float zoom_indicator = default;
        public HandRect last_hand_rect = null;
        public float default_depth = default;
        public float default_hand_length_size = default;

        public DepthEstimate() {}

        public virtual float PredictZoomIndicator(HandRect hand_rect, float current_hand_length_size) {

            if (this.default_hand_length_size == default) this.default_hand_length_size = current_hand_length_size;

            if (default == this.zoom_indicator || (null != hand_rect && !hand_rect.Equals(this.last_hand_rect))) {
                this.zoom_indicator = this.default_hand_length_size / current_hand_length_size;
                this.last_hand_rect = hand_rect;
            }

            return this.zoom_indicator;
        }

        public virtual float PredictDepth(float z_normalized) {
            // [20201123 xk] yo I think default depth just is always 0.5f... but zoom_indicator changes
            return default_depth * zoom_indicator + z_normalized * default_depth * 0.24f * zoom_indicator; 
        }

        public virtual bool Valid() {
            return default_depth != default;
        }
    }
}