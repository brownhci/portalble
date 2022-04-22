using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble.Functions.Grab {
    /// <summary>
    /// This is a Collider Visualier.
    /// It will find collider and generate a new sub object to render the collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GrabColliderVisualizer : MonoBehaviour {
        /// <summary>
        /// Material for collider visualizer
        /// </summary>
        public Material m_visualizerMaterial;
        private GameObject m_visualizer;
        private Collider m_collider;

        /// <summary>
        /// A static global function for visibility setting.
        /// </summary>
        /// <param name="visibility"></param>
        static public void SetVisible(bool visibility) {
            GrabColliderVisualizer[] gcvs = Resources.FindObjectsOfTypeAll<GrabColliderVisualizer>();
            foreach (GrabColliderVisualizer gcv in gcvs) {
                gcv.gameObject.SetActive(visibility);
            }
        }

        // Start is called before the first frame update
        void Start() {
            m_collider = GetComponent<Collider>();
            m_visualizer = null;

            if (m_collider is SphereCollider) {
                m_visualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            else if (m_collider is BoxCollider) {
                m_visualizer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else if (m_collider is CapsuleCollider) {
                m_visualizer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }
            else if (m_collider is MeshCollider) {
                m_visualizer = new GameObject("Collider Visualizer");
                MeshFilter mesh_filter = m_visualizer.AddComponent<MeshFilter>();
                MeshCollider mc = (MeshCollider)m_collider;
                mesh_filter.sharedMesh = mc.sharedMesh;
                m_visualizer.AddComponent<Renderer>();
            }

            // if we have available gameobject. set parent
            if (m_visualizer != null) {
                m_visualizer.name = "Collider Visualizer";
                m_visualizer.transform.SetParent(transform);
                m_visualizer.transform.localPosition = Vector3.zero;
                m_visualizer.transform.localRotation = Quaternion.identity;
                m_visualizer.transform.localScale = Vector3.one;

                // remove collider (created by CreatePrimitive)
                Collider collider_child = m_visualizer.GetComponent<Collider>();
                if (collider_child != null)
                    Destroy(collider_child);

                // set renderer material
                Renderer renderer = m_visualizer.GetComponent<Renderer>();
                if (renderer != null && m_visualizerMaterial != null) {
                    renderer.material = m_visualizerMaterial;
                }
            }
        }
    }
}
