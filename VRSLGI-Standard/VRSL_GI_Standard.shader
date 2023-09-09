// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "VRSL/GI-Addon/Standard Shader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Enum(R,0,RG,1,RGB,2,RGBA,3)] _ShadowMaskActiveChannels ("Shadow Mask Active Channels", Int) = 0

        [Enum(R,0,G,1,B,2,A,3)] _VRSLSmoothnessChannel ("Smoothness texture channel", Float) = 3
        [Enum(R,0,G,1,B,2,A,3)] _VRSLMetallicChannel ("Metallic texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0
        [ToggleOff] _VRSLGIToggle("Use VRSL GI", Float) = 1.0
        [ToggleOff] _UseGlobalVRSLLightTexture("Use Global VRSL Light Texture", Float) = 0.0
        [ToggleOff] useVRSLGISpecular("Use VRSL GI Specular", Float) = 1.0
        _VRSLDiffuseMix ("VRSL Diffuse Mix", Range(0, 1)) = 1.0
        _VRSLMetallicGlossMap("VRSL Metallic Gloss Map", 2D) = "white" {}
        [ToggleUI] _UseVRSLMetallicGlossMap ("Use VRSL Metallic Gloss Map", Int) = 0
        _VRSLMetallicMapStrength("VRSL Metallic Map Mix",  Range(0.0, 1.0)) = 1.0
        _VRSLGlossMapStrength("VRSL Gloss Map Mix",  Range(0.0, 1.0)) = 1.0
        _VRSLSpecularShine("VRSL Specular Shine",  Range(0.0, 1.0)) = 1.0

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

        _VRSLGlossiness("VRSL Smoothness", Range(0, 1)) = 1
        _VRSLSpecularStrength("VRSL Specular Strength", Range(0.0, 1.0)) = 0.5
        _VRSLGIStrength("GI Strength", Range(0.1, 500)) = 1.0
        _VRSLSpecularMultiplier("Specular Multiplier", Range(1, 10)) = 1.0

		//VRSL Stuff
		[ToggleUI] _VRSLToggle ("Enable VRSL", Int) = 0
        [Enum(Diffuse Shading,0,Simple Tint,1,Toon,2)] _VRSLGIDiffuseMode ("VRSL GI Diffuse Mode", Int) = 0
		[ToggleUI] _DMXEmissionMapMix ("Mixture", Int) = 0
		[ToggleUI] _UseLegacyDMXTextures ("Legacy DMX Textures", Int) = 0

		[NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridStrobeTimer ("OSC Grid Render Texture (For Strobe Timings", 2D) = "white" {}
        [Enum(UV0,0,UV1,1, UV2,2, UV3,3, UV4,4)]_VRSLShadowMaskUVSet("UV Set for occlusion map", Float) = 1

        [NoScaleOffset] _VRSL_LightTexture("VRSL Light Texture", 2D) = "white" {}
        [NoScaleOffset] _VRSL_LightCounter("VRSL Light Counter Texture", 2D) = "white" {}

        [ToggleUI] _UseVRSLShadowMask1 ("Use VRSL Shadow Mask 1", Int) = 0
        [NoScaleOffset] _VRSLShadowMask1("VRSL GI ShadowMask 1", 2D) = "white" {}
        _UseVRSLShadowMask1RStrength("VRSL SM 1 R Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask1GStrength("VRSL SM 1 G Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask1BStrength("VRSL SM 1 B Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask1AStrength("VRSL SM 1 A Strength", Range(0.0, 1.0)) = 1.0
        [ToggleUI] _UseVRSLShadowMask2 ("Use VRSL Shadow Mask 2", Int) = 0
        [NoScaleOffset] _VRSLShadowMask2("VRSL GI ShadowMask 2", 2D) = "white" {}
        _UseVRSLShadowMask2RStrength("VRSL SM 2 R Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask2GStrength("VRSL SM 2 G Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask2BStrength("VRSL SM 2 B Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask2AStrength("VRSL SM 2 A Strength", Range(0.0, 1.0)) = 1.0
        [ToggleUI] _UseVRSLShadowMask3 ("Use VRSL Shadow Mask 3", Int) = 0
        [NoScaleOffset] _VRSLShadowMask3("VRSL GI ShadowMask 3", 2D) = "white" {}
        _UseVRSLShadowMask3RStrength("VRSL SM 3 R Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask3GStrength("VRSL SM 3 G Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask3BStrength("VRSL SM 3 B Strength", Range(0.0, 1.0)) = 1.0
        _UseVRSLShadowMask3AStrength("VRSL SM 3 A Strength", Range(0.0, 1.0)) = 1.0
        [ToggleUI] _VRSLInvertSmoothnessMap("VRSL GI Invert Smoothness Map", Float) = 0.0 
        [ToggleUI] _VRSLInvertMetallicMap("VRSL GI Invert Metallic Map", Float) = 0.0

        [Enum(GGX,0, Beckman,1, Blinn Phong,2)]_VRSLSpecularFunction("VRSL Specular Function", Int) = 0

        

        
		_ThirteenChannelMode ("13-Channel Mode", Int) = 0
		_DMXChannel ("Starting DMX Channel", Int) = 0
		_DMXEmissionMap("DMX Emission Map", 2D) = "white" {}
		[ToggleUI] _NineUniverseMode ("Extended Universe Mode", Int) = 0
		[ToggleUI] _PanInvert ("Invert Mover Pan", Int) = 0
		[ToggleUI] _TiltInvert ("Invert Mover Tilt", Int) = 0
		[ToggleUI] _EnablePanMovement ("Enable Pan Movement", Int) = 0
		[ToggleUI] _EnableTiltMovement ("Enable Tilt Movement", Int) = 0
		[ToggleUI] _EnableStrobe ("Enable Strobe", Int) = 0
		[ToggleUI] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
		[ToggleUI] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
		_FixtureBaseRotationY("Mover Pan Offset (Blue + Green)", Range(-540,540)) = 0
		_FixtureRotationX("Mover Tilt Offset (Blue)", Range(-180,180)) = 0
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		_FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180
		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,500)) = 1.0
		[HDR]_EmissionDMX("Color", Color) = (1,1,1)

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

        [ToggleUI]_LTCGI("LTCGI", Int) = 0
    	[ToggleUI]_LTCGI_DIFFUSE_OFF("LTCGI Disable Diffuse", Int) = 0
		[ToggleUI]_LTCGI_SPECULAR_OFF("LTCGI Disable Specular", Int) = 0
		_LTCGI_mat("LTC Mat", 2D) = "black" {}
        _LTCGI_amp("LTC Amp", 2D) = "black" {}
        _LTCGIStrength("LTCGI Strength", Range(0, 2)) = 1

		_AreaLitOcclusion("Occlusion", 2D) = "white" {}
		[Enum(UV0,0,UV1,1, UV2,2, UV3,3, UV4,4)]_OcclusionUVSet("UV Set for occlusion map", Float) = 0
		//End VRSL Stuff


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    SubShader
    {
        Tags { "Queue" = "AlphaTest+1" "RenderType" = "Opaque" "PerformanceChecks"="False" }
        LOD 300


        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" 
            "LTCGI"="_LTCGI"}

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
           // #pragma enable_d3d11_debug_symbols
            #pragma target 5.0

            // -------------------------------------
            
            #pragma shader_feature_local _ _VRSL_GI_ON
            #pragma shader_feature_local _ _VRSL_GI_SPECULARHIGHLIGHTS
            #pragma shader_feature_local _ _VRSL_SPEC_R _VRSL_SPEC_G _VRSL_SPEC_B _VRSL_SPEC_A
            #pragma shader_feature_local _ _VRSL_SPECFUNC_GGX _VRSL_SPECFUNC_BECKMAN _VRSL_SPECFUNC_PHONG
            #pragma shader_feature_local _ _VRSL_METAL_R _VRSL_METAL_G _VRSL_METAL_B _VRSL_METAL_A
            #pragma shader_feature_local _ _VRSL_SHADOWMASK_UV0 _VRSL_SHADOWMASK_UV1 _VRSL_SHADOWMASK_UV2 _VRSL_SHADOWMASK_UV3 _VRSL_SHADOWMASK_UV4
            #pragma shader_feature_local _ _VRSL_DIFFUSETINT _VRSL_DIFFUSETOON
            #pragma shader_feature_local _VRSL_SHADOWMASK1
            #pragma shader_feature_local _VRSL_SHADOWMASK2
            #pragma shader_feature_local _VRSL_SHADOWMASK3
            #pragma shader_feature_local _VRSL_GLOBALLIGHTTEXTURE
            #pragma shader_feature_local _VRSL_MG_MAP
            #pragma shader_feature_local _ _VRSL_SHADOWMASK_RG _VRSL_SHADOWMASK_RGB _VRSL_SHADOWMASK_RGBA

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _PARALLAXMAP

            //VRSL Stuff
			#pragma shader_feature_local _VRSL_ON
			#pragma shader_feature_local _ _VRSLTHIRTEENCHAN_ON _VRSLONECHAN_ON
			#pragma shader_feature_local _VRSLPAN_ON
			#pragma shader_feature_local _VRSLTILT_ON
		//	#pragma shader_feature_local _STROBE_ON
			#pragma shader_feature_local _VRSL_MIX_MULT
			#pragma shader_feature_local _VRSL_MIX_ADD
			#pragma shader_feature_local _VRSL_MIX_MIX
			#pragma shader_feature_local _VRSL_LEGACY_TEXTURES
			//End VRSL Stuff


            #pragma shader_feature_local _ _AREALIT_ON
            #pragma shader_feature_local LTCGI
			#pragma shader_feature_local LTCGI_DIFFUSE_OFF
			#pragma shader_feature_local LTCGI_SPECULAR_OFF

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            //#define LTCGI_API_V2

            #pragma vertex vertBase
            #pragma fragment fragBase
            #define VRSL_ENABLED defined(_VRSL_ON)
            #define VRSL_GI_ENABLED defined(_VRSL_GI_ON)
            #include "CGIncludes/VRSLGIStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 5.0

            // -------------------------------------


            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP

            //VRSL Stuff
			#pragma shader_feature_local _VRSL_ON
			#pragma shader_feature_local _ _VRSLTHIRTEENCHAN_ON _VRSLONECHAN_ON
			#pragma shader_feature_local _VRSLPAN_ON
			#pragma shader_feature_local _VRSLTILT_ON
			//End VRSL Stuff

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #define VRSL_ENABLED defined(_VRSL_ON)
            #define VRSL_GI_ENABLED defined(_VRSL_GI_ON)
            #include "CGIncludes/VRSLGIStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 5.0

            // -------------------------------------


            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _PARALLAXMAP

            //VRSL Stuff
			#pragma shader_feature_local _VRSL_ON
			#pragma shader_feature_local _ _VRSLTHIRTEENCHAN_ON _VRSLONECHAN_ON
			#pragma shader_feature_local _VRSLPAN_ON
			#pragma shader_feature_local _VRSLTILT_ON
			//End VRSL Stuff

            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster
            #define VRSL_ENABLED defined(_VRSL_ON)
            #define VRSL_GI_ENABLED defined(_VRSL_GI_ON)
            #include "CGIncludes/VRSLGIStandardShadow.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            //VRSL Stuff
			#pragma shader_feature_local _VRSL_ON
			#pragma shader_feature_local _ _VRSLTHIRTEENCHAN_ON _VRSLONECHAN_ON
			#pragma shader_feature_local _VRSLPAN_ON
			#pragma shader_feature_local _VRSLTILT_ON
		//	#pragma shader_feature_local _STROBE_ON
			#pragma shader_feature_local _VRSL_MIX_MULT
			#pragma shader_feature_local _VRSL_MIX_ADD
			#pragma shader_feature_local _VRSL_MIX_MIX
			//End VRSL Stuff
            #define VRSL_ENABLED defined(_VRSL_ON)
            #define VRSL_GI_ENABLED defined(_VRSL_GI_ON)
            #include "CGIncludes/VRSLGIStandardMeta.cginc"
            ENDCG
        }
    }


    FallBack "VertexLit"
    CustomEditor "VRSL_GI_StandardShaderGUI"
}
