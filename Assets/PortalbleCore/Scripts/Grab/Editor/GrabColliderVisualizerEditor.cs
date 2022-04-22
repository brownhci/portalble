using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Portalble.Functions.Grab {
    [CustomEditor(typeof(GrabColliderVisualizer))]
    public class GrabColliderVisualizerEditor : Editor {
        private string default_material_path = "Assets/Portalble_Core/Scripts/Grab/ColliderVisualizer.mat";

        SerializedProperty mVisualizerMaterial;

        void OnEnable() {
            mVisualizerMaterial = serializedObject.FindProperty("m_visualizerMaterial");

            serializedObject.Update();
            if (mVisualizerMaterial.objectReferenceValue == null) {
                mVisualizerMaterial.objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath(default_material_path, typeof(Material)) as Material;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}