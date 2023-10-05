// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_STANDARD_INPUT_INCLUDED
#define UNITY_STANDARD_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "VRSLGIStandardConfig.cginc"
#include "UnityPBSLighting.cginc" // TBD: remove
#include "VRSLGIStandardUtils.cginc"

#include "Assets/VRSL Addons/VRSL-GI Shader Package/VRSLGI-Functions.cginc"


//VRSL Stuff
#if VRSL_ENABLED
	#include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/VRSLDMX.cginc"
#endif
//End VRSL Stuff

#if _VRSL_AUDIOLINK_ON
    #include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"
    #if defined(_VRSL_AUDIOLINK_ON) && !defined(_VRSL_ON)
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, _EnableAudioLink)
            UNITY_DEFINE_INSTANCED_PROP(float, _EnableColorChord)
            UNITY_DEFINE_INSTANCED_PROP(float, _NumBands)
            UNITY_DEFINE_INSTANCED_PROP(float, _Band)
            UNITY_DEFINE_INSTANCED_PROP(float, _BandMultiplier)
            UNITY_DEFINE_INSTANCED_PROP(float, _Delay)
            UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
            UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleX)
            UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleY)
            UNITY_DEFINE_INSTANCED_PROP(float, _ThemeColorTarget)
            UNITY_DEFINE_INSTANCED_PROP(uint, _EnableThemeColorSampling)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
            UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
            UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensityBlend)
            UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity) 
        UNITY_INSTANCING_BUFFER_END(Props)
        float4 getBaseEmission()
        {
            return UNITY_ACCESS_INSTANCED_PROP(Props, _Emission);
        }
        float getGlobalIntensity()
        {
            return lerp(1.0,UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity), UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensityBlend));
        }

        float getFinalIntensity()
        {
            return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
        }
    #endif

    Texture2D   _SamplingTexture;
    Texture2D   _AudioLinkEmissionMap;

    float getEnableAudioLink()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableAudioLink);

    }
    float getEnableColorChord()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableColorChord);
    }
    float getNumOfBands()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _NumBands);
    }
    float getBand()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _Band);
    }
    float getBandMultiplier()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _BandMultiplier);
    }
    float getDelay()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _Delay);
    }
    uint getEnableColoTextureSample()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableColorTextureSample);
    }    
    float getTextureColorSampleX()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _TextureColorSampleX);
    }
    float getTextureColorSampleY()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _TextureColorSampleY);
    }
    float getThemeColorTarget()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _ThemeColorTarget);
    } 
    uint getEnableThemeColor()
    {
        return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableThemeColorSampling);
    }          
#endif

#if _AREALIT_ON
	#include "Assets/AreaLit/Shader/Lighting.hlsl"
#endif

#ifdef LTCGI
	#include "Packages/at.pimaker.ltcgi/Shaders/LTCGI.cginc"
#endif
//---------------------------------------
// Directional lightmaps & Parallax require tangent space too
#if (_NORMALMAP || DIRLIGHTMAP_COMBINED || _PARALLAXMAP)
    #define _TANGENT_TO_WORLD 1
#endif

#if (_DETAIL_MULX2 || _DETAIL_MUL || _DETAIL_ADD || _DETAIL_LERP)
    #define _DETAIL 1
#endif

//---------------------------------------
half4       _Color;
half        _Cutoff;

sampler2D   _MainTex;
float4      _MainTex_ST;

sampler2D   _DetailAlbedoMap;
float4      _DetailAlbedoMap_ST;

sampler2D   _BumpMap;
half        _BumpScale;

sampler2D   _DetailMask;
sampler2D   _DetailNormalMap;
half        _DetailNormalMapScale;

sampler2D   _SpecGlossMap;
sampler2D   _MetallicGlossMap;
half        _Metallic;
float       _Glossiness;
float       _GlossMapScale;

sampler2D   _OcclusionMap;
half        _OcclusionStrength;

sampler2D   _ParallaxMap;
half        _Parallax;
half        _UVSec;

half4       _EmissionColor;
sampler2D   _EmissionMap;

#if VRSL_ENABLED
	Texture2D   	_DMXEmissionMap;
	int				_ThirteenChannelMode;
	int				_DMXEmissionMapMix;
	float4 			_FixtureRotationOrigin;
#endif
    float			_FixtureMaxIntensity;
	float			_UniversalIntensity;    


