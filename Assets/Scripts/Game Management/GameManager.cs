using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Config Parameters
    [SerializeField] public CharacterStats[] playerStatsArray = null;
    [SerializeField] Item[] referenceItems = null;
    [SerializeField] public string[] itemsHeld = null;
    [SerializeField] public int[] numberOfItems = null;
    [SerializeField] public int currentGold = 100;

    // Cached References
    PlayerController player = null;

    // State Variables
    [HideInInspector] public bool gameMenuOpen = false;
    [HideInInspector] public bool dialogueActive = false;
    [HideInInspector] public bool fadingScreen = false;
    [HideInInspector] public bool shopActive = false;
    [HideInInspector] public bool isBattleActive = false;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        SortItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) { player = FindObjectOfType<PlayerController>(); }

        if (gameMenuOpen || dialogueActive || fadingScreen || shopActive || isBattleActive)
        {
            player.CanMove(false);
        }
        else
        {
            player.CanMove(true);
        }
    }

    public Item GetItemDetails(string itemToGrab)
    {
        foreach (Item item in referenceItems)
        {
            if (item.itemName == itemToGrab)
            {
                return item;
            }
        }

        return null;
    }

    public void SortItems()
    {
        bool itemAfterSpace = true;

        while (itemAfterSpace)
        {
            itemAfterSpace = false;

            for (int i = 0; i < itemsHeld.Length - 1; i++)
            {
                if (itemsHeld[i] == "" || numberOfItems[i] == 0)
                {
                    itemsHeld[i] = itemsHeld[i + 1];
                    itemsHeld[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1];
                    numberOfItems[i + 1] = 0;

                    if (itemsHeld[i] != "")
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddGold(int gold)
    {
        currentGold += gold;
    }

    public void AddItem(string itemToAdd)
    {
        int newItemPosition = 0;
        bool foundSpaceForItem = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd)
            {
                newItemPosition = i;

                foundSpaceForItem = true;

                break;
            }
        }

        if (foundSpaceForItem)
        {
            bool itemExists = false;

            foreach (Item item in referenceItems)
            {
                if (item.itemName == itemToAdd)
                {
                    itemExists = true;

                    break;
                }
            }

            if (itemExists)
            {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;

                GameMenu.instance.AddItem();
            }
            else
            {
                Debug.LogError("Item " + itemToAdd + " does not exist!");
            }
        }

        GameMenu.instance.ShowItems();
    }

    public int FindSelectedItemAmount(string itemName)
    {
        int itemPosition = 0;
        bool foundItem = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == itemName)
            {
                itemPosition = i;

                foundItem = true;

                break;
            }
        }

        if (foundItem)
        {
            return numberOfItems[itemPosition];
        }
        else
        {
            return -1;
        }
    }

    public void RemoveItem(string itemToRemove)
    {
        int itemPosition = 0;
        bool foundItem = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == itemToRemove)
            {
                itemPosition = i;

                foundItem = true;

                break;
            }
        }

        if (foundItem)
        {
            numberOfItems[itemPosition]--;

            if (numberOfItems[itemPosition] <= 0)
            {
                itemsHeld[itemPosition] = "";
                numberOfItems[itemPosition] = 0;

                GameMenu.instance.ShowItems();
                GameMenu.instance.SelectFirstItem();
            }
            else
            {
                GameMenu.instance.ShowItems();
            }            
        }
        else
        {
            Debug.LogError("Couldn't find " + itemToRemove);
        }
    }

    public void SaveData()
    {
        if (!player) { player = FindObjectOfType<PlayerController>(); }

        string sceneName = SceneManager.GetActiveScene().name;
        Vector3 playerPosition = player.transform.position;
        Vector2 playerIdleDirection = player.GetIdleDirection();

        // Save the players location
        PlayerPrefs.SetString("Current_Scene", sceneName);
        PlayerPrefs.SetFloat("Player_Position_x", playerPosition.x);
        PlayerPrefs.SetFloat("Player_Position_y", playerPosition.y);
        PlayerPrefs.SetFloat("Player_Position_z", playerPosition.z);

        // Save the players idle direction
        PlayerPrefs.SetFloat("Player_Idle_Direction_x", playerIdleDirection.x);
        PlayerPrefs.SetFloat("Player_Idle_Direction_y", playerIdleDirection.y);

        // Save character stats for each character, as well as if they are active or not
        foreach (CharacterStats characterStat in playerStatsArray)
        {
            // 1 means active, 0 means not
            if (characterStat.gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_active", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_active", 0);
            }

            // Character Stats
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_Level", characterStat.playerLevel);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_CurrentExp", characterStat.currentEXP);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_CurrentHP", characterStat.currentHP);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_CurrentMP", characterStat.currentMP);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_MaxHP", characterStat.maxHP);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_MaxMP", characterStat.maxMP);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_Strength", characterStat.strength);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_Defense", characterStat.defense);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_WeaponPower", characterStat.weaponPower);
            PlayerPrefs.SetInt("Player_" + characterStat.characterName + "_ArmorPower", characterStat.armorPower);
            PlayerPrefs.SetString("Player_" + characterStat.characterName + "_EquippedWeapon", characterStat.equippedWeapon);
            PlayerPrefs.SetString("Player_" + characterStat.characterName + "_EquippetArmor", characterStat.equippedArmor);
        }

        // Inventory Data
        int i = 0;
        foreach (string item in itemsHeld)
        {
            PlayerPrefs.SetString("ItemInInventorySlot_" + i, item);
            PlayerPrefs.SetInt("ItemAmountInInventorySlot_" + i, numberOfItems[i]);

            i++;
        }

        PlayerPrefs.SetInt("PlayerGold", currentGold);
    }

    public void LoadData()
    {
        if (!player) { player = FindObjectOfType<PlayerController>(); }

        // Load the Players scene, position, and idle direction
        string savedScene = PlayerPrefs.GetString("Current_Scene");

        float savedPlayerPositionX = PlayerPrefs.GetFloat("Player_Position_x");
        float savedPlayerPositionY = PlayerPrefs.GetFloat("Player_Position_y");
        float savedPlayerPositionZ = PlayerPrefs.GetFloat("Player_Position_z");
        Vector3 savedPlayerPosition = new Vector3(savedPlayerPositionX, savedPlayerPositionY, savedPlayerPositionZ);
        
        float savedPlayerIdleDirectionX = PlayerPrefs.GetFloat("Player_Idle_Direction_x");
        float savedPlayerIdleDirectionY = PlayerPrefs.GetFloat("Player_Idle_Direction_y");
        Vector2 savedPlayerIdleDirection = new Vector3(savedPlayerIdleDirectionX, savedPlayerIdleDirectionY);

        // Set the Players scene, position, and idle direction
        SceneLoader.LoadSceneByName(savedScene);
        player.transform.position = savedPlayerPosition;
        player.SetIdleDirection(savedPlayerIdleDirection);

        // Load character stats for each character, if they are active
        foreach (CharacterStats characterStat in playerStatsArray)
        {
            int characterActive = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_active");

            if (characterActive == 0)
            {
                characterStat.gameObject.SetActive(false);
            }
            else
            {
                characterStat.gameObject.SetActive(true);
            }

            // Load Character Stats
            characterStat.playerLevel = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_Level");
            characterStat.currentEXP = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_CurrentExp");
            characterStat.currentHP = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_CurrentHP");
            characterStat.currentMP = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_CurrentMP");
            characterStat.maxHP = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_MaxHP");
            characterStat.maxMP = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_MaxMP");
            characterStat.strength = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_Strength");
            characterStat.defense = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_Defense");
            characterStat.weaponPower = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_WeaponPower");
            characterStat.armorPower = PlayerPrefs.GetInt("Player_" + characterStat.characterName + "_ArmorPower");
            characterStat.equippedWeapon = PlayerPrefs.GetString("Player_" + characterStat.characterName + "_EquippedWeapon");
            characterStat.equippedArmor = PlayerPrefs.GetString("Player_" + characterStat.characterName + "_EquippetArmor");
        }

        // Inventory Data
        int i = 0;
        foreach (string item in itemsHeld)
        {
            itemsHeld[i] = PlayerPrefs.GetString("ItemInInventorySlot_" + i);
            numberOfItems[i] = PlayerPrefs.GetInt("ItemAmountInInventorySlot_" + i);

            i++;
        }

        currentGold = PlayerPrefs.GetInt("PlayerGold", currentGold);
    }
}
