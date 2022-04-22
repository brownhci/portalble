using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portalble {
    /// <summary>
    /// Dwell System
    /// Declare a Dwell object and call doUpdate every frame to input a position.
    /// Call isReady() to check whether a dwell happenes. Call setScaleFactor() to set the sensitivity of system
    /// Call setCooldown() when isReady() to avoid continuously triggering dwell.
    /// </summary>
    public class Dwell {
        /* if the dwell counter is not counting,
         * it might be the scene's scale factor 
         * is not good fit, try to lower this number
         * in case that happens */
        public float ScaleFactor = 1f;

        /// <summary>
        /// Last Input pos
        /// </summary>
        private Vector3 m_prevPos;
        /// <summary>
        /// Current Input pos
        /// </summary>
        private Vector3 m_currPos;
        /// <summary>
        /// A timer for triggering a dwell
        /// </summary>
        private float dwellTriggerCounter = 0;
        /// <summary>
        /// How long the position is stable to trigger a dwell (in sec)
        /// </summary>
        private float dwellTriggerTime = 1.5f;
        /// <summary>
        /// A movement from previous pos to current pos
        /// </summary>
        private float m_movement = 0;
        /// <summary>
        /// Whether dwell is triggering.
        /// </summary>
        private bool m_isTrigger;
        /// <summary>
        /// How long between two dwell trigger at the same position
        /// </summary>
        private float m_triggerInterval = 0f;


        /// <summary>
        /// Constructor
        /// </summary>
        public Dwell() {
            m_prevPos = new Vector3(0, 0, 0);
            m_currPos = new Vector3(0, 0, 0);
            m_isTrigger = false;
        }

        /// <summary>
        /// Call this to update dwell status, call every fram is recommended
        /// </summary>
        public void doUpdate(Vector3 position) {  
            if (m_triggerInterval > 0f) {
                m_triggerInterval -= Time.unscaledDeltaTime;
            }

            m_currPos = position;
            if (isStable()) {
                dwellTriggerCounter += Time.unscaledDeltaTime;
                /* check on and off */
                if (!m_isTrigger) {
                    if (dwellTriggerCounter > dwellTriggerTime) {
                        m_isTrigger = true;
                    }
                }
            }
            else {
                m_isTrigger = false;
                dwellTriggerCounter = 0f;
                m_triggerInterval = 0f;
            }
            m_prevPos = m_currPos;
        }

        /// <summary>
        /// Reset trigger status
        /// </summary>
        public void resetMTrigger() {
            m_isTrigger = false;
        }

        /// <summary>
        /// Get if a dwell is triggered.
        /// </summary>
        /// <returns>true for triggered, while false for not triggered</returns>
        public bool isReady() {
            if (m_triggerInterval <= 0f)
                return m_isTrigger;
            else
                return false;
            /*return isStable(); */
        }

        /// <summary>
        /// Dwell Trigger Time.
        /// </summary>
        public float TriggerTime {
            get {
                return dwellTriggerTime;
            }
            set {
                if (value > 0) {
                    dwellTriggerTime = value;
                }
                else {
                    dwellTriggerTime = 1.5f;
                }
            }
        }

        /// <summary>
        /// Set a cooldown time in order to avoid continuously triggering dwell.
        /// </summary>
        /// <param name="interval"></param>
        public void SetCooldown(float interval) {
            if (interval < 0f)
                interval = 0f;
            m_triggerInterval = interval;
        } 

        /// <summary>
        /// Get dwell point.
        /// </summary>
        /// <returns>Dwell point</returns>
        public Vector3 GetDwellPoint() {
            return m_currPos;
        }

        private bool isStable() {
            m_movement = (m_currPos - m_prevPos).sqrMagnitude * ScaleFactor / Time.unscaledDeltaTime;
            return (m_movement < 0.075f);
        }

        /// <summary>
        /// Get process of dwell
        /// </summary>
        /// <returns>a float in 0..1</returns>
        public float getProcess() {
            return Mathf.Clamp01(dwellTriggerCounter / dwellTriggerTime);
        }

        /// <summary>
        /// Set scale factor for different use case.
        /// </summary>
        /// <param name="sf">the factor, the higher, the more sensitive. positive float number</param>
        /// <returns></returns>
        public void setScaleFactor(float sf) {
            if (sf <= 0f)
                sf = 1.0f;
            ScaleFactor = sf;
        }
    }
}