using TMPro;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text shotsText;

    private void Start()
    {
        Result gameResult = GameResultData.Result;
        scoreText.text = gameResult.Score.ToString();
        timeText.text = gameResult.DurationFormatted;
        shotsText.text = gameResult.AmmunitionShot.ToString();
    }

    public void Retry()
    {
        // Load the main menu scene
        SceneTransitionManager.singleton.GoToScene(1);
    }

    public void MainMenu()
    {
        // Load the main menu scene
        SceneTransitionManager.singleton.GoToScene(0);
    }
}
