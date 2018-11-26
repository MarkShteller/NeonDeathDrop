using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public Slider manaSlider;
    public Slider healthSlider;
    public Text scoreText;
    public Text scoreMultiplierText;

    public GameOverDialog gameOverDialog;

	void Awake () {
        Instance = this;
	}

    public void SetScore(int value)
    {
        scoreText.text = value.ToString("N0");
    }

    public void SetMana(float value)
    {
        manaSlider.value = value;
    }

    public void SetHealth(float value)
    {
        healthSlider.value = value;
    }

    public void ShowGameOverScreen(int score)
    {
        GameOverDialog dialog = Instantiate(gameOverDialog, this.transform);
        dialog.Init(score);
    }

    public void SetScoreMultiplier(float scoreMultiplier)
    {
        scoreMultiplierText.text = "x"+scoreMultiplier;
    }
}
