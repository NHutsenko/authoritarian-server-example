using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PreBuild : IPreprocessBuildWithReport {
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report) {
        Debug.Log("Compile local IP...");

        string dataScriptText =
            "public class Host { public const string HOST = \"IP\"; }".
                Replace("IP", Network.player.ipAddress);

        string pathToDataScript = Path.Combine(Application.dataPath, "Scripts/Host.cs");
        Debug.Log("pathToDataScript: " + pathToDataScript);

        File.WriteAllText(pathToDataScript, dataScriptText);
    }
}