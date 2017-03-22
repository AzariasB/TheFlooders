// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

     Shader "Custom/OtherAltShader"
     {
      Properties 
      {
      _Color ("Color", Color) = (0.5, 0.5, 0.5, 0.5)
      _Step ("Step", Float) = 50.0
      }
     
      SubShader
      {
          Pass
          {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      
      fixed4 _Color;
      float _Step;
      
      struct v2f
      {
          float4 pos : SV_POSITION;
          float3 worldPos : TEXCOORD0;
      };
      
      v2f vert (appdata_base v)
      {
          v2f o;
          o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
          o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
          return o;
      }
      
      half4 frag (v2f i) : COLOR
      {
          return _Color * fixed4(
        1.0 - pow(
          (float)((int)i.worldPos.y % (int)_Step) / _Step,
          2), 1, 0, 1);
      }
      ENDCG
          }
      }
     }