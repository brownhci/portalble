using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Portalble.Functions.Grab;


namespace Portalble
{
    /// <summary>
    /// A base controller class. Provide some useful AR application interfaces.
    /// Like visualize scanned planes and notify when user hit a scanned plane.
    /// It's highly recommended to derive a customized AR controller from this class.
    /// 
    /// This must be changed in order to compile with IOS
    /// Some functions from this files are copied from ARCORE 1.2 - 1.8, 
    /// for integratin with IOS, please be aware of this. 
    /// 
    /// Important!!!!
    /// For future updates, make sure 
    /// 1. Add a mesh collider to the scanned meshes
    /// 2. Setup the layer to DetectedPlane
    /// </summary>
    public class PortalbleGeneralController : MonoBehaviour
    {
        /// <summary>
        /// A static object refering to current portalble controller.
        /// </summary>
        public static PortalbleGeneralController main;
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image
        /// </summary>
        public Camera m_FirstPersonCamera;

        /// <summary>
        /// The id of layer that all unity ARCore planes belong to.
        /// Used for interaction with unity plane. -1 for disable.
        /// </summary>
        public int m_UnityPlaneInteractionLayer;

        /// <summary>
        /// Wether enable interaction with ARCore plane (Just ARCore data, not Unity)
        /// </summary>
        public bool m_EnableARPlaneInteraction;

        /// <summary>
        /// Wether enable interaction with Unity Plane. InteractionLayer must not be negative.
        /// </summary>
        public bool m_EnableUnityPlaneInteraction;

        /// <summary>
        /// Wether enable plane generator. Plane Visualizer must be set.
        /// </summary>
        /// public bool m_EnablePlaneGenerator;

        /// <summary>
        /// Wether enable hand distance warning system.
        /// </summary>
        public bool m_EnableHandDistanceWarning;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes. If PlaneGenerator is disabled, this field is ignored.
        /// </summary>
        // public GameObject m_DetectedPlanePrefab;

        /// <summary>
        /// AR Support class
        /// </summary>
        public PortalbleARSupport m_ARSupport;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        protected List<PortalbleARPlane> m_AllPlanes = new List<PortalbleARPlane>();

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        /// protected List<PortalbleARPlane> m_NewPlanes = new List<PortalbleARPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        protected bool m_IsQuitting = false;

        /// <summary>
        /// Left Hand Manager
        /// </summary>
        protected HandManager m_LeftHandMgr;
        /// <summary>
        /// Left Hand Gesture Control
        /// </summary>
        protected GestureControl m_LeftHandGestureControl;
        /// <summary>
        /// Left Hand Transform (its palm transform, use m_LeftHandMgr for hand object)
        /// </summary>
        protected Transform m_tLeftHand;

        /// <summary>
        /// Get hand manager of left hand
        /// </summary>
        public HandManager LeftHandManager
        {
            get
            {
                if (m_LeftHandMgr == null)
                {
                    UpdateHandReference();
                }
                return m_LeftHandMgr;
            }
        }

        /// <summary>
        /// Get left hand (palm) transform
        /// </summary>
        public Transform LeftHandTransform
        {
            get
            {
                if (m_tLeftHand == null)
                {
                    UpdateHandReference();
                }
                return m_tLeftHand;
            }
        }
        /// <summary>
        /// Get left hand gesture
        /// </summary>
        public string LeftHandGesture
        {
            get
            {
                if (m_LeftHandGestureControl == null)
                    UpdateHandReference();
                return m_LeftHandGestureControl.bufferedGesture();
            }
        }

        /// <summary>
        /// Right Hand Manager
        /// </summary>
        protected HandManager m_RightHandMgr;
        /// <summary>
        /// Right Hand Gesture Control
        /// </summary>
        protected GestureControl m_RightHandGestureControl;
        /// <summary>
        /// Right Hand Transform (its palm transform, use m_LeftHandMgr for hand object)
        /// </summary>
        protected Transform m_tRightHand;

        /// <summary>
        /// Get hand manager of right hand
        /// </summary>
        public HandManager RightHandManager
        {
            get
            {
                if (m_RightHandMgr == null)
                {
                    UpdateHandReference();
                }
                return m_RightHandMgr;
            }
        }

        /// <summary>
        /// Get right hand (palm) transform
        /// </summary>
        public Transform RightHandTransform
        {
            get
            {
                if (m_tRightHand == null)
                {
                    UpdateHandReference();
                }
                return m_tRightHand;
            }
        }

