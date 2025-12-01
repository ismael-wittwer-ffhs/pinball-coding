Shader "Universal Render Pipeline/EnergyField"
{
    Properties
    {
        [Header(Noise Settings)]
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Vector) = (0.1, 0.1, 0, 0)
        
        [Header(Energy Field Appearance)]
        [HDR] _EnergyColor ("Energy Color (HDR)", Color) = (0.2, 0.8, 1.0, 1.0)
        _EnergyIntensity ("Energy Intensity", Range(0, 20)) = 2.0
        _EdgeGlow ("Edge Glow", Range(0, 5)) = 1.5
        _FieldThickness ("Field Thickness", Range(0, 1)) = 0.3
        
        [Header(Rim Light)]
        [HDR] _RimColor ("Rim Color (HDR)", Color) = (1.0, 1.0, 1.0, 1.0)
        _RimPower ("Rim Power", Range(0.1, 10)) = 2.0
        _RimIntensity ("Rim Intensity", Range(0, 10)) = 1.0
        
        [Header(Intersection Glow)]
        [HDR] _IntersectionColor ("Intersection Color (HDR)", Color) = (1.0, 0.2, 0.2, 1.0)
        _IntersectionDistance ("Intersection Distance", Range(0, 1)) = 0.1
        _IntersectionIntensity ("Intersection Intensity", Range(0, 20)) = 5.0
        _IntersectionPower ("Intersection Power", Range(0.1, 10)) = 2.0
        
        [Header(Animation)]
        _ScrollSpeed ("Scroll Speed", Vector) = (0.2, 0.1, 0, 0)
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.0
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.2
        
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
            Name "EnergyFieldPass"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _DEPTH_TEXTURE
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
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
                float4 screenPos : TEXCOORD4;
                float fogCoord : TEXCOORD5;
            };
            
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _NoiseTex_ST;
                float _NoiseScale;
                float4 _NoiseSpeed;
                float4 _EnergyColor;
                float _EnergyIntensity;
                float _EdgeGlow;
                float _FieldThickness;
                float4 _ScrollSpeed;
                float _PulseSpeed;
                float _PulseAmount;
                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;
                float4 _IntersectionColor;
                float _IntersectionDistance;
                float _IntersectionIntensity;
                float _IntersectionPower;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _NoiseTex);
                output.worldPos = vertexInput.positionWS;
                output.worldNormal = normalInput.normalWS;
                
                // Calculate view direction for rim lighting
                float3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.viewDir = viewDirWS;
                
                // Screen position for depth sampling
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                // Time-based animation
                float time = _Time.y;
                
                // Animated UV coordinates for scrolling
                float2 animatedUV = input.uv * _NoiseScale;
                animatedUV += _ScrollSpeed.xy * time;
                
                // Sample noise texture multiple times for more complex patterns
                float noise1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, animatedUV).r;
                float noise2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, animatedUV * 1.5 + float2(time * 0.3, time * 0.2)).g;
                float noise3 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, animatedUV * 2.0 - float2(time * 0.2, time * 0.3)).b;
                
                // Combine noise samples for more interesting patterns
                float combinedNoise = (noise1 * 0.5 + noise2 * 0.3 + noise3 * 0.2);
                
                // Create pulsing effect
                float pulse = sin(time * _PulseSpeed) * _PulseAmount + 1.0;
                combinedNoise *= pulse;
                
                // Create energy field pattern - use noise to create field-like appearance
                float fieldPattern = smoothstep(_FieldThickness, _FieldThickness + 0.1, combinedNoise);
                fieldPattern = pow(fieldPattern, 2.0); // Sharper edges
                
                // Add edge glow effect
                float edgeDistance = abs(combinedNoise - _FieldThickness);
                float edgeGlow = 1.0 - smoothstep(0.0, 0.15, edgeDistance);
                edgeGlow = pow(edgeGlow, 1.0 / _EdgeGlow);
                
                // Combine field pattern with edge glow
                float finalPattern = max(fieldPattern, edgeGlow * 0.5);
                
                // Calculate energy color with intensity
                // HDR color values > 1.0 will trigger bloom in URP
                float3 energyColor = _EnergyColor.rgb * _EnergyIntensity * finalPattern;
                
                // Add some variation based on world position for more dynamic look
                float3 worldPosVariation = input.worldPos * 0.1;
                float worldNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, worldPosVariation.xy + time * 0.1).r;
                energyColor *= (0.8 + worldNoise * 0.4);
                
                // Calculate rim lighting (Fresnel effect)
                float3 normalWS = normalize(input.worldNormal);
                float3 viewDirWS = normalize(input.viewDir);
                float rimDot = 1.0 - saturate(dot(viewDirWS, normalWS));
                float rim = pow(rimDot, _RimPower);
                float3 rimLight = _RimColor.rgb * rim * _RimIntensity;
                
                // Calculate intersection glow using depth
                float3 intersectionGlow = float3(0, 0, 0);
                #if defined(_DEPTH_TEXTURE)
                    // Sample depth from depth texture
                    float2 screenUV = input.screenPos.xy / input.screenPos.w;
                    float sceneDepth = SampleSceneDepth(screenUV);
                    float linearSceneDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
                    float linearSurfaceDepth = LinearEyeDepth(input.positionCS.z, _ZBufferParams);
                    
                    // Calculate depth difference
                    float depthDifference = linearSceneDepth - linearSurfaceDepth;
                    
                    // Create intersection glow when objects are close
                    float intersectionFactor = 1.0 - saturate(depthDifference / _IntersectionDistance);
                    intersectionFactor = pow(intersectionFactor, _IntersectionPower);
                    
                    intersectionGlow = _IntersectionColor.rgb * intersectionFactor * _IntersectionIntensity;
                #endif
                
                // Combine energy color with rim light and intersection glow
                float3 finalColor = energyColor + rimLight + intersectionGlow;
                
                // Output final color with alpha
                // Note: HDR values in finalColor will trigger bloom post-processing
                float alpha = finalPattern * _EnergyColor.a;
                
                // Apply fog (but preserve HDR values for bloom)
                float3 foggedColor = MixFog(finalColor, input.fogCoord);
                
                return float4(foggedColor, alpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

