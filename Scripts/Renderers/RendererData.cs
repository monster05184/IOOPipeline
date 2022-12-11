using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IOOPipeline {
    public class RendererData : ScriptableObject {
        private CameraRenderer defaultRenderer;
        private void OnEnable() {
            defaultRenderer = new CameraRenderer(this);
        }
        protected RendererData() {

        }
        public virtual CameraRenderer GetRendererInstance() {
            return defaultRenderer;
        }
    }

}
