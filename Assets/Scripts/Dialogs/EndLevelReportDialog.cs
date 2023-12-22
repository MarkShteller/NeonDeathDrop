using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelReportDialog : MonoBehaviour
{
    public Text levelNameText;
    public Text scoreText;
    public Text maxMultiplierText;
    public Text timeText;
    public Text enemyCountText;
    public Text damageText;
    public Text scoreGradeText;
    public Text maxMultiplierGradeText;
    public Text timeGradeText;
    public Text enemyCountGradeText;
    public Text damageGradeText;

    public Text gradeText;

    private LevelScriptableObject levelData;

    private readonly char[] RANKS = { 'D', 'C', 'B', 'A', 'S' };
    private readonly int RANKS_COUNT = 5;

    // Start is called before the first frame update
    public void Init(int score, float maxMultiplier, float time, int enemyCount, float damage, LevelScriptableObject levelData)
    {
        scoreText.text = score.ToString("N0");
        maxMultiplierText.text = "x" + maxMultiplier.ToString("N0");
        timeText.text = string.Format("{0}:{1:00}", (int)time / 60, (int)time % 60);
        enemyCountText.text = enemyCount.ToString("N0");
        damageText.text = damage.ToString("N0");
        this.levelData = levelData;

        levelNameText.text = this.levelData.name + " complete!";

        char grade = CalcGrade(score, maxMultiplier, time, enemyCount, damage);
        gradeText.text = grade.ToString();

        GameManager.Instance.PlayerInstance.interactionEvent += PressNextAction;

    }

    private void PressNextAction()
    {
        print("pressing continue end level report");
        GameManager.Instance.PlayerInstance.interactionEvent -= PressNextAction;
        GameManager.Instance.PlayerInstance.playerInput.SwitchCurrentActionMap("Player");

        GameManager.Instance.NextLevel();
    }

    private char CalcGrade(int score, float maxMultiplier, float time, int enemyCount, float damage)
    {
        float scoreGrade=0, multGrade=0, timeGrade=4, enemyCountGrade=0, damageGrade=4, totalGrade;
        int rankIndex;

        for (int i = RANKS_COUNT; i > 0; i--)
        {
            if (time > levelData.targetTime + levelData.targetTime * (levelData.targetTimeRankMod * (i-1)))
            {
                timeGrade = RANKS_COUNT - i;
                break;
            }
        }
        rankIndex = Mathf.RoundToInt(timeGrade);
        timeGradeText.text = RANKS[rankIndex].ToString();


        for (int i = RANKS_COUNT; i > 0; i--)
        {
            if (damage > levelData.targetDamage * levelData.targetDamageRankMod * (i-1))
            {
                damageGrade = RANKS_COUNT - i;
                break;
            }
        }
        rankIndex = Mathf.RoundToInt(damageGrade);
        damageGradeText.text = RANKS[rankIndex].ToString();


        for (int i = 0; i < RANKS_COUNT; i++)
        {
            if (score >= levelData.targetScore - levelData.targetScore * levelData.targetScoreRankMod * i)
            {
                scoreGrade = RANKS_COUNT - i;
                break;
            }
        }
        rankIndex = Mathf.RoundToInt(scoreGrade);
        scoreGradeText.text = RANKS[rankIndex > 0 ? rankIndex - 1 : 0].ToString();

        for (int i = 0; i < RANKS_COUNT; i++)
        {
            if (maxMultiplier >= levelData.targetMaxMultiplier - levelData.targetMaxMultiplier * levelData.targetMaxMultiplierRankMod * i)
            {
                multGrade = RANKS_COUNT - i;
                break;
            }
        }
        rankIndex = Mathf.RoundToInt(multGrade);
        maxMultiplierGradeText.text = RANKS[rankIndex > 0 ? rankIndex - 1 : 0].ToString();

        for (int i = 0; i < RANKS_COUNT; i++)
        {
            if (enemyCount >= levelData.targetEnemyCount - levelData.targetEnemyCount * levelData.targetEnemyCountRankMod * i)
            {
                enemyCountGrade = RANKS_COUNT - i;
                break;
            }
        }
        rankIndex = Mathf.RoundToInt(enemyCountGrade);
        enemyCountGradeText.text = RANKS[rankIndex > 0 ? rankIndex - 1 : 0].ToString();

        totalGrade = scoreGrade + multGrade + timeGrade + enemyCountGrade + damageGrade;
        totalGrade /= 5;
        print("totalGrade: " + totalGrade);

        rankIndex = Mathf.CeilToInt(totalGrade);

        //clamp result between 0-4
        return RANKS[rankIndex > 0 ? rankIndex - 1 : 0];
    }

}
