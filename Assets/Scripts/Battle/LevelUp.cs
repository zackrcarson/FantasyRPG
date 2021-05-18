using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    public static LevelUp instance;

    // Config Parameters
    [SerializeField] GameObject levelUpScreen = null;
    [SerializeField] Text closeButtonText = null;
    [SerializeField] Text nameText = null;
    [SerializeField] Text[] oldStatsTexts = null;
    [SerializeField] Text[] newStatsTexts = null;
    [SerializeField] Text expText = null;
    [SerializeField] int levelUpSound = 38;

    // State Variables
    [HideInInspector] public bool isShowingRewards = false;
    bool isLastLevelUp = true;
    int currentLevelUp = 0;
    List<LevelUpInfo> currentLevelUpInfos = null;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Old/New levels appear in the order: LVL, HP, MP, Strength, Defense
    /// </summary>
    public void OpenLevelUpScreen(List<LevelUpInfo> levelUpInfos)
    {
        if (levelUpInfos.Count <= 0) { return; }

        if (levelUpInfos.Count == 1)
        {
            isLastLevelUp = true;
            currentLevelUp = 0;

            closeButtonText.text = "Close";
        }
        else
        {
            isLastLevelUp = false;
            currentLevelUp = 0;

            closeButtonText.text = "Next";
        }

        currentLevelUpInfos = levelUpInfos;

        ShowCurrentLevelUp();
    }

    private void ShowCurrentLevelUp()
    {
        int[] oldLevels = currentLevelUpInfos[currentLevelUp].oldLevels;
        int[] newLevels = currentLevelUpInfos[currentLevelUp].newLevels;
        string name = currentLevelUpInfos[currentLevelUp].characterName;
        int expToNextLevel = currentLevelUpInfos[currentLevelUp].expToNextLevel;

        FindObjectOfType<PlayerController>().CanMove(false);
        isShowingRewards = true;

        int i = 0;
        foreach (Text oldStat in oldStatsTexts)
        {
            oldStat.text = oldLevels[i].ToString();
            newStatsTexts[i].text = newLevels[i].ToString();

            i++;
        }

        nameText.text = name + " Leveled Up!";
        expText.text = expToNextLevel.ToString();

        AudioManager.instance.PlaySFX(levelUpSound);
        levelUpScreen.SetActive(true);
    }

    public void CloseLevelUpScreen()
    {
        if (isLastLevelUp)
        {
            currentLevelUp = 0;

            isShowingRewards = false;
            FindObjectOfType<PlayerController>().CanMove(true);

            levelUpScreen.SetActive(false);
            GameMenu.instance.ActivateIcons();
        }
        else
        {
            levelUpScreen.SetActive(false);

            currentLevelUp++;

            if (currentLevelUp == currentLevelUpInfos.Count - 1)
            {
                isLastLevelUp = true;
                ShowCurrentLevelUp();

                closeButtonText.text = "Close";
            }
            else
            {
                ShowCurrentLevelUp();
            }
        }
    }
}
