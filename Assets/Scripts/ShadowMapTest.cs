using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowMapTest : MonoBehaviour {

    [Range(0, 2000)]
    public float testValue = 1;
    public Light mainLight;
    CommandBuffer cmd;

    [Header("TestOption")]
    public bool useShadowmapCopy;
    public bool useCustomRay4WSCompute;
    public bool useInvProj4WSCompute;

    private void Start()
    {
        if (mainLight == null)
            return;

        cmd = new CommandBuffer();
        cmd.name = "Shadowmap copy";
        mainLight.AddCommandBuffer(LightEvent.AfterShadowMap, cmd);
    }

    private void OnPreRender()
    {
        if (cmd == null)
            return;

        cmd.Clear();
        if (useShadowmapCopy)
            cmd.EnableShaderKeyword("USESHADOWMAPCOPY");
        else
            cmd.DisableShaderKeyword("USESHADOWMAPCOPY");

        if(useCustomRay4WSCompute)
            cmd.EnableShaderKeyword("USECUSTOMRAY");
        else
            cmd.DisableShaderKeyword("USECUSTOMRAY");

        if (useInvProj4WSCompute)
            cmd.EnableShaderKeyword("USEINVPROJ");
        else
            cmd.DisableShaderKeyword("USEINVPROJ");

        cmd.SetGlobalFloat("testValue", testValue);

        int dest = Shader.PropertyToID("ShadowmapCopy");
        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        cmd.GetTemporaryRT(dest, 4096, 4096);
        cmd.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cmd.Blit(shadowmap, dest);
        cmd.SetGlobalTexture("ShadowmapCopy", dest);
    }
}
