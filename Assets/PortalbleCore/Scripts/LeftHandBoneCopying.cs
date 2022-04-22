using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandBoneCopying : MonoBehaviour {
    private Transform m_parent;

    public bool m_available;
    private float m_sizeFactor = 1.0f;

    private Transform[] m_selfFingers;
    private Transform[] m_targetFingers;

    /// <summary>
    /// The hand size factor
    /// </summary>
    public float HandSize {
        get {
            return m_sizeFactor;
        }
        set {
            if (value > 0f) {
                m_sizeFactor = value;
                m_selfFingers[0].localScale = Vector3.one * m_sizeFactor;
            }
        }
    }

    // Use this for initialization
    void Start() {
        m_available = false;
        m_parent = transform.parent;
        if (m_parent != null) {
            m_available = true;

            m_selfFingers = new Transform[16];
            m_targetFingers = new Transform[16];
            m_selfFingers[0] = transform.Find("L_Wrist/L_Palm");
            // Get Thumb
            m_selfFingers[1] = m_selfFingers[0].GetChild(0);
            m_selfFingers[2] = m_selfFingers[1].GetChild(0);
            m_selfFingers[3] = m_selfFingers[2].GetChild(0);
            int counter = 4;
            for (int i = 1; i < 5; ++i) {
                Transform tmp = m_selfFingers[0].GetChild(i);
                m_selfFingers[counter] = tmp.GetChild(0);
                for (int j = 0; j < 2; ++j) {
                    m_selfFingers[counter + 1] = m_selfFingers[counter].GetChild(0);
                    counter++;
                }
                counter++;
            }

            m_targetFingers[0] = m_parent.Find("palm");
            counter = 1;
            for (int i = 0; i < 5; ++i) {
                Transform tmp = m_parent.GetChild(i);
                m_targetFingers[counter++] = tmp.GetChild(0);
                m_targetFingers[counter++] = tmp.GetChild(1);
                m_targetFingers[counter++] = tmp.GetChild(2);
            }
        }
        HandSize = GlobalStates.globalConfigFile.MeshHandScale;
    }

    // Update is called once per frame
    void Update() {
        
        if (m_available) {

            // Palm
            Quaternion palmRot = Quaternion.Euler(0f, 0f, -90f) * Quaternion.Euler(90f, 0f, 0f);
            m_selfFingers[0].position = m_targetFingers[0].position;
            m_selfFingers[0].rotation = m_targetFingers[0].rotation * palmRot;
            m_selfFingers[0].position += m_selfFingers[0].right * 0.055f;

            Vector3 palmZ = m_selfFingers[0].forward;

            // For thumb
            Quaternion thumFT = Quaternion.FromToRotation(m_selfFingers[1].right, m_targetFingers[1].up);
            m_selfFingers[1].rotation = thumFT * m_selfFingers[1].rotation;
            thumFT = Quaternion.FromToRotation(m_selfFingers[2].right, m_targetFingers[2].up);
            m_selfFingers[2].rotation = thumFT * m_selfFingers[2].rotation;
            thumFT = Quaternion.AngleAxis(90f, m_selfFingers[2].forward);
            m_selfFingers[3].rotation = Quaternion.LookRotation(m_selfFingers[2].forward, thumFT * m_targetFingers[3].up);

            // For each finger (except thumb)
            for (int i = 1; i < 5; ++i) {

                int basicIndex = i * 3 + 1;
                // For Finger bones, the Y axis is most stable. Thus mainly calculate other axis from Y axis and palm
                Vector3 candZ = Vector3.Cross(m_targetFingers[basicIndex].up, m_targetFingers[basicIndex + 1].up);
                if (candZ == Vector3.zero) {
                    continue;
                }
                if (Vector3.Dot(candZ, palmZ) < 0f)
                    candZ = -candZ;
                // Now we know basic finger's two axis
                Quaternion axisRot = Quaternion.AngleAxis(90f, candZ);
                m_selfFingers[basicIndex].rotation = Quaternion.LookRotation(candZ, axisRot * m_targetFingers[basicIndex].up);
                m_selfFingers[basicIndex + 1].rotation = Quaternion.LookRotation(candZ, axisRot * m_targetFingers[basicIndex + 1].up);
                m_selfFingers[basicIndex + 2].rotation = Quaternion.LookRotation(candZ, axisRot * m_targetFingers[basicIndex + 2].up);
            }
        }
    }
}
