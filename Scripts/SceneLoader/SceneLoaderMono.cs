using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IOOPipeline {
    public class SceneLoaderMono : MonoBehaviour {
        SceneLoader sceneLoader;
        public Material GpuDrivenMaterial;
        private Light mainLight;
        public Texture texture;
        private void OnEnable() {
            sceneLoader = SceneLoader.GetInstance();
            GpuDrivenMaterial.SetVector("_MainLightDrection", new Vector3(1,1,0));
            sceneLoader.Initialize(GpuDrivenMaterial);
            sceneLoader.Load();
            GpuDrivenMaterial.SetTexture("_TestTex",texture);
            
        }
        private void OnDestroy() {
            sceneLoader.Release();
        }

    }
}

