using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portalble {
    /// <summary>
    /// The class for distance indicator using arc length style.
    /// </summary>
    public class DistanceIndicatorArc : IDistanceIndicator {
        public AudioSource notifySound;
        public LineRenderer distanceLine;
        public Slider arcSlider;
        private Image sliderColor;

        protected override void Start() {
            base.Start();
            scaleFactor = 0.01f;
            sliderColor = this.transform.Find("DistanceSlider").transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>();
        }

        // Update is called once per frame
        protected override void Update() {
            // Make it look towards camera
            Camera cam = Camera.main;
            if (cam != null) {
                transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
            }

            // If it's tracking, find distance
            /* what is this tracking object?*/
            if (trackingObject != null) {
                float minDis = 99999f;

                bool isLeftNear = true;

                /* use hand distance as indicator 
                 * we temporarily disable this
                 * Do not remove the code below
                 /*
                if (handLT != null) {
                    minDis = (trackingObject.position - handLT.position).magnitude;
                }
                if (handRT != null) {
                    float tmpDis = (trackingObject.position - handRT.position).magnitude;
                    // Same logic, since if handLT doesn't exist, minDis is -1, where tmpDis is impossible to be negative
                    if (tmpDis < minDis) {
                        minDis = tmpDis;
                        isLeftNear = false;
                    }
                }
                */

                minDis = (trackingObject.position - Camera.main.transform.position).magnitude;

                // pitch varies depends on distance
                notifySound.pitch = 4f / (1f + minDis * 10f);

                float disScaleFactor = (1200 / 4800.0f);

                minDis = minDis *  (1000 * disScaleFactor);

                // Set Arc
                float maxDis = arcSlider.maxValue;
                float upperbound = 0.95f * maxDis;
                float currentValue = Mathf.Clamp(minDis, 0f, upperbound);

                // Calculate Rotation degrees
                float degree = (1f - currentValue / maxDis) * 180f;
                arcSlider.transform.localRotation = Quaternion.Euler(0f, 0f, degree);
                //arcSlider.value = (maxDis - currentValue);

                /* current min is 500cm, max is 1200cm */
                arcSlider.value = (maxDis - currentValue);
                 float trans = MapTransparency(arcSlider.value, 500, 1200);
               // Debug.Log(trans);
                sliderColor.color = MapColor(arcSlider.value, sliderColor.color, trans);

                // Draw lines
                distanceLine.positionCount = 2;
                distanceLine.SetPosition(0, trackingObject.position);
                if (isLeftNear) {
                    distanceLine.SetPosition(1, handLT.position);
                }
                else {
                    distanceLine.SetPosition(1, handRT.position);
                }
            }

            base.Update();
        }

        private float MapTransparency(float v, int low, int high) {
            float transparency_threhold = 980;
            if (v < transparency_threhold)
            {
                float _v = Mathf.Pow((v + 200) / (high - low), 2) + 0.2f;
                if (_v > 1)
                    return 1;
                else
                    /* clamp transparency */
                    if (_v < 0.7f)
                    return 0.7f;
                else
                    return _v;
            
            }   
            // v > 950, very close to the object
            else {
                float max_fadeout = 1000;
                float __v = ((max_fadeout - transparency_threhold) - Clamp(v - transparency_threhold, 0, max_fadeout - transparency_threhold)) / ((max_fadeout - transparency_threhold));
                return __v;
            }
        }

        private float Clamp(float v, float l, float h) {
            if (v < l) return l;
            if (v > h) return h;
            return v;
        }
        
        private Color MapColor(float v, Color c, float transparency) {
            if (v > (1200 - 250)) {
                c.b += 0.05f;
                if (c.b >= 1)
                    c.b = 1;
            }

            if (v < (1200-250)) {
                c.b -= 0.1f;
                if (c.b < 0.29f)
                    c.b = 0.29f;
            }
            return new Color(c.r,c.g,c.b, transparency);
        }

        public override void UpdateConfig(IndicatorManager.DI_CONFIG config) {
            arcSlider.gameObject.SetActive(config.useSphereText);
            notifySound.gameObject.SetActive(config.useSound);
            distanceLine.gameObject.SetActive(config.useLine);
        }
    }
}