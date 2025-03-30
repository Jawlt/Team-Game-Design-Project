using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        // Previously assigned in AddRenderPasses, but now we set it in OnCameraSetup:
        public RenderTargetIdentifier source;

        private readonly Material _material;

        // We’ll allocate one temporary RT for the blit
        private RenderTargetHandle tempRenderTarget;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
            tempRenderTarget.Init("_TemporaryColorTexture");
        }

        // Called before Execute. Safe place to get cameraColorTarget:
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // This is now the correct place to grab the camera color target.
            source = renderingData.cameraData.renderer.cameraColorTarget;

            // If you want a specific descriptor (for example to ensure same size as camera):
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;

            // Use the descriptor to allocate your temporary RT
            cmd.GetTemporaryRT(tempRenderTarget.id, descriptor);
        }

        // The actual rendering logic
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Skip if it's a Reflection camera, as your logic suggests
            if (renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            var cmd = CommandBufferPool.Get("Water_Volume Pass");

            // Blit from source → temp → material, then temp → source
            Blit(cmd, source, tempRenderTarget.Identifier(), _material);
            Blit(cmd, tempRenderTarget.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources
        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Release the RT we requested in OnCameraSetup
            cmd.ReleaseTemporaryRT(tempRenderTarget.id);
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();
    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
            settings.material = (Material)Resources.Load("Water_Volume");

        m_ScriptablePass = new CustomRenderPass(settings.material)
        {
            renderPassEvent = settings.renderPass
        };
    }

    // We no longer set `m_ScriptablePass.source = renderer.cameraColorTarget;` here
    // because that triggers the invalid-scope error.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Simply enqueue our pass.
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
