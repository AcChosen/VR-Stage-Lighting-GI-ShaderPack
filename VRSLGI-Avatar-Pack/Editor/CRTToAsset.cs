using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CRTToAsset : EditorWindow
{
    string m_ScriptFilePath;
    string m_ScriptFolder;
    string path = "Assets/VRSL Addons/VRSL-GI Shader Package/VRSLGI-Avatar-Pack"; 
    string filename;
    public RenderTexture source;
    [MenuItem("Tools/RT To Asset")]
        static void Init()
    {
        EditorWindow window = GetWindowWithRect<CRTToAsset>(new Rect(0, 0, 600, 200));
        window.titleContent = new GUIContent("RT To Asset File");
        window.Show();
    }

    void ReadSetupData()
    {
        MonoScript ms = MonoScript.FromScriptableObject( this );
        m_ScriptFilePath = AssetDatabase.GetAssetPath( ms );

        FileInfo fi = new FileInfo( m_ScriptFilePath);
        m_ScriptFolder = fi.Directory.ToString();
        m_ScriptFolder.Replace( '\\', '/');

        //Debug.Log( m_ScriptFolder );
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBAFloat, false);
        Debug.Log("Copying Render Texture");
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        source = (RenderTexture )EditorGUILayout.ObjectField(source, typeof(RenderTexture), true);
        EditorGUILayout.EndHorizontal();
        path = EditorGUILayout.TextField(new GUIContent("Path"),path);
        filename = EditorGUILayout.TextField(new GUIContent("Name"),filename);
        if (GUILayout.Button("Export Render Texture"))
        {
            try{
            if(source is RenderTexture)
            {
                // RenderTexture extraTex = new RenderTexture(source.width, source.height, source.depth, source.format);
                // extraTex.Create();
                // Graphics.Blit(source, extraTex);
                Texture2D myTexture = toTexture2D(source);
                myTexture.filterMode = FilterMode.Point;
                AssetDatabase.CreateAsset(myTexture, path + "/" + filename +  ".asset");
            }
            }
            catch(Exception e)
            {
                Debug.LogError("Message: " + e.Message.ToString());
                Debug.LogError("StackTrace: " + e.StackTrace.ToString());
            }
        }
    }



}
