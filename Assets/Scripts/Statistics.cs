using UnityEngine;
using System.IO;

public class Statistics
{
    static string statisticsFilename = Application.dataPath + "/Statistics.txt";
    public static void LogStat(string logString)
    {
        TextWriter tw = new StreamWriter(statisticsFilename, true);
        tw.WriteLine(logString);
        tw.Close();
    }
}