#if _VRSL_AUDIOLINK_ON
    float4 RGBtoHSV(in float3 RGB)
    {
        float3 HSV = 0;
        HSV.z = max(RGB.r, max(RGB.g, RGB.b));
        float M = min(RGB.r, min(RGB.g, RGB.b));
        float C = HSV.z - M;
        if (C != 0)
        {
            HSV.y = C / HSV.z;
            float3 Delta = (HSV.z - RGB) / C;
            Delta.rgb -= Delta.brg;
            Delta.rg += float2(2,4);
            if (RGB.r >= HSV.z)
                HSV.x = Delta.b;
            else if (RGB.g >= HSV.z)
                HSV.x = Delta.r;
            else
                HSV.x = Delta.g;
            HSV.x = frac(HSV.x / 6);
        }
        return float4(HSV,1);
    }
    float3 Hue(float H)
    {
        float R = abs(H * 6 - 3) - 1;
        float G = 2 - abs(H * 6 - 2);
        float B = 2 - abs(H * 6 - 4);
        return saturate(float3(R,G,B));
    }
    float4 HSVtoRGB(in float3 HSV)
    {
        return float4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z,1);
    }

    float4 GetTextureSampleColor()
    {
        float4 rawColor = _SamplingTexture.SampleLevel(VRSL_BilinearClampSampler, float2(getTextureColorSampleX(),getTextureColorSampleY()), 0);
        float4 h = RGBtoHSV(rawColor.rgb);
        h.z = 1.0;
        return(HSVtoRGB(h) * _RenderTextureMultiplier);
    }
    float4 GetThemeSampleColor()
    {
        switch(UNITY_ACCESS_INSTANCED_PROP(Props, _ThemeColorTarget))
        {
            case 1:
                return AudioLinkData(ALPASS_THEME_COLOR0);
            case 2:
                return AudioLinkData(ALPASS_THEME_COLOR1);
            case 3:
                return AudioLinkData(ALPASS_THEME_COLOR2);
            case 4:
                return AudioLinkData(ALPASS_THEME_COLOR3);
            default:
                return float4(0,0,0,1);
        }
    }

    inline float AudioLinkLerp3_g5( int Band, float Delay )
    {
        return AudioLinkLerp( ALPASS_AUDIOLINK + float2( Delay, Band ) ).r;
    }

    float GetAudioReactAmplitude()
    {
        if(getEnableAudioLink() > 0)
        {
            return AudioLinkLerp3_g5(getBand(), getDelay()) * getBandMultiplier();
        }
        else
        {
            return 1;
        }
    }

    float4 GetColorChordLight()
    {
        return AudioLinkData(ALPASS_CCLIGHTS).rgba *3;
    }

    float4 GetAudioLinkEmissionColor()
    {
        float4 emissiveColor = UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
        float4 col =  getEnableColoTextureSample() > 0 ? ((emissiveColor.r + emissiveColor.g + emissiveColor.b)/3.0) * GetTextureSampleColor() : emissiveColor;
        col =  getEnableThemeColor() > 0 ? ((emissiveColor.r + emissiveColor.g + emissiveColor.b)/3.0) * GetThemeSampleColor() : col;
        col = (getEnableColorChord() == 1 ? GetColorChordLight() * 1.5:  col) * _FixtureMaxIntensity;
        float intensity = GetAudioReactAmplitude() * getGlobalIntensity() * getFinalIntensity() * _UniversalIntensity;
        return lerp(float4(0,0,0,0), col, intensity*intensity*intensity);
    }
    
    float4 getAudioLinkEmissionMask(float2 uv)
    {
        return _AudioLinkEmissionMap.Sample(VRSL_BilinearClampSampler, uv);
    }

#endif



struct SampleData {
	//float4 localPos;
	float3 objPos;
	float3 depthNormal;
	float3 worldPixelPos;
	float3 normal;
	float4 scaleTransform;
};



//-------------------------------------------------------------------------------------
// Input functions

struct VertexInput
{
    float4 vertex   : POSITION;
    half3 normal    : NORMAL;
    float4 color	: COLOR_Centroid;
    float2 uv0      : TEXCOORD0;
    float2 uv1      : TEXCOORD1;
//#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META) || 
    float2 uv2      : TEXCOORD2;
    float2 uv3      : TEXCOORD3;
    float2 uv4      : TEXCOORD4;
//#endif
#ifdef _TANGENT_TO_WORLD
    half4 tangent   : TANGENT;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

float4 TexCoords(VertexInput v)
{
    float4 texcoord;
    texcoord.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
    texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? v.uv0 : v.uv1), _DetailAlbedoMap);
    return texcoord;
}



