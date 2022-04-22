using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* open-source main */
/*todo, add last grabbed object */
namespace Portalble.Functions.Grab {
    /// <summary>
    /// Release event, triggered when user released an object.
    /// </summary>
    /// <param name="hand">The hand(palm) transform</param>
    /// <param name="grab_object">The object just released</param>
    public delegate void OnReleaseEvent(Transform hand, Grabbable grab_object);

    /// <summary>
    /// This class is a data structure used by Grab System.
    /// </summary>
    class GrabManipulationInfo {
        public Vector3 deltaPosition;
        public Quaternion inverseGrabRotation;
        public Transform grabHolder;

        public Transform indexTip;
        public Transform thumbTip;

        /* ThrowARble queue to record pre-throw history */
        public Queue<Vector3> velList = new Queue<Vector3>();

        /// <summary>
        /// For release velocity calculation
        /// </summary>
        private Vector3 releaseRawVelocity;
        public Vector3 ReleaseRawVelocity {
            get {
                return releaseRawVelocity;
            }
        }
        private Vector3 lastTipCenterPos;
        private bool firstCalculation;

        public Quaternion GetLastRotation() {
            return grabHolder.rotation * inverseGrabRotation;
        }

        public void RotateDeltaPosition(Quaternion rot) {
            deltaPosition = rot * deltaPosition;
        }

        public Vector3 GetTargetPosition() {
            return grabHolder.position + deltaPosition;
        }

        public void UpdateInverseRotation() {
            inverseGrabRotation = Quaternion.Inverse(grabHolder.rotation);
        }

        public void InitVelocityCalculation() {
            releaseRawVelocity = Vector3.zero;
            lastTipCenterPos = Vector3.zero;
            firstCalculation = true;
        }

        public void UpdateReleaseVelocity() {
            Vector3 centerPos = (indexTip.position + thumbTip.position) / 2f;
            if (firstCalculation) {
                lastTipCenterPos = centerPos;
                firstCalculation = false;
            }
            else {
                releaseRawVelocity = ((centerPos - lastTipCenterPos) + releaseRawVelocity) / 2f;
                lastTipCenterPos = centerPos;
            }

            /* copied from Throwable */
            velList.Enqueue(releaseRawVelocity);

            if (velList.Count > 30)
                velList.Dequeue();
        }
    }

    /// <summary>
    /// Grab System
    /// </summary>
    public class Grab : MonoBehaviour {
        private static readonly float REGRAB_COOLDOWN = 0.3f;
        private static readonly float tipDisThreshold = 0.08f;
        /// <summary>
        /// A cooldown time for regrab
        /// </summary>
        private float m_regrabCooldown;
        /// <summary>
        /// Left Hand Transform
        /// </summary>
        private Transform m_tLeftHand;
        private Transform m_tLeftIndexTip;
        private Transform m_tLeftThumbTip;
        /// <summary>
        /// Right Hand Transform
        /// </summary>
        private Transform m_tRightHand;
        private Transform m_tRightIndexTip;
        private Transform m_tRightThumbTip;
        /// <summary>
        /// Left Hand Gesture Control
        /// </summary>
        private GestureControl m_gcLeftHand;
        /// <summary>
        /// Right Hand Gesture Control
        /// </summary>
        private GestureControl m_gcRightHand;
        /// <summary>
        /// Some data used for object manipulation
        /// </summary>
        private GrabManipulationInfo m_grabInfo;

        /// <summary>
        /// If it's grabbing something now.
        /// </summary>
        private bool m_isGrabbing;
        public bool IsGrabbing {
            get {
                return m_isGrabbing;
            }
        }
        /// <summary>
        /// Current selected object.
        /// </summary>
        private Grabbable m_selectObj;
        public Grabbable SelectObject {
            get {
                return m_selectObj;
            }
        }


        /// <summary>
        /// queue for grabbing.
        /// </summary>
        private List<Grabbable> m_leftHandQueue;
        private List<Grabbable> m_rightHandQueue;

