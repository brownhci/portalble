using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using GoogerueARCore;
//using GoogleARCore.Examples.Common;
using Portalble;

/* make sure to update the commented session */
public class CupPlaceController : PortalbleGeneralController {
    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject CupPrefab;

    /// <summary>
    /// The rotation in degrees need to apply to model when the Andy model is placed.
    /// </summary>
    private const float k_ModelRotation = 180.0f;

    public Transform m_PlaneColliderPrefab;
    private bool m_SetInfinitePlane = false;
    private Transform m_InfinitePlane = null;
    private InfinitePlaneFloor m_InfinitePlaneComp = null;

    public Image m_PlaneSwitchButton;

    private bool m_HandActionToggleFlag = true;

    public void StartPickInfinitePlane() {
        m_SetInfinitePlane = !m_SetInfinitePlane;
        UpdateButton();
    }

    private void UpdateButton() {
        if (m_PlaneSwitchButton != null) {
            if (m_SetInfinitePlane) {
                m_PlaneSwitchButton.color = Color.grey;
            }
            else {
                m_PlaneSwitchButton.color = Color.white;
            }
        }
    }

    public void ToggleShadowVisibility() {
        GlobalStates.isShadowVisible = !GlobalStates.isShadowVisible;
        GameObject[] planes = GameObject.FindGameObjectsWithTag("PlaneGeneratedByARCore");

        foreach (GameObject plane in planes) {
            if (plane != null) {
                /* update commented session */
                // DetectedPlaneVisualizer dpv = plane.GetComponent<DetectedPlaneVisualizer>();
                //if (dpv != null) {
                //    dpv.updateVisible();
                //}
            }
        }
    }

    /* update commented session */
    //public override void OnARCorePlaneHit(TrackableHit hit) {
    //    if (m_SetInfinitePlane)
    //        return;
    //    // Place a cup
    //    if ((hit.Trackable is DetectedPlane) &&
    //            Vector3.Dot(m_FirstPersonCamera.transform.position - hit.Pose.position,
    //                hit.Pose.rotation * Vector3.up) < 0) {
    //        Debug.Log("Hit at back of the current DetectedPlane");
    //    }
    //    else {
    //        // Choose the Andy model for the Trackable that got hit.
    //        GameObject prefab = CupPrefab;

    //        // Instantiate Andy model at the hit pose.
    //        var andyObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

    //        // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
    //        andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
    //    }
    //}

    /* update commented session */
    //public override void OnUnityPlaneHit(RaycastHit hit) {
    //    if (m_SetInfinitePlane) {
    //        if (m_InfinitePlane == null) {
    //            m_InfinitePlane = Instantiate<Transform>(m_PlaneColliderPrefab);
    //            m_InfinitePlaneComp = m_InfinitePlane.GetComponent<InfinitePlaneFloor>();
    //        }
    //        /* update commented session */
    //        //DetectedPlaneVisualizer visualizer = hit.transform.GetComponent<DetectedPlaneVisualizer>();
    //        //DetectedPlane dp = visualizer.GetDetectedPlane();
    //        m_InfinitePlane.position = dp.CenterPose.position;
    //        m_InfinitePlane.localScale = new Vector3(500f, 1f, 500f);
    //        m_InfinitePlane.rotation = dp.CenterPose.rotation;
    //        m_InfinitePlaneComp.BindToPlane(hit.transform);
    //        m_SetInfinitePlane = false;
    //        UpdateButton();
    //    }
    //}

    public void ToggleHandAction() {
        m_HandActionToggleFlag = !m_HandActionToggleFlag;
        HandActionRecog.getInstance().SetEnabled(m_HandActionToggleFlag);
    }
}
