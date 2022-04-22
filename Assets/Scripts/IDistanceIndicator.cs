using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble.Functions.Grab;

namespace Portalble {
    public abstract class IDistanceIndicator : MonoBehaviour {
        protected Transform handLT;
        protected Transform handRT;
        protected Transform trackingObject;

        protected float outofdateValue = 0f;
        protected float scaleFactor = 0.012f;

        // Here, find Hand we need
        protected virtual void Start() {
            GameObject gobj = GameObject.Find("Hand_l");
            if (gobj != null)
                handLT = gobj.transform.Find("palm");
            gobj = GameObject.Find("Hand_r");
            if (gobj != null)
                handRT = gobj.transform.Find("palm");
        }

        protected virtual void Update() {
            // Refresh Status
            // Set size
            if (trackingObject != null) {
                Bounds bds = new Bounds();
                MeshFilter mf = trackingObject.GetComponent<MeshFilter>();
                if (mf != null) {
                    bds = mf.mesh.bounds;
                    // Find the max one
                    float maxvalue = 0f;
                    for (int i = 0; i < 3; ++i) {
                        if (maxvalue < bds.size[i])
                            maxvalue = bds.size[i];
                    }

                    bds.center = new Vector3(bds.center.x, bds.center.y + 0.15f, bds.center.z);
                    transform.position = trackingObject.position + bds.center;

                    // Adjust size
                    // float factor = scaleFactor * maxvalue;
                    //float factor = 0.1f;
                    transform.localScale = new Vector3(0.00035f, 0.00035f, 0.00035f);
                    //transform.localScale = trackingObject.localScale * factor;
                    /* huristic method for our study */
                }
                else {
                    // try skinned mesh
                    SkinnedMeshRenderer smr = trackingObject.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null) {
                        bds = smr.bounds;
                        // Find the max one
                        float maxvalue = 0f;
                        for (int i = 0; i < 3; ++i) {
                            if (maxvalue < bds.size[i])
                                maxvalue = bds.size[i];
                        }
                        transform.position = bds.center;
                        transform.localScale = maxvalue * Vector3.one * scaleFactor;
                    }
                    else {
                        transform.position = trackingObject.position;
                    }
                }
            }

            // out of date check
            outofdateValue -= Time.deltaTime;
            if (outofdateValue <= 0f) {
                outofdateValue = 0f;
                gameObject.SetActive(false);
            }
        }

        /* what does it do */

        public virtual void SetToAnInteractiveObject(Transform t, float activeTime = 1.2f) {
            // Set position
            // check proxy object
            Grabbable grabbable = t.GetComponent<Grabbable>();
            if (grabbable != null) {
                trackingObject = grabbable.GetProxy();
            }
            if (trackingObject == null)
                trackingObject = t;

            outofdateValue = activeTime;
            gameObject.SetActive(true);
        }

        public virtual void RefreshActiveTime(float time = 1.0f) {
            outofdateValue = time;
        }

        public abstract void UpdateConfig(IndicatorManager.DI_CONFIG config);

    }
}