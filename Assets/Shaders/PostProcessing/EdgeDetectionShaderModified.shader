Shader "Custom/EdgeDetectionShaderModified"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold", float) = 0.01
        _DarkenFactor("_DarkenFactor", float) = 0.1
        _SaturationIntensity("_SaturationIntensity", float) = 1
        _LuminanceFactor("_LuminanceFactor", float) = 1
        _EdgeColor("Edge color", Color) = (0,0,0,1)
    }
    SubShader
    {
    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
    
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _CameraDepthNormalsTexture;
            sampler2D _CameraDepthTexture;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
             
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Threshold;
            float _DarkenFactor;
            float _SaturationIntensity;
            float _LuminanceFactor;
            fixed4 _EdgeColor;
 
            float4 GetPixelValue(in float2 uv) {
                half3 normal;
                float depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, normal);
                
                
                float depthFromDepthTex = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);  // sample from depth texture
                depthFromDepthTex = Linear01Depth (depthFromDepthTex);
                
                return fixed4(normal, depth);
            }
            
            const float Epsilon = 1e-10;
            
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //fixed4 col = tex2D(_CameraDepthNormalsTexture, i.uv);
                fixed4 orValue = GetPixelValue(i.uv);
                float2 offsets[8] = {
                    float2(-1, -1),
                    float2(-1, 0),
                    float2(-1, 1),
                    float2(0, -1),
                    float2(0, 1),
                    float2(1, -1),
                    float2(1, 0),
                    float2(1, 1)
                };
                fixed4 sampledValue = fixed4(0,0,0,0);
                for(int j = 0; j < 8; j++) {
                    sampledValue += GetPixelValue(i.uv + offsets[j] * _MainTex_TexelSize.xy);
                }
                sampledValue /= 8;
                
                //saturate and modify luminance
                fixed lum = saturate(Luminance(col.rgb) * _LuminanceFactor);
				fixed4 saturated;
				saturated.rgb = lerp(col.rgb, fixed3(lum,lum,lum), _SaturationIntensity);
				saturated.a = col.a;
                 
                return lerp(col, saturated, step(_Threshold, length(orValue - sampledValue)));
            }
            ENDCG
        }
    }
}