using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
namespace IOOPipeline {
    public class FrustumCulling {

       
        private SceneEntity m_sceneEntity ;
        private CullingEntity cullingEntity = new CullingEntity();

        private SceneEntity sceneEntity
        {
            get
            {
                if (m_sceneEntity == null)
                {
                    m_sceneEntity = SceneEntity.GetInstance();
                }
                return m_sceneEntity;
            }
        }

        public FrustumCulling()
        {

            cullingEntity.Initialize();
            if (cullingEntity.cullingShader.shader == null) {
                Debug.LogError("Culling Shader Can not Load");
            }

        }

        //获取视锥体远平面的四个角
        private void GetFrustumCorner() {
            ref Camera camera = ref cullingEntity.frustumCullingCom.cam;
            ref var corners = ref cullingEntity.frustumCullingCom.corners;
            float rad = Mathf.Deg2Rad * camera.fieldOfView * 0.5f;
            float upLength = camera.farClipPlane * Mathf.Tan(rad);
            float rightLength = upLength * camera.aspect;
            Vector3 farPoint = camera.transform.position + camera.farClipPlane * camera.transform.forward;
            Vector3 upVec = upLength * camera.transform.up;
            Vector3 rightVec = rightLength * camera.transform.right;
            corners[0] = farPoint - upVec - rightVec;
            corners[1] = farPoint - upVec + rightVec;
            corners[2] = farPoint + upVec - rightVec;
            corners[3] = farPoint + upVec + rightVec;
        }

        //获取摄像机的裁剪平面
        public void GetFrustumPlanes() {
            ref Camera camera = ref cullingEntity.frustumCullingCom.cam;
            ref var corners = ref cullingEntity.frustumCullingCom.corners;
            ref var planes = ref cullingEntity.frustumCullingCom.planes;
            if (camera != null)
            {
                GetFrustumCorner();
                Vector3 position = camera.transform.position;
                Vector3 forward = camera.transform.forward.normalized;
                planes[0] = GetPlane(corners[1], corners[0], position);
                planes[1] = GetPlane(corners[2], corners[3], position);
                planes[2] = GetPlane(corners[0], corners[2], position);
                planes[3] = GetPlane(corners[3], corners[1], position);
                float disNear = Vector3.Dot((forward * camera.nearClipPlane + position), forward);
                float disFar = Vector3.Dot((forward * camera.farClipPlane + position), forward);
                planes[4] = new Vector4(-forward.x, -forward.y, -forward.z, -disNear);
                planes[5] = new Vector4(forward.x, forward.y, forward.z, disFar);    
            }
        }

        //根据三角形获取平面
        public Vector4 GetPlane(Vector3 a, Vector3 b, Vector3 c) {
            Vector3 normal = Vector3.Cross(b - a, c - a).normalized;
            float w = Vector3.Dot(normal, a);
            return new Vector4(normal.x, normal.y, normal.z, w);
        }


        //场景加载时得:初始化
        public bool InitializeData()
        {
            
            int clusterCullingKernel = cullingEntity.cullingShader.clusterCullingKernel;
            
            ref var cullingBuffer = ref sceneEntity.cullingBuffer;
            ref var sceneInfo = ref sceneEntity.sceneInfo;
            
            cullingEntity.cullingShader.shader.SetVectorArray(ShaderProperty.CullingShader.planesId, cullingEntity.frustumCullingCom.planes);
            cullingEntity.cullingShader.shader.SetBuffer(clusterCullingKernel,ShaderProperty.CullingShader.clusterBufferId, cullingBuffer.clusterBuffer);
            cullingEntity.cullingShader.shader.SetBuffer(clusterCullingKernel,ShaderProperty.CullingShader.resultBufferId, cullingBuffer.resultBuffer);
            cullingEntity.cullingShader.shader.SetBuffer(clusterCullingKernel,ShaderProperty.CullingShader.argsBufferId, cullingBuffer.argsBuffer);
            cullingEntity.cullingShader.shader.SetInt(ShaderProperty.CullingShader.clusterCountId, sceneInfo.clusterCount);
            
            
            
            return true;
        }
        //每帧调用：更新数据进行裁剪
        public bool UpdateData(Camera camera) {
            
            
            cullingEntity.Initialize();

            cullingEntity.frustumCullingCom.cam = camera;
            
            //TODO remove test

            //Test
            if(Application.isEditor)
            cullingEntity.frustumCullingCom.cam = Camera.main;
            //Test
            
            GetFrustumPlanes();

            ExcuteFrustumCulling();
            
            //Test
            //Test();
            //Test
            
            cullingEntity.Release();
            
            

            return true;
        }
        //每帧的GPU裁剪
        public bool ExcuteFrustumCulling() {

            ref var cullingShader = ref cullingEntity.cullingShader.shader;
            ref var cullingCom = ref cullingEntity.frustumCullingCom;
            ref var argsBuffer = ref sceneEntity.cullingBuffer.argsBuffer;
            
            argsBuffer.SetData(new int[] {2304, 0, 0, 0, 0 });

            cullingShader.SetVectorArray(ShaderProperty.CullingShader.planesId, cullingCom.planes);
            
            cullingShader.SetVector(ShaderProperty.CullingShader.camPosId, cullingCom.cam.transform.position);

            int group = (sceneEntity.sceneInfo.clusterCount / 64) + 1;
            cullingShader.Dispatch(cullingEntity.cullingShader.clusterCullingKernel, group, 1, 1);

            return true;
        }
        
    }
}

