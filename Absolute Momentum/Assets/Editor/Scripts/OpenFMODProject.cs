using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;

public class OpenFMODProject
{
    [MenuItem("Tools/OpenFMOD Project")]
    public static void OpenFMOD()
    {
        string relativePath = "../../FMOD Project/AbsoluteMomentum/AbsoluteMomentum.fspro";
        string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, relativePath));
        if (File.Exists(projectPath))
        {
            Process.Start(projectPath);
        }
        else
        {
            UnityEngine.Debug.LogError("FMOD project not found at: " + projectPath);
        }
    }

}
