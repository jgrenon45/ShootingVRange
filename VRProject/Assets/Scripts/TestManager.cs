using UnityEngine;
using System;
using System.Timers;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class TestManager
{
    private int score = 0;
    private bool timerStarted = false;
    private int ammunitionShot = 0;
    private static Timer timer;
    private DateTime startTime;

    private static TestManager _instance;

    private TestManager() { }

    public static TestManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new TestManager();
        }
        return _instance;
    }

    public void StartTimer()
    {
        if (!timerStarted)
        {
            timerStarted = true;

            startTime = DateTime.Now;
            timer = new Timer(10); // fires every 0.01 second
            timer.Elapsed += OnTimedEvent; // This will call OnTimedEvent whenever the timer elapses
            timer.AutoReset = true;
            timer.Enabled = true;

            Debug.Log("Timer started.");
        }
    }

    // Define the OnTimedEvent method to handle the timer's Elapsed event
    private void OnTimedEvent(object sender, ElapsedEventArgs e) {}

    public void IncreaseScore(int value)
    {
        score += value;
        Debug.Log($"Score updated: {score}");

        WriteToJSONIfFinished();
    }

    public void increaseAmmo()
    {
        ammunitionShot++;
    }

    public void WriteToJSONIfFinished()
    {
        if (score >= 500)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;

            string formattedDuration = string.Format("{0:D2}:{1:D2}.{2:D2}",
                (int)duration.TotalMinutes,
                duration.Seconds,
                duration.Milliseconds / 10);

            var newResult = new TestResult
            {
                Score = score,
                AmmunitionShot = ammunitionShot,
                DurationFormatted = formattedDuration
            };

            string path = Path.Combine(Application.persistentDataPath, "test_result.json");

            List<TestResult> allResults = new List<TestResult>();

            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                try
                {
                    allResults = JsonConvert.DeserializeObject<List<TestResult>>(existingJson) ?? new List<TestResult>();
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Couldn't parse existing JSON, starting fresh: " + e.Message);
                }
            }

            allResults.Add(newResult);

            string updatedJson = JsonConvert.SerializeObject(allResults, Formatting.Indented);  // Using Formatting.Indented for pretty print
            File.WriteAllText(path, updatedJson);

            Debug.Log("Appended test result to JSON file: " + path);
        }
    }
}