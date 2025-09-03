Shader "Custom/WindWaterEffectURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Vector) = (0.1, 0.1, 0, 0)
        _WaveStrength ("Wave Strength", Float) = 0.05
        _WaveFrequency ("Wave Frequency", Float) = 10
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float2 _WaveSpeed;
            float _WaveStrength;
            float _WaveFrequency;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float t = _Time.y;

                float waveX = sin(i.uv.y * _WaveFrequency + t * _WaveSpeed.x) * _WaveStrength;
                float waveY = cos(i.uv.x * _WaveFrequency + t * _WaveSpeed.y) * _WaveStrength;

                float2 distortedUV = i.uv + float2(waveX, waveY);

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
            }
            ENDHLSL
        }
    }
}
