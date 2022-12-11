using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IOOPipeline
{
    public class CullingTestEditor : Editor
    {
        [CustomEditor(typeof(CullingTest))]
        public class MeshGeneratorEditor : Editor {
            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                CullingTest cullingTest = (CullingTest)target;
                if (GUILayout.Button("保存Mesh")) {
                    cullingTest.TestResourse();
                }
            }
        }
    }
}

