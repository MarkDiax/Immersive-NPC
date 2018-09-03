using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MakeProjectFolders : EditorWindow {

    [MenuItem("Project Tools/Make Folders")]
    public static void MakeFolder()
    {
        GenerateFolders();
    }

    public static void GenerateFolders()
    {
        string path = Application.dataPath + "/";

        //add new folders here
        string[] folders = new string[] 
        { "Audio", "Materials", "Prefabs", "_Scenes", "Materials", "Sprites", "Textures", "Imported", "Meshes", "Fonts", "Resources", "Scripts", "Shaders", "Plugins" };

        for (int i = 0; i < folders.Length; i++)
        {
            Directory.CreateDirectory(path + folders[i]);
        }

        AssetDatabase.Refresh();
    }
}
