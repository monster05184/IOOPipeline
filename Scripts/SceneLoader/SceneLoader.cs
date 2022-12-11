using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System;
using UnityEditor.Experimental.GraphView;

namespace IOOPipeline{
    public class SceneLoader {
        
        
       

        public Material GpuDrivenMaterial;//GpuDriven材质
        public FrustumCulling frustumCulling;
        public int frameCount;
        public bool dataSended = false;
        

        public SceneEntity sceneEntity = SceneEntity.GetInstance();
        public TextureEntity textureEntity = new TextureEntity();

        public static SceneLoader Instance { get; private set; }
        public static SceneLoader GetInstance() {
            if (Instance == null) {
                Instance = new SceneLoader();
            }
            return Instance;
        }
        SceneLoader() {
            frameCount = 0;
        }
        public void Initialize(Material GpuDrivenMaterial) {
            this.GpuDrivenMaterial = GpuDrivenMaterial;
            sceneEntity.Initialize();
            frustumCulling = new FrustumCulling();
            
        }



        public void Load() {
            ref var vertexBuffer = ref sceneEntity.sceneBuffer.vertexBuffer;
            
            ref var indexBuffer = ref sceneEntity.sceneBuffer.indexBuffer;
            
            ref var argsBuffer = ref sceneEntity.cullingBuffer.argsBuffer;
            
            ref var resultBuffer = ref sceneEntity.cullingBuffer.resultBuffer;
            
            ref var clusterBuffer = ref sceneEntity.cullingBuffer.clusterBuffer;
            
            ref var renderObjectBuffer = ref sceneEntity.sceneBuffer.renderObjectBuffer;

            ref var sceneInfo = ref sceneEntity.sceneInfo;
            

            if (sceneEntity.sceneInfo.initialized) {
                frameCount = 0;
                MeshData meshData = AssetDatabase.LoadAssetAtPath<MeshData>(PipelineComponent.ResoursePaths.MeshData);
                //vertexBufferLoader
                 
                vertexBuffer = new ComputeBuffer(meshData.vertexCount, MeshData.Vertex.GetStride(), ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                vertexBuffer.SetData(meshData.vertices);
                GpuDrivenMaterial.SetBuffer("_VertexBuffer", vertexBuffer);

                //indexBufferLoader
                indexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Index, meshData.indexCount, sizeof(int));
                indexBuffer.SetData(meshData.indexs);

                //argBufferLoader
                argsBuffer = new ComputeBuffer(1, sizeof(int) * 5, ComputeBufferType.IndirectArguments);
               

                //renderObjectBufferLoader
                RenderObjectData renderObjectData = AssetDatabase.LoadAssetAtPath<RenderObjectData>(PipelineComponent.ResoursePaths.RenderObjectDataPath);
                renderObjectBuffer = new ComputeBuffer(renderObjectData.renderObjectCount, RenderObjectData.ObjectInfo.GetStride(), ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                renderObjectBuffer.SetData(renderObjectData.renderObjects);
                GpuDrivenMaterial.SetBuffer("_RenderObjectBuffer", renderObjectBuffer);
                sceneInfo.renderObjectCount = renderObjectData.renderObjectCount;

                //clusterBufferLoader
                ClusterData clusterData = AssetDatabase.LoadAssetAtPath<ClusterData>(PipelineComponent.ResoursePaths.ClusterDataPath);
                clusterBuffer = new ComputeBuffer(clusterData.clusterCount, ClusterData.ClusterInfo.GetStride(), ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                clusterBuffer.SetData(clusterData.clusterInfos);
                GpuDrivenMaterial.SetBuffer("_ClusterBuffer", clusterBuffer);
                sceneInfo.clusterCount = clusterData.clusterCount;
                
                argsBuffer.SetData(new int[] { meshData.indexCount, clusterData.clusterCount, 0, 0, 0 });
                
                //resultBufferLoader
                resultBuffer = new ComputeBuffer(clusterData.clusterCount, sizeof(int), ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                GpuDrivenMaterial.SetBuffer("_ResultBuffer",resultBuffer);
                

                dataSended = true;

            }
            if (vertexBuffer != null) 
                GC.SuppressFinalize(vertexBuffer);
            if (indexBuffer != null) 
                GC.SuppressFinalize(indexBuffer);
            if (argsBuffer != null) 
                GC.SuppressFinalize(argsBuffer);
            if (resultBuffer != null) 
                GC.SuppressFinalize(resultBuffer);
            if (clusterBuffer != null) 
                GC.SuppressFinalize(clusterBuffer);
            if (renderObjectBuffer != null) 
                GC.SuppressFinalize(renderObjectBuffer);
            
            LoadTexture();

            frustumCulling.InitializeData();
            
        }

        private void LoadTexture()
        {
            textureEntity.Initialize();
            foreach (var component in textureEntity.components)
            {
                TextureArray textureArray = component as TextureArray;
                if (textureArray == null)
                {
                    Debug.LogError("加载图片错误");
                    return;
                }

                GpuDrivenMaterial.SetTexture(textureArray.fieldName, textureArray.texture2DArray);
            }
        }



        public void Release() {
            sceneEntity.Release();
            textureEntity.Release();
            dataSended = false;
        }

    }

}
