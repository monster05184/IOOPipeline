using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace IOOPipeline {
    [ExecuteInEditMode]
    public class BasicAssetPipe : RenderPipelineAsset {

        public List<RendererData> renderers;
        public Color clearColor = Color.black;


        protected override RenderPipeline CreatePipeline() {
            CameraRenderer renderer = renderers.ToArray()[0].GetRendererInstance();
            return new BasicPipeInstance(clearColor, renderer);
        }

        [UnityEditor.MenuItem("SRP-Demo/01 - Create Basic Asset Pipeline")]
        static void CreateBasicAssetPipeline() {
            var instance = ScriptableObject.CreateInstance<BasicAssetPipe>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/BasicAssetPipe.asset");
        }
    }

}
