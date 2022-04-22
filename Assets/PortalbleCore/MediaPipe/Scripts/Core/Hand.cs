using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.HandTracking {
    public partial class Hand {
        private const float Z_MAX_DISTANCE_LANDMARK = 80f;

 
        private float length_size;
        private Vector3[] normalizedLandmarks;
        private Vector3[] landmarks;
        private Vector3[] rawLandmarks;

        private static Vector3[] mArrLandmarks;
        private HandRect hand_rect;
 

        private static float scale = 1f, offset = 0.33f;

        public Vector3 Position { get; private set; }
        public Vector3 GetLandmark(int index) => this.landmarks[index];
        public Vector3[] GetLandmarks() => this.landmarks;

        private static Camera ARCamera;
        

        private Hand() {
            this.length_size = 0;
            this.normalizedLandmarks = null;
            this.landmarks = null;
        }

        private Hand(Vector3 position, Vector3[] landmarks) {
            this.Position = position;
            this.landmarks = landmarks;
        }

        private Hand(Vector3[] landmarks) {
            this.Position = landmarks[0];
            this.landmarks = landmarks;
        }

        public static Hand MakeFrom(float[] arr_landmark_data, HandRect hand_rect) {
            if (null == arr_landmark_data || arr_landmark_data.Length < 63) return null;
            Vector3[] normalize_landmarks = new Vector3[21];
            Vector3[] raw_landmarks = new Vector3[21];

#if !UNITY_EDITOR && UNITY_ANDROID
            if (LandmarkConverter.INSTANCE == null || !LandmarkConverter.INSTANCE.Valid()) return null;
#endif
            for (int i = 0; i < 21; i++)
            {


                /* in the editor we do not need to process the landmarks since they are normalized
                   during the recording*/
#if !UNITY_EDITOR && UNITY_ANDROID
                normalize_landmarks[i] = LandmarkConverter.INSTANCE.Convert(arr_landmark_data[3 * i], arr_landmark_data[3 * i + 1], arr_landmark_data[3 * i + 2]);
#elif UNITY_EDITOR
                normalize_landmarks[i] = new Vector3(arr_landmark_data[3 * i], arr_landmark_data[3 * i + 1], arr_landmark_data[3 * i + 2]);
#endif
                raw_landmarks[i] = new Vector3(arr_landmark_data[3 * i], arr_landmark_data[3 * i + 1], arr_landmark_data[3 * i + 2]);
            }


            if (ARCamera == null)
                ARCamera = GameObject.Find("AR Camera").GetComponent<Camera>();


#if UNITY_ANDROID && !UNITY_EDITOR
            Builder builder = new Builder(DepthSetting.GetDepthEstimate());
#elif UNITY_EDITOR
            Builder builder = new Builder();
#endif
            builder.Set(hand_rect);
            builder.Set(normalize_landmarks);
            builder.SetRawLandmarks(raw_landmarks);
            return builder.Build();
        }

        /* return all the landmarks */
        public static Vector3[] GetLandmarksFromRaw()
        {
            return mArrLandmarks;
        }

        /* return normalized the landmarks */
        public string GetNormalizedLandmarks()
        {
            string tmp = "";
            for (int i = 0; i < normalizedLandmarks.Length; i += 1)
            {
                tmp = tmp + normalizedLandmarks[i].x.ToString() + "," + normalizedLandmarks[i].y.ToString() + "," + normalizedLandmarks[i].z.ToString() + ";";
            }
            return tmp;
        }

        public static void SetScale(float f)
        {
            scale = f;
        }

        public static void SetOffset(float f)
        {
            offset = f;
        }


        private class Builder {
            // define các cặp điểm đốt ngón tay.
            private static int[,] finger_couples = { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 4 }, { 0, 5 }, { 5, 6 }, { 6, 7 }, { 7, 8 }, { 0, 9 }, { 9, 10 }, { 10, 11 }, { 11, 12 }, { 0, 13 }, { 13, 14 }, { 14, 15 }, { 15, 16 }, { 0, 17 }, { 17, 18 }, { 18, 19 }, { 19, 20 } };

            private DepthEstimate depthEstimate;
            private Hand hand;
            private HandRect handRect;
            private Vector3[] normalizedLandmarks;
            private Vector3[] rawLandmarks;
            private bool depthEstimateReady = false;

            // Overloaded constructor for when no depth_estimate is ready yet
            public Builder()
            {
                this.hand = new Hand();
                this.hand.landmarks = new Vector3[21];
                Hand.mArrLandmarks = new Vector3[21];
            }

            public Builder(DepthEstimate depthEstimate) {
                this.depthEstimateReady = true;
                this.depthEstimate = depthEstimate;
                this.hand = new Hand();
                this.hand.landmarks = new Vector3[21];
                Hand.mArrLandmarks = new Vector3[21];
            }

            public void Set(HandRect handRect) => this.handRect = handRect;
            public void Set(Vector3[] normalizedLandmarks) => this.normalizedLandmarks = normalizedLandmarks;
            public void SetRawLandmarks(Vector3[] raw) => this.rawLandmarks = raw;

            public Hand Build() {
                this.hand.normalizedLandmarks = normalizedLandmarks;
                this.hand.rawLandmarks = rawLandmarks;
                // visualize the landmarks

                //  Calculates the hand magnitude(by the sum of the knuckles) as normalize
                for (int i = 0; i < 20; i++) this.hand.length_size += Vector3.Magnitude(normalizedLandmarks[finger_couples[i, 1]] - normalizedLandmarks[finger_couples[i, 0]]);

                // If plane has already been detected

                if(false)
                {
                    // calculate projection ratio before estimating depth
                    depthEstimate.PredictZoomIndicator(handRect, this.hand.length_size);

                    for (int i = 0; i < 21; i++)
                    {

                        this.hand.landmarks[i] = Camera.main.ScreenToWorldPoint(new Vector3(
                                Screen.width * this.hand.normalizedLandmarks[i].x,
                                Screen.height * this.hand.normalizedLandmarks[i].y,
                                this.depthEstimate.PredictDepth(this.hand.normalizedLandmarks[i].z)
                            ));


                        Hand.mArrLandmarks[i] = new Vector3(
                                Screen.width * this.hand.rawLandmarks[i].x,
                                Screen.height * this.hand.rawLandmarks[i].y,
                                Hand.scale * (this.hand.rawLandmarks[i].z + Hand.offset)
                            );

                    }
                }
                // Before any planes have been detected, default to ax + b
                else
                {
                    for (int i = 0; i < 21; i++)
                    {

                        this.hand.landmarks[i] = Camera.main.ScreenToWorldPoint(new Vector3(
                                Screen.width * this.hand.normalizedLandmarks[i].x,
                                Screen.height * this.hand.normalizedLandmarks[i].y,
                                Hand.scale * this.hand.normalizedLandmarks[i].z + Hand.offset

                            ));

                        /* print the z values every hand landmark */


                        Hand.mArrLandmarks[i] = new Vector3(
                                Screen.width * this.hand.rawLandmarks[i].x,
                                Screen.height * this.hand.rawLandmarks[i].y,
                                Hand.scale * (this.hand.rawLandmarks[i].z + Hand.offset)
                            );
                    }
                }

                this.hand.Position = this.hand.landmarks[0];
                return this.hand;
            }
        }
    }
}