using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Portalble {
    [CustomEditor(typeof(GrabbableObject))]
    public class GrabbableObjectEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            GrabbableObject gobj = (GrabbableObject)target;
            if (GUILayout.Button("Generate")) {
                gobj.GenerateGrabObject();
            }
        }
    }
}