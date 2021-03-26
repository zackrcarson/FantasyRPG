using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour
{
    public static BattleReward instance;


    // Config Parameters
    [SerializeField] Text rewardsText = null;
    [SerializeField] GameObject rewardScreen = null;

    // State Variables
    string[] rewardItems = null;
    int[] numberOfRewards = null;
    int expEarned = 0;
    int goldEarned = 0;

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
                rewardsText.text += reward + "\n";
            }
            else
            {
                rewardsText.text += reward + " (x" + numberOfRewards[i] + ")\n";
            }

            i++;
        }

        rewardScreen.SetActive(true);
    }

    public void CloseRewardsScreen()
    {
        AddRewards();

        rewardScreen.SetActive(false);
        BattleManager.instance.isBattleActive = false;
        GameManager.instance.isBattleActive = false;
    }

    private void AddRewards()
    {
        foreach (CharacterStats playerStat in GameManager.instance.playerStatsArray)
        {
            if (playerStat.gameObject.activeInHierarchy)
            {
                playerStat.AddEXP(expEarned);
            }
        }

        foreach (string reward in rewardItems)
        {
            GameManager.instance.AddItem(reward);
        }

        GameManager.instance.AddGold(goldEarned);
    }
}