        /// <summary>
        /// Get left hand gesture
        /// </summary>
        public string RightHandGesture
        {
            get
            {
                if (m_RightHandGestureControl == null)
                    UpdateHandReference();
                return m_RightHandGestureControl.bufferedGesture();
            }
        }

        /// <summary>
        /// Get hand manager of active hand
        /// </summary>
        public HandManager ActiveHandManager
        {
            get
            {

                string active = GestureController.getActiveHand();
                switch (active)
                {
                case "NO_HAND":
                    return null;
                case "LEFT_HAND":
                    return LeftHandManager;
                case "RIGHT_HAND":
                    return RightHandManager;
                }
                return null;
            }
        }

        /// <summary>
        /// Get index finger (tip) transform of active hand
        /// </summary>
        public Transform ActiveHandIndexFingerTransform
        {
            get
            {
                string active = GestureController.getActiveHand();
                switch (active)
                {
                    case "NO_HAND":
                        return null;
                    case "LEFT_HAND":
                        return LeftHandTransform.Find("../index/bone3");
                    case "RIGHT_HAND":
                        return RightHandTransform.Find("../index/bone3"); ;
                }
                return null;
            }
        }

        /// <summary>
        /// Get index finger (tip) transform of active hand
        /// </summary>
        public Transform ActiveHandThumbTransform
        {
            get
            {
                string active = GestureController.getActiveHand();
                switch (active)
                {
                    case "NO_HAND":
                        return null;
                    case "LEFT_HAND":
                        return LeftHandTransform.Find("../thumb/bone3");
                    case "RIGHT_HAND":
                        return RightHandTransform.Find("../thumb/bone3"); ;
                }
                return null;
            }
        }

        /// <summary>
        /// Get hand (palm) transform of active hand
        /// </summary>
        public Transform ActiveHandTransform
        {
            get
            {
                string active = GestureController.getActiveHand();
                switch (active)
                {
                    case "NO_HAND":
                        return null;
                    case "LEFT_HAND":
                        return LeftHandTransform;
                    case "RIGHT_HAND":
                        return RightHandTransform;
                }
                return null;
            }
        }
        /// <summary>
        /// Get active hand gesture
        /// </summary>
        public string ActiveHandGesture
        {
            get
            {
                string active = GestureController.getActiveHand();
                switch (active)
                {
                    case "NO_HAND":
                        return null;
                    case "LEFT_HAND":
                        return LeftHandGesture;
                    case "RIGHT_HAND":
                        return RightHandGesture;
                }
                return null;
            }
        }

        /// <summary>
        /// Used for red screen distance check.
        /// </summary>
        public GameObject m_RedScreen;

        /// <summary>
        /// The instance of red screen object.
        /// </summary>
        protected GameObject m_RedScreenInstance;

        /// <summary>
        /// Websocket Manager
        /// </summary>
       protected GestureControl m_GestureController;

        /// <summary>
        /// Get Web socket manager.
        /// </summary>
        public GestureControl GestureController
        {
            get
            {
                if (m_GestureController == null)
                {
                    m_GestureController = GameObject.FindObjectOfType<GestureControl>();
                }
                return m_GestureController;
            }
        }

        /// <summary>
        /// Transparent material for planes
        /// </summary>
        public Material m_planeTransparentMaterial;

        /// <summary>
        /// The material of ar planes
        /// </summary>
        protected Material m_ARPlaneMaterial;

        /// <summary>
        /// if the plane is visible.
        /// </summary>
        private bool m_isPlaneVisible = true;

