Shader "Custom/Floor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CellColor ("Cell Color", Color) = (0,0,0,0)
		_LineColor ("Line Color", Color) = (1,1,1,1)
		_LineWidth ("Line Width",Float) = 0.01
		_Scale ("Cell Size", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float3 _CellColor;
		float3 _LineColor;
		float _LineWidth;
		float _Scale;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp( _CellColor, _LineColor, saturate( step( 0, frac( IN.worldPos.x / _Scale ) - 1 + _LineWidth ) + step( 0, frac( IN.worldPos.z / _Scale ) - 1 + _LineWidth ) ) );
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
