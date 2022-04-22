namespace Mediapipe.HandTracking {
    public class HandRect {

        public float Width { get; private set; }
        public float Height { get; private set; }
        public float XCenter { get; private set; }
        public float YCenter { get; private set; }
        public float Rotaion { get; private set; }

        private HandRect(float width, float height, float x_center, float y_center, float rotation) {
            this.Width = width;
            this.Height = height;
            this.XCenter = x_center;
            this.YCenter = y_center;
            this.Rotaion = rotation;
        }

        public static HandRect ParseFrom(float[] arr) {
            if (null == arr || arr.Length < 5) return null;
            return new HandRect(arr[0], arr[1], arr[2], arr[3], arr[4]);
        }

        public bool Equals(HandRect obj) {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Width == obj.Width
                    && this.Height == obj.Height
                    && this.XCenter == obj.XCenter
                    && this.XCenter == obj.YCenter
                    && this.Rotaion == obj.Rotaion;
        }
    }
}