        /// <summary>
        /// The last grabbed object
        /// </summary>
        private Grabbable m_lastGrabObject;
        public Grabbable LastGrabbedObject
        {
            get
            {
                return m_lastGrabObject;
            }
        }

        /// <summary>
        /// Release Event.
        /// </summary>
        public OnReleaseEvent OnRelease;

        /// <summary>
        /// A flag for whether using different material for manipulation feedback
        /// </summary>
        private bool m_useMaterialChange = true;
        public bool UseMaterialChange {
            get {
                return m_useMaterialChange;
            }
            set {
                m_useMaterialChange = value;
                if (m_selectObj != null) {
                    m_selectObj.OnMaterialConfigChanged();
                }
            }
        }

        /// <summary>
        /// Whether or not use vibration feedback
        /// </summary>
        private bool m_useVibration = true;
        public bool UseVibration {
            get {
                return m_useVibration;
            }
            set {
                m_useVibration = value;
            }
        }


        // For Singleton.
        private static Grab m_Instance;
        public static Grab Instance {
            get {
                if (m_Instance == null) {
                    //  create a gameobject and bind a script to it
                    GameObject gobj = new GameObject("GrabSystem");
                    m_Instance = gobj.AddComponent<Grab>();
                    if (m_Instance != null) {
                        m_Instance.Reset();
                    }
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Get if the new grabbing system is running
        /// </summary>
        /// <returns>true: it's running, false: not</returns>
        public static bool IsUsing() {
            return (m_Instance != null);
        }

        void Update() {
            // Regrab cooldown
            if (m_regrabCooldown > 0f)
                m_regrabCooldown -= Time.deltaTime;

            // when update, first try to pick out a selected object
            if (m_selectObj == null) {
                // TODO: when we have hand manager, use their active status to choose a queue.
                // Now we only choose right first
                if (m_rightHandQueue.Count > 0) {
                    SelectObj(m_rightHandQueue[0]);
                }
                else if (m_leftHandQueue.Count > 0) {
                    SelectObj(m_leftHandQueue[0]);
                }
                else {
                    return;
                }
            }

            // This line of code could be removed. I wrote it here just to make sure m_selectobj isn't null
            if (m_selectObj == null)
                return;

            //check if the obejct distance is too far so we have to release it
            if (GlobalStates.latestManipulatedObj != null)
            {
                if (Vector3.Distance(GlobalStates.latestManipulatedObj.transform.position, GameObject.Find("R_Palm").transform.position) > 0.8f)
                {
                    // end grab when the latest manipulated object is further than 80 cm from the hand
                    GlobalStates.resetFingerCount = true;
                    EndGrab(true);
                }
            }

           // If there's a selected object check if it's grabbing
            if (m_isGrabbing) {
                // If it's grabbing, check if it's releasing
                if (IsReleaseGesture()) {
                    EndGrab();
                }
                else {
                    ManipulateSelected();
                    m_grabInfo.UpdateReleaseVelocity();
                }
            }
            else {
                if (IsGrabGesture()) {
                    StartGrab();
                }
            }
        }
        /// <summary>
        /// Reset the grab system.
        /// </summary>
        public void Reset() {
            GameObject gobj = GameObject.Find("Hand_l");
            if (gobj != null) {
                m_tLeftHand = gobj.transform;
                if (m_tLeftHand != null){
                    m_tLeftIndexTip = m_tLeftHand.Find("index/bone3");
                    m_tLeftThumbTip = m_tLeftHand.Find("thumb/bone3");
                }
                m_gcLeftHand = gobj.GetComponent<GestureControl>();
            }
            gobj = GameObject.Find("Hand_r");
            if (gobj != null) {
                m_tRightHand = gobj.transform;
                if (m_tRightHand != null) {
                    m_tRightIndexTip = m_tRightHand.Find("index/bone3");
                    m_tRightThumbTip = m_tRightHand.Find("thumb/bone3");
                }
                m_gcRightHand = gobj.GetComponent<GestureControl>();
            }

            if (m_leftHandQueue != null) {
                m_leftHandQueue.Clear();
            }
            m_leftHandQueue = new List<Grabbable>();

            if (m_rightHandQueue != null) {
                m_rightHandQueue.Clear();
            }
            m_rightHandQueue = new List<Grabbable>();

            m_grabInfo = new GrabManipulationInfo();
            m_regrabCooldown = -1f;
        }

        /// <summary>
        /// Check if the hand is right now grabbing something
        /// </summary>
        /// <returns>true if hand is grabbing, else is false</returns>
        public bool IsBusy() {
            return m_isGrabbing;
        }

        /// <summary>
        /// On Destroy
        /// </summary>
        void OnDestroy() {
            if (m_Instance == this) {
                m_Instance = null;
            }
        }

        /// <summary>
        /// Tell system that obj is ready for being grabbed
        /// *** Grabbable is required to any object that can be grabbed***
        /// </summary>
        /// <param name="obj">Grabbable is required to any object that can be grabbed</param>
        internal void WaitForGrabbing(Grabbable obj) {
            // check hand type to add to the corresponding queue.
            if (obj == null)
                return;
            if (obj.IsLeftHanded) {
                // check if left hand queue has this obj
                if (!m_leftHandQueue.Contains(obj)) {
                    m_leftHandQueue.Add(obj);
                }
            }
            else {
                if (!m_rightHandQueue.Contains(obj)) {
                    m_rightHandQueue.Add(obj);
                }
            }
        }

        /// <summary>
        /// The obj notifies that it is no longer grabbable
        /// </summary>
        /// <param name="obj">The grabbable obj</param>
        internal void ExitGrabbingQueue(Grabbable obj) {
            if (obj == null)
                return;


            DeSelectObj(obj);

            if (obj.IsLeftHanded) {
                m_leftHandQueue.Remove(obj);
            } else {
                m_rightHandQueue.Remove(obj);
            }
        }

        /// <summary>
        /// Select an object.
        /// </summary>
        /// <param name="obj"></param>
        private void SelectObj(Grabbable obj) {
            if (obj != null) {
                m_selectObj = obj;
                obj.OnSelected();

                // when select new object, cancel regrab cooldown
                m_regrabCooldown = -1f;
            }
        }

        /// <summary>
        /// Deselect an object.
        /// </summary>
        /// <param name="obj"></param>
        private void DeSelectObj(Grabbable obj) {
            if (m_selectObj == obj) {
                if (m_isGrabbing) {
                    EndGrab();
                }
                m_selectObj = null;
                obj.OnDeSelected();
            }
        }

        /// <summary>
        /// Manipulate selected object
        /// </summary>
        private void ManipulateSelected() {
            // illegal function call.
            if (!m_isGrabbing || m_selectObj == null)
                return;

            // Basic Manipulation
            // TODO: add rotation lock.
            // Get Delta Rotation
            Quaternion rotation = m_grabInfo.GetLastRotation();
            // Rotate grabDeltaPositon get
            m_grabInfo.RotateDeltaPosition(rotation);
            Vector3 expectPos = m_grabInfo.GetTargetPosition();
            Vector3 deltaPos = expectPos - m_selectObj.transform.position;
            // Check grab object lock
            Vector3 posLockVec = m_selectObj.m_configuration.getPositionLockVector();
            if (posLockVec != Vector3.one) {
                // Lock position
                Vector3 relativePos = m_selectObj.transform.InverseTransformDirection(deltaPos);
                relativePos.Scale(posLockVec);
                deltaPos = m_selectObj.transform.TransformDirection(relativePos);
            }
            m_selectObj.transform.position += deltaPos;
            m_selectObj.transform.rotation = rotation * m_selectObj.transform.rotation;
            m_grabInfo.UpdateInverseRotation();
        }

        /// <summary>
        /// Check if the hand is in grab gestures.
        /// </summary>
        /// <returns></returns>
        /// /* ThrowARble has different implementation here, check in the future */
        private bool IsGrabGesture() {
            if (m_selectObj == null)
                return false;

            string gesture;
            if (m_selectObj.IsLeftHanded) {
                // check left hand gesture
                gesture = m_gcLeftHand.bufferedGesture();
            } else {
                gesture = m_gcRightHand.bufferedGesture();
            }
            return (gesture == "pinch" || gesture == "fist");
        }

        /* TODO: investigate how bufferedGesture is used */
        private bool IsReleaseGesture() {
            if (m_selectObj == null)
                return false;

            string gesture;
            if (m_selectObj.IsLeftHanded) {
                // check left hand gesture
                gesture = m_gcLeftHand.bufferedGesture();
            }
            else {
                gesture = m_gcRightHand.bufferedGesture();
            }

            float tipDis = Vector3.Distance(m_grabInfo.thumbTip.position, m_grabInfo.indexTip.position);
            return (gesture == "palm" || (tipDis > tipDisThreshold && gesture != "fist" && gesture != "pinch"));
        }


        /// <summary>
        /// Start to grab
        /// </summary>
        private void StartGrab() {
            // unexpected call
            if (m_isGrabbing || m_selectObj == null)
                return;

            // Event call
            m_selectObj.OnGrabStart();

            // Set grab info
            if (m_selectObj.IsLeftHanded) {
                /// TODO: need to change palm gameobject for metalhand_r in the future
                m_grabInfo.grabHolder = m_tLeftHand.Find("palm/grabHolder");
                m_grabInfo.indexTip = m_tLeftIndexTip;
                m_grabInfo.thumbTip = m_tLeftThumbTip;
            } else {

                /* Get the palm of the metalhand_r within Hand_r model */
                m_grabInfo.grabHolder = m_tRightHand.GetChild(6).GetChild(0).GetChild(0);
                m_grabInfo.indexTip = m_tRightIndexTip;
                m_grabInfo.thumbTip = m_tRightThumbTip;
            }
            m_grabInfo.InitVelocityCalculation();
            m_grabInfo.UpdateInverseRotation();
            m_grabInfo.deltaPosition = m_selectObj.transform.position - m_grabInfo.grabHolder.position;

            m_isGrabbing = true;
        }

        /* copied from throwarble: velocity stack */
        private Vector3 getAverageVel(Queue<Vector3> q, int scaleFactor) {

            Vector3[] t = q.ToArray();
            Vector3 v = new Vector3(0, 0, 0);
            int beginIdx = 10;
            int endIdx = 29;

            if (t.Length < 29)
                endIdx = 29;

            if (t.Length < beginIdx) {
                beginIdx = 0;
                endIdx = t.Length;
            }

            for (int i = beginIdx; i < endIdx; i++)
                v += t[i];
            //while (q.Count > 0)
            //   v += q.Dequeue();
            Vector3 finalSpeed = (v * scaleFactor);
            finalSpeed.y += 1f;
            return (finalSpeed / (endIdx - beginIdx));
        }
        /// <summary>
        /// Stop to grab
        /// </summary>
        private void EndGrab(bool FORCE_END = false) {
            // unexpected call
            if ((!m_isGrabbing || m_selectObj == null) && FORCE_END == false)
                return;

            m_selectObj.OnGrabStop(getAverageVel(m_grabInfo.velList, 15));
            m_isGrabbing = false;
            m_regrabCooldown = REGRAB_COOLDOWN;

            /* ask xiangyu */
            m_lastGrabObject = m_selectObj;
        }

        /// <summary>
        /// Get currently grabbing object
        /// </summary>
        /// <returns>Grabbable object, null if no object is grabbing</returns>
        public Grabbable GetGrabbingObject() {
            if (IsGrabbing == false)
                return null;
            return SelectObject;
        }
    }
}
