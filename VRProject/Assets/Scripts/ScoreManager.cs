using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int Score { get; private set; }

    [SerializeField] private TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        scoreText.text = "Score : " + Score;
    }

    public void AddScore(int score)
    {
        Score += score;
        scoreText.text = "Score : " + Score;
    }


}
