using UnityEngine;
using System.IO;

public class LogToFile : MonoBehaviour
{
    static string logFilename;
    private void Awake()
    {
        logFilename = Application.dataPath + "/Logs.txt";
    }
    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }
    public void Log(string logString, string stackTrace, LogType type)
    {
        TextWriter tw = new StreamWriter(logFilename, true);
        tw.WriteLine(logString);
        tw.Close();
    }
}
