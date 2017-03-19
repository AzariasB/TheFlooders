Shader "Custom/TerrainIsoMap" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
        
    _BGColor ("Background color", Color) = (0,0,0,1)
    _LinesColor ("Line color", Color) = (1,1,1,1)
    _LinesSpacing ("Lines spacing", float) = 10
    _LinesWidth ("Lines width", float) = 0.5
  }
  SubShader {Tags { "RenderType"="Opaque" "DisableBatching"="True" }
    LOD 200
           
    CGPROGRAM
    #pragma surface surf Lambert
    #pragma target 3.0
            
    struct Input {
      float3 worldPos;
    };
            
    fixed4 _BGColor;
    fixed4 _LinesColor;
    float _LinesSpacing;
    float _LinesWidth;
            
    sampler2D _MainTex;
    void surf (Input IN, inout SurfaceOutput o) {
      // On ramène sur [0, 1] la position du point dans sa "tranche"
      float pointWorldRelPos = (IN.worldPos.y / _LinesSpacing) - floor(IN.worldPos.y / _LinesSpacing);
      float lineLimitRelPos = _LinesWidth / _LinesSpacing;
                  
      o.Albedo =
      	step((1-pointWorldRelPos), (1 - lineLimitRelPos))* _BGColor +
      	step(pointWorldRelPos, lineLimitRelPos) * _LinesColor;
    }
    ENDCG
  } 
  Fallback "Diffuse"
}