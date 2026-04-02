Shader "Custom/Test"
{
     Properties
    {
        _BlackColor("Black Color", Color) = (1, 1, 1, 1)
        _WhiteColor("White Color", Color) = (1, 1, 1, 1)
        
        _LerpNumber("Lerp", Float) = 0.5
        _PixelateNumber("Pixelate", Float) = 1
        _LightNumber("Light", Float) = 1
        _CompareNumber("Compare", Float) = 0.5
        _EdgeThreshold("Edge", Float) = 0.5
    }
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        
        Pass
        {
            Name "Test"

            HLSLPROGRAM
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BlackColor;
                half4 _WhiteColor;
                float4 _BaseMap_TexelSize;
                float _LerpNumber;
                float _PixelateNumber;
                float _LightNumber;
                float _CompareNumber;
                float _EdgeThreshold;
            CBUFFER_END

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                
                float mat[64] = {
                    0,48,12,60,3,51,15,63,
                    32,16,44,28,35,19,47,31,
                    8,56,4,52,11,59,7,55,
                    40,24,36,20,43,27,39,23,
                    2,50,14,62,1,49,13,61,
                    34,18,46,30,33,17,45,29,
                    10,58,6,54,9,57,5,53,
                    42,26,38,22,41,25,37,21
                };
                
                /*
                float2 tileSize = float2(_BaseMap_TexelSize.z, _BaseMap_TexelSize.w);
                
                float2 screenPixel = input.texcoord * _ScreenParams.xy;
                
                float2 snappedPixel = floor(screenPixel / _PixelateNumber) * _PixelateNumber;
                float2 withinTile = fmod(snappedPixel, tileSize);
                float tileWidth  = _BaseMap_TexelSize.z;
                float tileHeight = _BaseMap_TexelSize.w;
                
                float tilesX = _ScreenParams.x / tileWidth;
                float tilesY = _ScreenParams.y / tileHeight;
                tileSize = float2(tilesX, tilesY); 
                float2 tiledUV = frac(input.texcoord * tileSize);
                
                
                float2 pixelUV = floor(input.texcoord*_PixelateNumber)/_PixelateNumber;
                half4 img   = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelUV);
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, tiledUV);
                img = img * _LightNumber/10;
                float col = (lerp(img.r, color.r, _LerpNumber/10));
                
                float2 texel = 1.0 / _ScreenParams.xy;
                
                float3 right  = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelUV + float2(texel.x, 0)).rgb;
                float3 left   = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord - float2(texel.x, 0)).rgb;
                
                float3 up     = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelUV + float2(0, texel.y)).rgb;
                float3 down   = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelUV - float2(0, texel.y)).rgb;
                
                float dx = length(right - left);
                float dy = length(up - down);
                
                float edge = dx + dy;
                
                edge = step(_EdgeThreshold, edge);
                
                //return half4(edge, edge, edge, 1.0);
                color = col < _CompareNumber/10
                   ? half4(0.1804, 0.1333, 0.3098, 1.0)
                    : half4(0.8902, 0.6902, 0.4549, 1.0);
                //color = edge > 0? half4(0.1804, 0.1333, 0.3098, 1.0) : color;
                return color;
                */
                float2 baseUV = input.texcoord;
                float2 coord = baseUV * _ScreenParams.xy;
                float2 pixalateCoord = round(coord / (_PixelateNumber/10)) * (_PixelateNumber/10);
                float2 pixalateUV = pixalateCoord / _ScreenParams.xy;
                float n = 8.0;
                float2 mapPosition = float2(pixalateCoord.x % n, pixalateCoord.y % 4);
                float M = (mat[mapPosition.y + mapPosition.x*n] * (1/(n*n))) - 0.5;
                half4 img   = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixalateUV);
                half4 bwimg = half4(img.r,img.r,img.r, 1.0);
                img = bwimg + _LerpNumber * M;
                img = floor(img+0.5);
                img = img.r < _CompareNumber/10? _BlackColor : _WhiteColor;
                return img;
            }
            ENDHLSL
        }
    }
}
