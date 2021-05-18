using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour
{
    public static BattleReward instance;

    // Config Parameters
    [SerializeField] Text rewardsText = null;
    [SerializeField] GameObject rewardScreen = null;
    [SerializeField] int rewardSound = 37;

    // State Variables
    string[] rewardItems = null;
    int[] numberOfRewards = null;
    int expEarned = 0;
    int goldEarned = 0;
    [HideInInspector] public bool markQuestComplete = false;
    [HideInInspector] public string questToComplete = null;

    [HideInInspector] public bool shouldAddQuest = false;
    [HideInInspector] public string questToAdd = "";

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenRewardScreen(int exp, int gold, string[] rewards, int[] numbers)
    {
        expEarned = exp;
        goldEarned = gold;
        rewardItems = rewards;
        numberOfRewards = numbers;

        rewardsText.text = "Everyone earned " + expEarned + " EXP!\n" + goldEarned + "g\n";

        int i = 0;
        foreach (string reward in rewardItems)
        {
            if (numberOfRewards[i] <= 1)
            {
                rewardsText.text += reward;
            }
            else
            {
                rewardsText.text += reward + " (x" + numberOfRewards[i] + ")";
            }

            if (i < rewardItems.Length - 1)
            {
                rewardsText.text += "\n";
            }

            i++;
        }

        AudioManager.instance.PlaySFX(rewardSound);
        rewardScreen.SetActive(true);
    }

    public void CloseRewardsScreen()
    {
        rewardScreen.SetActive(false);
        BattleManager.instance.isBattleActive = false;
        GameManager.instance.isBattleActive = false;
        
        AddRewards();
    }

    private void AddRewards()
    {
        int i = 0;
        foreach (string reward in rewardItems)
        {
            for (int j = 0; j < numberOfRewards[i]; j++)
            {
                GameManager.instance.AddItem(reward);
            }
                
            i++;
        }

        GameManager.instance.AddGold(goldEarned);

        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }

        if (shouldAddQuest)
        {
            QuestManager.instance.AddQuest(questToAdd);
        }

        List<LevelUpInfo> levelUpInfos = new List<LevelUpInfo> { };
        foreach (CharacterStats playerStat in GameManager.instance.playerStatsArray)
        {
            if (playerStat.gameObject.activeInHierarchy)
            {
                LevelUpInfo levelUpInfo = playerStat.AddEXP(expEarned);

                if (levelUpInfo != null)
                {
                    levelUpInfos.Add(levelUpInfo);
                }
            }
        }

        if (levelUpInfos.Count > 0)
        {
            LevelUp.instance.OpenLevelUpScreen(levelUpInfos);
        }
        else
        {
            GameMenu.instance.ActivateIcons();
        }
    }
}