#ifdef LTCGI
    float3 get_camera_pos_vrsl() {
        float3 worldCam;
        worldCam.x = unity_CameraToWorld[0][3];
        worldCam.y = unity_CameraToWorld[1][3];
        worldCam.z = unity_CameraToWorld[2][3];
        return worldCam;
    }
    static float3 camera_pos = get_camera_pos_vrsl();
#endif

//#ifdef _VRSL_GI
    float2 VRSLShadowMaskCoords(VertexInput v)
    {
        
        float2 texcoord;
        #if _VRSL_SHADOWMASK_UV0
            texcoord = v.uv0; // Always source from uv0
        #elif _VRSL_SHADOWMASK_UV1
            texcoord = v.uv1; // Always source from uv0
        #elif _VRSL_SHADOWMASK_UV2
            texcoord = v.uv2; // Always source from uv0
        #elif _VRSL_SHADOWMASK_UV3
            texcoord = v.uv3 // Always source from uv0
        #elif _VRSL_SHADOWMASK_UV4
            texcoord = v.uv4; // Always source from uv0
        #endif
        return texcoord;
    }
//#endif
float2 SelectUVSet(VertexInput v, int selection)
{
	float2 uvs[] = {v.uv0, v.uv1, v.uv2, v.uv3, v.uv4};
	return uvs[selection];
}


    float2 AreaLitCoords(VertexInput v)
    {
        float2 texcoord = SelectUVSet(v, _OcclusionUVSet);
        texcoord = TRANSFORM_TEX(texcoord, _MainTex);
        return texcoord;
    }

    float2 LTCGICoords(VertexInput v)
    {
        return TRANSFORM_TEX(v.uv1, _MainTex);
    }



half DetailMask(float2 uv)
{
    return tex2D (_DetailMask, uv).a;
}

float4 SampleTexture(Texture2D tex, float2 uv){
	float4 col = 0;
	col = tex.Sample(VRSL_BilinearClampSampler, uv);
	return col;
}

half3 Albedo(float4 texcoords)
{
    half3 albedo = _Color.rgb * tex2D (_MainTex, texcoords.xy).rgb;
#if _DETAIL
    #if (SHADER_TARGET < 30)
        // SM20: instruction count limitation
        // SM20: no detail mask
        half mask = 1;
    #else
        half mask = DetailMask(texcoords.xy);
    #endif
    half3 detailAlbedo = tex2D (_DetailAlbedoMap, texcoords.zw).rgb;
    #if _DETAIL_MULX2
        albedo *= LerpWhiteTo (detailAlbedo * unity_ColorSpaceDouble.rgb, mask);
    #elif _DETAIL_MUL
        albedo *= LerpWhiteTo (detailAlbedo, mask);
    #elif _DETAIL_ADD
        albedo += detailAlbedo * mask;
    #elif _DETAIL_LERP
        albedo = lerp (albedo, detailAlbedo, mask);
    #endif
#endif
    return albedo;
}

half Alpha(float2 uv)
{
#if defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
    return _Color.a;
#else
    return tex2D(_MainTex, uv).a * _Color.a;
#endif
}

half Occlusion(float2 uv)
{
#if (SHADER_TARGET < 30)
    // SM20: instruction count limitation
    // SM20: simpler occlusion
    return tex2D(_OcclusionMap, uv).g;
#else
    half occ = tex2D(_OcclusionMap, uv).g;
    return LerpOneTo (occ, _OcclusionStrength);
#endif
}



half4 SpecularGloss(float2 uv)
{
    half4 sg;
#ifdef _SPECGLOSSMAP
    #if defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
        sg.rgb = tex2D(_SpecGlossMap, uv).rgb;
        sg.a = tex2D(_MainTex, uv).a;
    #else
        sg = tex2D(_SpecGlossMap, uv);
    #endif
    sg.a *= _GlossMapScale;
#else
    sg.rgb = _SpecColor.rgb;
    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        sg.a = tex2D(_MainTex, uv).a * _GlossMapScale;
    #else
        sg.a = _Glossiness;
    #endif
#endif
    return sg;
}

