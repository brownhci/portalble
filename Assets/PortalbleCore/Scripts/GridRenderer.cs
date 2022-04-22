using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble {
    public class GridRenderer : MonoBehaviour {
        // make it singleton
        static protected GridRenderer _instance;
        static public GridRenderer Instance {
            get {
                return _instance;
            }
        }

        static public void setVisibility(bool flag) {
            if (_instance != null) {
                _instance.gameObject.SetActive(flag);
            }
        }

        /// <summary>
        /// Grid orientation
        /// </summary>
        public enum GridOrientation {
            WorldSpace,
            CameraSpace,
            Fixed
        }

        /// <summary>
        /// Material for lines
        /// </summary>
        [SerializeField]
        protected Material m_lineMaterial;

        public Material LineMaterial {
            get {
                return m_lineMaterial;
            }
            set {
                m_lineMaterial = value;
                UpdateLineMaterial();
            }
        }

        /// <summary>
        /// Size of each cell.
        /// </summary>
        [SerializeField]
        protected float m_gridSize = 0.2f;
        public float GridSize {
            get {
                return m_gridSize;
            }
            set {
                if (value > 0.0f) {
                    m_gridSize = value;
                    UpdateLineNumber();
                }
            }
        }

        /// <summary>
        /// The boundary of grid.
        /// </summary>
        [SerializeField]
        protected float m_gridBound = 3.0f;
        public float GridBound {
            get {
                return m_gridBound;
            }
            set {
                if (value > 0.0f) {
                    m_gridBound = value;
                    UpdateLineNumber();
                }
            }
        }

        [SerializeField]
        protected GridOrientation m_orientation = GridOrientation.WorldSpace;
        public GridOrientation Orientation {
            get {
                return m_orientation;
            }
            set {
                m_orientation = value;
            }
        }

        [SerializeField]
        protected float m_lineWidth = 0.0075f;
        public float LineWidth {
            get {
                return m_lineWidth;
            }
            set {
                if (value > 0) {
                    m_lineWidth = value;
                    UpdateLineWidth();
                }
            }
        }

        /// <summary>
        /// Line Renderers
        /// </summary>
        private List<LineRenderer> m_lineRenderers;

        // Start is called before the first frame update
        void Start() {
            // check if it's doubled
            if (_instance == null) {
                _instance = this;
            }
            else if (_instance != this) {
                Debug.LogWarning("You have two GridRenderer instances in the scene, one of them " + gameObject.name + " will be destroyed");
                Destroy(gameObject);
                return;
            }

            m_lineRenderers = new List<LineRenderer>();
            if (m_gridSize < 0)
                m_gridSize = 0.1f;
            if (m_gridBound < 0)
                m_gridBound = 3.0f;
            UpdateLineNumber();
            UpdateLineMaterial();
            UpdateLineWidth();
            UpdateLinePosition();
        }

        // Update is called once per frame
        void Update() {
            // No material, then don't draw lines
            if (m_lineRenderers == null)
                return;

            if (m_orientation == GridOrientation.Fixed) {
                // we still need to follow camera but in local space
                if (Camera.main != null) {
                    Vector3 local_cam = transform.InverseTransformPoint(Camera.main.transform.position);
                    local_cam = local_cam / m_gridSize;
                    Vector3 anchor_pos = new Vector3(Mathf.Round(local_cam.x), Mathf.Round(local_cam.y), Mathf.Round(local_cam.z));
                    anchor_pos = anchor_pos * m_gridSize;
                    transform.position = transform.TransformPoint(anchor_pos);
                }
                return;
            }

            if (m_orientation == GridOrientation.CameraSpace && Camera.main != null) {
                // follow the camera
                Transform cam_trans = Camera.main.transform;
                if (cam_trans != null) {
                    transform.position = cam_trans.position;
                    transform.rotation = cam_trans.rotation;
                }
            }
            else if (m_orientation == GridOrientation.WorldSpace && Camera.main != null) {
                // Compute the anchor based on Camera Pos
                Vector3 cam_pos = Camera.main.transform.position;
                cam_pos = cam_pos / m_gridSize;
                Vector3 anchor_pos = new Vector3(Mathf.Round(cam_pos.x), Mathf.Round(cam_pos.y), Mathf.Round(cam_pos.z));
                anchor_pos = anchor_pos * m_gridSize;
                transform.position = anchor_pos;
                transform.rotation = Quaternion.identity;
            }
        }

        void UpdateLineNumber() {
            // compute number
            int size = (int)(m_gridBound / m_gridSize);
            if (size <= 0)
                size = 1;

            int lineNumber = (3 * size + 3) * (size + 1);

            // NOTICE:If the line number is often changed, it better make a buffer here.
            if (m_lineRenderers.Count > lineNumber) {
                for (int i = lineNumber; i < m_lineRenderers.Count; ++i) {
                    Destroy(m_lineRenderers[i].gameObject);
                }
            }
            else if (m_lineRenderers.Count < lineNumber) {
                for (int i = m_lineRenderers.Count; i < lineNumber; ++i) {
                    GameObject gobj = new GameObject("Grid_Line_Renderer");
                    gobj.transform.parent = transform;
                    gobj.transform.localPosition = Vector3.zero;
                    gobj.transform.localRotation = Quaternion.identity;
                    LineRenderer lr = gobj.AddComponent<LineRenderer>();
                    m_lineRenderers.Add(lr);
                }
            }
        }

        void UpdateLineMaterial() {
            if (m_lineMaterial == null)
                return;
            foreach (LineRenderer lr in m_lineRenderers) {
                lr.material = m_lineMaterial;
            }
        }

        void UpdateLinePosition() {
            int size = (int)(m_gridBound / m_gridSize);
            float half_bound = m_gridBound / 2.0f;
            size = size + 1;
            for (int i = 0; i < size; ++i) {
                float plane_z = -half_bound + i * m_gridSize;

                // draw X axis
                for (int j = 0; j < size; ++j) {
                    float plane_y = -half_bound + j * m_gridSize;
                    int index = i * (2 * size) + j;
                    Vector3[] p = new Vector3[2] {new Vector3(-half_bound, plane_y, plane_z),
                                                    new Vector3(half_bound, plane_y, plane_z)};
                    m_lineRenderers[index].useWorldSpace = false;
                    m_lineRenderers[index].SetPositions(p);
                }

                // draw Y axis
                for (int j = 0; j < size; ++j) {
                    float plane_x = -half_bound + j * m_gridSize;
                    int index = i * (2 * size) + size + j;
                    Vector3[] p = new Vector3[2] {new Vector3(plane_x, -half_bound, plane_z),
                                                new Vector3(plane_x, half_bound, plane_z)};
                    m_lineRenderers[index].useWorldSpace = false;
                    m_lineRenderers[index].SetPositions(p);
                }
            }

            // draw Z axis
            int base_index = size * (size + size);
            for (int i = 0; i < size; ++i) {
                float plane_x = -half_bound + i * m_gridSize;
                for (int j = 0; j < size; ++j) {
                    float plane_y = -half_bound + j * m_gridSize;
                    int index = base_index + i * size + j;
                    Vector3[] p = new Vector3[2] { new Vector3(plane_x, plane_y, -half_bound),
                                                    new Vector3(plane_x, plane_y, half_bound)};
                    m_lineRenderers[index].useWorldSpace = false;
                    m_lineRenderers[index].SetPositions(p);
                }
            }
        }

        void UpdateLineWidth() {
            foreach (LineRenderer lr in m_lineRenderers) {
                lr.startWidth = m_lineWidth;
                lr.endWidth = m_lineWidth;
            }
        }

        public Vector3 GetSnapPoint(Vector3 point) {
            Vector3 localP = transform.InverseTransformPoint(point);
            Vector3 half_bound = (m_gridBound / 2.0f) * Vector3.one;
            localP = localP + half_bound;
            // compute the nearest point
            Vector3 gridSpace = localP / m_gridSize;
            Vector3 roundGrid = new Vector3(Mathf.Round(gridSpace.x), Mathf.Round(gridSpace.y), Mathf.Round(gridSpace.z));
            gridSpace = roundGrid * m_gridSize - half_bound;
            return transform.TransformPoint(gridSpace);
        }
    }
}