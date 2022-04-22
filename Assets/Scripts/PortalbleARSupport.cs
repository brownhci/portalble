using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace Portalble
{
    /// <summary>
    /// A wrapper for unity XR planes. This wrapper only used in the case that unity XR data
    /// structures change in the future.
    /// </summary>
    public class PortalbleARPlane
    {
        public PortalbleARPlane(ARPlane plane)
        {
            m_arplane = plane;
        }

        private ARPlane m_arplane;

        public Vector2 centerInPlaceSpace { get { return m_arplane.centerInPlaneSpace; } }
        public Vector3 center { get { return m_arplane.center; } }
        public Vector3 normal { get { return m_arplane.normal; } }
        public Vector2 extents { get { return m_arplane.extents; } }
        public Vector2 size { get { return m_arplane.size; } }

        public static PortalbleARPlane getARPlaneFromUnityObject(GameObject gobj) {
            ARPlane arplane = gobj.GetComponent<ARPlane>();
            if (gobj != null && arplane != null) {
                return new PortalbleARPlane(arplane);
            }
            return null;
        }
    }

    /// <summary>
    /// A wrapper for unity XR ray cast result
    /// </summary>
    public class PortalbleHitResult
    {
        public PortalbleHitResult(ARRaycastHit hit)
        {
            m_hit = hit;
        }

        private ARRaycastHit m_hit;

        public Pose Pose { get { return m_hit.pose; } }
    }

    [RequireComponent(typeof(ARPlaneManager))]
    public class PortalbleARSupport : MonoBehaviour
    {

        public List<PortalbleARPlane> getPlanes()
        {
            List<PortalbleARPlane> planes = new List<PortalbleARPlane>();
            foreach (var plane in m_planeManager.trackables)
            {
                planes.Add(new PortalbleARPlane(plane));
            }

            return planes;
        }

        public bool Raycast(Vector2 screenPoint,
            List<PortalbleHitResult> hitResults,
            TrackableType trackableTypes = TrackableType.PlaneWithinPolygon)
        {
            ARRaycastManager arrm = GetComponent<ARRaycastManager>();
            if (arrm != null)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (arrm.Raycast(screenPoint, hits, trackableTypes))
                {
                    hitResults.Clear();
                    foreach (ARRaycastHit hit in hits)
                    {
                        hitResults.Add(new PortalbleHitResult(hit));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool Raycast(Ray ray,
            List<PortalbleHitResult> hitResults,
            TrackableType trackableTypes = TrackableType.PlaneWithinPolygon)
        {
            ARRaycastManager arrm = GetComponent<ARRaycastManager>();
            if (arrm != null)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (arrm.Raycast(ray, hits, trackableTypes))
                {
                    hitResults.Clear();
                    foreach (ARRaycastHit hit in hits)
                    {
                        hitResults.Add(new PortalbleHitResult(hit));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        void Awake()
        {
            m_planeManager = GetComponent<ARPlaneManager>();
        }

        ARPlaneManager m_planeManager;
    }
}
