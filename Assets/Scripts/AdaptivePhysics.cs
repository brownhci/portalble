using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble.Functions.Grab;

public class AdaptivePhysics : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // If want to make OnTriggerEnter be called, rigidbody is necessary.
		if (GetComponent<Rigidbody>() == null) {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        /* this is WRONG!
         * becasuse 1) when it is grabbed, it will continuously vibrate
         * it will not vibrate when touching other objects as intended
        if (Grab.Instance.UseVibration)
            Vibration.Vibrate(100);
            */

        // First, check if it's fingers
        if (other.transform.parent != null && other.transform.parent.parent != null
             && other.transform.parent.parent.name.StartsWith("Hand_"))
            return;

        // Then make sure not to interact with its parent
        if (other.transform == transform.parent)
            return;

        // Now deal with other objects
        if (Grab.Instance.UseVibration) {
            Grabbable g = Grab.Instance.GetGrabbingObject();
            if (g != null && g.transform == transform) {
                Vibration.Vibrate(100);
            }
        }
    }
}
