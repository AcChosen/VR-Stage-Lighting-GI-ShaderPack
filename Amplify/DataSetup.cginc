#ifdef      _VRSLGIGLOBALLIGHTTEXTURE_ON
    Texture2D   _Udon_VRSL_GI_LightTexture;
#else
    Texture2D   _VRSL_LightTexture;
#endif

uniform float4  _VRSL_LightTexture_TexelSize;
SamplerState    VRSL_BilinearClampSampler, VRSLGI_PointClampSampler;
int     _Udon_VRSL_GI_LightCount;



#ifndef VRSL_GI_PROJECTOR
half        _VRSLMetallicMapStrength;
half        _VRSLGlossMapStrength;
half        _VRSLSmoothnessChannel;
half        _VRSLMetallicChannel;
half        _VRSLInvertMetallicMap;
half        _VRSLInvertSmoothnessMap;


int         _UseVRSLShadowMask1;
int         _UseVRSLShadowMask2;
int         _UseVRSLShadowMask3;

#else
int         _VRSLGIVertexFalloff;
float       _VRSLGIVertexAttenuation;

#endif

half        _VRSLGISpecularClamp;
half        _VRSLGIDiffuseClamp;

half        _VRSLGIStrength;


half        _UseVRSLShadowMask1RStrength;
half        _UseVRSLShadowMask1GStrength;
half        _UseVRSLShadowMask1BStrength;
half        _UseVRSLShadowMask1AStrength;

half        _UseVRSLShadowMask2RStrength;
half        _UseVRSLShadowMask2GStrength;
half        _UseVRSLShadowMask2BStrength;
half        _UseVRSLShadowMask2AStrength;

half        _UseVRSLShadowMask3RStrength;
half        _UseVRSLShadowMask3GStrength;
half        _UseVRSLShadowMask3BStrength;
half        _UseVRSLShadowMask3AStrength;
half        _RenderTextureMultiplier;

half        _VRSLShadowMaskUVSet;

//float4      _ProjectorColor;
half        _VRSLProjectorStrength;
void GetData()
{
return;
}
