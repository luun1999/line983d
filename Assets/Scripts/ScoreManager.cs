using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore;
    // Start is called before the first frame update
    void Start()
    {
        score.text = "Score: 0000";
        highScore.text = "High score: " + PlayerPrefs.GetInt("HighScore", 0).ToString("0000");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int score)
    {
        this.score.text = "Score: " + score.ToString("0000");
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            this.highScore.text = "High score: " + score.ToString("0000");
        }
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteAll();
        this.highScore.text = "High score: 0000";
    }
}
