using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
namespace IOOPipeline {
    public class GpuDrivenRenderer : CameraRenderer {
        List<ShaderTagId> shaderTags = new List<ShaderTagId>();
        ShaderTagId GpuDrivenTagId;
        public Material GpuDrivenMaterial;
        List<RendererPass> rendererPasses = new List<RendererPass>();

        public GpuDrivenRenderer(GpuDrivenRendererData renderData) : base(renderData) {
            this.GpuDrivenMaterial = renderData.GpuDrivenMaterial;
            GpuDrivenTagId = new ShaderTagId(renderData.LightModeTag);
            InitializePass();
        }


        public override void Render(ScriptableRenderContext context, Camera cam) {
            //Debug.Log("FrameBegin");
            base.Render(context, cam);
            context.SetupCameraProperties(camera);
            CullingResults cullingResults;

            //�޳�
            if (camera.TryGetCullingParameters(out var cullingParameters)) {
                cullingResults = context.Cull(ref cullingParameters);
            } else {
                cullingParameters = new ScriptableCullingParameters();
                cullingResults = context.Cull(ref cullingParameters);
            }


            //��ͼ����
            DrawingSettings drawingSettings;
            var sortingSettings = new SortingSettings(camera);
            drawingSettings = new DrawingSettings();
            drawingSettings.sortingSettings = sortingSettings;
            drawingSettings.SetShaderPassName(0, GpuDrivenTagId);


            //ɸѡ����Terrain��
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
            //int TerrainLayer = LayerMask.NameToLayer("Terrain");
            //filteringSettings.layerMask = TerrainLayer;




            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

            //InitializePass();
            ExcuteRenderPasses();

            //������պ�
            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null) {
                context.DrawSkybox(camera);
            }

            context.Submit();
            //Debug.Log("FrameEnd");

        }
        public void InitializePass() {
            GpuDrivenPass gpuDrivenPass = new GpuDrivenPass(this);
            rendererPasses.Add(gpuDrivenPass);
        }

        public void ExcuteRenderPasses() {
            if (rendererPasses.Count != 0) {
                foreach (RendererPass pass in rendererPasses) {
                    pass.Excute(context, camera);

                }
            }

        }

    }

}
