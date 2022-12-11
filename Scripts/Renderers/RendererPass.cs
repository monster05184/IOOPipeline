using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace IOOPipeline {
    public class RendererPass {
        protected ScriptableRenderContext context;
        protected Camera camera;


        protected RendererPass(CameraRenderer renderer) {
            this.context = renderer.context;
            this.camera = renderer.camera;

        }
        public virtual void Excute(ScriptableRenderContext context,Camera camera) {
            this.context = context;
            this.camera = camera;
        }
    }
}

