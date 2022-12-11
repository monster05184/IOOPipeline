using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace IOOPipeline {
    public class CameraRenderer {
        public ScriptableRenderContext context;
        public Camera camera;
        public RendererData renderData;

        public CameraRenderer(RendererData rendererData) {
            this.renderData = rendererData;
        }

        public virtual void Render(ScriptableRenderContext context, Camera cam)
        {

          
            this.context = context;
            this.camera = cam;
        }
    }
}

