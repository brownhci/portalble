using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble;

namespace Portalble.Examples.BasicGrab {
    public class GrabDemoController : PortalbleGeneralController {
        public Transform placePrefab;
        public float offset = 0.15f;
        public bool EnablePlacement;

        public GameObject m_startButton;
        public GameObject m_endButton;

        private bool m_start_show = true;

        public override void OnARPlaneHit(PortalbleHitResult hit) {
            if (!EnablePlacement)
                return;

            base.OnARPlaneHit(hit);

            if (placePrefab != null) {
                Transform m =  Instantiate(placePrefab, hit.Pose.position + hit.Pose.rotation * Vector3.up * offset, hit.Pose.rotation);
                m.name = "GrabbingCube";
            }
        }

        public void toggleStartEnd()
        {
            m_start_show = !m_start_show;
            m_endButton.SetActive(!m_start_show);
            m_startButton.SetActive(m_start_show);
        }
    }
}