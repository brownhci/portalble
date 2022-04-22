using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Portalble {
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour {
        // Use this for initialization
        public void GenerateGrabObject() {
#if UNITY_EDITOR
            // Create a child
            GameObject gobj = new GameObject("GrabCollider");
            gobj.transform.parent = transform;
            gobj.transform.localPosition = Vector3.zero;
            gobj.transform.localRotation = Quaternion.identity;
            gobj.transform.localScale = Vector3.one;

            // Copy collider, add rigidbody
            Collider cd = GetComponent<Collider>();
            UnityEditorInternal.ComponentUtility.CopyComponent(cd);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gobj);
            Collider ncd = gobj.GetComponent<Collider>();
            ncd.isTrigger = true;

            Rigidbody nrb = gobj.AddComponent<Rigidbody>();
            nrb.useGravity = false;

            // Add Script
            GrabCollider grabc = gobj.AddComponent<GrabCollider>();
            grabc.SetBindObject(transform);

            grabc.newMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/OutlineEffect/OutlineEffect/VerticesOutline.mat", typeof(Material));
            grabc.AutomaticExpand = true;
#endif
        }
    }
}
