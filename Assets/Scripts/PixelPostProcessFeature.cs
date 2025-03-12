using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelPostProcessFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material blitMaterial;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRendering;
    }

    public Settings settings = new Settings();

    class CustomPostProcessPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private string profilerTag = "Custom PostProcess Pass";

        // 通过 int 与 RenderTargetIdentifier 组合代替 RenderTargetHandle
        private int tempRTId = Shader.PropertyToID("_TempColorTex");
        private RenderTargetIdentifier tempRTIdentifier;

        public CustomPostProcessPass(Material mat, RenderPassEvent passEvent)
        {
            this.blitMaterial = mat;
            this.renderPassEvent = passEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (blitMaterial == null) return;

            // 获取Camera描述
            var cameraDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraDesc.depthBufferBits = 0;  // 无需深度的后处理RT可这么设

            // 分配临时RT
            cmd.GetTemporaryRT(tempRTId, cameraDesc, FilterMode.Bilinear);
            tempRTIdentifier = new RenderTargetIdentifier(tempRTId);

            // 如果想在此Pass中用 ConfigureTarget 来指定输出，可以这么做
            // （视需求而定，有时只需要在 Execute 中 Blit 即可）
            // ConfigureTarget(tempRTIdentifier);
            // ConfigureClear(ClearFlag.None, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (blitMaterial == null) return;

            var cmd = CommandBufferPool.Get(profilerTag);

            // 在 Pass 中获取 cameraColorTarget（请勿在 AddRenderPasses 里取）
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // 1) 把源内容拷贝到临时RT
            cmd.Blit(source, tempRTIdentifier);
            // 2) 从临时RT使用材质 Blit 回源
            cmd.Blit(tempRTIdentifier, source, blitMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // 释放临时RT
            if (blitMaterial != null)
            {
                cmd.ReleaseTemporaryRT(tempRTId);
            }
        }
    }

    CustomPostProcessPass m_Pass;

    public override void Create()
    {
        if (settings.blitMaterial == null)
            Debug.LogWarning("Blit Material is null. The pass will do nothing.");

        m_Pass = new CustomPostProcessPass(settings.blitMaterial, settings.passEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 如果材质为空，就跳过
        if (settings.blitMaterial == null)
            return;

        // 如果只想在 Game 摄像机上执行，可加判断
        if (renderingData.cameraData.cameraType != CameraType.Game) return;

        // 加入渲染队列
        renderer.EnqueuePass(m_Pass);
    }
}
