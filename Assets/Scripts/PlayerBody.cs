using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour {
    public float Health = 1000f;

    /* update commented session */
    //public RedFlashEffect RedEffect;

    private long[] vibratePattern = new long[] { 0, 200, 50, 200, 50, 300 };

    void OnTriggerEnter(Collider other) {
    }

}
