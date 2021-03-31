using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;

    // Config Parameters
    [SerializeField] GameObject chestRewardPanel = null;
    [SerializeField] Text rewardsText = null;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator OpenChest(string[] treasures, int[] treasureNumbers, int coins, float screenTime)
    {
        GameManager.instance.AddGold(coins);
        rewardsText.text = coins.ToString() + "g\n";

        int i = 0;
        foreach (string treasure in treasures)
        {
            for (int j = 0; j < treasureNumbers[i]; j++)
            {
                GameManager.instance.AddItem(treasure);
            }

            if (treasureNumbers[i] == 1)
            {
                rewardsText.text += treasure;
            }
            else
            {
                rewardsText.text += treasure + " (x" + treasureNumbers[i] + ")";
            }

            if (i < treasures.Length - 1)
            {
                rewardsText.text += "\n";
            }

            i++;
        }

        chestRewardPanel.SetActive(true);

        yield return new WaitForSeconds(screenTime);

        chestRewardPanel.SetActive(false);
    }
}
