#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class SetGlobalTexture : MonoBehaviour
{
    public Texture2D[] targetTextures;
    public Texture2D offTexture2D;
    int selectedTexture;
    

    public void _SetGlobalTexture(int index)
    {
        if(targetTextures[index])
        {
            int textureID = Shader.PropertyToID("_Udon_VRSL_GI_LightTexture");
            Shader.SetGlobalTexture(textureID, targetTextures[index]);
            selectedTexture = index;
        }
    }

    public void _ClearGlobalTexture()
    {
        if(offTexture2D)
        {
            int textureID = Shader.PropertyToID("_Udon_VRSL_GI_LightTexture");
            Shader.SetGlobalTexture(textureID, offTexture2D);
        }
    }
    void Start()
    {
        _SetGlobalTexture(selectedTexture);
    }
}
[CustomEditor(typeof(SetGlobalTexture))]
public class SetGlobalTexture_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        SetGlobalTexture setGlobalTexture = target as SetGlobalTexture;
        for(int i = 0; i < setGlobalTexture.targetTextures.Length; i++)
        {
            if(GUILayout.Button("Set Texture: " + setGlobalTexture.targetTextures[i].name))
            {
                setGlobalTexture._SetGlobalTexture(i);
            }  
        } 
        if(GUILayout.Button("Clear VRSL GI Texture"))
        {
            setGlobalTexture._ClearGlobalTexture();
        }   
        DrawDefaultInspector();

    }
}
#endif