#if defined(_VRSL_GI) && defined(_VRSL_GI_SPECULARHIGHLIGHTS) && !defined(_VRSL_MG_MAP) && !defined(VRSL_GI_PROJECTOR)
    half4 MetallicGlossMap(float2 uv)
    {
        return tex2D(_MetallicGlossMap, uv);
    }
    half4 MainTexture(float2 uv)
    {
        return tex2D(_MainTex, uv);
    }
    half2 MetallicGlossStandard(float4 mgm, float4 mainTex)
    {
        half2 mg;
        #ifdef _METALLICGLOSSMAP
            #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                mg.r = mgm.r;
                mg.g = mainTex.a;
            #else
                mg = mgm.ra;
            #endif
            mg.g *= _GlossMapScale;
        #else
            mg.r = _Metallic;
            #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                mg.g = mainTex.a * _GlossMapScale;
            #else
                mg.g = _Glossiness;
            #endif
        #endif
            return mg;
    }

    half2 MetallicGlossVRSL(float4 mgm)
    {
        half2 mg = half2(1,1);
        float4 tex = mgm;

        #if _VRSL_METAL_R
            mg.x = tex.r;
        #elif _VRSL_METAL_G
            mg.x = tex.g;
        #elif _VRSL_METAL_B
            mg.x = tex.b;
        #elif _VRSL_METAL_A
            mg.x = tex.a;
        #endif

        #if _VRSL_SPEC_R
            mg.y = tex.r;
        #elif _VRSL_SPEC_G
            mg.y = tex.g;
        #elif _VRSL_SPEC_B
            mg.y = tex.b;
        #elif _VRSL_SPEC_A
            mg.y = tex.a;
        #endif
        return mg;
    }
#else
    half2 MetallicGloss(float2 uv)
    {
        half2 mg;

    #ifdef _METALLICGLOSSMAP
        #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            mg.r = tex2D(_MetallicGlossMap, uv).r;
            mg.g = tex2D(_MainTex, uv).a;
        #else
            mg = tex2D(_MetallicGlossMap, uv).ra;
        #endif
        mg.g *= _GlossMapScale;
    #else
        mg.r = _Metallic;
        #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            mg.g = tex2D(_MainTex, uv).a * _GlossMapScale;
        #else
            mg.g = _Glossiness;
        #endif
    #endif
        return mg;
    }
#endif

#ifdef VRSL_GI_PROJECTOR
    half2 VRSLMetallicGloss(float2 uv)
    {
        return float2(1,1);
    }
#endif

#ifndef VRSL_GI_PROJECTOR
    half2 VRSLMetallicGloss(float2 uv)
    {
        half2 mg = half2(1,1);
        float4 tex = tex2D(_VRSLMetallicGlossMap, uv);

        #if _VRSL_METAL_R
            mg.x = tex.r;
        #elif _VRSL_METAL_G
            mg.x = tex.g;
        #elif _VRSL_METAL_B
            mg.x = tex.b;
        #elif _VRSL_METAL_A
            mg.x = tex.a;
        #endif

        #if _VRSL_SPEC_R
            mg.y = tex.r;
        #elif _VRSL_SPEC_G
            mg.y = tex.g;
        #elif _VRSL_SPEC_B
            mg.y = tex.b;
        #elif _VRSL_SPEC_A
            mg.y = tex.a;
        #endif
        //mg = saturate(mg);
        return mg;
    }


#endif

#if _AREALIT_ON
float4 AreaLitOcclussionMask(float2 uv)
{
    return _AreaLitOcclusion.SampleLevel(VRSL_BilinearClampSampler, uv, 0);
}

#endif

half2 MetallicRough(float2 uv)
{
    half2 mg;
#ifdef _METALLICGLOSSMAP
    mg.r = tex2D(_MetallicGlossMap, uv).r;
#else
    mg.r = _Metallic;
#endif

#ifdef _SPECGLOSSMAP
    mg.g = 1.0f - tex2D(_SpecGlossMap, uv).r;
#else
    mg.g = 1.0f - _Glossiness;
#endif
    return mg;
}

half3 Emission(float2 uv)
{
#ifndef _EMISSION
    return 0;
#else
    return tex2D(_EmissionMap, uv).rgb * _EmissionColor.rgb;
#endif
}

