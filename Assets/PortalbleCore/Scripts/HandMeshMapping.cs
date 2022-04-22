using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble {
    public class HandMeshMapping : MonoBehaviour {
        public SkinnedMeshRenderer m_renderer;

        private Transform m_parent;

        private bool m_available;
        private bool m_reset;

        private Transform[] m_selfFingers;
        private Transform[] m_targetFingers;

        // Use this for initialization
        void Start() {
            if (m_available == false)
                Init();
        }

        // Update is called once per frame
        void Update() {
            if (m_available && m_reset) {
                for (int i = 0; i < 16; ++i) {
                    m_selfFingers[i].position = m_targetFingers[i].position;
                    m_selfFingers[i].rotation = m_targetFingers[i].rotation;
                }
            }
        }

        public void Init() {
            m_available = false;
            m_reset = false;
            m_parent = transform.parent;
            if (m_parent != null) {
                m_available = true;

                m_selfFingers = new Transform[16];
                m_targetFingers = new Transform[16];

                m_selfFingers[0] = transform.Find("HandArm/HandRoot/palm");

                int counter = 1;
                for (int i = 0; i < 5; ++i) {
                    m_selfFingers[counter] = m_selfFingers[0].GetChild(i);
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
        }

        public void ResetBindPose() {
            SkinnedMeshRenderer rend = m_renderer;
            if (rend != null) {
                Transform[] bones = rend.bones;
                Mesh mesh = rend.sharedMesh;

                //rend.bones = null;

                for (int i = 0; i < 16; ++i) {
                    m_selfFingers[i].position = m_targetFingers[i].position;
                    m_selfFingers[i].rotation = m_targetFingers[i].rotation;
                }
                // Reset bind gesture
                Matrix4x4[] bindposes = mesh.bindposes;
                for (int i = 0; i < bones.Length; ++i) {
                    bindposes[i] = bones[i].worldToLocalMatrix * m_renderer.transform.localToWorldMatrix;
                }
                mesh.bindposes = bindposes;
                m_reset = true;
            }
        }
    }
}
