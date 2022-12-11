using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace IOOPipeline {
    [ExecuteAlways]
    public class GpuDrivenPass : RendererPass
    {

        Material GpuDrivenMaterial;
        SceneLoader loader;
        SceneEntity sceneEntity;


        public GpuDrivenPass(GpuDrivenRenderer renderer):base(renderer) {
            GpuDrivenMaterial = renderer.GpuDrivenMaterial;
            sceneEntity = SceneEntity.GetInstance();
        
        }

        public override void Excute(ScriptableRenderContext context, Camera camera) {
            if (loader == null)
            {
                loader = SceneLoader.GetInstance();
            }

            
            base.Excute(context,camera);

            if (Application.isPlaying)
            {
                Culling();
        
                Draw();
                
            }


        }
    
        private void Draw()
        {

            
            if (loader.dataSended) {
                ref var indexBuffer = ref sceneEntity.sceneBuffer.indexBuffer;
                ref var argsBuffer = ref sceneEntity.cullingBuffer.argsBuffer;
                
                
                CommandBuffer cmd;
                cmd = CommandBufferPool.Get("frame" + loader.frameCount.ToString());
                cmd.ClearRenderTarget(true, true, Color.black);

                cmd.DrawProceduralIndirect(indexBuffer, Matrix4x4.identity, GpuDrivenMaterial, -1, MeshTopology.Triangles, argsBuffer);
                //cmd.DrawProceduralIndirect(indexBuffer, Matrix4x4.identity, GpuDrivenMaterial, -1, MeshTopology.Triangles, argsBuffer);
                //cmd.DrawProceduralIndirect(Matrix4x4.identity, GpuDrivenMaterial, -1, MeshTopology.Quads, argsBuffer);
                

            
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                CommandBufferPool.Release(cmd);

                loader.frameCount++;
            }
        }

        private void Culling()
        {
            if (loader.dataSended)
            {
                loader.frustumCulling.UpdateData(camera);
            }
            
        }




    }

}
