Shader "Mindstorm/Cannon/Cannon" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
	_SpecTex ("Specular Texture", 2D) = "white" {}
	_SelfIllumColor ("Self Illum Color", Color) = (1,1,1,1)
	_SelfIllum ("SelfIllum Texture", 2D) = "black"{}
	_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	_BumpMap ("Normalmap", 2D) = "bump" {}
}

SubShader {
	Tags { "RenderType"="Opaque"}
	//LOD 400
	Cull Off
	//Zwrite On
	//Ztest Less
	
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0

sampler2D _MainTex;
sampler2D _SpecTex;
sampler2D _SelfIllum;
sampler2D _BumpMap;
samplerCUBE _Cube;

fixed4 _Color;
fixed4 _ReflectColor;
half _Shininess;
fixed4 _SelfIllumColor;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 worldRefl;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 spectex = tex2D(_SpecTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	fixed4 illumtex = tex2D(_SelfIllum, IN.uv_MainTex);
	
	o.Albedo = c.rgb;
	
	o.Gloss = spectex;
	o.Specular = _Shininess;
	
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	fixed4 cuberefl = texCUBE (_Cube, worldRefl);
	//cuberefl *= tex.a;
	o.Emission = cuberefl.rgb * _ReflectColor.rgb * spectex.rgb + _SelfIllumColor.rgb*illumtex.rgb;
}
ENDCG
}

FallBack "Reflective/Bumped Diffuse"
}
