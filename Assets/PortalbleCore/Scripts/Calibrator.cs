using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace Portalble {
    /// <summary>
    /// A controller for Portal-ble Calibration
    /// </summary>
    public class Calibrator : MonoBehaviour {
        public Transform m_Stage1;
        public Transform m_Stage2;
        public Transform m_Stage3;

        public Sync m_leftHandSync;
        public Sync m_rightHandSync;
        public LeftHandBoneCopying m_leftHandBC;
        public RightHandBoneCopying m_rightHandBC;
        public Transform m_leftHandPalm;
        public Transform m_rightHandPalm;

        public string m_nextSceneName;

        private int m_Stage;
#if UNITY_EDITOR
        private const float m_touchScale = 0.005f;
#else
        private const float m_touchScale = 0.0005f;
#endif

        // Use this for initialization
        void Start() {
            m_Stage = 0;
        }

        // Update is called once per frame
        void Update() {
            switch(m_Stage) {
                case 0:
                    Update_Stage_0();
                    break;
                case 1:
                    Update_Stage_1();
                    break;
                case 2:
                    Update_Stage_2();
                    break;
            }
        }

        private void Update_Stage_0() {
            // Check touch
            Touch touch;
            if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Moved) {
                if (EventSystem.current == null || !(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)) {
                    Vector3 leapoffset = m_leftHandSync.InitialHandOffset;
                    leapoffset.x += touch.deltaPosition.x * m_touchScale;
                    leapoffset.y += touch.deltaPosition.y * m_touchScale;
                    m_leftHandSync.InitialHandOffset = leapoffset;
                    m_rightHandSync.InitialHandOffset = leapoffset;
                }
            }
        }

        private void Update_Stage_1() {

        }

        private void Update_Stage_2() {

        }

        public void S0_Slider_OnValueChanged(float newvalue) {
            m_leftHandBC.HandSize = newvalue;
            m_rightHandBC.HandSize = newvalue;
        }

        public void S0_OK_OnHit() {
            GlobalStates.globalConfigFile.HandOffset = m_leftHandSync.InitialHandOffset;
            GlobalStates.globalConfigFile.MeshHandScale = m_leftHandBC.HandSize;
            m_Stage = 1;
            m_Stage1.gameObject.SetActive(false);
            m_Stage2.gameObject.SetActive(true);
        }

        public void S1_OK_OnHit() {
            GestureControl ws = FindObjectOfType<GestureControl>();
            if (ws != null) {
                string active_hand = ws.getActiveHand();
                float dis = -1.0f;
                if (active_hand == "LEFT_HAND") {
                    dis = Vector3.Distance(m_leftHandPalm.position, Camera.main.transform.position);
                }
                else if (active_hand == "RIGHT_HAND") {
                    dis = Vector3.Distance(m_rightHandPalm.position, Camera.main.transform.position);
                }

                if (dis >= 0f) {
                    GlobalStates.globalConfigFile.NearLeapOutboundDistance = dis;
                    m_Stage = 2;
                    m_Stage2.gameObject.SetActive(false);
                    m_Stage3.gameObject.SetActive(true);
                }
            }
        }

        public void S2_OK_OnHit() {
            GestureControl ws = FindObjectOfType<GestureControl>();
            if (ws != null) {
                string active_hand = ws.getActiveHand();
                float dis = -1.0f;
                if (active_hand == "LEFT_HAND") {
                    dis = Vector3.Distance(m_leftHandPalm.position, Camera.main.transform.position);
                }
                else if (active_hand == "RIGHT_HAND") {
                    dis = Vector3.Distance(m_rightHandPalm.position, Camera.main.transform.position);
                }

                if (dis > GlobalStates.globalConfigFile.NearLeapOutboundDistance) {
                    GlobalStates.globalConfigFile.FarLeapOutboundDistance = dis;
                    m_Stage = 3;
                    m_Stage3.gameObject.SetActive(false);
                    OnFinished();
                }
            }
        }

        private void OnFinished() {
            GlobalStates.globalConfigFile.SaveConfig();
            SceneManager.LoadScene(m_nextSceneName);
        }

        public void OnSkip() {
            SceneManager.LoadScene(m_nextSceneName);
        }
    }
}
