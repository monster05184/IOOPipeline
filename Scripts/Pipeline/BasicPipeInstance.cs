using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace IOOPipeline {
    [ExecuteAlways]
    class BasicPipeInstance : RenderPipeline {
        private Color m_ClearColor = Color.black;
        private CameraRenderer renderer;

        public BasicPipeInstance(Color clearColor, CameraRenderer renderer) {
            m_ClearColor = clearColor;

            this.renderer = renderer;
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {

            // clear buffers to the configured color
            /*
            var cmd = new CommandBuffer();
            cmd.ClearRenderTarget(true, true, m_ClearColor);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
            */
            //Öð¸öäÖÈ¾ÉãÏñ»ú
            foreach (var cam in cameras) {
                if (renderer != null) {

                    renderer.Render(context, cam);

                }

            }


        }
    }

}
