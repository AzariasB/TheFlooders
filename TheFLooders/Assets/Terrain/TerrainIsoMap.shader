
Shader "Terrain/FlatTerrainMixGeo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main color", Color) = (1, 0, 0, 1)
        _LineColor ("Line color", Color) = (0, 1, 0, 1)
        _LineYPeriod ("Line vertical periodicity", float) = 0.1
        _LineWidth ("Line width", float) = 0.5
        _Smoothness ("Line smoothness", Range(0.01, 1)) = 0.2
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
                float3 objSpacePos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            half4 _MainColor;
            half4 _LineColor;
            half _LineYPeriod;
            half _LineWidth;
            half _Smoothness;

            v2f vert (
                float4 vertex : POSITION, // vertex position input
                float4 uv : TEXCOORD0 // texture coordinate input
                )
            {
                v2f o;
                o.uv = uv;
                o.vertex = UnityObjectToClipPos(vertex);
                o.objSpacePos = vertex;
                return o;
            }

            // pixel shader, no inputs needed
            fixed4 frag (v2f i) : SV_Target
            {
            	// const half sqrt3 = 1.73205080757;

                // Extraction des infos passées au shader dans
                // les canaux 3 et 4 des coordonnées UV.
                // Il s'agit du gradient de la heightmap, interpollé entre chaque
                // sommet du mesh.
                half2 yGradInUVSpace = i.uv.zw;

                // Hauteur des lignes, projetée sur l'espace vertical décrit par la heightmap
                half linePaintHeight = length(yGradInUVSpace) * _LineWidth;

            	// Récupération de l'information de hauteur
                half heightValue = dot(i.objSpacePos, float3(0, 1, 0));

                // Décalage du pixel courant par rapport au début de sa période
                float periodStart = floor(heightValue / _LineYPeriod) * _LineYPeriod;
                float periodEnd = periodStart + _LineYPeriod;

                // Fin de la bande
                float maxHeight = periodStart + linePaintHeight;

                // lissage des limites pour cacher des pb d'échantillonnage
                const float smooth = linePaintHeight*_Smoothness;
                float afterStart = smoothstep(periodStart, periodStart + smooth, heightValue);
                float beforeEnd = smoothstep(_LineYPeriod - linePaintHeight - smooth, _LineYPeriod - linePaintHeight, periodEnd - heightValue);
                float inLine = afterStart * beforeEnd;

                //return float4((heightValue - periodStart) / _LineYPeriod, 0, 0, 1);
                return inLine * _LineColor + (1 - inLine) * _MainColor;
            }
            ENDCG
        }
    }
}