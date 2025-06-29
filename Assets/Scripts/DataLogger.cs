using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DataLogger : MonoBehaviour
{
    [SerializeField] float logEndTime = 10f;
    List<double> cpuDataLog;
    List<float> fpsDataLog;
    string logMessage;
    bool saved = false;

    int FPS;
    [SerializeField]
    private float logInterval = 0.1f;
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
        public List<double> cpuData;
        public List<float> fpsData;
    }

    private void Start()
    {
        cpuDataLog = new List<double>();
        fpsDataLog = new List<float>();
    }

    void Update()
    {
        // Only log data for a specific amount of time after program start.
        if (Time.time < logEndTime)
        {
            GetAverageFPS();
        }
        else if(!saved)
        {
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

        if (Time.time - lastSampleTime >= logInterval / fpsSampleRate)
        {
            sampleFPS[sampleCount] = (int)(1.0f / Time.deltaTime);
            lastSampleTime = Time.time;
            sampleCount++;
        }

        if (Time.time - lastAverageTime >= logInterval)
        {
            float averageFPS = (int)sampleFPS.Average();
            lastAverageTime = Time.time;
            sampleCount = 0;
            LogData(averageFPS);
        }
    }

    double GetCpuUsage(FrameTiming frameTime)
    {
        return frameTime.cpuMainThreadFrameTime / frameTime.cpuFrameTime * 100;
    }


    void LogData(float avgFPS)
    {
        // Get CPU usage
        double cpuUsage = -1;
        FrameTimingManager.CaptureFrameTimings();
        var ret = FrameTimingManager.GetLatestTimings((uint)m_FrameTimings.Length, m_FrameTimings);
        if (ret > 0) { cpuUsage = GetCpuUsage(m_FrameTimings[0]); }

        // If the cpu usage isn't an impossible value, we log the data.
        if (cpuUsage >= 0)
        {
            Debug.LogFormat("Time: {0} FPS: {1} Average FPS: {2} CPU: {3}", Time.time, GetFPS(), avgFPS, cpuUsage);
            cpuDataLog.Add(cpuUsage);
            fpsDataLog.Add(avgFPS);
        }
    }

    //void SaveCPU(ref List<double> pCpuData)
    //{
    //    pCpuData = cpuDataLog;
    //}

    //void SaveFPS(ref List<float> pFpsData)
    //{
    //    pFpsData = fpsDataLog;
    //}

    string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".data";
        Debug.Log("Save file location: " + saveFile);
        return saveFile;
    }

    void Save()
    {
        _saveData.cpuData = cpuDataLog;
        _saveData.fpsData = fpsDataLog;
        Debug.LogFormat("Saving {0} cpu and {1} items.", _saveData.cpuData.Count, _saveData.fpsData.Count);
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData));
    }
}
