using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using IOOPipeline;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;

namespace IOOPipeline
{
     public class CullingShader : Component
     {
          private string ShaderPath = "Assets/Shader/ComputeShader/FrustumCulling.compute";
          public ComputeShader shader;
          public int clusterCullingKernel;

          public CullingShader()
          {
               shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(ShaderPath);
               if (shader != null)
               {
                    clusterCullingKernel = shader.FindKernel("ClusterCulling");
               }
          }
     }
     
}