        // Use this for initialization
        protected virtual void Start()
        {
            // Empty
            m_UnityPlaneInteractionLayer = 0x1 << m_UnityPlaneInteractionLayer;

            if (main != null && main != this)
            {
                Debug.LogWarning("Warning: two or more portalble controllers detected.");
            }
            main = this;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // _UpdateApplicationLifecycle();
            if (m_ARSupport == null)
                return;

            m_AllPlanes = m_ARSupport.getPlanes();

            // If Interaction with ARCore planes is available.
            if (m_EnableARPlaneInteraction)
            {
                if (Input.touchCount == 0)
                    return;

                Touch touch = Input.GetTouch(0);
                // if current touch is valid. (notice: here is a ! mark before the logic)
                if (!(Input.touchCount < 1 || touch.phase != TouchPhase.Began))
                {
                    // if hitting on a gui button, then don't do any interaction.
                    /* condition only passes when no gui is hit */
                    if (EventSystem.current == null || !(EventSystem.current.IsPointerOverGameObject()
                        || EventSystem.current.currentSelectedGameObject != null))
                    {
                        List<PortalbleHitResult> hits = new List<PortalbleHitResult>();
                        if (m_ARSupport.Raycast(touch.position, hits))
                        {
                            OnARPlaneHit(hits[0]);
                        }
                    }
                }
            }

            // If interaction with Unity planes is available
            if (m_EnableUnityPlaneInteraction && m_UnityPlaneInteractionLayer >= 0)
            {
                if (m_FirstPersonCamera)
                {
                    Touch touch;
                    // If current touch is valid. (Notice: Here is a ! mark before the logic)
                    if (!(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began))
                    {
                        // If hitting on a gui button, then don't do any interaction.
                        if (EventSystem.current == null || !(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null))
                        {
                            Ray ray = m_FirstPersonCamera.ScreenPointToRay(touch.position);
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit, 9999f, 0x0fffffff))
                            {
                                if (hit.transform.GetComponent<UnityEngine.XR.ARFoundation.ARPlane>() != null)
                                    OnUnityPlaneHit(hit);
                            }
                        }
                    }
                }
            }

