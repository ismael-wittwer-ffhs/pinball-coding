Shader "Universal Render Pipeline/Ghost"
{
    Properties
    {
        [Header(Base Color)]
        [HDR] _BaseColor ("Base Color (HDR)", Color) = (0.5, 0.8, 1.0, 0.3)
        _BaseIntensity ("Base Intensity", Range(0, 5)) = 1.0
        
        [Header(Fresnel Rim Light)]
        [HDR] _RimColor ("Rim Color (HDR)", Color) = (1.0, 1.0, 1.0, 1.0)
        _RimPower ("Rim Power", Range(0.1, 10)) = 2.0
        _RimIntensity ("Rim Intensity", Range(0, 10)) = 2.0
        
        [Header(Ghost Effect)]
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Vector) = (0.1, 0.1, 0, 0)
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 10
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Float) = 0
        _Cull ("Cull", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]
        
        Pass
        {
            Name "GhostPass"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float fogCoord : TEXCOORD4;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _BaseIntensity;
                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;
                float _Transparency;
                float _DistortionStrength;
                float _NoiseScale;
                float4 _NoiseSpeed;
            CBUFFER_END
            
            // Simple noise function for ghost effect
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            float smoothNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = noise(i);
                float b = noise(i + float2(1.0, 0.0));
                float c = noise(i + float2(0.0, 1.0));
                float d = noise(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.worldPos = vertexInput.positionWS;
                output.worldNormal = normalInput.normalWS;
                
                // Calculate view direction for fresnel rim lighting
                float3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.viewDir = viewDirWS;
                
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                // Time-based animation
                float time = _Time.y;
                
                // Animated UV for noise
                float2 animatedUV = input.uv * _NoiseScale;
                animatedUV += _NoiseSpeed.xy * time;
                
                // Generate noise for ghost effect
                float ghostNoise = smoothNoise(animatedUV);
                ghostNoise = smoothNoise(animatedUV * 2.0 + ghostNoise * 0.5);
                
                // Calculate fresnel rim lighting effect
                float3 normalWS = normalize(input.worldNormal);
                float3 viewDirWS = normalize(input.viewDir);
                float fresnel = 1.0 - saturate(dot(viewDirWS, normalWS));
                float rim = pow(fresnel, _RimPower);
                
                // Apply rim color and intensity
                float3 rimLight = _RimColor.rgb * rim * _RimIntensity;
                
                // Base ghost color with noise variation
                float noiseVariation = lerp(0.8, 1.2, ghostNoise);
                float3 baseColor = _BaseColor.rgb * _BaseIntensity * noiseVariation;
                
                // Combine base color with rim light
                float3 finalColor = baseColor + rimLight;
                
                // Calculate alpha based on fresnel and transparency
                // More transparent in the center, more opaque at edges (rim)
                float alpha = lerp(_Transparency, 1.0, rim) * _BaseColor.a;
                
                // Apply fog
                float3 foggedColor = MixFog(finalColor, input.fogCoord);
                
                return float4(foggedColor, alpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

