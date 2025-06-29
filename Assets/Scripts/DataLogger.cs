using UnityEngine;
using System.IO;
using System.Linq;
using System.Diagnostics;

public class DataLogger : MonoBehaviour
{
    [SerializeField] float logEndTime = 10f;
    [SerializeField] float logInterval = 0.1f;
    float lastLogTime = 0;
    string[] dataLog;
    string logMessage;

    int FPS;
    [SerializeField]
    private float fpsInterval = 0.1f;
    private int fpsSampleRate = 10;

    float lastAverageTime = 0;
    float lastSampleTime = 0;
    int[] sampleFPS;
    int sampleCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Only log data for a specific amount of time after program start.
        if (Time.time < logEndTime) { GetAverageFPS(); }
    }

    private float GetFPS()
    {
        FPS = (int)(1.0f / Time.unscaledDeltaTime);
        return FPS;
    }

    private void GetAverageFPS()
    {
        if (lastSampleTime == 0)
        {
            sampleFPS = new int[fpsSampleRate];
        }

        if (Time.time - lastSampleTime >= fpsInterval / fpsSampleRate)
        {
            sampleFPS[sampleCount] = (int)(1.0f / Time.deltaTime);
            lastSampleTime = Time.time;
            sampleCount++;
        }

        if (Time.time - lastAverageTime >= fpsInterval)
        {
            float averageFPS = (int)sampleFPS.Average();
            lastAverageTime = Time.time;
            sampleCount = 0;
            LogData(averageFPS);
        }
    }

    public object GetCPUCounter()
    {

        PerformanceCounter cpuCounter = new PerformanceCounter();
        cpuCounter.CategoryName = "Processor";
        cpuCounter.CounterName = "% Processor Time";
        cpuCounter.InstanceName = "_Total";

        // will always start at 0
        dynamic firstValue = cpuCounter.NextValue();
        //System.Threading.Thread.Sleep(1000);
        // now matches task manager reading
        dynamic secondValue = cpuCounter.NextValue();

        return secondValue;

    }

    void LogData(float avgFPS)
    {
        if (Time.time > lastLogTime + logInterval)
        {
            lastLogTime = Time.time;
            UnityEngine.Debug.LogFormat("Time: {0} FPS: {1} Average FPS: {2} CPU: {3}", Time.time, GetFPS(), avgFPS, GetCPUCounter());
            //logMessage = string.Format("Time: {0} FPS: {1} CPU: {2}", Time.time, GetFPS(),  );
        }
    }

    public static string SaveFileName()
    {
        UnityEngine.Debug.Log("Getting save file name.");
        string saveFile = Application.persistentDataPath + "/save" + ".data";
        return saveFile;
    }

    public static void Save()
    {
        UnityEngine.Debug.Log("Saving progress.");
        //File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData));
    }
}