            if (m_EnableHandDistanceWarning)
            {
                // Find two hands
                UpdateHandTransform();

                bool foc = false;

                // Check visualization availability..
                if (m_RedScreenInstance == null)
                {
                    m_RedScreenInstance = GameObject.Instantiate(m_RedScreen);
                }

                // Currently, only check for right hand. Cause we don't know activity of hand.
                if (m_GestureController == null)
                {
                    m_GestureController = GameObject.FindObjectOfType<GestureControl>();
                }
                if (m_GestureController != null && m_RedScreenInstance != null)
                {
                    string activeHand = m_GestureController.getActiveHand();
                    bool distanceCheckResult = false;
                    if (activeHand == "NO_HAND")
                    {
                        m_RedScreenInstance.SetActive(false);
                    }
                    else if (activeHand == "LEFT_HAND")
                    {
                        distanceCheckResult = m_tLeftHand != null && CheckDistance(m_tLeftHand, out foc);
                    }
                    else
                    {
                        distanceCheckResult = m_tRightHand != null && CheckDistance(m_tRightHand, out foc);
                    }

                    if (distanceCheckResult)
                    {
                        if (m_RedScreenInstance != null && m_RedScreenInstance.activeSelf == false)
                            m_RedScreenInstance.SetActive(true);
                        OnHandInappropriateDistance(foc);
                    }
                    else
                    {
                        if (m_RedScreenInstance != null && m_RedScreenInstance.activeSelf == true)
                            m_RedScreenInstance.SetActive(false);
                    }
                }
            }
        }


        /// <summary>
        /// Update hand transform objects
        /// </summary>
        private void UpdateHandTransform()
        {
            if (m_tLeftHand == null)
            {
                GameObject gobj = GameObject.Find("Hand_l");
                if (gobj != null)
                {
                    m_tLeftHand = gobj.transform.Find("palm");
                }
            }
            if (m_tRightHand == null)
            {
                GameObject gobj = GameObject.Find("Hand_r");
                if (gobj != null)
                {
                    m_tRightHand = gobj.transform.Find("palm");
                }
            }
        }

        /// <summary>
        /// Update member variable references to hands
        /// </summary>
        private void UpdateHandReference()
        {
            GameObject gobj = GameObject.Find("Hand_l");
            if (gobj != null)
            {
                m_LeftHandMgr = gobj.GetComponent<HandManager>();
                m_LeftHandGestureControl = gobj.GetComponent<GestureControl>();
            }
            m_tLeftHand = gobj.transform.Find("palm");

            gobj = GameObject.Find("Hand_r");
            if (gobj != null)
            {
                m_RightHandMgr = gobj.GetComponent<HandManager>();
                m_RightHandGestureControl = gobj.GetComponent<GestureControl>();
            }
            m_tRightHand = gobj.transform.Find("palm");
        }

        /// <summary>
        /// Check if given transform is either too far or too close to camera.
        /// </summary>
        /// <param name="t">The transform for checking</param>
        /// <param name="FarOrClose">true if too far, false if too close</param>
        /// <returns>true if the distance is inappropriate while false if the distance is fine</returns>
        private bool CheckDistance(Transform t, out bool FarOrClose)
        {
            if (m_FirstPersonCamera == null)
            {
                FarOrClose = false;
                return false;
            }

            float nearD = GlobalStates.globalConfigFile.NearLeapOutboundDistance;
            float farD = GlobalStates.globalConfigFile.FarLeapOutboundDistance;
            if (nearD == 0f && farD == 0f)
            {
                FarOrClose = false;
                return false;
            }

            Vector3 disV = t.position - m_FirstPersonCamera.transform.position;
            float dis = disV.magnitude;
            if (dis < nearD)
            {
                FarOrClose = false;
                return true;
            }
            else if (dis > farD)
            {
                FarOrClose = true;
                return true;
            }

            FarOrClose = false;
            return false;
        }


        /// <summary>
        /// It's called when user touched an ARCore Plane on the screen.
        /// </summary>
        /// <param name="hit">TrackableHit Object</param>
        public virtual void OnARPlaneHit(PortalbleHitResult hit) { }

        /// <summary>
        /// It's called when user touched an Unity Visualized Plane on the screen.
        /// </summary>
        /// <param name="hit">RaycastHit Object</param>
        public virtual void OnUnityPlaneHit(RaycastHit hit) { }

        /// <summary>
        /// It's called when user's hand stay from Leapmotion either too far or too close.
        /// </summary>
        /// <param name="FarOrClose">ture for too far while false for too close</param>
        /// 
        public virtual void OnHandInappropriateDistance(bool FarOrClose)
        {
            Text text = null;
            if (m_RedScreenInstance != null)
            {
                Transform t = m_RedScreenInstance.transform.Find("Text");
                if (t != null)
                {
                    text = t.GetComponent<Text>();
                }
            }
            // Check if the instance exists.
            if (text != null)
            {
                if (FarOrClose)
                {
                    text.text = "Move Phone or your self closer to the hand";
                }
                else
                {
                    text.text = "Too Close";
                }
            }
        }

        /// <summary>
        /// Set ar scanned plane visibility
        /// </summary>
        /// <param name="visibility">true for visible, false for invisible.</param>
        public void setPlaneVisible(bool visibility) {
            if (m_isPlaneVisible == visibility)
                return;

            // Find the prefab
            ARPlaneManager arpm = FindObjectOfType<ARPlaneManager>();
            if (arpm != null) {
                Renderer prefabRenderer = arpm.planePrefab.GetComponent<Renderer>();
                if (prefabRenderer == null)
                    return;
                if (visibility == false) {
                    // need to remember the old material
                    m_ARPlaneMaterial = prefabRenderer.material;
                    prefabRenderer.material = m_planeTransparentMaterial;
                }
                else {
                    prefabRenderer.material = m_ARPlaneMaterial;
                }

                ARPlane[] planes = FindObjectsOfType<ARPlane>();
                foreach (ARPlane plane in planes) {
                    Renderer render = plane.GetComponent<Renderer>();
                    // no renderer, no need to update
                    if (render == null) {
                        continue;
                    }

                    if (visibility == true) {
                        render.material = m_ARPlaneMaterial;
                    }
                    else {
                        render.material = m_planeTransparentMaterial;
                    }
                }

                m_isPlaneVisible = visibility;
            }
        }

        /// <summary>
        /// The visibility of ar scanned plane.
        /// </summary>
        public bool planeVisibility {
            get {
                return m_isPlaneVisible;
            }
            set {
                setPlaneVisible(value);
            }
        }

        /// <summary>
        /// Set vibration
        /// </summary>
        /// <param name="f"></param>
        public void setVibration(bool f) {
            Grab.Instance.UseVibration = f;
        }

        /// <summary>
        /// Vibration
        /// </summary>
        public bool UseVibration {
            get {
                return Grab.Instance.UseVibration;
            }
            set {
                setVibration(value);
            }
        }

        /// <summary>
        /// Set if grab system use highlight
        /// </summary>
        /// <param name="f">true for using, false for not.</param>
        public void SetGrabHighLight(bool f) {
            Grab.Instance.UseMaterialChange = f;
        }

        /// <summary>
        /// Set and get for grab highlight switch.
        /// </summary>
        public bool GrabHighLight {
            get {
                return Grab.Instance.UseMaterialChange;
            }
            set {
                SetGrabHighLight(value);
            }
        }

        /// <summary>
        /// Set hand action enabled
        /// </summary>
        /// <param name="e">true for enabled, false for not</param>
        public void SetActionRecogEnabled(bool e) {
            HandActionRecog.getInstance().SetEnabled(e);
        }

        public bool HandActionRecogEnabled {
            get {
                return HandActionRecog.getInstance().SystemEnabled;
            }
            set {
                HandActionRecog.getInstance().SystemEnabled = value;
            }
        }
    }
}
