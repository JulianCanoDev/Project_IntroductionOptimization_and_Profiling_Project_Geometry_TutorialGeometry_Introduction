using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureProcessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        //only process texture added to the Scene 3 folder
        if(!assetPath.Contains("Scene 3"))
            return;
        
        if (assetPath.IndexOf("_normal", StringComparison.InvariantCultureIgnoreCase) >= 0)
        {
            TextureImporter textureImporter  = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.NormalMap;
        }
        else if (assetPath.IndexOf("_ui", StringComparison.InvariantCultureIgnoreCase) >= 0)
        {
            TextureImporter textureImporter  = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.GUI;
        }
    }
}
