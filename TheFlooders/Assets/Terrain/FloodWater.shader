
Shader "Terrain/FloodWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OverlayAlpha ("Overlay alpha", float) = 1
        _OverlayUVShiftX ("Overlay UV shift X", float) = 0.0
        _OverlayUVShiftY ("Overlay UV shift Y", float) = 0.0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            struct v2f {
                fixed4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float _OverlayAlpha;
            float _OverlayUVShiftX;
            float _OverlayUVShiftY;

            v2f vert (
                float4 vertex : POSITION, // vertex position input
                float4 uv : TEXCOORD0 // texture coordinate input
                )
            {
                v2f o;
                o.uv = uv;
                o.vertex = UnityObjectToClipPos(vertex);
                return o;
            }

            // pixel shader, no inputs needed
            fixed4 frag (v2f i) : SV_Target
            {
            	half4 baseColor = tex2D(_MainTex, i.uv.xy);
            	half4 additionalColor = tex2D(_MainTex, i.uv.xy + float2(_OverlayUVShiftX, _OverlayUVShiftY));
            	float additionalColorAlpha = additionalColor.a * _OverlayAlpha;
            	return (1-additionalColorAlpha) * baseColor + additionalColorAlpha * additionalColor;
            	return float4(i.vertex * i.vertex);
            }
            ENDCG
        }
    }
}