half3 DMXEmission(float2 uv)
{
	#if VRSL_ENABLED
		// float3 emissTex = SampleTexture(_DMXEmissionMap, uv, sd);
		float3 emissionDMX = pow(getAltBaseEmission().rgb, 2.2);
		float3 emissTex = emissionDMX;
        float dmxIntensity = 1.0f;
		uint dmxChannel = GetDMXChannel();
		float4 dmxColor = float4(1,1,1,1);
		#if _VRSLTHIRTEENCHAN_ON
			#ifdef _VRSL_LEGACY_TEXTURES
                dmxIntensity = ReadDMX(dmxChannel +(uint)5, _OSCGridRenderTextureRAW);
				dmxColor = dmxIntensity * GetDMXColor(dmxChannel+(uint)7);
			#else
                dmxIntensity = ReadDMX(dmxChannel +(uint)5, _Udon_DMXGridRenderTexture);
				dmxColor = dmxIntensity * GetDMXColor(dmxChannel+(uint)7);
			#endif
			//#if _STROBE_ON
				float strobe = GetImmediateStrobeOutput(dmxChannel + (uint)6);
				dmxColor *= strobe;
			//#endif
		#elif _VRSLONECHAN_ON
            #ifdef _VRSL_LEGACY_TEXTURES
            dmxIntensity = ReadDMX(dmxChannel, _OSCGridRenderTextureRAW);
            dmxColor = float4(emissionDMX,1) * dmxIntensity;
            #else
            dmxIntensity = ReadDMX(dmxChannel, _Udon_DMXGridRenderTexture);
            dmxColor = float4(emissionDMX,1) * dmxIntensity;
            #endif
        #else
		//5-Channel Mode
			#ifdef _VRSL_LEGACY_TEXTURES
                dmxIntensity = ReadDMX(dmxChannel, _OSCGridRenderTextureRAW);
				dmxColor = dmxIntensity * GetDMXColor(dmxChannel+(uint)1);
			#else
                dmxIntensity = ReadDMX(dmxChannel, _Udon_DMXGridRenderTexture);
				dmxColor = dmxIntensity * GetDMXColor(dmxChannel+(uint)1);
			#endif
			//#if _STROBE_ON
				float strobe = GetImmediateStrobeOutput(dmxChannel + (uint)4);
				dmxColor *= strobe;
		//	#endif
		#endif
		emissTex = (emissTex * _FixtureMaxIntensity) * dmxColor.rgb;
        float sliders = getGlobalIntensity() * getFinalIntensity() * _UniversalIntensity;
//		emissTex = Filtering(emissTex, _HueEmiss, _SaturationEmiss, _BrightnessEmiss, _ContrastEmiss, 0);
        dmxIntensity *= sliders;
        emissTex = lerp(float3(0,0,0), emissTex, dmxIntensity * dmxIntensity * dmxIntensity);
		return emissTex;
	#else
		return 0;
	#endif
}

#if VRSL_ENABLED
float4 DMXMovement(float4 input, float4 vertexColor, int normalsCheck, float pan, float tilt, float4 rotationOrigin)
{
	#if VRSL_ENABLED
		#if _VRSLTHIRTEENCHAN_ON
			return calculateRotations(input, vertexColor, normalsCheck, pan, tilt, rotationOrigin);
		#else
			return input;
		#endif
	#else
		return input;
	#endif
}
#endif

#ifdef _NORMALMAP
half3 NormalInTangentSpace(float4 texcoords)
{
    half3 normalTangent = UnpackScaleNormal(tex2D (_BumpMap, texcoords.xy), _BumpScale);

#if _DETAIL && defined(UNITY_ENABLE_DETAIL_NORMALMAP)
    half mask = DetailMask(texcoords.xy);
    half3 detailNormalTangent = UnpackScaleNormal(tex2D (_DetailNormalMap, texcoords.zw), _DetailNormalMapScale);
    #if _DETAIL_LERP
        normalTangent = lerp(
            normalTangent,
            detailNormalTangent,
            mask);
    #else
        normalTangent = lerp(
            normalTangent,
            BlendNormals(normalTangent, detailNormalTangent),
            mask);
    #endif
#endif

    return normalTangent;
}
#endif

float4 Parallax (float4 texcoords, half3 viewDir)
{
#if !defined(_PARALLAXMAP) || (SHADER_TARGET < 30)
    // Disable parallax on pre-SM3.0 shader target models
    return texcoords;
#else
    half h = tex2D (_ParallaxMap, texcoords.xy).g;
    float2 offset = ParallaxOffset1Step (h, _Parallax, viewDir);
    return float4(texcoords.xy + offset, texcoords.zw + offset);
#endif

}

#endif // UNITY_STANDARD_INPUT_INCLUDED
