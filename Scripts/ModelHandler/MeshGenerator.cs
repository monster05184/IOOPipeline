using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace IOOPipeline {
    [ExecuteInEditMode]
    public class MeshGenerator : MonoBehaviour {
        MeshFilter meshFilter;
        [HideInInspector]public Mesh mesh;
        [HideInInspector]public bool showMaterial;
        
        public int objectIndex = 0;

        public void Start() {
            meshFilter = GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh;
        }


        private Vector3[] GeneratorVertex(Vector3[] vertices, int[] indexs, int indexCount) {
            Vector3[] verticesWithOutIndex = new Vector3[indexCount];
            for (int i = 0; i < indexCount; i++) {
                verticesWithOutIndex[i] = vertices[indexs[i]];
            }
            return verticesWithOutIndex;
        }

        

        public bool GetMeshData() {

            MeshData meshData = ScriptableObject.CreateInstance<MeshData>();
            RenderObjectData renderObjectData = ScriptableObject.CreateInstance<RenderObjectData>();
            RenderObjectData storedRenderObjectData = AssetDatabase.LoadAssetAtPath<RenderObjectData>(PipelineComponent.ResoursePaths.RenderObjectDataPath);
            ClusterData clusterData = ScriptableObject.CreateInstance<ClusterData>();
            ClusterData storedClusterData = AssetDatabase.LoadAssetAtPath<ClusterData>(PipelineComponent.ResoursePaths.ClusterDataPath);
            
            if (!mesh)
                return false;
            else {
                //д��MeshData
                meshData.indexs = mesh.triangles;
                
                meshData.vertexCount = mesh.vertexCount;
                meshData.indexCount = (int)mesh.GetIndexCount(0);
                int count = mesh.vertices.Length;
                MeshData.Vertex[] vertices = new MeshData.Vertex[count];
                for (int i = 0; i < count; i++)
                {
                    vertices[i].pos = mesh.vertices[i];
                    vertices[i].uv = mesh.uv[i];
                    vertices[i].normal = mesh.normals[i];
                    vertices[i].tangent = mesh.tangents[i];
                }

                meshData.vertices = vertices;
                
                AssetDatabase.CreateAsset(meshData, "Assets/Resourses/MeshData.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //д��RenderObjectData
                RenderObjectData.ObjectInfo objectInfo = new RenderObjectData.ObjectInfo();
                objectInfo.local2WorldMatrix = transform.localToWorldMatrix;
                objectInfo.materialProperty.mainColor = Color.blue;
                if (!storedRenderObjectData) {
                    renderObjectData.AddRenderObject(objectInfo);
                } else {
                    storedRenderObjectData.AddRenderObject(objectInfo);
                    renderObjectData.renderObjects = storedRenderObjectData.renderObjects;
                    renderObjectData.renderObjectCount = storedRenderObjectData.renderObjectCount;
                }
                
                AssetDatabase.CreateAsset(renderObjectData, "Assets/Resourses/RenderObjects.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //����Cluster����


                ClusterData.ClusterInfo cluster = new ClusterData.ClusterInfo();
                cluster.bounds.extent = mesh.bounds.extents;
                cluster.bounds.center = mesh.bounds.center;
                cluster.objIndex = objectIndex;
                if (!storedClusterData) {
                    clusterData.AddCluster(cluster);
                } else {
                    storedClusterData.AddCluster(cluster);
                    clusterData.clusterInfos = storedClusterData.clusterInfos;
                    clusterData.clusterCount = storedClusterData.clusterCount;
                }
                AssetDatabase.CreateAsset(clusterData, "Assets/Resourses/Cluster.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();


            }

            return true;

        }

        
    }

}
