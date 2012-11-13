Shader "Mindstorm/PictureFrame" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Layer ("Top Layer (RGB)", 2D) = "black" {}
}

SubShader {
	Tags {"Queue"="Geometry" "RenderType"="opaque"}
	Lighting Off
CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _Layer;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed4 cl = tex2D(_Layer, IN.uv_MainTex);
	o.Albedo = c.rgb+cl;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
