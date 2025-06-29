using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DataLogger : MonoBehaviour
{
    [SerializeField] float logEndTime = 10f;
    List<double> cpuDataLog;
    //List<float> fpsDataLog;
    string logMessage;
    [SerializeField] float logInterval = 0.1f;
    bool saved = false;

    int FPS;
    [SerializeField]
    private float fpsInterval = 0.1f;
    private int fpsSampleRate = 10;

    float lastAverageTime = 0;
    float lastSampleTime = 0;
    int[] sampleFPS;
    int sampleCount = 0;

    FrameTiming[] m_FrameTimings = new FrameTiming[1];

    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        //public List<double> cpuData;
        //public List<float> fpsData;
        public double avgCpuUsageData;
        public float avgFpsData;
    }

    private void Start()
    {
        cpuDataLog = new List<double>();
        //fpsDataLog = new List<float>();
    }

    void Update()
    {
        // Only log data for a specific amount of time after program start.
        if (Time.time < logEndTime)
        {
            GetAverageFPS();
            LogCpuUsage();
        }
        else if(!saved)
        {
            Debug.Log("All data has been logged.");
            // Export data once all of it has been logged.
            saved = true;
            Save();
        }
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
            _saveData.avgFpsData = averageFPS;
            //LogData(averageFPS);
        }
    }

    double GetCpuUsage(FrameTiming frameTime)
    {
        return frameTime.cpuMainThreadFrameTime / frameTime.cpuFrameTime * 100;
    }

    void LogCpuUsage()
    {
        FrameTimingManager.CaptureFrameTimings();
        var ret = FrameTimingManager.GetLatestTimings((uint)m_FrameTimings.Length, m_FrameTimings);
        if (ret > 0) 
        { 
            double cpuUsage = GetCpuUsage(m_FrameTimings[0]);
            cpuDataLog.Add(cpuUsage);
        }
    }


    //void LogData(float avgFPS)
    //{
    //    // Get CPU usage
    //    //double cpuUsage = -1;
    //    //FrameTimingManager.CaptureFrameTimings();
    //    //var ret = FrameTimingManager.GetLatestTimings((uint)m_FrameTimings.Length, m_FrameTimings);
    //    //if (ret > 0) { cpuUsage = GetCpuUsage(m_FrameTimings[0]); }

    //    // If the cpu usage isn't an impossible value, we log the data.
    //    //if (cpuUsage >= 0)
    //    {
    //        //cpuDataLog.Add(cpuUsage);
    //        //fpsDataLog.Add(avgFPS);
    //    }
    //}

    string SaveFileLocation()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".data";
        return saveFile;
    }

    void Save()
    {
        Debug.Log("Saving progress");
        _saveData.avgCpuUsageData = cpuDataLog.Average();
        //_saveData.fpsData = fpsDataLog;
        File.WriteAllText(SaveFileLocation(), JsonUtility.ToJson(_saveData));
    }
}
