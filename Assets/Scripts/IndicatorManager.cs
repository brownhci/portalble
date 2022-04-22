using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portalble {
    /**
     * Used to manage indicator.
     */
    public class IndicatorManager : MonoBehaviour {
        public Transform indicatorPrefab;
        public Transform backupPrefab;

        /* this distance is how far the system scans for the indicator, 
         * the actual calculation of the indicate completion can be different from this value 
         * if nothing is wrong, keep it at 4.8 metesters, you may change this scanning range depending on your applicaiton
          */
        public float trackStartDistance = 4800f;
        public float soundCylinderRadius = 0.1f;

        private float realTrackDistance;
        private float realTrackOffset;

        private Transform handLT;
        private Transform handRT;

        private List<IDistanceIndicator> m_indicators;
        private Dictionary<Collider, IDistanceIndicator> m_trackingObjects;

        private bool m_testSingleSound = false;
        private IDistanceIndicator m_lastSingleSound;

        public UITextPanel screenText;


        public class DI_CONFIG {
            public bool useSphereText;
            public bool useLine;
            public bool useSound;

            public DI_CONFIG() {
                useSphereText = true; // default to indicator on
                useLine = false;
                useSound = false;
            }
        }

        private DI_CONFIG m_configs;

        // Use this for initialization
        void Start() {
            GameObject gobj = GameObject.Find("Hand_l");
            if (gobj != null)
                handLT = gobj.transform.Find("palm");
            gobj = GameObject.Find("Hand_r");
            if (gobj != null)
                handRT = gobj.transform.Find("palm");

            m_indicators = new List<IDistanceIndicator>();
            m_trackingObjects = new Dictionary<Collider, IDistanceIndicator>();

            m_configs = new DI_CONFIG();

            GlobalStates.isIndicatorEnabled = m_configs.useSphereText;
            foreach (IDistanceIndicator di in m_indicators)
            {
                di.UpdateConfig(m_configs);
            }
        }

        // Update is called once per frame
        void Update() {
            // Not always search, start a thread that refresh the status 2 times per second.
            float freshTime = 0.5f;
            InvokeRepeating("RefreshTracking", freshTime, freshTime);
        }

        void RefreshTracking() {
            RefreshTrackingList();
            if (indicatorPrefab == null)
                return;

            realTrackDistance = trackStartDistance / 1000f;
            int colliderMask = 0x1 << 11;
            // Find hand
            float minimalDistance = 9999f;
            int minimalObject = -1;

            Collider[] lcolliders = new Collider[1];
            Collider[] rcolliders = new Collider[1];

            if (handLT != null) {
                lcolliders = Physics.OverlapSphere(handLT.position, realTrackDistance, colliderMask);
                for (int i = 0; i < lcolliders.Length; ++i) {
                    // Check if it already has an indicator
                    if (lcolliders[i].tag == "InteractableObj") {
                        if (m_trackingObjects.ContainsKey(lcolliders[i]))
                            m_trackingObjects[lcolliders[i]].RefreshActiveTime();
                        else
                            BindIndicatorToObject(lcolliders[i]);
                    }

                    // cylinder sound test
                    Vector3 v = handLT.position - Camera.main.transform.position;
                    Vector3 u = lcolliders[i].transform.position - Camera.main.transform.position;
                    float ulength = u.magnitude;
                    float uvdot = Vector3.Dot(u, v);

                    // if the hand is behind camera or the object is behind the camera, ignore it
                    if (uvdot > 0 && Vector3.Dot(v, Camera.main.transform.forward) > 0) {
                        float cos_theta = uvdot / (v.magnitude * ulength);

                        // use cos^2 + sin^2 = 1 and use ucos <= R is equal to u^2 cos^2 <= R^2.
                        // so that we avoid using arccos and sin to calculate triangular functions.
                        float sin_theta2 = 1.0f - cos_theta * cos_theta;
                        if (ulength * ulength * sin_theta2 <= soundCylinderRadius * soundCylinderRadius
                            && (u - v).magnitude < minimalDistance) {
                            minimalDistance = (u - v).magnitude;
                            minimalObject = -(i + 1);
                        }
                    }
                }
            }

            if (handRT != null) {
                rcolliders = Physics.OverlapSphere(handRT.position, realTrackDistance, colliderMask);
                for (int i = 0; i < rcolliders.Length; ++i) {
                    // Check if it already has an indicator
                    if (rcolliders[i].tag == "InteractableObj") {
                        if (m_trackingObjects.ContainsKey(rcolliders[i]))
                            m_trackingObjects[rcolliders[i]].RefreshActiveTime();
                        else
                            BindIndicatorToObject(rcolliders[i]);
                    }

                    // cylinder sound test
                    Vector3 v = handRT.position - Camera.main.transform.position;
                    Vector3 u = rcolliders[i].transform.position - Camera.main.transform.position;
                    float ulength = u.magnitude;
                    float uvdot = Vector3.Dot(u, v);

                    // if the hand is behind camera or the object is behind the camera, ignore it
                    if (uvdot > 0 && Vector3.Dot(v, Camera.main.transform.forward) > 0) {
                        float cos_theta = uvdot / (v.magnitude * ulength);

                        // use cos^2 + sin^2 = 1 and use ucos <= R is equal to u^2 cos^2 <= R^2.
                        // so that we avoid using arccos and sin to calculate triangular functions.
                        float sin_theta2 = 1.0f - cos_theta * cos_theta;
                        if (ulength * ulength * sin_theta2 <= soundCylinderRadius * soundCylinderRadius
                            && (u - v).magnitude < minimalDistance) {
                            minimalDistance = (u - v).magnitude;
                            minimalObject = (i + 1);
                        }
                    }
                }
            }

            if (m_testSingleSound && minimalDistance != 9999f) {
                
                IDistanceIndicator newIndicator = null;
                if (minimalObject < 0) {
                    newIndicator = m_trackingObjects[lcolliders[-minimalObject - 1]];
                }
                else {
                    newIndicator = m_trackingObjects[rcolliders[minimalObject - 1]];
                }

                if (m_lastSingleSound != null && m_lastSingleSound != newIndicator) {
                    setSoundPlayForIndicator(m_lastSingleSound, false);
                }
                m_lastSingleSound = newIndicator;
                setSoundPlayForIndicator(newIndicator, true);
            }
            else if (m_lastSingleSound != null) {
                setSoundPlayForIndicator(m_lastSingleSound, false);
                m_lastSingleSound = null;
            }


            // Text Update
            if (screenText != null) {
                float thresholdDis = 0.3f * realTrackDistance;
                if (m_trackingObjects.Count == 0) {
                    screenText.setText("no objects tracked");
                }
                else if (minimalDistance >= thresholdDis * thresholdDis) {
                    // In this way because minimalDistance is sqrt magnitude
                    screenText.setText("reach closer to object");
                }
                else {
                    screenText.setText("");
                }
            }
        }

        void RefreshTrackingList() {
            List<Collider> removeList = new List<Collider>();
            foreach (KeyValuePair<Collider, IDistanceIndicator> k in m_trackingObjects) {
                if (k.Value.gameObject == null || k.Value.gameObject.activeInHierarchy == false) {
                    removeList.Add(k.Key);
                }
            }

            foreach (Collider c in removeList) {
                m_trackingObjects.Remove(c);
            }
        }

        void BindIndicatorToObject(Collider cd) {
            // Find an available indicator
            for (int i = 0; i < m_indicators.Count; ++i) {
                if (m_indicators[i].gameObject.activeInHierarchy == false) {
                    // use this
                    m_indicators[i].SetToAnInteractiveObject(cd.transform);
                    m_trackingObjects.Add(cd, m_indicators[i]);
                    return;
                }
            }

            // Or no available indicator, create one
            Transform t = GameObject.Instantiate(indicatorPrefab);
            IDistanceIndicator di = t.GetComponent<IDistanceIndicator>();
            if (di == null) {
                GameObject.Destroy(t.gameObject);
                return;
            }

            di.UpdateConfig(m_configs);
            di.SetToAnInteractiveObject(cd.transform);
            m_trackingObjects.Add(cd, di);
            m_indicators.Add(di);

        }

        void setSoundPlayForIndicator(IDistanceIndicator indi, bool play) {
            bool preValue = m_configs.useSound;
            m_configs.useSound = play;
            indi.UpdateConfig(m_configs);
            m_configs.useSound = preValue;
        }

        public void toggleSphereText() {
            m_configs.useSphereText = !m_configs.useSphereText;
            GlobalStates.isIndicatorEnabled = m_configs.useSphereText;
            foreach (IDistanceIndicator di in m_indicators) {
                di.UpdateConfig(m_configs);
            }
        }

        public void toggleLine() {
            m_configs.useLine = !m_configs.useLine;
            foreach (IDistanceIndicator di in m_indicators) {
                di.UpdateConfig(m_configs);
            }
        }

        public void toggleSound() {
            m_configs.useSound = !m_configs.useSound;
            foreach (IDistanceIndicator di in m_indicators) {
                di.UpdateConfig(m_configs);
            }
        }

        public void toggleSingleSound() {
            if (m_lastSingleSound != null && m_testSingleSound == true) {
                setSoundPlayForIndicator(m_lastSingleSound, false);
            }
            m_testSingleSound = !m_testSingleSound;
        }

        public void toggleIndicatorPrefab() {
            Transform tmp = backupPrefab;
            backupPrefab = indicatorPrefab;
            indicatorPrefab = tmp;

            // Now change all exist object.
            List<IDistanceIndicator> newlist = new List<IDistanceIndicator>(m_indicators.Count);
            foreach (IDistanceIndicator idi in m_indicators) {
                Destroy(idi.gameObject);
                Transform newobj = Instantiate<Transform>(indicatorPrefab);
                newobj.gameObject.SetActive(false);
                IDistanceIndicator nidi = newobj.GetComponent<IDistanceIndicator>();
                nidi.UpdateConfig(m_configs);
                newlist.Add(nidi);
            }
            m_indicators.Clear();
            m_indicators = newlist;

            // Now refresh collider list
            m_trackingObjects.Clear();
        }
    }
}
