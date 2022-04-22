using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Mediapipe.HandTracking.ARCore {
    public class ARCoreDepthSetting : DepthSetting {

        [SerializeField]
        private ARRaycastManager raycast_manager = null;

        private List<ARRaycastHit> out_hits = new List<ARRaycastHit>();
        private Pose current_pose = default;
        private float current_distance = default;

        private new void Awake() {
            // instantiate depth estimate object
            depth_estimate = new DepthEstimate();
            base.Awake();
        }

        public void FixedUpdate() {
            // Basically, if a plane has been detected update the current_distance variable
            if (raycast_manager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), out_hits, TrackableType.PlaneWithinPolygon))
            {
                 
                current_pose = out_hits[0].pose;
                current_distance = 0.5f; // If this is always 0.5f then we can just use a boolean??
            }
        }

        public bool SetDepth() {
            // If no plane has been detected...
            if (current_distance == default) return false;

            // Otherwise once the plane has been detected, enable the depth estimate input and return
            depth_estimate.default_depth = current_distance;
            base.EnableDepthInput();
            return true;
        }
    }
}