using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinitePlaneFloor : MonoBehaviour {
    private Transform m_bindPlane;

    public void BindToPlane(Transform t) {
        if (m_bindPlane != null) {
            MeshCollider mc = m_bindPlane.GetComponent<MeshCollider>();
            if (mc != null) {
                mc.enabled = true;
            }
        }

        m_bindPlane = t;
        if (m_bindPlane != null) {
            MeshCollider mc = m_bindPlane.GetComponent<MeshCollider>();
            if (mc != null) {
                mc.enabled = false;
            }
        }
    }
}
