// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "TUIO/Unlit Transparent Cutout Overlay" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	 _Mask ("Culling Mask", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0
}

SubShader {
	Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	ZTest Always
	ZWrite Off
	Cull Off
	Blend SrcAlpha OneMinusSrcAlpha 
	Pass {
		Lighting Off
		Alphatest Greater [_Cutoff]
		
		SetTexture [_Mask] {
			constantColor [_Color]
			combine texture*constant
		}
		
		SetTexture [_MainTex] {
			constantColor [_Color]
			combine texture*constant, texture*previous
		} 
	}
}
}