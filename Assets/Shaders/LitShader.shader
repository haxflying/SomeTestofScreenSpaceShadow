Shader "Unlit/LitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle]_UseCopy("USe copy", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #pragma multi_compile __ _USECOPY_ON
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_SHADOW_COORDS(2)
                float3 worldPos : TEXCOORD3;
                float3 worldNormal : TEXCOORD4;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            sampler2D ScreenShadowmapCopy1;

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed diffuse = saturate(dot(normalize(i.worldNormal), _WorldSpaceLightPos0));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                #if _USECOPY_ON
                float2 spos = i._ShadowCoord.xy / i._ShadowCoord.w;
                atten = tex2D(ScreenShadowmapCopy1, spos);
                #endif


                return col * atten;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
