using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


namespace IOOPipeline {
    [CustomEditor(typeof(MeshGenerator))]
    public class MeshGeneratorEditor : Editor
    {
        private MeshGenerator meshGenerator;

        private MaterialPropertyData data;

        private Editor materialEditor;

        private bool hasMaterial = false;
        
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (!hasMaterial)
            {
                if (GUILayout.Button("保存Mesh并创建材质"))
                {
                    
                    data = ScriptableObject.CreateInstance<MaterialPropertyData>();
                    if (StoreMaterialData(data, meshGenerator))
                    {
                        hasMaterial = true;
                    }
                    meshGenerator.GetMeshData();
                    
                }
                
            }



            if (data != null)
            {
                DrawMaterial(data, ref meshGenerator.showMaterial, ref materialEditor);
            }
            
        }

        private void DrawMaterial(Object material, ref bool foldout, ref Editor editor)
        {
            if (material != null)
            {
                
                foldout = EditorGUILayout.InspectorTitlebar (foldout, material);
                using (var check = new EditorGUI.ChangeCheckScope ()) {
                    if (foldout) {
                        CreateCachedEditor (material, null, ref editor);
                        editor.OnInspectorGUI ();
                    }
                    
                }
                
            }
            
        }
        
        private MaterialPropertyData GetMaterialData()
        {
            MaterialPropertyData materialPropertyData = AssetDatabase.LoadAssetAtPath<MaterialPropertyData>(GetMaterialDataPath());
            return materialPropertyData;
        }

        private bool StoreMaterialData(MaterialPropertyData materialPropertyData, MeshGenerator meshGenerator)
        {
            string sceneDataPath = "Assets/Resourses/" + SceneInfoHandler.GetSceneName()+"SceneData.asset";
            SceneData sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(sceneDataPath);
            int renderObjectId = 0;
            if (!sceneData)
            {
                sceneData = ScriptableObject.CreateInstance<SceneData>();
                AssetDatabase.CreateAsset(sceneData,sceneDataPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
            }
            else
            {
                renderObjectId = sceneData.GetRenderObjectId();
                SerializedObject sceneDataSO = new SerializedObject(sceneData);
                SerializedProperty maxRenderObjectIdSP = sceneDataSO.FindProperty("maxRenderObjectId");
                maxRenderObjectIdSP.intValue++;
                sceneDataSO.ApplyModifiedProperties();
                sceneDataSO.Update();
            }

          
            if (materialPropertyData != null)
            {
                materialPropertyData.renderObjectId = renderObjectId;
                meshGenerator.objectIndex = renderObjectId;
                AssetDatabase.CreateAsset(materialPropertyData, GetMaterialDataPath());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return true;
        }

        private string GetMaterialDataPath()
        {
            string path = "Assets/Resourses/Materials/" + SceneInfoHandler.GetSceneName() + meshGenerator.gameObject.name + "Material.asset";
            return path;
        }

        private void OnEnable()
        {
            meshGenerator = (MeshGenerator)target;
            
            data = GetMaterialData();
                
            if (data != null)
            {
                hasMaterial = true;
            }
            else
            {
                hasMaterial = false;
            }
            
        }
        
    }
    
}
