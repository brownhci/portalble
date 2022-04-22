using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used on collider of an object which can be grabbed by user.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrabCollider : MonoBehaviour {
	[SerializeField]
	private Transform BindObject;		// The object bind to this collider.
	[SerializeField]
	[Range(1.0f, 3.0f)]
	private float ExpandScale = 1.2f;           // The scale that the collider will expand if it's entered.
    private Vector3 ExpandOffset = Vector3.zero;

    public bool AutomaticExpand = false;

	private Material originMat;
	public Material newMaterial;

	private int LeftHandFingerIn = 0;		// How many left hand's fingers are in the collider right now.
    private int RightHandFingerIn = 0;      // How many right hand's fingers are in teh collider right now.

	private const int CHandFingerThreshold = 3;		// How many fingers in it can trigger a state switch.

	private enum GRABCOLLIDER_STATE {
		TO_ENTER,
		TO_EXIT_LEFT,
        TO_EXIT_RIGHT
	}

	GRABCOLLIDER_STATE State;

	// Use this for initialization
	void Start () {
		// make sure this object has a collider
		Collider cd = GetComponent<Collider>();
		if (cd == null) {
			gameObject.SetActive (false);
			return;
		}

		// check if the collider is triggered.
		if (cd.isTrigger == false) {
			cd.isTrigger = true;
		}

		// if band object is null, defaultly set it to its parent
		if (BindObject == null) {
			BindObject = transform.parent;
		}

        // If it has bind object, make sure that object has collider and rigidbody
        if (BindObject != null) {
            Collider bcd = BindObject.GetComponent<Collider>();
            Rigidbody brb = BindObject.GetComponent<Rigidbody>();
            if (bcd == null || brb == null) {
                Debug.LogWarning("Object:" + BindObject.name + " is supposed to have collider and rigidbody to be grabbed.");
            }
        }

		// set materials issues.
		Renderer rdr = BindObject.GetComponent<Renderer> ();
		if (rdr != null) {
			originMat = rdr.material;
		}

		State = GRABCOLLIDER_STATE.TO_ENTER;
		LeftHandFingerIn = 0;
	}

    void OnDisable() {
        // if this is disabled or destroyed when it's being holding.
        // It should inform the hand to release it.
        if (State == GRABCOLLIDER_STATE.TO_EXIT_LEFT) {
            Debug.Log("Drop from left hand");
            HandManager hm = GameObject.Find("Hand_l").GetComponent<HandManager>();
            if (hm != null && hm.IsGrabbing && hm.getHandObject() == BindObject.gameObject) {
                Debug.Log("Released from left hand");
                BindObject.GetComponent<Renderer>().material = originMat;
                ReleaseSelf();
                SwitchToReadyEnter();
            }
        }
        else if (State == GRABCOLLIDER_STATE.TO_EXIT_RIGHT) {
            Debug.Log("Drop from right hand");
            HandManager hm = GameObject.Find("Hand_r").GetComponent<HandManager>();
            if (hm != null && hm.IsGrabbing && hm.getHandObject() == BindObject.gameObject) {
                BindObject.GetComponent<Renderer>().material = originMat;
                ReleaseSelf();
                SwitchToReadyEnter();
            }
        }
    }

    // Receive Trigger Message
    void OnTriggerEnter(Collider other) {
		// Don't want palm
		if (other.name == "palm")
			return;
        if (other.transform.parent == null || other.transform.parent.parent == null)
            return;
		
		if (other.transform.parent.parent.name == "Hand_l") {
			LeftHandFingerIn++;
		}
        else if (other.transform.parent.parent.name == "Hand_r") {
            RightHandFingerIn++;
        }

		// If it's not waiting for enter, just ignore it.
		if (State != GRABCOLLIDER_STATE.TO_ENTER)
			return;

		if (LeftHandFingerIn >= CHandFingerThreshold) {
			// Tell it to be grabbed
			if (BindObject != null) {
				// Try get hand manager
				HandManager hm = other.transform.parent.parent.GetComponent<HandManager>();
                Debug.Log("Just before hand check");
				if (hm != null && !hm.checkHandBusy()) {
                    Debug.Log("Hand check succeed");
                    // High light it
                    HighLightSelf();
					hm.setHandObject (BindObject.gameObject);
				}
			}

			SwitchToReadyExit (true);
		}
        else if (RightHandFingerIn >= CHandFingerThreshold) {
            // Tell it to be grabbed
            if (BindObject != null) {
                // Try get hand manager
                HandManager hm = other.transform.parent.parent.GetComponent<HandManager>();
                if (hm != null && !hm.checkHandBusy()) {
                    // High light it
                    HighLightSelf();
                    hm.setHandObject(BindObject.gameObject);
                }
            }

            SwitchToReadyExit(false);
        }
    }

	// Receive Trigger Message
	void OnTriggerExit (Collider other) {
		if (other.name == "palm")
			return;

        if (other.transform.parent == null || other.transform.parent.parent == null)
            return;
		
		if (other.transform.parent.parent.name == "Hand_l") {
			LeftHandFingerIn--;
		}
        else if (other.transform.parent.parent.name == "Hand_r") {
            RightHandFingerIn--;
        }

		if (State == GRABCOLLIDER_STATE.TO_EXIT_LEFT && LeftHandFingerIn < CHandFingerThreshold) {
			HandManager hm = GameObject.Find ("Hand_l").GetComponent<HandManager> ();
			if (hm != null && !hm.IsGrabbing) {
				BindObject.GetComponent<Renderer> ().material = originMat;
				ReleaseSelf ();
				SwitchToReadyEnter ();
			}
		}
        else if (State == GRABCOLLIDER_STATE.TO_EXIT_RIGHT && RightHandFingerIn < CHandFingerThreshold) {
            HandManager hm = GameObject.Find("Hand_r").GetComponent<HandManager>();
            if (hm != null && !hm.IsGrabbing) {
                BindObject.GetComponent<Renderer>().material = originMat;
                ReleaseSelf();
                SwitchToReadyEnter();
            }
        }
    }

	// Exit Grab State
	public void ReleaseSelf() {
        HandManager hm = null;
        if (State == GRABCOLLIDER_STATE.TO_EXIT_LEFT)
		    hm = GameObject.Find ("Hand_l").GetComponent<HandManager> ();
        else if (State==GRABCOLLIDER_STATE.TO_EXIT_RIGHT)
            hm = GameObject.Find("Hand_r").GetComponent<HandManager>();
        if (hm != null) {
			hm.removeHandObject();
		}
	}

	// Called when User grab this and then release this
	public void OnGrabFinished() {
		ReleaseSelf ();
        if (State == GRABCOLLIDER_STATE.TO_EXIT_LEFT) {
            if (LeftHandFingerIn < CHandFingerThreshold) {
                BindObject.GetComponent<Renderer>().material = originMat;
                SwitchToReadyEnter();
            }
            else {
                HighLightSelf();
                HandManager hm = GameObject.Find("Hand_l").GetComponent<HandManager>();
                if (hm != null && !hm.checkHandBusy()) {
                    hm.setHandObject(BindObject.gameObject);
                }
            }
		}

        if (State == GRABCOLLIDER_STATE.TO_EXIT_RIGHT) {
            if (RightHandFingerIn < CHandFingerThreshold) {
                BindObject.GetComponent<Renderer>().material = originMat;
                SwitchToReadyEnter();
            }
            else {
                HighLightSelf();
                HandManager hm = GameObject.Find("Hand_r").GetComponent<HandManager>();
                if (hm != null && !hm.checkHandBusy()) {
                    hm.setHandObject(BindObject.gameObject);
                }
            }
        }
    }

	private void SwitchToReadyExit(bool isLeftHand) {
        if (isLeftHand)
            State = GRABCOLLIDER_STATE.TO_EXIT_LEFT;
        else
            State = GRABCOLLIDER_STATE.TO_EXIT_RIGHT;

        ExpandOffset = Vector3.zero;

        // expand issue
        if (!AutomaticExpand) {
            transform.localScale = transform.localScale * ExpandScale;
        }
        else {
            // Automatic expand
            // Get Collider Type see if it's supported for automatically expand.
            // Final distance
            GameObject gobj;
            if (isLeftHand) {
                gobj = GameObject.Find("Hand_l");
            }
            else {
                gobj = GameObject.Find("Hand_r");
            }

            if (gobj == null)
                return;

            Transform t = gobj.transform.Find("palm");
            // Debug.Log("Grab:" + gobj);

            float finalDistance = (t.position - transform.position).magnitude;
            Collider cd = GetComponent<Collider>();
            if (cd is SphereCollider) {
                SphereCollider tmp = (SphereCollider)cd;
                ExpandScale = finalDistance / tmp.radius;
            }
            else if (cd is BoxCollider) {
                Ray r = new Ray(t.position, transform.position);
                RaycastHit[] rh = Physics.RaycastAll(r, finalDistance + 1.0f);
                foreach (RaycastHit h in rh) {
                    if (h.collider.name == gameObject.name) {
                        ExpandScale = finalDistance / (finalDistance - h.distance);
                    }
                }
            }
            else if (cd is MeshCollider) {
                Vector3 centerPoint = cd.bounds.center;
                Ray r = new Ray(t.position, centerPoint);
                RaycastHit[] rh = Physics.RaycastAll(r, finalDistance + 1.0f);
                foreach (RaycastHit h in rh) {
                    if (h.collider.name == gameObject.name) {
                        ExpandScale = finalDistance / (finalDistance - h.distance);
                        ExpandOffset = (centerPoint - transform.position) * (ExpandScale - 1.0f);
                    }
                }
            }
            else {
                Debug.LogWarning("You want to apply automatic collider expand to an unsupport collider");
            }

            ExpandScale = Mathf.Clamp(ExpandScale, 0.95f, 1.5f);

            transform.localScale = transform.localScale * ExpandScale;
            // transform.localPosition += ExpandOffset;
        }
	}

	private void SwitchToReadyEnter() {
		State = GRABCOLLIDER_STATE.TO_ENTER;
		transform.localScale = transform.localScale / ExpandScale;
        // transform.localPosition -= ExpandOffset;
    }

    private void HighLightSelf() {
        if (newMaterial != null && GlobalStates.isIndicatorEnabled) {
            newMaterial.mainTexture = originMat.mainTexture;
            newMaterial.color = originMat.color;
            newMaterial.SetColor("_OutlineColor", Color.green);
            BindObject.GetComponent<Renderer>().material = newMaterial;
        }
    }

    public void OnBeingGrabbed() {
        if (newMaterial != null) {
            newMaterial.mainTexture = originMat.mainTexture;
            newMaterial.mainTexture = originMat.mainTexture;
            newMaterial.color = originMat.color;
            newMaterial.SetColor("_OutlineColor", Color.blue);
            BindObject.GetComponent<Renderer>().material = newMaterial;
        }
    }

    public void SetBindObject(Transform bindobject) {
        BindObject = bindobject;
    }
}
