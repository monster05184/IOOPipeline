using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace IOOPipeline {
    public class GpuDrivenRendererData : RendererData {
        private GpuDrivenRenderer renderer;
        public string LightModeTag = "GpuDriven";
        public Material GpuDrivenMaterial;


        private void OnEnable() {
            renderer = new GpuDrivenRenderer(this);
        }
        GpuDrivenRendererData() {

        }

        public override CameraRenderer GetRendererInstance() {
            return renderer;
        }
        [UnityEditor.MenuItem("SRP-Demo/02 - Create GPU Driven Renderer Data")]
        static void CreateBasicAssetPipeline() {
            var instance = ScriptableObject.CreateInstance<GpuDrivenRendererData>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/GpuDrivenRender.asset");
        }
    }
}

