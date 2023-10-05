// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

using System;
using UnityEngine;
using UnityEditor;
using System.IO;

// namespace VRSL
// {
    internal class VRSL_GI_StandardShaderGUI : ShaderGUI
    {
            public static string GetVersion()
        {
            string path = Application.dataPath;
            // path = path.Replace("Assets","");
            // path += "Packages"  + "\\" + "com.acchosen.vr-stage-lighting" + "\\";
            //path += "Runtime" + "\\"  + "VERSION.txt";
            path += "\\" +  "VRSL Addons" + "\\" + "VRSL-GI Shader Package" + "\\" + "VERSION.txt";

            StreamReader reader = new StreamReader(path); 
            string versionNum = reader.ReadToEnd();
            string ver = "VR Stage Lighting ver:" + " <b><color=#b33cff>" + versionNum + "</color></b>";
            return ver;
    }
        public static Texture logo = Resources.Load("VRStageLighting-Logo") as Texture;
        private enum WorkflowMode
        {
            Specular,
            Metallic,
            Dielectric
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
        }

        public enum SmoothnessMapChannel
        {
            SpecularMetallicAlpha,
            AlbedoAlpha,
        }

        private static class Styles
        {
            public static GUIContent uvSetLabel = EditorGUIUtility.TrTextContent("UV Set");

            public static GUIContent albedoText = EditorGUIUtility.TrTextContent("Albedo", "Albedo (RGB) and Transparency (A)");
            public static GUIContent alphaCutoffText = EditorGUIUtility.TrTextContent("Alpha Cutoff", "Threshold for alpha cutoff");
            public static GUIContent specularMapText = EditorGUIUtility.TrTextContent("Specular", "Specular (RGB) and Smoothness (A)");
            public static GUIContent metallicMapText = EditorGUIUtility.TrTextContent("Metallic", "Metallic (R) and Smoothness (A)");
            public static GUIContent smoothnessText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness value");
            public static GUIContent smoothnessScaleText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness scale factor");
            public static GUIContent smoothnessMapChannelText = EditorGUIUtility.TrTextContent("Source", "Smoothness texture and channel");
            public static GUIContent highlightsText = EditorGUIUtility.TrTextContent("Specular Highlights", "Specular Highlights");
            public static GUIContent reflectionsText = EditorGUIUtility.TrTextContent("Reflections", "Glossy Reflections");
            public static GUIContent normalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");
            public static GUIContent heightMapText = EditorGUIUtility.TrTextContent("Height Map", "Height Map (G)");
            public static GUIContent occlusionText = EditorGUIUtility.TrTextContent("Occlusion", "Occlusion (G)");
            public static GUIContent emissionText = EditorGUIUtility.TrTextContent("Color", "Emission (RGB)");
            public static GUIContent detailMaskText = EditorGUIUtility.TrTextContent("Detail Mask", "Mask for Secondary Maps (A)");
            public static GUIContent detailAlbedoText = EditorGUIUtility.TrTextContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
            public static GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");

            public static string primaryMapsText = "Main Maps";
            public static string secondaryMapsText = "Secondary Maps";
            public static string forwardText = "Forward Rendering Options";
            public static string renderingMode = "Rendering Mode";
            public static string advancedText = "Advanced Options";
            public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
        }

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty specularMap = null;
        MaterialProperty specularColor = null;
        MaterialProperty metallicMap = null;
        MaterialProperty metallic = null;
        MaterialProperty smoothness = null;
        MaterialProperty smoothnessScale = null;
        MaterialProperty smoothnessMapChannel = null;
        MaterialProperty _VRSLGlossiness = null;
        MaterialProperty _VRSLSpecularStrength = null;
        MaterialProperty _ProjectorColor = null;
        MaterialProperty _VRSLProjectorStrength = null;
        MaterialProperty highlights = null;
        MaterialProperty reflections = null;
        MaterialProperty bumpScale = null;
        MaterialProperty bumpMap = null;
        MaterialProperty occlusionStrength = null;
        MaterialProperty occlusionMap = null;
        MaterialProperty heigtMapScale = null;
        MaterialProperty heightMap = null;
        MaterialProperty emissionColorForRendering = null;
        MaterialProperty emissionMap = null;
        MaterialProperty detailMask = null;
        MaterialProperty detailAlbedoMap = null;
        MaterialProperty detailNormalMapScale = null;
        MaterialProperty detailNormalMap = null;
        MaterialProperty uvSetSecondary = null;
        MaterialProperty _ZTest = null;
        MaterialProperty _ZWrite = null;
        MaterialProperty _StencilComp = null;
        MaterialProperty _StencilOp = null;
        MaterialProperty _StencilFail = null;
        MaterialProperty _StencilZFail = null;
        //MaterialProperty useVRSLGI = null;
        //MaterialProperty useVRSLGISpecular = null;


        	//VRSL Stuff
	MaterialProperty vrslToggle = null;
	MaterialProperty thirteenChannelMode = null;
	MaterialProperty dmxChannel = null;
	MaterialProperty dmxEmissionMap = null;
	MaterialProperty nineUniverseMode = null;
	MaterialProperty invertPan = null;
	MaterialProperty invertTilt = null;
	MaterialProperty panToggle = null;
	MaterialProperty tiltToggle = null;
	MaterialProperty strobeToggle = null;
	MaterialProperty verticalToggle = null;
	MaterialProperty compatibilityToggle = null;
	MaterialProperty baseRotationY = null;
	MaterialProperty baseRotationX = null;
	MaterialProperty finalIntensity = null;
	MaterialProperty globalIntensity = null;
    MaterialProperty _GlobalIntensityBlend = null;
	MaterialProperty universalIntensity = null;
	MaterialProperty rotationOrigin = null;
	MaterialProperty maxMinPanAngle = null;
	MaterialProperty maxMinTiltAngle = null;
	MaterialProperty fixtureMaxIntensity = null;
    MaterialProperty _RenderTextureMultiplier = null;
	MaterialProperty dmxEmissionMapMix = null;
	MaterialProperty dmxEmissionColor = null;

    	MaterialProperty legacyVRSLTextures = null;

	MaterialProperty _OSCGridRenderTextureRAW = null;
	MaterialProperty _OSCGridRenderTexture = null;
	MaterialProperty _OSCGridStrobeTimer = null;
    MaterialProperty _VRSLGIStrength = null;
    MaterialProperty _VRSLDiffuseMix = null;
    MaterialProperty _VRSLSpecularMultiplier = null;
    MaterialProperty _VRSLMetallicGlossMap = null;
    MaterialProperty _UseVRSLMetallicGlossMap = null;
    MaterialProperty _VRSLMetallicMapStrength = null;
    MaterialProperty _VRSLGlossMapStrength = null;
    MaterialProperty _VRSLSmoothnessChannel = null;
    MaterialProperty _VRSLMetallicChannel = null;
    MaterialProperty _VRSLSpecularShine = null;
    MaterialProperty _VRSLShadowMask1 = null;
    MaterialProperty _VRSLShadowMask2 = null;
    MaterialProperty _VRSLShadowMask3 = null;
    MaterialProperty _UseVRSLShadowMask1 = null;
    MaterialProperty _UseVRSLShadowMask2 = null;
    MaterialProperty _UseVRSLShadowMask3 = null;
    MaterialProperty _VRSL_LightTexture = null;
    MaterialProperty _VRSLGIQuadLightingSystem = null;
    MaterialProperty _VRSLGIVertexFalloff = null;
    MaterialProperty _VRSLGIVertexAttenuation = null;
    MaterialProperty _VRSLInvertSmoothnessMap = null;
    MaterialProperty _VRSLInvertMetallicMap = null;
    MaterialProperty _UseGlobalVRSLLightTexture = null;

    MaterialProperty _UseVRSLShadowMask1RStrength = null;
    MaterialProperty _UseVRSLShadowMask1GStrength = null;
    MaterialProperty _UseVRSLShadowMask1BStrength = null;
    MaterialProperty _UseVRSLShadowMask1AStrength = null;

    MaterialProperty _UseVRSLShadowMask2RStrength = null;
    MaterialProperty _UseVRSLShadowMask2GStrength = null;
    MaterialProperty _UseVRSLShadowMask2BStrength = null;
    MaterialProperty _UseVRSLShadowMask2AStrength = null;

    MaterialProperty _UseVRSLShadowMask3RStrength = null;
    MaterialProperty _UseVRSLShadowMask3GStrength = null;
    MaterialProperty _UseVRSLShadowMask3BStrength = null;
    MaterialProperty _UseVRSLShadowMask3AStrength = null;
    MaterialProperty _ShadowMaskActiveChannels = null;
    MaterialProperty _VRSLSpecularFunction = null;

    MaterialProperty _VRSLGISpecularClamp = null;
    MaterialProperty _VRSLGIDiffuseClamp = null;

    MaterialProperty _VRSLShadowMaskUVSet = null;
    MaterialProperty _VRSLGIDiffuseMode = null;
    MaterialProperty _IncludeDirectionAndSpotAngle = null;


    MaterialProperty _AudioLinkToggle = null;
    MaterialProperty _EnableColorChord = null;
    MaterialProperty _Band = null;
    MaterialProperty _Delay = null;
    MaterialProperty _BandMultiplier = null;
    MaterialProperty _EnableColorTextureSample = null;
    MaterialProperty _SamplingTexture = null;
    MaterialProperty _TextureColorSampleX = null;
    MaterialProperty _TextureColorSampleY = null;
    MaterialProperty _EnableThemeColorSampling = null;
    MaterialProperty _ThemeColorTarget = null;
    MaterialProperty _NumBands = null;
    MaterialProperty _AudioLinkEmissionMap = null;
    MaterialProperty _EnableAudioLink = null;
    MaterialProperty _Emission = null;
	//End VRSL Stuff


    MaterialProperty areaLitToggle = null;
	MaterialProperty areaLitStrength = null;
	MaterialProperty areaLitRoughnessMult = null;
	MaterialProperty lightMesh = null;
	MaterialProperty lightTex0 = null;
	MaterialProperty lightTex1 = null;
	MaterialProperty lightTex2 = null;
	MaterialProperty lightTex3 = null;
	MaterialProperty opaqueLights = null;
	//MaterialProperty mirrorToggle = null;
	MaterialProperty occlusionUVSet = null;
	MaterialProperty areaLitOcclusion = null;

    MaterialProperty ltcgi = null;
	MaterialProperty ltcgi_diffuse_off = null;
	MaterialProperty ltcgi_spec_off = null;
	MaterialProperty ltcgiStrength = null;



        MaterialEditor m_MaterialEditor;
        WorkflowMode m_WorkflowMode = WorkflowMode.Specular;

        bool m_FirstTimeApply = true;

        public void FindProperties(MaterialProperty[] props, Material material)
        {
            blendMode = FindProperty("_Mode", props);
            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_Color", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            specularMap = FindProperty("_SpecGlossMap", props, false);
            specularColor = FindProperty("_SpecColor", props, false);
            metallicMap = FindProperty("_MetallicGlossMap", props, false);
            metallic = FindProperty("_Metallic", props, false);
            if (specularMap != null && specularColor != null)
                m_WorkflowMode = WorkflowMode.Specular;
            else if (metallicMap != null && metallic != null)
                m_WorkflowMode = WorkflowMode.Metallic;
            else
                m_WorkflowMode = WorkflowMode.Dielectric;
            smoothness = FindProperty("_Glossiness", props);
            smoothnessScale = FindProperty("_GlossMapScale", props, false);
            smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", props, false);
            highlights = FindProperty("_SpecularHighlights", props, false);
            reflections = FindProperty("_GlossyReflections", props, false);
            bumpScale = FindProperty("_BumpScale", props);
            bumpMap = FindProperty("_BumpMap", props);
            heigtMapScale = FindProperty("_Parallax", props);
            heightMap = FindProperty("_ParallaxMap", props);
            occlusionStrength = FindProperty("_OcclusionStrength", props);
            occlusionMap = FindProperty("_OcclusionMap", props);
            emissionColorForRendering = FindProperty("_EmissionColor", props);
            emissionMap = FindProperty("_EmissionMap", props);
            detailMask = FindProperty("_DetailMask", props);
            detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
            detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
            detailNormalMap = FindProperty("_DetailNormalMap", props);
            uvSetSecondary = FindProperty("_UVSec", props);
            _VRSLGlossiness = FindProperty("_VRSLGlossiness", props);
            _VRSLSpecularStrength = FindProperty("_VRSLSpecularStrength", props);
            _VRSLGIStrength = FindProperty("_VRSLGIStrength", props);
            _VRSLDiffuseMix = FindProperty("_VRSLDiffuseMix", props); 
            _VRSLSpecularMultiplier = FindProperty("_VRSLSpecularMultiplier", props);  
            _VRSLSpecularShine = FindProperty("_VRSLSpecularShine", props);
            _VRSLSpecularFunction = FindProperty("_VRSLSpecularFunction", props); 
            _VRSLGIDiffuseMode = FindProperty("_VRSLGIDiffuseMode", props);
            _VRSL_LightTexture = FindProperty("_VRSL_LightTexture", props);
            _UseGlobalVRSLLightTexture = FindProperty("_UseGlobalVRSLLightTexture", props);
            _VRSLGISpecularClamp = FindProperty("_VRSLGISpecularClamp", props);
            _VRSLGIDiffuseClamp = FindProperty("_VRSLGIDiffuseClamp", props);
            _IncludeDirectionAndSpotAngle = FindProperty("_IncludeDirectionAndSpotAngle", props);
            if(material.shader.name.Contains("Project"))
            {
                _ProjectorColor = FindProperty("_ProjectorColor", props);
                _VRSLProjectorStrength = FindProperty("_VRSLProjectorStrength", props);
                _VRSLGIQuadLightingSystem = FindProperty("_VRSLGIQuadLightingSystem", props);
                _VRSLGIVertexFalloff = FindProperty("_VRSLGIVertexFalloff", props);
                _VRSLGIVertexAttenuation = FindProperty("_VRSLGIVertexAttenuation", props);
                _ZTest = FindProperty("_ZTest", props);
                _ZWrite = FindProperty("_ZWrite", props);
                _StencilComp = FindProperty("_StencilComp", props);
                _StencilOp = FindProperty("_StencilOp", props);
                _StencilFail = FindProperty("_StencilFail", props);
                _StencilZFail = FindProperty("_StencilZFail", props);
            }
            else
            {
                _VRSLShadowMaskUVSet = FindProperty("_VRSLShadowMaskUVSet", props);
                _UseVRSLShadowMask1RStrength = FindProperty("_UseVRSLShadowMask1RStrength", props);
                _UseVRSLShadowMask1GStrength = FindProperty("_UseVRSLShadowMask1GStrength", props);
                _UseVRSLShadowMask1BStrength = FindProperty("_UseVRSLShadowMask1BStrength", props);
                _UseVRSLShadowMask1AStrength = FindProperty("_UseVRSLShadowMask1AStrength", props);

                _UseVRSLShadowMask2RStrength = FindProperty("_UseVRSLShadowMask2RStrength", props);
                _UseVRSLShadowMask2GStrength = FindProperty("_UseVRSLShadowMask2GStrength", props);
                _UseVRSLShadowMask2BStrength = FindProperty("_UseVRSLShadowMask2BStrength", props);
                _UseVRSLShadowMask2AStrength = FindProperty("_UseVRSLShadowMask2AStrength", props);

                _UseVRSLShadowMask3RStrength = FindProperty("_UseVRSLShadowMask3RStrength", props);
                _UseVRSLShadowMask3GStrength = FindProperty("_UseVRSLShadowMask3GStrength", props);
                _UseVRSLShadowMask3BStrength = FindProperty("_UseVRSLShadowMask3BStrength", props);
                _UseVRSLShadowMask3AStrength = FindProperty("_UseVRSLShadowMask3AStrength", props);

                _ShadowMaskActiveChannels = FindProperty("_ShadowMaskActiveChannels", props);

                _VRSLInvertSmoothnessMap = FindProperty("_VRSLInvertSmoothnessMap", props);
                _VRSLInvertMetallicMap = FindProperty("_VRSLInvertMetallicMap", props);

                _UseVRSLShadowMask1 = FindProperty("_UseVRSLShadowMask1", props);
                _UseVRSLShadowMask2 = FindProperty("_UseVRSLShadowMask2", props);
                _UseVRSLShadowMask3 = FindProperty("_UseVRSLShadowMask3", props);
                _VRSLShadowMask3 = FindProperty("_VRSLShadowMask3", props);
                _VRSLShadowMask2 = FindProperty("_VRSLShadowMask2", props);
                _VRSLShadowMask1 = FindProperty("_VRSLShadowMask1", props);
                _VRSLMetallicChannel = FindProperty("_VRSLMetallicChannel", props); 
                _VRSLSmoothnessChannel = FindProperty("_VRSLSmoothnessChannel", props); 
                _VRSLGlossMapStrength = FindProperty("_VRSLGlossMapStrength", props);
                _VRSLMetallicMapStrength = FindProperty("_VRSLMetallicMapStrength", props); 
                _VRSLMetallicGlossMap = FindProperty("_VRSLMetallicGlossMap", props);
                _UseVRSLMetallicGlossMap = FindProperty("_UseVRSLMetallicGlossMap", props);
                vrslToggle = FindProperty("_VRSLToggle", props);
                thirteenChannelMode = FindProperty("_ThirteenChannelMode", props);
                dmxChannel = FindProperty("_DMXChannel", props);
                dmxEmissionMap = FindProperty("_DMXEmissionMap", props);
                dmxEmissionMapMix = FindProperty("_DMXEmissionMapMix", props);
                dmxEmissionColor = FindProperty("_EmissionDMX", props);
                legacyVRSLTextures = FindProperty("_UseLegacyDMXTextures", props);
                _OSCGridRenderTextureRAW = FindProperty("_OSCGridRenderTextureRAW", props);
                _OSCGridRenderTexture = FindProperty("_OSCGridRenderTexture", props);
                _OSCGridStrobeTimer = FindProperty("_OSCGridStrobeTimer", props);
                nineUniverseMode = FindProperty("_NineUniverseMode", props);
                invertPan = FindProperty("_PanInvert", props);
                invertTilt = FindProperty("_TiltInvert", props);
                panToggle = FindProperty("_EnablePanMovement", props);
                tiltToggle = FindProperty("_EnableTiltMovement", props);
                strobeToggle = FindProperty("_EnableStrobe", props);
                verticalToggle = FindProperty("_EnableVerticalMode", props);
                compatibilityToggle = FindProperty("_EnableCompatibilityMode", props);
                baseRotationY = FindProperty("_FixtureBaseRotationY", props);
                baseRotationX = FindProperty("_FixtureRotationX", props);
                finalIntensity = FindProperty("_FinalIntensity", props);
                globalIntensity = FindProperty("_GlobalIntensity", props);
                _GlobalIntensityBlend = FindProperty("_GlobalIntensityBlend", props);
                universalIntensity = FindProperty("_UniversalIntensity", props);
                rotationOrigin = FindProperty("_FixtureRotationOrigin", props);
                maxMinPanAngle = FindProperty("_MaxMinPanAngle", props);
                maxMinTiltAngle = FindProperty("_MaxMinTiltAngle", props);
                fixtureMaxIntensity = FindProperty("_FixtureMaxIntensity", props);
                _RenderTextureMultiplier = FindProperty("_RenderTextureMultiplier", props);

                _AudioLinkToggle = FindProperty("_AudioLinkToggle", props);
                _EnableColorChord = FindProperty("_EnableColorChord", props);
                _Band = FindProperty("_Band", props);
                _BandMultiplier = FindProperty("_BandMultiplier", props);
                _Delay = FindProperty("_Delay", props);
                _EnableColorTextureSample = FindProperty("_EnableColorTextureSample", props);
                _SamplingTexture = FindProperty("_SamplingTexture", props);
                _TextureColorSampleX = FindProperty("_TextureColorSampleX", props);
                _TextureColorSampleY = FindProperty("_TextureColorSampleY", props);
                _EnableThemeColorSampling = FindProperty("_EnableThemeColorSampling", props);
                _ThemeColorTarget = FindProperty("_ThemeColorTarget", props);
                _NumBands = FindProperty("_NumBands", props);
                _AudioLinkEmissionMap = FindProperty("_AudioLinkEmissionMap", props);
                _EnableAudioLink = FindProperty("_EnableAudioLink", props);
                _Emission = FindProperty("_Emission", props);
            }
            //	mirrorToggle = FindProperty("_MirrorToggle", props);
                areaLitToggle = FindProperty("_AreaLitToggle", props);
                lightMesh = FindProperty("_LightMesh", props);
                lightTex0 = FindProperty("_LightTex0", props);
                lightTex1 = FindProperty("_LightTex1", props);
                lightTex2 = FindProperty("_LightTex2", props);
                lightTex3 = FindProperty("_LightTex3", props);
                opaqueLights = FindProperty("_OpaqueLights", props);
                areaLitStrength = FindProperty("_AreaLitStrength", props);
		        areaLitRoughnessMult = FindProperty("_AreaLitRoughnessMult", props);
                occlusionUVSet = FindProperty("_OcclusionUVSet", props);
                areaLitOcclusion = FindProperty("_AreaLitOcclusion", props);

                ltcgi = FindProperty("_LTCGI", props);
                ltcgi_diffuse_off = FindProperty("_LTCGI_DIFFUSE_OFF", props);
                ltcgi_spec_off = FindProperty("_LTCGI_SPECULAR_OFF", props);
                ltcgiStrength = FindProperty("_LTCGIStrength", props);
		//End VRSL Stuff
        }

        public static void ToggleGroup(bool isToggled){
			EditorGUI.BeginDisabledGroup(isToggled);
		}
		public static void ToggleGroupEnd(){
			EditorGUI.EndDisabledGroup();
		}

        		public static void BoldLabel(string text){
			EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
		}
        		public static float GetInspectorWidth(){
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			return GUILayoutUtility.GetLastRect().width;
		}

		public static float GetPropertyWidth(){
			float lw = EditorGUIUtility.labelWidth;
			float iw = GetInspectorWidth();
			return iw - lw;
		}

		// Check if the name of the shader contains a specified string
		static bool CheckName(string name, Material mat){
			return mat.shader.name.Contains(name);
		}

		// Shorthand Scale Offset func with fixed spacing
		public static void TextureSO(MaterialEditor me, MaterialProperty prop){
			me.TextureScaleOffsetProperty(prop);
		}

		// Scale offset property with added scrolling x/y
		public static void TextureSOScroll(MaterialEditor me, MaterialProperty tex, MaterialProperty vec){
			me.TextureScaleOffsetProperty(tex);
			SpaceN2();
			Vector2Field(vec, "Scrolling");
		}

		public static void TextureSOScroll(MaterialEditor me, MaterialProperty tex, MaterialProperty vec, bool shouldDisplay){
			if (shouldDisplay){
				me.TextureScaleOffsetProperty(tex);
				SpaceN2();
				Vector2Field(vec, "Scrolling");
			}
		}

		// Shorthand Scale Offset func with fixed spacing
		public static void TextureSO(MaterialEditor me, MaterialProperty prop, bool shouldDisplay){
			if (shouldDisplay){
				me.TextureScaleOffsetProperty(prop);
			}
		}

		// Shorthand for displaying an error window
		public static void ErrorBox(string message){
			EditorUtility.DisplayDialog("Error", message, "Close");
		}

		public static void PropertyGroup(Action action){
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			Space1();
			action();
			Space1();
			EditorGUILayout.EndVertical();
			Space2();
		}

		public static void PropertyGroup(bool shouldDisplay, Action action){
			if (shouldDisplay){
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				Space2();
				action();
				Space2();
				EditorGUILayout.EndVertical();
				Space2();
			}
		}

		public static void PropertyGroupLayer(Action action){
			Color col = GUI.backgroundColor;
			GUI.backgroundColor = new Color(col.r * 0.3f, col.g * 0.3f, col.b * 0.3f);
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUI.backgroundColor = col;
			Space4();
			action();
			Space4();
			EditorGUILayout.EndVertical();
		}
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = materialEditor.target as Material;
            FindProperties(props, material); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            m_MaterialEditor = materialEditor;
            

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a standard shader.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (m_FirstTimeApply)
            {
                MaterialChanged(material, m_WorkflowMode, false);
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        bool FloatToBool(float input)
        {
            return input == 1.0f;
        }

        GUIStyle AlignRight()
        {
            GUIStyle x = new GUIStyle("label");
            x.alignment = TextAnchor.MiddleRight;
            x.fontSize = 10;
            return x;
        }
        GUIStyle AlignLeft()
        {
            GUIStyle x = new GUIStyle("label");
            x.alignment = TextAnchor.MiddleLeft;
            x.fontSize = 10;
            return x;
        }
        void SliderStrengthUnderLables()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Less Map Strength", AlignRight());
            EditorGUILayout.LabelField("", "More Map Strength", AlignRight(), GUILayout.MinWidth(100f));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }

    void GUILine(float height = 0f)
    {
        GUILine(Color.black, height);
    }

    void GUILine(Color color, float height = 0f)
    {
        Rect position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

        if (Event.current.type == EventType.Repaint)
        {
            Color orgColor = GUI.color;
            GUI.color = orgColor * color;
            LineStyle.Draw(position, false, false, false, false);
            GUI.color = orgColor;
        }
    }
    
    static public GUIStyle _LineStyle;
    static public GUIStyle LineStyle
    {
        get
        {
            if (_LineStyle == null)
            {
                _LineStyle = new GUIStyle();
                _LineStyle.normal.background = EditorGUIUtility.whiteTexture;
                _LineStyle.stretchWidth = true;
            }

            return _LineStyle;
        }
    }
    void ShurikenHeader(string title)
    {
        DrawShuriken(title, new Vector2(6f, -2f), 22);
    }

    void ShurikenHeaderCentered(string title)
    {
        DrawShurikenCenteredTitle(title, new Vector2(0f, -2f), 22);
    }

    private static Rect DrawShurikenPartingLineText(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);

        GUI.Box(rect, title, style);
        return rect;
    }

    void DrawShurikenPartingLineText(string title)
    {
        DrawShurikenPartingLineText(title, new Vector2(6f, -2f), 22);
    }
    void DrawLogo()
    {
        ///GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
        // GUILayout.FlexibleSpace();
        //GUI.DrawTexture(pos,logo,ScaleMode.ScaleToFit);
        //EditorGUI.DrawPreviewTexture(new Rect(0,0,400,150), logo);
        Vector2 contentOffset = new Vector2(0f, -2f);
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fixedHeight = 200;
        //style.fixedWidth = 300;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(300f, 190f, style);
        //GUILayout.Label(logo,style, GUILayout.MaxWidth(500), GUILayout.MaxHeight(200));
        GUI.Box(rect, logo,style);
        //GUILayout.Label(logo);
        // GUILayout.FlexibleSpace();
        //GUILayout.EndArea();
    }

    Rect DrawShuriken(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);
        GUI.Box(rect, title, style);
        return rect;
    }

    /// indent support
    Rect DrawShuriken(string title, Vector2 contentOffset, int HeaderHeight, int indent)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);
        rect = new Rect(rect.x + indent, rect.y, rect.width - indent, rect.height);
        GUI.Box(rect, title, style);
        return rect;
    }

    Rect DrawShurikenCenteredTitle(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);

        GUI.Box(rect, title, style);
        return rect;
    }
    void PartingLine()
    {
        GUILayout.Space(5);
        GUILine(new Color(0.5f, 0.5f, 0.5f), 1.5f);
        GUILayout.Space(5);
    }

        void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            bool blendModeChanged = false;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                DrawLogo();
                ShurikenHeaderCentered(GetVersion());
               // VRSLStyles.ShurikenHeaderCentered(GetShaderType());
                PartingLine();
                //DepthPassWarning();
                Space10();
                blendModeChanged = BlendModePopup();

                // Primary properties
                GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);
                DoAlbedoArea(material);
                DoSpecularMetallicArea();
                DoNormalArea();
                m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);
                m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
                m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, detailMask);
                DoEmissionArea(material);
                
                
                EditorGUI.BeginChangeCheck();
                m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
                if (EditorGUI.EndChangeCheck())
                    emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake

                EditorGUILayout.Space();

                // Secondary properties
                GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel);
                m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap);
                m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
                m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
                m_MaterialEditor.ShaderProperty(uvSetSecondary, Styles.uvSetLabel.text);
                EditorGUILayout.Space(5);
                //material.SetFloat("useVRSLGI",EditorGUILayout.Toggle("Use VRSL GI",FloatToBool(material.GetFloat("useVRSLGI"))) ? 1.0f : 0.0f);
                PropertyGroup(()=>{
                //m_MaterialEditor.ShaderProperty(vrslToggle, "Enable");
                material.SetFloat("useVRSLGI",EditorGUILayout.Toggle("Enable VRSL GI",FloatToBool(material.GetFloat("useVRSLGI"))) ? 1.0f : 0.0f);
                ToggleGroup(material.GetFloat("useVRSLGI") == 0);
                PropertyGroupLayer(()=>{
                    if(material.shader.name.Contains("Project") == true){
                        m_MaterialEditor.ShaderProperty(_VRSLGIQuadLightingSystem, "Priority System");
                        if(material.GetInt("_VRSLGIQuadLightingSystem") == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(_VRSLGIVertexFalloff, "Falloff Algorithm");
                            m_MaterialEditor.ShaderProperty(_VRSLGIVertexAttenuation, "Falloff Amount");
                            EditorGUI.indentLevel--;
                        }
                    }
                    PropertyGroup(()=>{

                    material.SetFloat("useVRSLGISpecular",EditorGUILayout.Toggle("Specular",FloatToBool(material.GetFloat("useVRSLGISpecular"))) ? 1.0f : 0.0f);
                    ToggleGroup(material.GetFloat("useVRSLGISpecular") == 0);
                    PropertyGroupLayer(()=>{
                    //m_MaterialEditor.ShaderProperty(_VRSLGlossiness, "VRSL Specular Smoothness");
                    m_MaterialEditor.ShaderProperty(_VRSLSpecularFunction, "Specular Function");
                    SetVRSLSpecularFunctionKeyword(Mathf.RoundToInt(_VRSLSpecularFunction.floatValue),material);
                    string s = "Smoothness";
                    if(material.shader.name.Contains("Project") == false)
                    {
                        string smoothnessDescriptor = _VRSLInvertSmoothnessMap.floatValue == 1.0f ? "Roughness" : "Smoothness";
                        m_MaterialEditor.ShaderProperty(_UseVRSLMetallicGlossMap, "Use Seperate Metallic "+ smoothnessDescriptor +" Map");
                        if(Mathf.RoundToInt(_UseVRSLMetallicGlossMap.floatValue) == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Metallic " + smoothnessDescriptor +" Map"), _VRSLMetallicGlossMap);
                            EditorGUI.indentLevel--;
                        }
                        Space8();
                        m_MaterialEditor.ShaderProperty(_VRSLSmoothnessChannel, smoothnessDescriptor + " Channel");
                        SetVRSLSpecMapKeyword(Mathf.RoundToInt(_VRSLSmoothnessChannel.floatValue), material);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(_VRSLInvertSmoothnessMap, "Invert Smoothness (To Roughness)");
                        
                        
                        s = smoothnessDescriptor;
                       // bool invertSmoothness = EditorGUILayout.Toggle("Invert Smoothness",_VRSLInvertSmoothnessMap.floatValue == -1.0f);
                      //  material.SetFloat("_VRSLInvertSmoothnessMap", invertSmoothness ? -1.0f : 1.0f);
                        m_MaterialEditor.ShaderProperty(_VRSLGlossMapStrength, smoothnessDescriptor + " Map Blend");

                        SliderStrengthUnderLables();

                        EditorGUI.indentLevel--;
                        m_MaterialEditor.ShaderProperty(_VRSLMetallicChannel, "Metallic Channel");
                        SetVRSLMetalMapKeyword(Mathf.RoundToInt(_VRSLMetallicChannel.floatValue), material);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(_VRSLInvertMetallicMap, "Invert Metallics");
                       // bool invertMetallics= EditorGUILayout.Toggle("Invert Metallics",_VRSLInvertMetallicMap.floatValue == -1.0f);
                      //  material.SetFloat("_VRSLInvertMetallicMap", invertMetallics ? -1.0f : 1.0f);
                        m_MaterialEditor.ShaderProperty(_VRSLMetallicMapStrength, "Metallic Map Blend");
                        SliderStrengthUnderLables();
                        EditorGUI.indentLevel--;


                        Space8();
                    }
                    float g = EditorGUILayout.Slider("VRSL Base " + s,Mathf.InverseLerp(1.0f, 0.1f, _VRSLGlossiness.floatValue),0.0f, 1.0f);
                    _VRSLGlossiness.floatValue = Mathf.Lerp(1.0f, 0.1f, g);
                    m_MaterialEditor.ShaderProperty(_VRSLSpecularStrength, "VRSL Base Metallic");
                    EditorGUILayout.Space(4);
                    
                    m_MaterialEditor.ShaderProperty(_VRSLSpecularMultiplier,"VRSL Specular Multiplier");
                    m_MaterialEditor.ShaderProperty(_VRSLSpecularShine, "Specular Shine Power");
                    Space2();
                    m_MaterialEditor.ShaderProperty(_VRSLGISpecularClamp, "Specular Clamp (Sets Max Brightness)");
                    });});ToggleGroupEnd();
                    if(material.shader.name.Contains("Project") == false)
                    {
                    Space8();
                    PropertyGroup(()=>{
                        m_MaterialEditor.ShaderProperty(_VRSLShadowMaskUVSet, "UV Set");
                        SetVRSLShadowMaskUVKeyword(Mathf.RoundToInt(_VRSLShadowMaskUVSet.floatValue),material);
                        m_MaterialEditor.ShaderProperty(_ShadowMaskActiveChannels, "Active Channels");
                        int activeChannelsSetting = Mathf.RoundToInt(_ShadowMaskActiveChannels.floatValue);
                        SetVRSLShadowMaskActiveChannelsKeyword(activeChannelsSetting,material);
                        m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask1, "Use Shadow Mask 1");
                        ToggleGroup(material.GetFloat("_UseVRSLShadowMask1") == 0);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Shadow Mask 1"), _VRSLShadowMask1);
                        m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask1RStrength, "R Strength");
                        if(activeChannelsSetting > 0)
                        {
                            m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask1GStrength, "G Strength");
                            if(activeChannelsSetting > 1)
                            {
                                m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask1BStrength, "B Strength");
                                if(activeChannelsSetting > 2)
                                {
                                    m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask1AStrength, "A Strength");
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                        ToggleGroupEnd();
                        
                        m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask2, "Use Shadow Mask 2");
                        ToggleGroup(material.GetFloat("_UseVRSLShadowMask2") == 0);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Shadow Mask 2"), _VRSLShadowMask2);
                        m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask2RStrength, "R Strength");
                        if(activeChannelsSetting > 0)
                        {
                            m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask2GStrength, "G Strength");
                            if(activeChannelsSetting > 1)
                            {
                                m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask2BStrength, "B Strength");
                                if(activeChannelsSetting > 2)
                                {
                                    m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask2AStrength, "A Strength");
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                        ToggleGroupEnd();

                        m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask3, "Use Shadow Mask 3");
                        ToggleGroup(material.GetFloat("_UseVRSLShadowMask3") == 0);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Shadow Mask 3"), _VRSLShadowMask3);
                        m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask3RStrength, "R Strength");
                        if(activeChannelsSetting > 0)
                        {
                            m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask3GStrength, "G Strength");
                            if(activeChannelsSetting > 1)
                            {
                                m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask3BStrength, "B Strength");
                                if(activeChannelsSetting > 2)
                                {
                                    m_MaterialEditor.ShaderProperty(_UseVRSLShadowMask3AStrength, "A Strength");
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                        ToggleGroupEnd();
                        
                    });
                    }Space8();

                    m_MaterialEditor.ShaderProperty(_UseGlobalVRSLLightTexture, "Use Global Light Texture");
                    EditorGUI.indentLevel++;
                    if(_UseGlobalVRSLLightTexture.floatValue == 1.0f)
                    {
                        EditorGUILayout.LabelField("Using Global Texture: _Udon_VRSL_GI_LightTexture");
                    }
                    else
                    {
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("VRSL Light Texture"), _VRSL_LightTexture);
                    }
                    EditorGUI.indentLevel--;

                    m_MaterialEditor.ShaderProperty(_VRSLGIDiffuseMode, "Diffuse Mode");
                    m_MaterialEditor.ShaderProperty(_VRSLGIStrength,"Strength");
                    m_MaterialEditor.ShaderProperty(_VRSLDiffuseMix,"Diffuse Mix");
                    Space2();
                   
                    Space2();
                    m_MaterialEditor.ShaderProperty(_IncludeDirectionAndSpotAngle, "Support Directional GI");
                if(material.shader.name.Contains("Project"))
                {
                    m_MaterialEditor.ShaderProperty(_ProjectorColor, "VRSL Projector Color");
                    m_MaterialEditor.ShaderProperty(_VRSLProjectorStrength, "VRSL Projector Strength");
                    m_MaterialEditor.ShaderProperty(_VRSLGIDiffuseClamp, "Max Brightness");
                }
                
                });});ToggleGroupEnd();
                EditorGUILayout.Space(5);
                if(material.shader.name.Contains("Project") == false)
                {
                    DoVRSLArea(material,m_MaterialEditor);
                    Space5();
                    DoAudioLinkArea(material,m_MaterialEditor);
                    Space5();
                }
                

			// AreaLit
                if (Shader.Find("AreaLit/Standard") != null){
                    // bool arealitFoldout = Foldouts.DoSmallFoldoutBold(foldouts, material, me, "AreaLit");
                    // if (arealitFoldout){
                        DoAreaLitArea(material,m_MaterialEditor);
                    // }
                }
                else {
                    areaLitToggle.floatValue = 0f;
                    material.SetInt("_AreaLitToggle", 0);
                    material.DisableKeyword("_AREALIT_ON");
                }
                Space5();
                			// LTCGI
                if (Shader.Find("LTCGI/Blur Prefilter") != null){
                    // bool ltcgiFoldout = Foldouts.DoSmallFoldoutBold(foldouts, material, me, "LTCGI");
                    // if (ltcgiFoldout){
                        DoLTCGIArea(material,m_MaterialEditor);
                    // }
                }
                else {
                    ltcgi.floatValue = 0;
                    ltcgi_diffuse_off.floatValue = 0;
                    ltcgi_spec_off.floatValue = 0;
                    material.SetInt("_LTCGI", 0);
                    material.SetInt("_LTCGI_DIFFUSE_OFF", 0);
                    material.SetInt("_LTCGI_SPECULAR_OFF", 0);
                    material.DisableKeyword("LTCGI");
                    material.DisableKeyword("LTCGI_DIFFUSE_OFF");
                    material.DisableKeyword("LTCGI_SPECULAR_OFF");
                }
                if(material.shader.name.Contains("Project") == true)
                {
                    m_MaterialEditor.ShaderProperty(_StencilComp, "Stencil Comp");
                    m_MaterialEditor.ShaderProperty(_StencilOp, "Stencil Operation");
                    m_MaterialEditor.ShaderProperty(_StencilFail, "Stencil Fail");
                    m_MaterialEditor.ShaderProperty(_ZWrite, "Z Write");
                    m_MaterialEditor.ShaderProperty(_ZTest, "Z Test");
                    m_MaterialEditor.ShaderProperty(_StencilZFail, "Stencil Z Fail");
                    //m_MaterialEditor.ShaderProperty(_ZTest, "Z Test");
                }
                // Third properties
                GUILayout.Label(Styles.forwardText, EditorStyles.boldLabel);
                if (highlights != null)
                    m_MaterialEditor.ShaderProperty(highlights, Styles.highlightsText);
                if (reflections != null)
                    m_MaterialEditor.ShaderProperty(reflections, Styles.reflectionsText);

                EditorGUILayout.Space();

                GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);

                m_MaterialEditor.RenderQueueField();
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendMode.targets)
                    MaterialChanged((Material)obj, m_WorkflowMode, blendModeChanged);
            }

            m_MaterialEditor.EnableInstancingField();
            m_MaterialEditor.DoubleSidedGIField();
        }

        void DetermineWorkflow(MaterialProperty[] props)
        {
            if (FindProperty("_SpecGlossMap", props, false) != null && FindProperty("_SpecColor", props, false) != null)
                m_WorkflowMode = WorkflowMode.Specular;
            else if (FindProperty("_MetallicGlossMap", props, false) != null && FindProperty("_Metallic", props, false) != null)
                m_WorkflowMode = WorkflowMode.Metallic;
            else
                m_WorkflowMode = WorkflowMode.Dielectric;
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"), true);
                return;
            }

            BlendMode blendMode = BlendMode.Opaque;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                blendMode = BlendMode.Cutout;
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                blendMode = BlendMode.Fade;
            }
            material.SetFloat("_Mode", (float)blendMode);

            DetermineWorkflow(MaterialEditor.GetMaterialProperties(new Material[] { material }));
            MaterialChanged(material, m_WorkflowMode, true);
        }

        bool BlendModePopup()
        {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var mode = (BlendMode)blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
            bool result = EditorGUI.EndChangeCheck();
            if (result)
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                blendMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;

            return result;
        }

        void DoNormalArea()
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
            if (bumpScale.floatValue != 1
                && UnityEditorInternal.InternalEditorUtility.IsMobilePlatform(EditorUserBuildSettings.activeBuildTarget))
                if (m_MaterialEditor.HelpBoxWithButton(
                    EditorGUIUtility.TrTextContent("Bump scale is not supported on mobile platforms"),
                    EditorGUIUtility.TrTextContent("Fix Now")))
                {
                    bumpScale.floatValue = 1;
                }
        }

        void DoAlbedoArea(Material material)
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
            if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
            {
                m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
            }
        }

        void DoEmissionArea(Material material)
        {
            // Emission for GI?
            if (m_MaterialEditor.EmissionEnabledProperty())
            {
                bool hadEmissionTexture = emissionMap.textureValue != null;

                // Texture and HDR color controls
                m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering, false);

                // If texture was assigned and color was black set color to white
                float brightness = emissionColorForRendering.colorValue.maxColorComponent;
                if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                    emissionColorForRendering.colorValue = Color.white;

                // change the GI flag and fix it up with emissive as black if necessary
                m_MaterialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
            }
        }

        	//VRSL Stuff
	void DoVRSLArea(Material material, MaterialEditor me)
	{
		PropertyGroup(()=>{
			me.ShaderProperty(vrslToggle, "Enable VRSL DMX");
			ToggleGroup(vrslToggle.floatValue == 0);
			PropertyGroupLayer(()=>{
				SpaceN2();
				me.ShaderProperty(dmxChannel, "DMX Channel");
				int chanMode = Mathf.FloorToInt(thirteenChannelMode.floatValue);
				chanMode = EditorGUILayout.IntPopup("DMX Mappings",chanMode,new string[]{"5-Channel", "13-Channel","1-Channel"}, new int[]{0,1,2});
				material.SetInt("_ThirteenChannelMode", chanMode);

				int gridMode = 0;
				if(verticalToggle.floatValue == 1 && compatibilityToggle.floatValue == 0){
					gridMode = 1;
				}
				else if(verticalToggle.floatValue == 0 && compatibilityToggle.floatValue == 1){
					gridMode = 2;
				}else{
					gridMode = 0;
				}
				me.ShaderProperty(legacyVRSLTextures, "Use Legacy VRSL Textures");

				if(legacyVRSLTextures.floatValue == 1)
				{

					EditorGUI.indentLevel++;
					me.TexturePropertySingleLine(new GUIContent("DMX Texture For Lights"), _OSCGridRenderTextureRAW);
					me.TexturePropertySingleLine(new GUIContent("DMX Texture For Movement"), _OSCGridRenderTexture);
					me.TexturePropertySingleLine(new GUIContent("DMX Texture For Strobe"), _OSCGridStrobeTimer);
					EditorGUI.indentLevel--;
				}

				gridMode = EditorGUILayout.IntPopup("Grid Mode",gridMode,new string[]{"Horizontal", "Vertical", "Legacy"}, new int[]{0,1,2});
				material.SetInt("_EnableVerticalMode", gridMode == 1 ? 1 : 0);
				material.SetInt("_EnableCompatibilityMode", gridMode == 2 ? 1 : 0);
				me.ShaderProperty(nineUniverseMode, new GUIContent("9-Universe Mode", "Enable/Disable 9-Universe Mode Compatibility"));
				BoldLabel("Emission");
				PropertyGroupLayer(()=>{
					me.TexturePropertySingleLine(new GUIContent("DMX Emission Map/Mask", "An emission that determins where the DMX Emission will affect the mesh"), dmxEmissionMap);
					me.ShaderProperty(dmxEmissionColor, "Emission Color");
					//CheckTrilinear(dmxEmissionMap.textureValue);
					int mapmix = (int)(dmxEmissionMapMix.floatValue);
					mapmix = EditorGUILayout.IntPopup("Emission Map Blending",mapmix,new string[]{"Multiply", "Add", "Mix"}, new int[]{0,1,2});
					material.SetInt("_DMXEmissionMapMix", mapmix);
					me.ShaderProperty(universalIntensity, "Universal Intensity");
					me.ShaderProperty(finalIntensity, "Final Intensity");
					me.ShaderProperty(globalIntensity, "Global Intensity");
                    EditorGUI.indentLevel++;
                    me.ShaderProperty(_GlobalIntensityBlend, "Global Intensity Blend");
                    EditorGUI.indentLevel--;
					me.ShaderProperty(fixtureMaxIntensity, "Fixture Max Intensity");
					me.ShaderProperty(strobeToggle, "Enable Strobe");
				});

				BoldLabel("Movement");
				PropertyGroupLayer(()=>{
					me.ShaderProperty(rotationOrigin, "Fixture Pivot/Roptation Origin");
					me.ShaderProperty(panToggle, "Pan");
					ToggleGroup(panToggle.floatValue == 0);
					PropertyGroupLayer(()=>{
						me.ShaderProperty(maxMinPanAngle, "Max/Min Pan Angle (-x, x)");
						me.ShaderProperty(baseRotationY, "Pan Offset");
						me.ShaderProperty(invertPan, "Invert Pan");
						});
					ToggleGroupEnd();
					me.ShaderProperty(tiltToggle, "Tilt");
					ToggleGroup(tiltToggle.floatValue == 0);
					PropertyGroupLayer(()=>{
						me.ShaderProperty(maxMinTiltAngle, "Max/Min Tilt Angle (-y, y)");
						me.ShaderProperty(baseRotationX, "Tilt Offset");
						me.ShaderProperty(invertTilt, "Invert Tilt");
						});
					});
					ToggleGroupEnd();
				
				SpaceN2();
			});
		});
		ToggleGroupEnd();
	}
	//End VRSL Stuff

    	void DoAreaLitArea(Material material, MaterialEditor me){
		PropertyGroup(()=>{
			me.ShaderProperty(areaLitToggle, "Enable AreaLit");
			ToggleGroup(areaLitToggle.floatValue == 0);
			PropertyGroupLayer(()=>{
				SpaceN2();
				me.ShaderProperty(areaLitStrength, "Strength");
				me.ShaderProperty(areaLitRoughnessMult, "Roughness Multiplier");
				me.ShaderProperty(opaqueLights, new GUIContent("Opaque Lights"));
				SpaceN2();
			});
			PropertyGroupLayer(()=>{
				SpaceN2();
				var lightMeshText = !lightMesh.textureValue ? new GUIContent("Mesh", "Light mesh data") : new GUIContent(
					"Mesh" + $" (max: {lightMesh.textureValue.height})", "Light mesh data"
				);
				me.TexturePropertySingleLine(lightMeshText, lightMesh);
				me.TexturePropertySingleLine(new GUIContent("Texture 0", "Light texture 0"), lightTex0);
				// CheckTrilinear(lightTex0.textureValue);
				me.TexturePropertySingleLine(new GUIContent("Texture 1", "Light texture 1"), lightTex1);
				// CheckTrilinear(lightTex1.textureValue);
				me.TexturePropertySingleLine(new GUIContent("Texture 2", "Light texture 2, intended for indirect light"), lightTex2);
				// CheckTrilinear(lightTex2.textureValue);
				me.TexturePropertySingleLine(new GUIContent("Texture 3+","Light texture 3 and above, intended for static lights"), lightTex3);
				// CheckTrilinear(lightTex3.textureValue);
				me.TexturePropertySingleLine(new GUIContent("Occlusion"), areaLitOcclusion);
				if (areaLitOcclusion.textureValue){
					me.ShaderProperty(occlusionUVSet, "UV Set");
				}
				TextureSO(me, areaLitOcclusion, areaLitOcclusion.textureValue);
				SpaceN2();
			});
			ToggleGroupEnd();
			// DisplayInfo("Note that the AreaLit package files MUST be inside a folder named AreaLit (case sensitive) directly in the Assets folder (Assets/AreaLit)");
		});
	}

    	void DoLTCGIArea(Material material, MaterialEditor me){
		PropertyGroup(()=>{
			me.ShaderProperty(ltcgi, "Enable LTCGI");
			ToggleGroup(ltcgi.floatValue == 0);
			PropertyGroupLayer(()=>{
				SpaceN2();
				me.ShaderProperty(ltcgiStrength, "Strength");
				me.ShaderProperty(ltcgi_spec_off, "Disable Specular");
				me.ShaderProperty(ltcgi_diffuse_off, "Disable Diffuse");
				SpaceN2();
			});
			ToggleGroupEnd();
		});
        }

        void DoAudioLinkArea(Material material, MaterialEditor me){
		PropertyGroup(()=>{
			me.ShaderProperty(_AudioLinkToggle, "Enable VRSL AudioLink");
			ToggleGroup(_AudioLinkToggle.floatValue == 0);
			PropertyGroupLayer(()=>{
				SpaceN2();
                me.TexturePropertySingleLine(new GUIContent("AudioLink Emission Map/Mask", "AudioLink Emission Map/Mask"), _AudioLinkEmissionMap);
                me.ShaderProperty(_Emission, "AudioLink Emission Color");
                me.ShaderProperty(_EnableAudioLink, "Enable AudioLink");
				me.ShaderProperty(_Band, "Band");
                me.ShaderProperty(_BandMultiplier,"Band Multiplier");
				//me.ShaderProperty(_NumBands, "Number Of Bands");
				me.ShaderProperty(_Delay, "Delay");
                Space8();
                me.ShaderProperty(universalIntensity, "Universal Intensity");
                me.ShaderProperty(finalIntensity, "Final Intensity");
                me.ShaderProperty(globalIntensity, "Global Intensity");
                EditorGUI.indentLevel++;
                me.ShaderProperty(_GlobalIntensityBlend, "Global Intensity Blend");
                EditorGUI.indentLevel--;
                me.ShaderProperty(fixtureMaxIntensity, "Fixture Max Intensity");
                
                Space8();
                me.ShaderProperty(_EnableColorChord, "Enable Color Chord");
                //Space2();
                me.ShaderProperty(_EnableColorTextureSample, "Enable Texture Sampling");
                if(_EnableColorTextureSample.floatValue == 1f)
                {
                   EditorGUI.indentLevel++;
                   me.TexturePropertySingleLine(new GUIContent("Texture To Sample From", "Texture To Sample From"), _SamplingTexture);
                   me.ShaderProperty(_TextureColorSampleX, "X Coordinate (UV)");
                   me.ShaderProperty(_TextureColorSampleY, "Y Coordinate (UV)");
                   me.ShaderProperty(_RenderTextureMultiplier, "Render Texture Multiplier");
                   EditorGUI.indentLevel--;
                   Space8();
                }
                
                me.ShaderProperty(_EnableThemeColorSampling, "Enable Theme Colors");
                if(_EnableThemeColorSampling.floatValue == 1f)
                {
                   EditorGUI.indentLevel++;
                   me.ShaderProperty(_ThemeColorTarget, "Theme Color Target");
                   EditorGUI.indentLevel--;
                }
				SpaceN2();
			});
			ToggleGroupEnd();
		});
	}


        void SetVRSLMetalMapKeyword(int input, Material material)
        {
            switch(input)
            {
                case 0:
                    material.EnableKeyword("_VRSL_METAL_R");
                    material.DisableKeyword("_VRSL_METAL_G");
                    material.DisableKeyword("_VRSL_METAL_B");
                    material.DisableKeyword("_VRSL_METAL_A");
                    break;
                case 1:
                    material.DisableKeyword("_VRSL_METAL_R");
                    material.EnableKeyword("_VRSL_METAL_G");
                    material.DisableKeyword("_VRSL_METAL_B");
                    material.DisableKeyword("_VRSL_METAL_A");
                    break;
                case 2:
                    material.DisableKeyword("_VRSL_METAL_R");
                    material.DisableKeyword("_VRSL_METAL_G");
                    material.EnableKeyword("_VRSL_METAL_B");
                    material.DisableKeyword("_VRSL_METAL_A");
                    break;
                case 3:
                    material.DisableKeyword("_VRSL_METAL_R");
                    material.DisableKeyword("_VRSL_METAL_G");
                    material.DisableKeyword("_VRSL_METAL_B");
                    material.EnableKeyword("_VRSL_METAL_A");
                    break;
                default:
                    break;
            }
        }

        void SetVRSLSpecularFunctionKeyword(int input, Material material)
        {
            switch(input)
            {
                case 0:
                    material.EnableKeyword("_VRSL_SPECFUNC_GGX");
                    material.DisableKeyword("_VRSL_SPECFUNC_BECKMAN");
                    material.DisableKeyword("_VRSL_SPECFUNC_PHONG");
                    break;
                case 1:
                    material.DisableKeyword("_VRSL_SPECFUNC_GGX");
                    material.EnableKeyword("_VRSL_SPECFUNC_BECKMAN");
                    material.DisableKeyword("_VRSL_SPECFUNC_PHONG");
                    break;
                case 2:
                    material.DisableKeyword("_VRSL_SPECFUNC_GGX");
                    material.DisableKeyword("_VRSL_SPECFUNC_BECKMAN");
                    material.EnableKeyword("_VRSL_SPECFUNC_PHONG");
                    break;
                default:
                    break;
            }
        }

        void SetVRSLShadowMaskActiveChannelsKeyword(int input, Material material)
        {
            switch(input)
            {
                case 0:
                    material.DisableKeyword("_VRSL_SHADOWMASK_RG");
                    material.DisableKeyword("_VRSL_SHADOWMASK_RGB");
                    material.DisableKeyword("_VRSL_SHADOWMASK_RGBA");;
                    break;
                case 1:
                    material.EnableKeyword("_VRSL_SHADOWMASK_RG");
                    material.DisableKeyword("_VRSL_SHADOWMASK_RGB");
                    material.DisableKeyword("_VRSL_SHADOWMASK_RGBA");
                    break;
                case 2:
                    material.DisableKeyword("_VRSL_SHADOWMASK_RG");
                    material.EnableKeyword("_VRSL_SHADOWMASK_RGB");
                    material.DisableKeyword("_VRSL_SHADOWMASK_RGBA");
                    break;
                case 3:
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV0");
                    material.DisableKeyword("_VRSL_SHADOWMASK_RGB");
                    material.EnableKeyword("_VRSL_SHADOWMASK_RGBA");
                    break;               
                default:
                    break;
            }
        }

        void SetVRSLShadowMaskUVKeyword(int input, Material material)
        {
            switch(input)
            {
                case 0:
                    material.EnableKeyword("_VRSL_SHADOWMASK_UV0");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV1");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV2");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV3");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV4");
                    break;
                case 1:
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV0");
                    material.EnableKeyword("_VRSL_SHADOWMASK_UV1");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV2");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV3");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV4");
                    break;
                case 2:
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV0");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV1");
                    material.EnableKeyword("_VRSL_SHADOWMASK_UV2");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV3");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV4");
                    break;
                case 3:
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV0");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV1");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV2");
                    material.EnableKeyword("_VRSL_SHADOWMASK_UV3");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV4");
                    break;               
                case 4:
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV0");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV1");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV2");
                    material.DisableKeyword("_VRSL_SHADOWMASK_UV3");
                    material.EnableKeyword("_VRSL_SHADOWMASK_UV4");
                    break;
                default:
                    break;
            }
        }
        void SetVRSLSpecMapKeyword(int input, Material material)
        {
            switch(input)
            {
                case 0:
                    material.EnableKeyword("_VRSL_SPEC_R");
                    material.DisableKeyword("_VRSL_SPEC_G");
                    material.DisableKeyword("_VRSL_SPEC_B");
                    material.DisableKeyword("_VRSL_SPEC_A");
                    break;
                case 1:
                    material.DisableKeyword("_VRSL_SPEC_R");
                    material.EnableKeyword("_VRSL_SPEC_G");
                    material.DisableKeyword("_VRSL_SPEC_B");
                    material.DisableKeyword("_VRSL_SPEC_A");
                    break;
                case 2:
                    material.DisableKeyword("_VRSL_SPEC_R");
                    material.DisableKeyword("_VRSL_SPEC_G");
                    material.EnableKeyword("_VRSL_SPEC_B");
                    material.DisableKeyword("_VRSL_SPEC_A");
                    break;
                case 3:
                    material.DisableKeyword("_VRSL_SPEC_R");
                    material.DisableKeyword("_VRSL_SPEC_G");
                    material.DisableKeyword("_VRSL_SPEC_B");
                    material.EnableKeyword("_VRSL_SPEC_A");
                    break;
                default:
                    break;
            }
        }



        void DoSpecularMetallicArea()
        {
            bool hasGlossMap = false;
            if (m_WorkflowMode == WorkflowMode.Specular)
            {
                hasGlossMap = specularMap.textureValue != null;
                m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap, hasGlossMap ? null : specularColor);
            }
            else if (m_WorkflowMode == WorkflowMode.Metallic)
            {
                hasGlossMap = metallicMap.textureValue != null;
                m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap, hasGlossMap ? null : metallic);
            }

            bool showSmoothnessScale = hasGlossMap;
            if (smoothnessMapChannel != null)
            {
                int smoothnessChannel = (int)smoothnessMapChannel.floatValue;
                if (smoothnessChannel == (int)SmoothnessMapChannel.AlbedoAlpha)
                    showSmoothnessScale = true;
            }

            int indentation = 2; // align with labels of texture properties
            m_MaterialEditor.ShaderProperty(showSmoothnessScale ? smoothnessScale : smoothness, showSmoothnessScale ? Styles.smoothnessScaleText : Styles.smoothnessText, indentation);

            ++indentation;
            if (smoothnessMapChannel != null)
                m_MaterialEditor.ShaderProperty(smoothnessMapChannel, Styles.smoothnessMapChannelText, indentation);
        }

        public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode, bool overrideRenderQueue)
        {
            int minRenderQueue = -1;
            int maxRenderQueue = 5000;
            int defaultRenderQueue = -1;
            bool isProjector = material.shader.name.Contains("Project");
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    if(!isProjector){
                        material.SetInt("_ZWrite", 1);
                    }
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    minRenderQueue = -1;
                    maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest - 1;
                    defaultRenderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    if(!isProjector){
                        material.SetInt("_ZWrite", 1);
                    }
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    minRenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast;
                    defaultRenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    if(!isProjector){
                        material.SetInt("_ZWrite", 0);
                    }
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    minRenderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast + 1;
                    maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Overlay - 1;
                    defaultRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    if(!isProjector){
                        material.SetInt("_ZWrite", 0);
                    }
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    minRenderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast + 1;
                    maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Overlay - 1;
                    defaultRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }

            if (overrideRenderQueue || material.renderQueue < minRenderQueue || material.renderQueue > maxRenderQueue)
            {
                if (!overrideRenderQueue)
                    //Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Render queue value outside of the allowed range ({0} - {1}) for selected Blend mode, resetting render queue to default", minRenderQueue, maxRenderQueue);
                material.renderQueue = defaultRenderQueue;
            }
        }

        static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
        {
            int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
            if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
                return SmoothnessMapChannel.AlbedoAlpha;
            else
                return SmoothnessMapChannel.SpecularMetallicAlpha;
        }

        static void SetDiffuseMode(Material material, int state)
        {
            switch(state)
            {
                case 0:
                    SetKeyword(material, "_VRSL_DIFFUSETINT", false);
                    SetKeyword(material, "_VRSL_DIFFUSETOON", false);
                    break;
                case 1:
                    SetKeyword(material, "_VRSL_DIFFUSETINT", true);
                    SetKeyword(material, "_VRSL_DIFFUSETOON", false);
                    break;
                case 2:
                    SetKeyword(material, "_VRSL_DIFFUSETINT", false);
                    SetKeyword(material, "_VRSL_DIFFUSETOON", true);
                    break;
                default:
                    break;
            }
        }
        static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
        {
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
            if (workflowMode == WorkflowMode.Specular)
                SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
            else if (workflowMode == WorkflowMode.Metallic)
                SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
            SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
            SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));
            int ltcgiToggle = material.GetInt("_LTCGI");
            // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
            // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
            // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
            MaterialEditor.FixupEmissiveFlag(material);
            bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

            if (material.HasProperty("_SmoothnessTextureChannel"))
            {
                SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
            }
            SetKeyword(material,"_VRSL_GI", material.GetFloat("useVRSLGI") == 1.0f);
            SetKeyword(material,"_VRSL_GI_SPECULARHIGHLIGHTS", material.GetFloat("useVRSLGISpecular") == 1.0f);
            SetKeyword(material, "_VRSL_GLOBALLIGHTTEXTURE", material.GetFloat("_UseGlobalVRSLLightTexture") == 1.0f);
            SetKeyword(material, "_VRSL_GI_ANGLES", material.GetFloat("_IncludeDirectionAndSpotAngle") == 1.0f);
            SetDiffuseMode(material,material.GetInt("_VRSLGIDiffuseMode"));
            //SetKeyword(material, "_VRSL_DIFFUSETINT", material.GetFloat("_VRSLGIDiffuseMode") == 1.0f);
            //VRSL Stuff
            if(material.shader.name.Contains("Project") == false)
            {
                SetKeyword(material, "_VRSL_ON", material.GetInt("_VRSLToggle") == 1);
                SetKeyword(material, "_VRSL_LEGACY_TEXTURES", material.GetInt("_UseLegacyDMXTextures") == 1);
                SetKeyword(material, "_VRSLTHIRTEENCHAN_ON", material.GetInt("_ThirteenChannelMode") == 1);
                SetKeyword(material, "_VRSLONECHAN_ON", material.GetInt("_ThirteenChannelMode") == 2);
                SetKeyword(material, "_VRSLPAN_ON", material.GetInt("_EnablePanMovement") == 1);
                SetKeyword(material, "_VRSLTILT_ON", material.GetInt("_EnableTiltMovement") == 1);
            //    SetKeyword(material, "_STROBE_ON", material.GetInt("_EnableStrobe") == 1);
                SetKeyword(material, "_VRSL_MIX_MULT", material.GetInt("_DMXEmissionMapMix") == 0);
                SetKeyword(material, "_VRSL_MIX_ADD", material.GetInt("_DMXEmissionMapMix") == 1);
                SetKeyword(material, "_VRSL_MIX_MIX", material.GetInt("_DMXEmissionMapMix") == 2);

                SetKeyword(material, "_VRSL_SHADOWMASK1", material.GetInt("_UseVRSLShadowMask1") == 1);
                SetKeyword(material, "_VRSL_SHADOWMASK2", material.GetInt("_UseVRSLShadowMask2") == 1);
                SetKeyword(material, "_VRSL_SHADOWMASK3", material.GetInt("_UseVRSLShadowMask3") == 1);
                SetKeyword(material,"_VRSL_MG_MAP",material.GetInt("_UseVRSLMetallicGlossMap") == 1);
                SetKeyword(material, "_VRSL_AUDIOLINK_ON", material.GetInt("_AudioLinkToggle") == 1);
            }
            else
            {
                SetKeyword(material, "_VRSL_GI_ENFORCELIMIT", material.GetInt("_VRSLGIQuadLightingSystem") == 1);
            }
                SetKeyword(material, "_AREALIT_ON", material.GetInt("_AreaLitToggle") == 1);
                SetKeyword(material, "LTCGI", ltcgiToggle == 1);
		        SetKeyword(material, "LTCGI_DIFFUSE_OFF", material.GetInt("_LTCGI_DIFFUSE_OFF") == 1 && ltcgiToggle == 1);
		        SetKeyword(material, "LTCGI_SPECULAR_OFF", material.GetInt("_LTCGI_SPECULAR_OFF") == 1 && ltcgiToggle == 1);
                
            //End VRSL Stuff
        }

        static void MaterialChanged(Material material, WorkflowMode workflowMode, bool overrideRenderQueue)
        {
            SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"), overrideRenderQueue);

            SetMaterialKeywords(material, workflowMode);
        }

        static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }

        		// Shorthand spacing funcs
		public static void SpaceN24(){ GUILayout.Space(-24); }
		public static void SpaceN22(){ GUILayout.Space(-22); }
		public static void SpaceN20(){ GUILayout.Space(-20); }
		public static void SpaceN18(){ GUILayout.Space(-18); }
		public static void SpaceN16(){ GUILayout.Space(-16); }
		public static void SpaceN14(){ GUILayout.Space(-14); }
		public static void SpaceN12(){ GUILayout.Space(-12); }
		public static void SpaceN10(){ GUILayout.Space(-10); }
		public static void SpaceN8(){ GUILayout.Space(-8); }
		public static void SpaceN6(){ GUILayout.Space(-6); }
		public static void SpaceN5(){ GUILayout.Space(-5); }
		public static void SpaceN4(){ GUILayout.Space(-4); }
		public static void SpaceN3(){ GUILayout.Space(-3); }
		public static void SpaceN2(){ GUILayout.Space(-2); }
		public static void SpaceN1(){ GUILayout.Space(-1); }
		public static void Space1(){ GUILayout.Space(1); }
		public static void Space2(){ GUILayout.Space(2); }
		public static void Space3(){ GUILayout.Space(3); }
		public static void Space4(){ GUILayout.Space(4); }
		public static void Space5(){ GUILayout.Space(5); }
		public static void Space6(){ GUILayout.Space(6); }
		public static void Space8(){ GUILayout.Space(8); }
		public static void Space10(){ GUILayout.Space(10); }
		public static void Space12(){ GUILayout.Space(12); }
		public static void Space14(){ GUILayout.Space(14); }
		public static void Space16(){ GUILayout.Space(16); }
		public static void Space18(){ GUILayout.Space(18); }
		public static void Space20(){ GUILayout.Space(20); }
		public static void Space22(){ GUILayout.Space(22); }
		public static void Space24(){ GUILayout.Space(24); }

        
        		// Vector2 property with corrected width scaling
		public static void Vector2Field(MaterialProperty vec, string label){
			SpaceN2();
			Vector4 newVec = vec.vectorValue;
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = GetPropertyWidth()/2;

			EditorGUILayout.LabelField(label);
			SpaceN20();
			Rect r = EditorGUILayout.GetControlRect();
			r.x += labelWidth;
			EditorGUIUtility.labelWidth = 10f;

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = vec.hasMixedValue;

				// X Field
				r.width = fieldWidth-2f;
				newVec.x = EditorGUI.FloatField(r, "X", newVec.x);
				
				// Y Field
				r.x += fieldWidth+2f;
				newVec.y = EditorGUI.FloatField(r, "Y", newVec.y);

			if (EditorGUI.EndChangeCheck())
				vec.vectorValue = newVec;
			EditorGUI.showMixedValue = false;
			EditorGUIUtility.labelWidth = labelWidth;
		}
    }
 // namespace UnityEditor
