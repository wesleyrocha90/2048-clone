using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    private int score;
    public static ScoreTracker Instance;
    public Text ScoreText;
    public Text HighscoreText;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            ScoreText.text = score.ToString();

            if (PlayerPrefs.GetInt("Highscore") < score)
            {
                PlayerPrefs.SetInt("Highscore", score);
                HighscoreText.text = score.ToString();
            }
        }
    }

    void Awake()
    {
        // PlayerPrefs.DeleteAll();
        Instance = this;

        if (!PlayerPrefs.HasKey("Highscore"))
            PlayerPrefs.SetInt("Highscore", 0);

        ScoreText.text = "0";
        HighscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
    }
}
