using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.Networking;

public class PreBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Compile local IP...");

        string hostAddressClass =
            string.Format("public class Host {{public const string HOST = \"{0}\"; }}",
            GetLocalIPAddress());

        string pathToDataScript = Path.Combine(Application.dataPath, "Scripts/Host.cs");
        Debug.Log("pathToDataScript: " + pathToDataScript);

        File.WriteAllText(pathToDataScript, hostAddressClass);
    }

    private static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}