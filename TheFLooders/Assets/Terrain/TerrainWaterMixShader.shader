
Shader "Terrain/FlatTerrainMix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main color", Color) = (1, 0, 0, 1)
        _BandColor ("Band color", Color) = (0, 1, 0, 1)
        _BandPeriodicity ("Band periodicity", float) = 0.1
        _BandWidth ("Band width", float) = 0.5
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
            half4 _MainColor;
            half4 _BandColor;
            half _BandPeriodicity;
            half _BandWidth;

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
                // Extraction des infos passées au shader dans
                // les canaux 3 et 4 des coordonnées UV.
                half2 yGradInUVSpace = i.uv.zw;
                half bandWidthCoef = _BandWidth * length(yGradInUVSpace);

                // Hauteur des bandes
                half bandHeight = bandWidthCoef * _BandWidth;

            	// Récupération de l'information de hauteur
                half4 c = tex2D (_MainTex, i.uv.xy);
                half h = length(c);

                // Début de la périodicité
                half fromPeriodStart = frac(h / _BandPeriodicity);
                half4 finalColor =
                	step(bandHeight, fromPeriodStart) * _MainColor +
                	step((1 - bandHeight), (1 - fromPeriodStart)) * _BandColor;
//                half4 finalColor =
//                	smoothstep(bandHeight *0.9, bandHeight*1.1, fromPeriodStart) * _MainColor +
//                	smoothstep((1 - bandHeight) *0.9, (1 - bandHeight)*1.1, (1 - fromPeriodStart)) * _BandColor;

            	return finalColor;
                // return fixed4(bandWidthCoef, bandWidthCoef, bandWidthCoef, 1);
            }
            ENDCG
        }
    }
}