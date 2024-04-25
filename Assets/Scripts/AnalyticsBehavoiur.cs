using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using Event = Unity.Services.Analytics.Event;

public class AnalyticsBehavoiur : MonoBehaviour
{
	async void Start()
	{
		await UnityServices.InitializeAsync();

		AskForConsent();
	}

	void AskForConsent()
	{
		// ... show the player a UI element that asks for consent.
		//ConsentGiven();
	}

	public void ConsentGiven()
	{
		print("## data collection consent granted");
		AnalyticsService.Instance.StartDataCollection();
	}
}

public class GameOverEvent : Event
{
    public GameOverEvent() : base("GameOver")
	{
	}

	public int currentLevelIndex { set { SetParameter("currentLevelIndex", value); } }
	public string lastPlayerPosition { set { SetParameter("lastPlayerPosition", value); } }
}

public class LevelFinishedEvent : Event
{
	public LevelFinishedEvent() : base("LevelFinished")
	{
	}

	public int currentLevelIndex { set { SetParameter("currentLevelIndex", value); } }
	public int score { set { SetParameter("score", value); } }
	public int maxMultiplier { set { SetParameter("maxMultiplier", value); } }
	public float totalTime { set { SetParameter("totalTime", value); } }
	public int enemyCount { set { SetParameter("enemyCount", value); } }
	public int damageTaken { set { SetParameter("damageTaken", value); } }
}

public class SettingsDialogEvent : Event
{
	public SettingsDialogEvent() : base("SettingsDialog")
	{
	}

	public int currentLevelIndex { set { SetParameter("currentLevelIndex", value); } }
	public int qualityLevelIndex { set { SetParameter("qualityLevelIndex", value); } }
	public string inputRebinds { set { SetParameter("inputRebinds", value); } }
}
