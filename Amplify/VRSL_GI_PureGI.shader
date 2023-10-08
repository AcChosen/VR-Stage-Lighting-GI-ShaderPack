// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRSL_GI_PureGI"
{
	Properties
	{
		[NoScaleOffset][SingleLineTexture]_VRSLShadowMask3("GI Shadow Mask 3", 2D) = "white" {}
		[NoScaleOffset][SingleLineTexture]_VRSLShadowMask2("GI Shadow Mask 2", 2D) = "white" {}
		[NoScaleOffset][SingleLineTexture]_VRSL_LightTexture("Custom GI Light Texture", 2D) = "white" {}
		[NoScaleOffset][SingleLineTexture]_VRSLShadowMask1("GI Shadow Mask 1", 2D) = "white" {}
		[KeywordEnum(REALISTIC,TOON,TINT)] _VRSLGIDiffuseType("VRSLGI Diffuse Type", Float) = 0
		[KeywordEnum(GGX,BECKMAN,PHONG)] _VRSLGISpecularType("VRSLGI Specular Type", Float) = 0
		[Toggle(_VRSLGIGLOBALLIGHTTEXTURE_ON)] _VRSLGIGlobalLightTexture("VRSLGI Use Global Light Texture", Float) = 1
		_GISpecularStrength("GI Specular Strength", Range( 0.01 , 3)) = 0.01
		[Toggle(_VRSLGIENABLESPECULAR_ON)] _VRSLGIEnableSpecular("VRSLGI Enable Specular", Float) = 0
		[KeywordEnum(UV0,UV1,UV2,UV3,UV4)] _VRSLGIShadowMaskUV("VRSLGIShadowMaskUV", Float) = 0
		[Toggle(_VRSLGISHADOWMASK3_ON)] _VRSLGIShadowMask3("VRSLGI Enable ShadowMask 3", Float) = 0
		[Toggle(_VRSLGISHADOWMASK1_ON)] _VRSLGIShadowMask1("VRSLGI Enable ShadowMask 1", Float) = 0
		[Toggle(_VRSLGISHADOWMASK2_ON)] _VRSLGIShadowMask2("VRSLGI Enable ShadowMask 2", Float) = 0
		_VRSLGIStrength("GI Overall Strength", Float) = 1
		_UseVRSLShadowMask2RStrength("GI ShadowMask 2 R Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask2GStrength("GI ShadowMask 2 G Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask1RStrength("GI ShadowMask 1 R Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask2AStrength("GI ShadowMask 2 A Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask3RStrength("GI ShadowMask 3 R Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask1AStrength("GI ShadowMask 1 A Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask1GStrength("GI ShadowMask 1 G Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask3AStrength("GI ShadowMask 3 A Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask3BStrength("GI ShadowMask 3 B Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask1BStrength("GI ShadowMask 1 B Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask2BStrength("GI ShadowMask 2 B Strength", Range( 0 , 1)) = 0
		_UseVRSLShadowMask3GStrength("GI ShadowMask 3 G Strength", Range( 0 , 1)) = 0
		[KeywordEnum(R,RG,RGB,RGBA)] _VRLSGIShadowMaskChannel("VRLSGI Set Shadow Mask Channel", Float) = 0
		[Toggle(_VRSLGIENABLEDIRECTIONALSPOTLIGHTS_ON)] _VRSLGIEnableDirectionalSpotLights("VRSLGI Enable Directional Spot Lights", Float) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0
		_AlbedoColor("Albedo Color", Color) = (1,1,1,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "DataSetup.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VRSLGIDIFFUSETYPE_REALISTIC _VRSLGIDIFFUSETYPE_TOON _VRSLGIDIFFUSETYPE_TINT
			#pragma shader_feature_local _VRSLGISHADOWMASKUV_UV0 _VRSLGISHADOWMASKUV_UV1 _VRSLGISHADOWMASKUV_UV2 _VRSLGISHADOWMASKUV_UV3 _VRSLGISHADOWMASKUV_UV4
			#pragma shader_feature_local _VRSLGIGLOBALLIGHTTEXTURE_ON
			#pragma shader_feature_local _VRSLGISPECULARTYPE_GGX _VRSLGISPECULARTYPE_BECKMAN _VRSLGISPECULARTYPE_PHONG
			#pragma shader_feature_local _VRLSGISHADOWMASKCHANNEL_R _VRLSGISHADOWMASKCHANNEL_RG _VRLSGISHADOWMASKCHANNEL_RGB _VRLSGISHADOWMASKCHANNEL_RGBA
			#pragma shader_feature_local _VRSLGIENABLEDIRECTIONALSPOTLIGHTS_ON
			#pragma shader_feature_local _VRSLGIENABLESPECULAR_ON
			#pragma shader_feature_local _VRSLGISHADOWMASK1_ON
			#pragma shader_feature_local _VRSLGISHADOWMASK2_ON
			#pragma shader_feature_local _VRSLGISHADOWMASK3_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _VRSLShadowMask2;
			uniform sampler2D _VRSLShadowMask1;
			uniform sampler2D _VRSLShadowMask3;
			uniform float _Roughness;
			uniform float _Metallic;
			uniform float4 _AlbedoColor;
			uniform float _GISpecularStrength;
			float BeckmanSpec( float3 N, float3 V, float3 L, float roughness )
			{
				        float3 H = normalize(V+L);
				        float NdotH = saturate(dot(N,H));
				        float roughnessSqr = roughness*roughness * (roughness * 0.5);
				        float NdotHSqr = NdotH*NdotH;
				        return max(0.000001,(1.0 / (3.1415926535*roughnessSqr*NdotHSqr*NdotHSqr))
				        * exp((NdotHSqr-1)/(roughnessSqr*NdotHSqr)));
			}
			
			float PhongSpec( float3 N, float3 V, float3 L, float roughness )
			{
				    float speculargloss = (1/(roughness * roughness)) * 20;
				    float specularpower = lerp(15, 2, roughness);
				    float3 H = normalize(V+L);
				    float NdotH = saturate(dot(N,H));
				    float Distribution = pow(NdotH,speculargloss) * specularpower;
				    Distribution *= (2+specularpower) / (2*3.1415926535);
				    return Distribution;
			}
			
			float GGXSpec( float3 N, float3 V, float3 L, float roughness )
			{
				         float alpha = roughness*roughness;
				        float3 H = normalize(V+L);
				        float dotNL = saturate(dot(N,L));
				        //float dotNV = saturate(dot(N,V));
				        float dotNH = saturate(dot(N,H));
				        float dotLH = saturate(dot(L,H));
				        float D, vis;
				        // D
				        float alphaSqr = alpha*alpha;
				        float pi = 3.14159f;
				        float denom = dotNH * dotNH *(alphaSqr-1.0) + 1.0f;
				        D = alphaSqr/(pi * denom * denom);
				        // F
				        //float dotLH5 = pow(1.0f-dotLH,5);
				        //F = F0 + (1.0-F0)*(dotLH5);
				        // V
				        float k = alpha/2.0f;
				        float k2 = k*k;
				        float invK2 = 1.0f-k2;
				        vis = rcp(dotLH*dotLH*invK2 + k2);
				        //vis = G1V(dotLH,k)*G1V(dotLH,k);
				        //vis = G1V(dotNL,k)*G1V(dotNV,k);
				        float specular = dotNL * D * vis;
				        return specular;
			}
			
			void SetShadowMaskChannel( int maskChannel, inout float s1, inout float s1Strength, float4 mask, float4 strength )
			{
				         s1Strength = 0.0;
				        #if _VRLSGISHADOWMASKCHANNEL_RG
				            [branch]
				            switch(maskChannel)
				            {
				                case 0:
				                    s1 = mask.r;
				                    s1Strength = strength.r;
				                    break;
				                case 1:
				                    s1 = mask.g;
				                    s1Strength = strength.g;
				                    break;
				                default:
				                    break;
				            }
				        #elif _VRLSGISHADOWMASKCHANNEL_RGB
				            [branch]
				            switch(maskChannel)
				            {
				                case 0:
				                    s1 = mask.r;
				                    s1Strength = strength.r;
				                    break;
				                case 1:
				                    s1 = mask.g;
				                    s1Strength = strength.g;
				                    break;
				                case 2:
				                    s1 = mask.b;
				                    s1Strength = strength.b;
				                    break;
				                default:
				                    break;
				            }
				        #elif _VRLSGISHADOWMASKCHANNEL_RGBA
				            s1 = mask[maskChannel];
				            s1Strength = strength[maskChannel];
				        #else
				        if(maskChannel == 0)
				        {
				            s1 = mask.r;
				            s1Strength =strength.r;
				        }
				        #endif
			}
			
			void DiffuseFunction( float3 worldPos, float3 worldNormal, float3 lightPos, out float3 lightDirection, out float falloff, out float atten, float rangeController, out float range )
			{
				            range = distance(worldPos, lightPos.xyz);
				            lightDirection = normalize(lightPos.xyz - worldPos);
				            #if _VRSLGIDIFFUSETYPE_TOON
				                atten = saturate(dot(lightDirection, worldNormal) );
				                atten = lerp(0.0025, 1.0, atten);
				                atten = smoothstep(0,0.01,atten);
				            #elif _VRSLGIDIFFUSETYPE_TINT
				                atten = 1.0f;
				            #else
				                atten = saturate(dot(lightDirection, worldNormal) );
				            #endif
				            range*= rangeController;
				            falloff = 1.0 / (range * range);
			}
			
			float CalculateShadowMask( float4 mask1Strength, float4 mask2Strength, float4 mask3Strength, int maskChannel, int maskSelection, float4 shadowMask1, float4 shadowMask2, float4 shadowMask3 )
			{
				        float s1 =  1.0f;
				        float s1Strength = 0.0f;
				        [branch]
				        switch(maskSelection)
				        {
				            default:
				                break;
				            #if defined(_VRSLGISHADOWMASK1_ON)
				            case 1:
				                SetShadowMaskChannel(maskChannel,s1,s1Strength,shadowMask1,mask1Strength);
				                break;
				            #endif
				            #if defined(_VRSLGISHADOWMASK2_ON)
				            case 2:
				                SetShadowMaskChannel(maskChannel,s1,s1Strength,shadowMask2,mask2Strength);
				                break;
				            #endif
				            #if defined(_VRSLGISHADOWMASK3_ON)
				            case 3:
				                SetShadowMaskChannel(maskChannel,s1,s1Strength,shadowMask3,mask3Strength);
				                break;  
				            #endif              
				        }
				        return lerp(1,s1,s1Strength);
			}
			
			int GetLightCount23_g13(  )
			{
				        #ifdef _VRSLGIGLOBALLIGHTTEXTURE_ON
				            int lightCount = _Udon_VRSL_GI_LightTexture.Load( int3(0, 2, 0) );
				        #else
				            int lightCount = _VRSL_LightTexture.Load( int3(0, 2, 0) );
				        #endif
				        return lightCount;
			}
			
			float3 GetEyeVector59_g13( float3 n )
			{
				    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
				        return normalize(n);
				    #else
				        return n; // will normalize per-pixel instead
				    #endif
			}
			
			float3 GetGI24_g13( int lightCount, float3 worldNormal, float3 viewDirection, float3 worldPos, float occlusion, float roughness, float metallic, float3 diffuseColor, float4 shadowmask1, float4 shadowmask2, float4 shadowmask3, float specularMultiply, float overallStrength )
			{
				float shadowmask = 1.0;
				 float3 finalOut = 0.0;      
				 [loop]
				        for (int x = 0; x < lightCount; x++)
				        {
				            
				            //Begin Diffuse Stuff
				            #if _VRSLGIENABLEDIRECTIONALSPOTLIGHTS_ON
				                #ifdef _VRSLGIGLOBALLIGHTTEXTURE_ON
				                    float4 rawLightColor = _Udon_VRSL_GI_LightTexture.Load( int3(x, 0, 0) );
				                    float4 lightPos = _Udon_VRSL_GI_LightTexture.Load( int3(x, 1, 0) );
				                #else
				                    float4 rawLightColor = _VRSL_LightTexture.Load( int3(x, 0, 0) );
				                    float4 lightPos = _VRSL_LightTexture.Load( int3(x, 1, 0) );
				                #endif
				                float isSpotlight = lightPos.w;
				            #else
				                #ifdef _VRSLGIGLOBALLIGHTTEXTURE_ON
				                    float4 rawLightColor = _Udon_VRSL_GI_LightTexture.Load( int3(x, 0, 0) );   
				                    float4 lightPos = _Udon_VRSL_GI_LightTexture.Load( int3(x, 1, 0) );
				                #else
				                    float4 rawLightColor = _VRSL_LightTexture.Load( int3(x, 0, 0) );   
				                    float4 lightPos = _VRSL_LightTexture.Load( int3(x, 1, 0) );
				                #endif
				            #endif
				            float3 lightColor = rawLightColor.rgb * (0.5 * rawLightColor.a);
				            float specular = 1.0;
				            
				            float falloff = 1.0;
				            float atten = 1.0;
				            float range = 1.0;
				            float3 lightDirection = float3(1.0,1.0,1.0);
				            
				            //Begin Diffuse Stuff
				            DiffuseFunction(worldPos, worldNormal, lightPos.xyz, lightDirection, falloff, atten, rawLightColor.a, range);
				            //End Diffuse Stuff
				            //Begin Specular Stuff
				            #if _VRSLGIENABLESPECULAR_ON
				                #ifdef _VRSLGISPECULARTYPE_GGX
				                    specular = GGXSpec(worldNormal, viewDirection, lightDirection, roughness);
				                #elif _VRSLGISPECULARTYPE_BECKMAN
				                    specular = BeckmanSpec(worldNormal, viewDirection, lightDirection, roughness);
				                #elif _VRSLGISPECULARTYPE_PHONG
				                    specular = PhongSpec(worldNormal, viewDirection, lightDirection, roughness);
				                #endif
				                specular = lerp(1.0, specular, metallic) * specularMultiply;
				                specular = specular * specular;
				                specular *= (1/(range*0.5));
				                //specular *= CalcLuminance(rawLightColor.rgb);
				            #endif
				            //End Specular Stuff
				            //Begin ShadowMask Stuff
				            #ifndef VRSL_GI_PROJECTOR
				                #if defined(_VRSLGISHADOWMASK1_ON) || defined(_VRSLGISHADOWMASK2_ON) || defined(_VRSLGISHADOWMASK3_ON)
				                    lightPos.w = (frac(lightPos.w * 0.1)) * 10;
				                    int maskSelection = (int) floor(lightPos.w);
				                    int maskChannel = (int) floor(frac(lightPos.w) * 10);
				                    shadowmask = saturate(CalculateShadowMask(float4(_UseVRSLShadowMask1RStrength,
				                    _UseVRSLShadowMask1GStrength,_UseVRSLShadowMask1BStrength, _UseVRSLShadowMask1AStrength),
				                    float4(_UseVRSLShadowMask2RStrength, _UseVRSLShadowMask2GStrength, _UseVRSLShadowMask2BStrength, _UseVRSLShadowMask2AStrength),
				                    float4(_UseVRSLShadowMask3RStrength, _UseVRSLShadowMask3GStrength, _UseVRSLShadowMask3BStrength, _UseVRSLShadowMask3AStrength)
				                    , maskChannel, maskSelection,
				                    shadowmask1, shadowmask2, shadowmask3));
				                #endif             
				                
				            #endif
				            //End Shadowmask Stuff
				            //Combine
				            lightColor = lerp(float3(0,0,0), lightColor * overallStrength, shadowmask);
				            
				            #if _VRSLGIENABLEDIRECTIONALSPOTLIGHTS_ON
				                
				               if(isSpotlight > 180.0f)
				               {
				                    #ifdef _VRSLGIGLOBALLIGHTTEXTURE_ON
				                        float4 rawLightDirection = _Udon_VRSL_GI_LightTexture.Load( int3(x, 3, 0) );    
				                    #else
				                        float4 rawLightDirection = _VRSL_LightTexture.Load( int3(x, 3, 0) );    
				                    #endif
				                    float3 spotlightDir = rawLightDirection.xyz;
				                    float angle = (floor(rawLightDirection.w - 1)) / 255;
				                    float blend = frac(rawLightDirection.w);
				                    angle = angle * 180.0;
				                    float theta = dot(lightDirection, normalize(-spotlightDir));
				                    float outerCone = cos(radians(angle));
				                    float spotlight = clamp(theta - outerCone,0.0,1.0);
				                    atten = lerp(atten, atten*spotlight, blend);
				                    specular = lerp(specular, specular*spotlight, blend);
				               }
				            #endif
				            finalOut += falloff * lightColor  * atten * diffuseColor * specular;
				            //finalOut += (spotlight);
				            
				        }
				return finalOut * occlusion;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord2.zw = v.ase_texcoord1.xy;
				o.ase_texcoord3.xy = v.ase_texcoord2.xy;
				o.ase_texcoord3.zw = v.ase_texcoord3.xy;
				o.ase_texcoord4.xy = v.ase_texcoord4.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord4.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				int localGetLightCount23_g13 = GetLightCount23_g13();
				int lightCount24_g13 = localGetLightCount23_g13;
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float3 worldNormal24_g13 = normalizedWorldNormal;
				float3 temp_output_1_0_g13 = WorldPosition;
				float3 n59_g13 = ( temp_output_1_0_g13 - _WorldSpaceCameraPos );
				float3 localGetEyeVector59_g13 = GetEyeVector59_g13( n59_g13 );
				float3 normalizeResult10_g13 = normalize( ( localGetEyeVector59_g13 * float3( -1,-1,-1 ) ) );
				float3 viewDirection24_g13 = normalizeResult10_g13;
				float3 worldPos24_g13 = temp_output_1_0_g13;
				float localGetData32_g13 = ( 0.0 );
				GetData(  );
				float occlusion24_g13 = ( localGetData32_g13 + 1.0 );
				float roughness24_g13 = _Roughness;
				float metallic24_g13 = _Metallic;
				float3 diffuseColor24_g13 = _AlbedoColor.rgb;
				float4 color89_g13 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				#if defined(_VRSLGISHADOWMASKUV_UV0)
				float2 staticSwitch72_g13 = i.ase_texcoord2.xy;
				#elif defined(_VRSLGISHADOWMASKUV_UV1)
				float2 staticSwitch72_g13 = i.ase_texcoord2.zw;
				#elif defined(_VRSLGISHADOWMASKUV_UV2)
				float2 staticSwitch72_g13 = i.ase_texcoord3.xy;
				#elif defined(_VRSLGISHADOWMASKUV_UV3)
				float2 staticSwitch72_g13 = i.ase_texcoord3.zw;
				#elif defined(_VRSLGISHADOWMASKUV_UV4)
				float2 staticSwitch72_g13 = i.ase_texcoord4.xy;
				#else
				float2 staticSwitch72_g13 = i.ase_texcoord2.xy;
				#endif
				#ifdef _VRSLGISHADOWMASK1_ON
				float4 staticSwitch87_g13 = tex2D( _VRSLShadowMask1, staticSwitch72_g13 );
				#else
				float4 staticSwitch87_g13 = color89_g13;
				#endif
				float4 shadowmask124_g13 = staticSwitch87_g13;
				#ifdef _VRSLGISHADOWMASK2_ON
				float4 staticSwitch90_g13 = tex2D( _VRSLShadowMask2, staticSwitch72_g13 );
				#else
				float4 staticSwitch90_g13 = color89_g13;
				#endif
				float4 shadowmask224_g13 = staticSwitch90_g13;
				#ifdef _VRSLGISHADOWMASK3_ON
				float4 staticSwitch92_g13 = tex2D( _VRSLShadowMask3, staticSwitch72_g13 );
				#else
				float4 staticSwitch92_g13 = color89_g13;
				#endif
				float4 shadowmask324_g13 = staticSwitch92_g13;
				float specularMultiply24_g13 = _GISpecularStrength;
				float overallStrength24_g13 = _VRSLGIStrength;
				float3 localGetGI24_g13 = GetGI24_g13( lightCount24_g13 , worldNormal24_g13 , viewDirection24_g13 , worldPos24_g13 , occlusion24_g13 , roughness24_g13 , metallic24_g13 , diffuseColor24_g13 , shadowmask124_g13 , shadowmask224_g13 , shadowmask324_g13 , specularMultiply24_g13 , overallStrength24_g13 );
				
				
				finalColor = float4( localGetGI24_g13 , 0.0 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback "Standard"
}
/*ASEBEGIN
Version=18935
537.8571;-1080;1920;1019;1332.533;618.2734;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;107;-861.34,-85.00671;Inherit;False;Property;_Metallic;Metallic;29;0;Create;True;0;0;0;False;0;False;0;0.198;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-867.34,-155.0067;Inherit;False;Property;_Roughness;Roughness;30;0;Create;True;0;0;0;False;0;False;0;0.657;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;95;-863.34,2.993286;Inherit;False;Property;_AlbedoColor;Albedo Color;31;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;161;-464.2018,-103.2101;Inherit;False;VRSL_GI_Function_GetGI;0;;13;8094ff91e0e2ed145978a14e00a9083d;0;6;119;FLOAT;0;False;120;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;5;FLOAT3;1,1,1;False;8;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;-121.5283,-105.9696;Float;False;True;-1;2;ASEMaterialInspector;100;1;VRSL_GI_PureGI;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;Standard;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;161;119;108;0
WireConnection;161;120;107;0
WireConnection;161;5;95;0
WireConnection;2;0;161;0
ASEEND*/
//CHKSM=BEE05E72780372EC31E579795C25E49DA40C0F98