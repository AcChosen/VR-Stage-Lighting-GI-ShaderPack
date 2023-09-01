Shader "VRSL/GI-Addon/Standard Projector" {
Properties {
	[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Int) = 2
	[Enum(Equal,3,LessEqual,4)] _ZTest("ZTest", Int) = 3
	[Enum(GGX,0,Beckman,1,Blinn Phong,2)] _VRSLSpecularFunction ("VRSL Specular Function", Int) = 0


	[ToggleOff] _VRSLGIToggle("Use VRSL GI", Float) = 1.0
	[ToggleOff] useVRSLGISpecular("Use VRSL GI", Float) = 1.0
	[Enum(Pixel Lighting (All),0,Vertex Lighting (Four),1)] _VRSLGIQuadLightingSystem("ZTest", Int) = 0
	_VRSLGlossiness("VRSL Smoothness", Range(0, 1)) = 0.5
	_VRSLDiffuseMix ("VRSL Diffuse Mix", Range(0, 1)) = 1.0
	_VRSLSpecularStrength("VRSL Specular Strength", Range(0.0, 1)) = 0.5
	_VRSLProjectorStrength("VRSL Projector Strength", Range(1, 5000)) = 500
	_VRSLGIStrength("GI Strength", Range(0.1, 100)) = 1.0
	_VRSLSpecularMultiplier("Specular Multiplier", Range(1, 10)) = 1.0
	_VRSLSpecularShine("VRSL Specular Shine",  Range(0.0, 1.0)) = 1.0
	[ToggleOff] _UseGlobalVRSLLightTexture("Use Global VRSL Light Texture", Float) = 0.0
	[Enum(Squared, 0, Linear, 1, Modified, 2)] _VRSLGIVertexFalloff ("Falloff (use Modified if unsure)", Int) = 2
	[HideInInspector]_VRSLGIVertexAttenuation ("Falloff", Range(0, 10)) = 1


	[HDR] _ProjectorColor("Projector Color", Color) = (1,1,1,1)

	[Enum(UV0,0,UV1,1)] _OcclusionUVSet ("UV Set for occlusion map", Float) = 0

	//// Standard properties ////

	_Color("Color", Color) = (1,1,1,1)
	_MainTex("Albedo", 2D) = "white" {}

	_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

	_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
	_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
	[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

	[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
	_MetallicGlossMap("Metallic", 2D) = "white" {}

	[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
	[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

	_BumpScale("Scale", Float) = 1.0
	[Normal] _BumpMap("Normal Map", 2D) = "bump" {}

	_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
	_ParallaxMap ("Height Map", 2D) = "black" {}

	_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
	_OcclusionMap("Occlusion", 2D) = "white" {}

	_EmissionColor("Color", Color) = (0,0,0)
	_EmissionMap("Emission", 2D) = "white" {}

	_DetailMask("Detail Mask", 2D) = "white" {}

	_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
	_DetailNormalMapScale("Scale", Float) = 1.0
	[Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

	[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

	[Enum(Diffuse Shading,0,Simple Tint,1,Toon,2)] _VRSLGIDiffuseMode ("VRSL GI Diffuse Mode", Int) = 0


        [ToggleUI]_AreaLitToggle("Enable", Int) = 0
		_AreaLitMask("Mask", 2D) = "white" {}
		_AreaLitStrength("Strength", Float) = 1
		_AreaLitRoughnessMult("Roughness Multiplier", Float) = 1
		[NoScaleOffset]_LightMesh("Light Mesh", 2D) = "black" {}
		[NoScaleOffset]_LightTex0("Light Texture 0", 2D) = "white" {}
		[NoScaleOffset]_LightTex1("Light Texture 1", 2D) = "black" {}
		[NoScaleOffset]_LightTex2("Light Texture 2", 2D) = "black" {}
		[NoScaleOffset]_LightTex3("Light Texture 3", 2DArray) = "black" {}
		[ToggleOff]_OpaqueLights("Opaque Lights", Float) = 1.0

		_AreaLitOcclusion("Occlusion", 2D) = "white" {}

        [ToggleUI]_LTCGI("LTCGI", Int) = 0
    	[ToggleUI]_LTCGI_DIFFUSE_OFF("LTCGI Disable Diffuse", Int) = 0
		[ToggleUI]_LTCGI_SPECULAR_OFF("LTCGI Disable Specular", Int) = 0
		_LTCGI_mat("LTC Mat", 2D) = "black" {}
        _LTCGI_amp("LTC Amp", 2D) = "black" {}
        _LTCGIStrength("LTCGI Strength", Float) = 1

		[NoScaleOffset] _VRSL_LightTexture("VRSL Light Texture", 2D) = "white" {}
		[NoScaleOffset] _VRSL_LightCounter("VRSL Light Counter Texture", 2D) = "white" {}		

	// Blending state
	[HideInInspector] _Mode ("__mode", Float) = 0.0
	// [HideInInspector] _SrcBlend ("__src", Float) = 1.0
	// [HideInInspector] _DstBlend ("__dst", Float) = 0.0
	// [HideInInspector] _ZWrite ("__zw", Float) = 1.0
}
CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
ENDCG
 CustomEditor "VRSL_GI_StandardShaderGUI"
SubShader {
	Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
	Pass {
		Name "FORWARD"
		Tags { "LightMode"="ForwardBase" }
		Cull [_Cull]
		Blend DstColor OneMinusSrcAlpha, Zero One
		ZTest [_ZTest] ZWrite Off
		CGPROGRAM
		#pragma target 5.0
		#define VRSL_GI_PROJECTOR
		#pragma shader_feature_local _ _VRSL_GI_ON
		#pragma shader_feature_local _ _VRSL_GI_SPECULARHIGHLIGHTS
		#pragma shader_feature _EMISSION
		#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
		#pragma shader_feature_local _ _VRSL_SPECFUNC_GGX _VRSL_SPECFUNC_BECKMAN _VRSL_SPECFUNC_PHONG
		#pragma shader_feature_local _ _VRSL_DIFFUSETINT _VRSL_DIFFUSETOON
		#pragma shader_feature_local _VRSL_GI_ENFORCELIMIT
		#pragma shader_feature_local _VRSL_GLOBALLIGHTTEXTURE

		#define VERTEXLIGHT_ON
		
		#pragma shader_feature_local _ _AREALIT_ON
		#pragma shader_feature_local LTCGI
		#pragma shader_feature_local LTCGI_DIFFUSE_OFF
		#pragma shader_feature_local LTCGI_SPECULAR_OFF

		#pragma multi_compile_instancing
		#define _VRSL_GI_PROJECTOR
		// #pragma multi_compile_fwdbase


		#pragma vertex vertBase
		#pragma fragment fragBaseProj
		#include "CGIncludes/VRSLGIStandardCoreForward.cginc"
		ENDCG
	}
}
}