Shader "Custom/Test"
{
     Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _LerpNumber("Lerp", Float) = 0.5
        _PixelateNumber("Pixelate", Float) = 1
        _LightNumber("Light", Float) = 1
        _CompareNumber("Compare", Float) = 0.5
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
                float4 _BaseMap_TexelSize;
                float _LerpNumber;
                float _PixelateNumber;
                float _LightNumber;
                float _CompareNumber;
            CBUFFER_END

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
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
                
                    float2 pixelUV = snappedPixel / _ScreenParams.xy;
                    half4 img   = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, floor(input.texcoord*_PixelateNumber)/_PixelateNumber);
                    half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, tiledUV);
                    img = img * _LightNumber/10;
                    float col = (lerp(img.r, color.r, _LerpNumber/10));
                    return col < _CompareNumber/10
                        ? half4(0.1804, 0.1333, 0.3098, 1.0)
                        : half4(0.8902, 0.6902, 0.4549, 1.0);
                return img;
            }
            ENDHLSL
        }
    }
}
