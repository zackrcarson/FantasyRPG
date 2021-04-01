using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;

    // Config Parameters
    [SerializeField] GameObject chestRewardPanel = null;
    [SerializeField] Text rewardsText = null;

    // State Variables
    Dictionary<string, List<string[]>> masterChestDictionary = null;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        masterChestDictionary = new Dictionary<string, List<string[]>>();

        NewScene();
    }

    public void ExternalNewScene()
    {
        StartCoroutine(DelayedNewScene());
    }

    private IEnumerator DelayedNewScene()
    {
        yield return new WaitForSeconds(.1f);

        NewScene();
    }

    private void NewScene()
    {
        string sceneName = SceneLoader.GetCurrentScene();
        GenerateGUID[] chestGUIDsInScene = FindObjectsOfType<GenerateGUID>();

        if (!masterChestDictionary.ContainsKey(sceneName))
        {
            List<string[]> chests = new List<string[]>();
            
            foreach (GenerateGUID chestGUID in chestGUIDsInScene)
            {
                chests.Add(new string[] { chestGUID.GUID, "false"});
            }

            if (chests.Count > 0)
            {
                masterChestDictionary.Add(sceneName, chests);
            }
        }
        else
        {
            List<string[]> chestsInDictionary = masterChestDictionary[sceneName];
            foreach (string[] chestGuidInDictionary in chestsInDictionary)
            {
                if (chestGuidInDictionary[1] == "true")
                {
                    foreach(GenerateGUID chestGUIDInScene in chestGUIDsInScene)
                    {
                        if (chestGUIDInScene.GUID == chestGuidInDictionary[0])
                        {
                            chestGUIDInScene.GetComponent<Chest>().OpenChestOnSceneLoad();
                        }
                    }
                }
            }
        }

        //DebugMasterChestDictionary();
    }

    private void DebugMasterChestDictionary()
    {
        foreach (KeyValuePair<string, List<string[]>> kvp in masterChestDictionary)
        {
            Debug.Log("Key = " + kvp.Key);
            foreach (string[] guid in kvp.Value)
            {
                Debug.Log("GUID = " + guid[0] + ", is open = " + guid[1]);
            }
        }
    }
    public void ChestOpened(string sceneName, string guid)
    {
        List<string[]> chestsInScene = masterChestDictionary[sceneName];

        foreach (string[] chest in chestsInScene)
        {
            if (chest[0] == guid)
            {
                chest[1] = "true";

                break;
            }
        }

        //DebugMasterChestDictionary();
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

    public void DeactivateChestRewardPanel()
    {
        chestRewardPanel.SetActive(false);
    }

    public void SaveChestDictionary()
    {
        foreach (string scene in masterChestDictionary.Keys)
        {
            if (masterChestDictionary[scene].Count > 0)
            {
                int i = 0;
                foreach (string[] chest in masterChestDictionary[scene])
                {
                    PlayerPrefs.SetString(scene + "_chest_" + i, chest[0] + "+" + chest[1]);

                    i++;
                }
            }
        }
    }

    public void LoadChestDictionary()
    {
        masterChestDictionary = new Dictionary<string, List<string[]>>();

        List<string> scenes = SceneLoader.GetAllScenes();

        foreach (string scene in scenes)
        {
            //Debug.Log("Searching the " + scene + " scene:");

            if (PlayerPrefs.GetString(scene + "_chest_" + 0) != "")
            {
                List<string[]> chestsInScene = new List<string[]>();

                int i = 0;
                while (true)
                {
                    string[] guid = PlayerPrefs.GetString(scene + "_chest_" + i).Split('+');

                    if (guid[0] != "")
                    {
                        //Debug.Log("Found a chest at " + i + ", with guid = " + guid[0] + ", and isOpen = " + guid[1]);
                        chestsInScene.Add(guid);

                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                masterChestDictionary.Add(scene, chestsInScene);
            }
        }
        //Debug.Log("Done Searching! Printing out Master Chest Dictionary:");
        //DebugMasterChestDictionary();

        NewScene();
    }
}