
Shader "Terrain/TerrainIsoMap"
{
    Properties
    {
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
                float4 vertex : SV_POSITION;
                fixed2 gradient : TEXCOORD0;
                float3 objSpacePos : TEXCOORD1;
            };

            half4 _MainColor;
            half4 _LineColor;
            half _LineYPeriod;
            half _LineWidth;
            half _Smoothness;

            v2f vert (
                float4 vertex : POSITION, // Position du sommet
                // float4 uv : TEXCOORD0, // Coordonnées de texture, non utilisées ici
                float4 uv2 : TEXCOORD1 // UV secondaires : gradient du terrain
                )
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.gradient = uv2.xy;
                o.objSpacePos = vertex;
                return o;
            }

            // pixel shader, no inputs needed
            fixed4 frag (v2f i) : SV_Target
            {
                // Extraction des infos passées au shader dans
                // les canaux 3 et 4 des coordonnées UV.
                // Il s'agit du gradient de la heightmap, interpollé entre chaque
                // sommet du mesh.
                half2 objectSpacegradient = i.gradient.xy;

                // Hauteur des lignes, projetée sur l'espace vertical décrit par la heightmap
                half linePaintHeight = length(objectSpacegradient) * _LineWidth;

            	// Récupération de l'information de hauteur
                half heightValue = dot(i.objSpacePos, float3(0, 1, 0));

                // Décalage du pixel courant par rapport au début de sa période
                float periodStart = floor(heightValue / _LineYPeriod) * _LineYPeriod;
                float periodEnd = periodStart + _LineYPeriod;

                // Fin de la bande
                float maxHeight = periodStart + linePaintHeight;

                // lissage des limites pour faire plus joli
                const float smooth = linePaintHeight*_Smoothness;
                float afterStart = smoothstep(periodStart, periodStart + smooth, heightValue);
                float beforeEnd = smoothstep(_LineYPeriod - linePaintHeight - smooth, _LineYPeriod - linePaintHeight, periodEnd - heightValue);
                float inLine = afterStart * beforeEnd;

                //return float4(linePaintHeight*2, 0, 0, 1);
                return inLine * _LineColor + (1 - inLine) * _MainColor;
            }
            ENDCG
        }
    }
}