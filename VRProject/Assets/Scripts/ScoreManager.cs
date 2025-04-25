using Newtonsoft.Json;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int Score { get; private set; }
    public bool timerStarted = false;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float startTime = 60f;

    private float timer;
    private int ammoShot = 0;
    private TestResult result;

    void Start()
    {
        timer = startTime;
        instance = this;
        scoreText.text = "Score : " + Score;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Update()
    {
        if (timerStarted) 
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0f);

            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timer <= 0f)
            {
                OnTimerEnd();
            }
        }
    }


    public void AddScore(int score)
    {
        Score += score;
        scoreText.text = "Score : " + Score;
    }

    public void StartTimer()
    {
        if (!timerStarted)
        {
            timerStarted = true;                    
        }
    }

    void OnTimerEnd()
    {
        timerStarted = false;
        WriteToJSONIfFinished();
        SceneTransitionManager.singleton.GoToScene(2);
    }

    public void IncreaseAmmo()
    {
        ammoShot++;
    }

    public void WriteToJSONIfFinished()
    {
        TimeSpan duration = TimeSpan.FromSeconds((double)(new decimal(startTime - timer)));

        string formattedDuration = string.Format("{0:D2}:{1:D2}.{2:D2}",
            (int)duration.TotalMinutes,
            duration.Seconds,
            duration.Milliseconds / 10);

        GameResultData.Result = new Result
        {
            Score = this.Score,
            AmmunitionShot = ammoShot,
            DurationFormatted = formattedDuration
        };

        string path = Path.Combine(Application.persistentDataPath, "test_result.json");

        List<Result> allResults = new List<Result>();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            try
            {
                allResults = JsonConvert.DeserializeObject<List<Result>>(existingJson) ?? new List<Result>();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Couldn't parse existing JSON, starting fresh: " + e.Message);
            }
        }

        allResults.Add(GameResultData.Result);

        string updatedJson = JsonConvert.SerializeObject(allResults, Formatting.Indented);  // Using Formatting.Indented for pretty print
        File.WriteAllText(path, updatedJson);

        Debug.Log("Appended test result to JSON file: " + path);
        
    }


}
