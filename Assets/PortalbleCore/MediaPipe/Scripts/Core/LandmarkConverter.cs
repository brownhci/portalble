using UnityEngine;

namespace Mediapipe.HandTracking {
    public abstract class LandmarkConverter {
        public static LandmarkConverter INSTANCE = null;

        public static LandmarkConverter Create(Orientation orientation) {
            switch (orientation) {
                case Orientation.PORTRAIT:
                case Orientation.PORTRAIT_UP_SIDE_DOWN:
                    return new PortraitUpSideDownLandmarkConverter();
                case Orientation.LANDSCAPE_RIGHT:
                    return new LandscapeRightLandmarkConverter();
                default:
                    return null;
            }
        }

        protected float input_ratio = default;
        protected float output_ratio = default;

        public bool Valid() {
            return input_ratio != default && output_ratio != default;
        }

        public void SetInput(float ratio) => this.input_ratio = ratio;
        public void SetOutput(float ratio) => this.output_ratio = ratio;
        public override string ToString() { return ("input_ratio: " + input_ratio + ", output_ratio: " + output_ratio); }

        public abstract Vector3 Convert(float x, float y, float z);
    }

    /* enmin print output ratio here*/
    public class PortraitUpSideDownLandmarkConverter : LandmarkConverter {
        public override Vector3 Convert(float x, float y, float z) {
            float alpha = this.output_ratio * this.input_ratio;
            float delta = 0.5f - alpha * 0.5f;
            return new Vector3((y - delta) / alpha, x, z);
        }

        public override string ToString() {
            return "PortraitLandmarkConverter: [ " + base.ToString() + " ]";
        }
    }

    public class LandscapeRightLandmarkConverter : LandmarkConverter {
        public override Vector3 Convert(float x, float y, float z) {
            float alpha = this.input_ratio / this.output_ratio;
            float delta = 0.5f + alpha * 0.5f;
            return new Vector3(x, (delta - y) / alpha, z);
        }

        public override string ToString() {
            return "LandscapeRightLandmarkConverter: [ " + base.ToString() + " ]";
        }
    }

    public enum Orientation {
        PORTRAIT, PORTRAIT_UP_SIDE_DOWN, LANDSCAPE_RIGHT
    }
}