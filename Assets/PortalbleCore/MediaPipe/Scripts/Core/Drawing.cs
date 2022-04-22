using UnityEngine;

namespace Mediapipe.HandTracking {
    public class Drawing : MonoBehaviour {

        private int[,] fingers_knuckles_follow = { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 4 }, { 0, 5 }, { 1, 5 }, { 2, 5 }, { 5, 6 }, { 6, 7 }, { 7, 8 }, { 5, 9 }, { 0, 9 }, { 9, 10 }, { 10, 11 }, { 11, 12 }, { 9, 13 }, { 0, 13 }, { 13, 14 }, { 14, 15 }, { 15, 16 }, { 13, 17 }, { 0, 17 }, { 17, 18 }, { 18, 19 }, { 19, 20 } };

        private GameObject[] fingers;
        private GameObject[] knuckles;

        [SerializeField]
        private float finger_size = 0.1f;
        [SerializeField]
        private float kuckle_size = 0.07f;
        [SerializeField]
        private GameObject finger_object = null, knuckle_object = null;

        private void Awake() {
            fingers = new GameObject[21];
            knuckles = new GameObject[25];
        }

        private void Start() {
            for (int i = 0; i < 21; i++) {
                // create finger
                fingers[i] = Instantiate(finger_object);
                fingers[i].transform.parent = this.transform;
                fingers[i].transform.localScale = Vector3.one * finger_size;
                fingers[i].AddComponent<Finger>().index = i;
            }
            for (int i = 0; i < 25; i++) {
                // create knuckle
                knuckles[i] = Instantiate(knuckle_object);
                knuckles[i].transform.parent = this.transform;
                knuckles[i].transform.localScale = Vector3.one * kuckle_size;
                Knuckle knuck = knuckles[i].AddComponent<Knuckle>();
                knuck.first = fingers[fingers_knuckles_follow[i, 0]].transform;
                knuck.last = fingers[fingers_knuckles_follow[i, 1]].transform;
            }
        }

    }
}