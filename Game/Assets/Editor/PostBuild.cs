using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MyCustomBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildReport report)
    {
        string filePath = "";
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c " + filePath;
        process.Start();
    }
    [MenuItem("Custom/Build and Run")]
    public static void BuildAndRun()
    {
        // ������ ��θ� ������ �迭�� �����մϴ�.
        string[] scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
        BuildPipeline.BuildPlayer(scenes, "C:/project_a/RoteSquare.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}
