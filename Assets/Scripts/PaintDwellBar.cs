using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Portalble;

namespace Portalble {
    public class PaintDwellBar : MonoBehaviour {
        public Slider m_Bar;

        private Transform m_bindObj;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (m_bindObj != null) {
                transform.position = m_bindObj.position;
                transform.LookAt(m_bindObj.position - m_bindObj.up, -m_bindObj.forward);
            }
        }

        public void SetPercent(float p) {
            if (m_Bar != null) {
                if (p > 1f)
                    p = 1f;
                m_Bar.value = p;
            }
        }

        public void BindToObject(Transform t) {
            m_bindObj = t;
        }
    }
}