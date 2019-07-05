using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class ScreenSpaceShadowTest : MonoBehaviour
{
    public Light mainLight;
    public Shader shader;
    private Material mat;
    CommandBuffer cmd;
    void Start()
    {
        if (mainLight == null)
            return;

        cmd = new CommandBuffer();
        cmd.name = "Shadowmap copy";
        mainLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, cmd);
        mat = new Material(shader);
    }

    void Update()
    {
        if (cmd == null)
            return;

        cmd.Clear();
        int dest = Shader.PropertyToID("ScreenShadowmapCopy");
        int dest1 = Shader.PropertyToID("ScreenShadowmapCopy1");
        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        cmd.GetTemporaryRT(dest, 4096, 4096, 0, FilterMode.Trilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 8);
        cmd.GetTemporaryRT(dest1, 1024, 1024, 0, FilterMode.Trilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 8);
        //cmd.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cmd.Blit(shadowmap, dest);
        cmd.Blit(dest, dest1, mat);
    }
}
