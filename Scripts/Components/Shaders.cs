using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
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
          public int hizCullingKernel;

          public CullingShader()
          {
               shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(ShaderPath);
               if (shader != null)
               {
                    clusterCullingKernel = shader.FindKernel("ClusterCulling");
                    hizCullingKernel = shader.FindKernel("HizCulling");
               }
               else
               {
                    Debug.LogError("Can not Load Shader at " + ShaderPath);
               }
          }
     }

     public class HizGeneratorShader : Component
     {
          public string ShaderPath = "Assets/Shader/Hiz/DepthMipmapGenerator.shader";
          public Shader shader;
          public HizGeneratorShader()
          {
               shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
               if (shader == null)
               {
                    Debug.LogError("Can not Load Shader at " + ShaderPath);
               }
          }
     }

}

