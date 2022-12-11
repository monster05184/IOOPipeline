using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IOOPipeline {
    public class PipelineComponent {
        public class PipelineBuffers {
            public ComputeBuffer resultBuffer;//剔除结果
            public ComputeBuffer clusterBuffer;//Cluster信息
            public ComputeBuffer argsBuffer;//{indexNum,in}
            public ComputeBuffer vertexBuffer;//顶点数据
            public ComputeBuffer renderObjectBuffer;//渲染物体数据

            public GraphicsBuffer indexBuffer;//顶点索引数据


        }

        public static class ResoursePaths {
            public static string ClusterDataPath = "Assets/Resourses/Cluster.asset";
            public static string RenderObjectDataPath = "Assets/Resourses/RenderObjects.asset";
            public static string MeshData = "Assets/Resourses/MeshData.asset";
        }
        
       



    }
}

