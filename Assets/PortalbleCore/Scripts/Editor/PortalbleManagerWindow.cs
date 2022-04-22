using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Portalble {
    public class PortalbleManagerWindow : EditorWindow {


        /// <summary>
        /// Set current object to a grabbable object
        /// </summary>
        [MenuItem("Portalble/Set Grabbable")]
        public static void SetGrabbable() {
            GameObject targetObj = Selection.activeGameObject;
            if (targetObj != null) {

                // check if the target has children colliders, ask if composite collider is needed.
                bool composite = false;
                Collider[] childColliders = targetObj.GetComponentsInChildren<Collider>();
                if (childColliders.Length > 1) {
                    composite = UnityEditor.EditorUtility.DisplayDialog("Composite Collider", "Do you want a bounding box " +
                        "including all children colliders?", "Yes", "No");
                    if (composite && targetObj.transform.rotation != Quaternion.identity) {
                        Debug.LogWarning("The object " + targetObj.name + " is trying to be set as a grabbable object with " +
                            "composite grab collider. But it has non-zero rotation, which may make grab collider behave wrongly.");
                    }
                }

                Collider cd = targetObj.GetComponent<Collider>();
                // No collider and no composite, we can do nothing.
                if (cd == null && !composite) {
                    UnityEditor.EditorUtility.DisplayDialog("Error", "No available colliders components. " +
                        "Please make sure objects are active.", "OK");
                    return;
                }

                // check ridigbody
                Rigidbody rd = targetObj.GetComponent<Rigidbody>();
                if (rd == null) {
                    rd = targetObj.AddComponent<Rigidbody>();
                    rd.useGravity = false;
                }

                // Create child
                GameObject checkobj = ObjectFactory.CreateGameObject("GrabCollider");
                targetObj.layer = 11;
                checkobj.transform.parent = targetObj.transform;
                checkobj.transform.localPosition = Vector3.zero;
                checkobj.transform.localRotation = Quaternion.identity;
                checkobj.transform.localScale = Vector3.one;

                if (composite) {
                    // calculate biggest bounding box
                    Bounds bounds;
                    if (cd != null) {
                        bounds = new Bounds(cd.bounds.center, cd.bounds.size);
                    }
                    else {
                        bounds = new Bounds();
                    }

                    foreach (var collider in childColliders) {
                        bounds.Encapsulate(collider.bounds);
                    }


                    BoxCollider cbc = checkobj.AddComponent<BoxCollider>();
                    cbc.center = checkobj.transform.InverseTransformPoint(bounds.center);
                    cbc.size = bounds.size;
                }
                else {
                    // Copy collider
                    UnityEditorInternal.ComponentUtility.CopyComponent(cd);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(checkobj);
                }

                // Set trigger
                Collider ccd = checkobj.GetComponent<Collider>();
                ccd.isTrigger = true;
                // Add rigidbody
                Rigidbody crb = checkobj.AddComponent<Rigidbody>();
                crb.useGravity = false;
                // Add Script
                Portalble.Functions.Grab.Grabbable grabbable =
                    targetObj.AddComponent<Portalble.Functions.Grab.Grabbable>();
                Portalble.Functions.Grab.GrabCollider cgc =
                    checkobj.AddComponent<Portalble.Functions.Grab.GrabCollider>();

                Functions.Grab.GrabColliderVisualizer coliderViz = checkobj.AddComponent<Functions.Grab.GrabColliderVisualizer>();
                coliderViz.m_visualizerMaterial = AssetDatabase.LoadAssetAtPath(colliderVizMatPath,
                    typeof(Material)) as Material;

                cgc.m_automaticExpand = false;
                cgc.m_grabObj = grabbable;
                grabbable.m_selectedMaterial = AssetDatabase.LoadAssetAtPath(grabMaterialPath,
                    typeof(Material)) as Material;
                grabbable.m_grabbedMaterial = AssetDatabase.LoadAssetAtPath(grabMaterialPath,
                    typeof(Material)) as Material;
                // Set tag
                targetObj.tag = "InteractableObj";
            }
        }

        /// <summary>
        /// Google Arcore path (Partially legacy)
        /// </summary>
        private const string googleARCorePath = "Assets/GoogleARCore/Prefabs/";

        private const string detectedPlanePath = "Assets/GoogleARCore/Examples/Common/" +
            "Prefabs/DetectedPlaneVisualizer.prefab";

        private const string defaultPortalblePrefabPath = "Assets/Portalble_Core/Prefabs/";

        private const string outlineMaterialPath = "Assets/Materials/Glow.mat";

        private const string grabMaterialPath = "Assets/Materials/Glow.mat";

        private const string transparentMaterialPath = "Assets/Materials/TransparentARGrid.mat";

        private const string colliderVizMatPath = "Assets/PortalbleCore/Scripts/Grab/ColliderVisualizer.mat";
        /// <summary>
        /// New support. Unity XR
        /// </summary>
        private const string unityXRPrefabPath = "Assets/Prefabs";

        // Basic UI
        private void OnGUI() {
            GUILayout.Label("Portalble-Component", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            //------------------------------------------------------------------------------
            // Check Google ARCore
            //bool testRet = false;


            //------------------------------------------------------------------------------
            // Check Portalble Controller
            if (GameObject.FindObjectOfType<Portalble.PortalbleGeneralController>() != null)
                GUILayout.Label("PortalbleController: √");
            else
                GUILayout.Label("PortalbleController: x");
            EditorGUILayout.EndVertical();
            // ==============================================================================
            EditorGUILayout.BeginVertical();
            //-------------------------------------------------------------------------------
            // Check two hands
            GameObject leftH = GameObject.Find("Hand_l");
            GameObject rightH = GameObject.Find("Hand_r");
            if (leftH == null) {
                if (rightH == null) {
                    GUILayout.Label("Hand Objects: None");
                } else {
                    GUILayout.Label("Hand Objects: Only Right");
                }
            } else if (rightH == null) {
                GUILayout.Label("Hand Objects: Only Left");
            } else {
                GUILayout.Label("Hand Objects: √");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            //==================================================================================
            // Function Buttons
            //==================================================================================
            GUILayout.Label("Functions", EditorStyles.boldLabel);
            if (GUILayout.Button("Set up a default portalble Scene")) {
                SetupDefaultPortalble();
            }
            if (GUILayout.Button("Set Up AR Support")) {
                // AddGoogleARCore();
            }
            if (GUILayout.Button("Set up General Portalble Controller")) {
                SetupPortalbleGeneralController();
            }
            if (GUILayout.Button("Add Experimental Function UI")) {
                AddExperimentalFunctionUI();
            }
        }

        /// <summary>
        /// Set up a default portalble scene
        /// </summary>
        void SetupDefaultPortalble() {
            // Set google arcore and basic data objects
            // AddGoogleARCore();
            AddARSupport();

            // Set Default Portalble controller
            SetupPortalbleGeneralController();

            // Create Data objects
            InstantiatePrefabAtPath(defaultPortalblePrefabPath + "gDataManager.prefab");
            InstantiatePrefabAtPath(defaultPortalblePrefabPath + "WebsocketManager.prefab");

            // Create Two hands
            InstantiatePrefabAtPath(defaultPortalblePrefabPath + "Hand_l.prefab");
            InstantiatePrefabAtPath(defaultPortalblePrefabPath + "Hand_r.prefab");
        }

        /// <summary>
        /// Set up google arcore support
        /// </summary>
        void AddGoogleARCore() {
            // Create ARCore Camera
            GameObject arcorePrefab = AssetDatabase.LoadAssetAtPath(googleARCorePath + "ARCore Device.prefab", typeof(GameObject)) as GameObject;
            if (arcorePrefab != null) {
                GameObject gobj = GameObject.Instantiate(arcorePrefab, Vector3.zero, Quaternion.identity);
                gobj.name = arcorePrefab.name;
                Camera cam = gobj.transform.GetComponentInChildren<Camera>();
                if (cam != null) {
                    cam.gameObject.AddComponent<AudioListener>();
                }
            }

            // Create ARCore Environmental light
            GameObject environmentalLight = AssetDatabase.LoadAssetAtPath(googleARCorePath + "Environmental Light.prefab",
                typeof(GameObject)) as GameObject;
            if (environmentalLight != null) {
                GameObject gobj = GameObject.Instantiate(environmentalLight, Vector3.zero, Quaternion.identity);
                gobj.name = environmentalLight.name;
            }
        }

        /// <summary>
        /// Set up Unity AR Support
        /// </summary>
        void AddARSupport() {
            GameObject session_origin = AssetDatabase.LoadAssetAtPath(System.IO.Path.Combine(unityXRPrefabPath,
                "ARKIT AR Session.prefab"), typeof(GameObject)) as GameObject;

            GameObject ar_session = AssetDatabase.LoadAssetAtPath(System.IO.Path.Combine(unityXRPrefabPath,
                "AR Session.prefab"), typeof(GameObject)) as GameObject;

            if (session_origin != null)
                GameObject.Instantiate(session_origin, Vector3.zero, Quaternion.identity);
            if (ar_session != null)
                GameObject.Instantiate(ar_session, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Set up general portalble controller
        /// </summary>
        void SetupPortalbleGeneralController() {
            GameObject pgc = ObjectFactory.CreateGameObject("PortalbleCtrl",
                typeof(Portalble.PortalbleGeneralController));
            pgc.transform.position = Vector3.zero;
            pgc.transform.rotation = Quaternion.identity;

            Portalble.PortalbleGeneralController generalController = pgc.GetComponent<PortalbleGeneralController>();
            generalController.m_UnityPlaneInteractionLayer = 12;


            generalController.m_ARSupport = GameObject.FindObjectOfType<PortalbleARSupport>();

            if (generalController.m_ARSupport != null) {
                generalController.m_FirstPersonCamera = generalController.m_ARSupport.GetComponentInChildren<Camera>();
            }

            generalController.m_planeTransparentMaterial = AssetDatabase.LoadAssetAtPath(transparentMaterialPath, typeof(Material)) as Material;

        }

        /// <summary>
        /// Add UI for user study
        /// </summary>
        void AddExperimentalFunctionUI() {
            InstantiatePrefabAtPath(System.IO.Path.Combine(defaultPortalblePrefabPath, "defaultUI.prefab"));
        }

        /// <summary>
        /// Instantiate a prefab at certen path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>the instantiated gameobject</returns>
        private GameObject InstantiatePrefabAtPath(string path) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefab != null) {
                GameObject gobj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                gobj.name = prefab.name;
                return gobj;
            }
            return null;
        }
    }
}