using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static GameMenu instance;

    // Config Parameters
    [Header("Configs")]
    [SerializeField] public GameObject menu = null;
    [SerializeField] GameObject[] characterStatHolders = null;
    [SerializeField] GameObject[] menuWindows = null;
    [SerializeField] ButtonToggle[] menuButtons = null;
    [SerializeField] string mainMenuName = "Main Menu";
    [SerializeField] GameObject loadButton = null;
    [SerializeField] Color deactiveButtonColor;

    [Header("SFX")]
    [SerializeField] int openCloseMenuSound = 6;
    [SerializeField] int menuButtonsSound = 5;
    [SerializeField] int itemSlotSound = 8;

    [Header("Items Panel Config")]
    [SerializeField] ItemButton[] itemButtons = null;
    ButtonToggle[] itemButtonToggles = null;
    [SerializeField] Text itemName = null;
    [SerializeField] Text itemDescription = null;
    [SerializeField] Text useButtonText = null;
    [SerializeField] GameObject useButton = null;
    [SerializeField] GameObject discardButton = null;
    [SerializeField] Text goldText = null;

    [Header("Items Character Select Panel Config")]
    [SerializeField] GameObject itemCharacterSelectionMenu = null;
    [SerializeField] Text[] itemCharacterNames = null;

    [Header("Character Quick Stat Arrays")]
    [SerializeField] Text[] nameTexts = null;
    [SerializeField] Text[] HPTexts = null;
    [SerializeField] Text[] MPTexts = null;
    [SerializeField] Text[] LVLTexts = null;
    [SerializeField] Text[] EXPTexts = null;
    [SerializeField] Slider[] EXPSliders = null;
    [SerializeField] Image[] characterImages = null;

    [Header("Player Stats Arrays")]
    [SerializeField] GameObject[] statsButtons = null;
    [SerializeField] GameObject unequipWeaponButton = null;
    [SerializeField] GameObject unequipArmorButton = null;
    [SerializeField] ButtonToggle[] statsButtonToggles = null;
    [SerializeField] Image statsImage = null;
    [SerializeField] Text statsName = null;
    [SerializeField] Text statsHP = null;
    [SerializeField] Text statsMP = null;
    [SerializeField] Text statsStrength = null;
    [SerializeField] Text statsDefense = null;
    [SerializeField] Text statsWeapon = null;
    [SerializeField] Text statsWeaponPower = null;
    [SerializeField] Text statsArmor = null;
    [SerializeField] Text statsArmorPower = null;
    [SerializeField] Text statsEXP = null;
    [SerializeField] Text lvlText = null;

    [Header("Quests")]
    [SerializeField] Text[] questTexts = null;
    [SerializeField] Color completedQuestTextColor;
    Color defaultQuestTextColor;

    [Header("Notifications")]
    [SerializeField] Text notificationText = null;
    [SerializeField] float saveLoadNotificationActiveTime = 3f;
    [SerializeField] Color saveLoadNotificationColor;
    [SerializeField] float itemMessageTime = 5f;
    [SerializeField] Color itemMessageColor;

    // State Variables
    // string selectedItem = null;
    Item activeItem = null;
    bool isInventoryEmpty = false;
    int activePlayer = 0;

    // Cached References
    CharacterStats[] characterStats = null;
    Color defaultButtonColor;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        defaultQuestTextColor = questTexts[0].color;

        itemButtonToggles = new ButtonToggle[itemButtons.Length];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtonToggles[i] = itemButtons[i].gameObject.GetComponent<ButtonToggle>();
        }

        Image buttonImage = loadButton.GetComponent<Image>();
        defaultButtonColor = buttonImage.color;

        CheckLoadButton();
    }

    private void CheckLoadButton()
    {
        // Gray out Continue button if no save data found
        if (!PlayerPrefs.HasKey("Current_Scene"))
        {
            Button button = loadButton.GetComponent<Button>();
            Image buttonImage = loadButton.GetComponent<Image>();
            Text buttonText = loadButton.GetComponentInChildren<Text>();

            button.enabled = false;
            buttonImage.color = deactiveButtonColor;
            buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, deactiveButtonColor.a);
        }
        else
        {
            Button button = loadButton.GetComponent<Button>();
            Image buttonImage = loadButton.GetComponent<Image>();
            Text buttonText = loadButton.GetComponentInChildren<Text>();

            button.enabled = true;
            buttonImage.color = defaultButtonColor;
            buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPauseButton();
    }

    private void CheckPauseButton()
    {
        if (LevelUp.instance.isShowingRewards || DialogueManager.instance.isTalking() || Shop.instance.IsShopping() || BattleManager.instance.isBattleActive) { return; }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        {
            if (menu.activeInHierarchy)
            {
                CloseMenu();
            }
            else
            {
                menu.SetActive(true);
                CheckLoadButton();
                GameManager.instance.gameMenuOpen = true;

                UpdateMainStats();
                ShowItems();
            }

            AudioManager.instance.PlaySFX(openCloseMenuSound);
        }
    }

    private void UpdateMainStats()
    {
        characterStats = GameManager.instance.playerStatsArray;

        int i = 0;
        foreach (CharacterStats characterStat in characterStats)
        {
            // Set active/deactive players if they are active/inactive in the game manager
            characterStatHolders[i].SetActive(characterStat.gameObject.activeInHierarchy);

            // If the character card was deactivated to start, the EXPs per level wont be set up yet
            if (characterStat.expToNextLevel.Length == 0)
            {
                characterStat.SetUpLevelEXPs();
            }

            // Update all of the stats in the given stat card
            UpdateStats(i, characterStat);

            i++;
        }

        goldText.text = GameManager.instance.currentGold.ToString() + "g";
    }

    private void UpdateStats(int characterNumber, CharacterStats characterStat)
    {
        nameTexts[characterNumber].text = characterStat.characterName;
        HPTexts[characterNumber].text = "HP: " + characterStat.currentHP + "/" + characterStat.maxHP;
        MPTexts[characterNumber].text = "MP: " + characterStat.currentMP + "/" + characterStat.maxMP;
        LVLTexts[characterNumber].text = "Lvl: " + characterStat.playerLevel;
        EXPTexts[characterNumber].text = "" + characterStat.currentEXP + "/" + characterStat.expToNextLevel[characterStat.playerLevel];
        EXPSliders[characterNumber].maxValue = characterStat.expToNextLevel[characterStat.playerLevel];
        EXPSliders[characterNumber].value = characterStat.currentEXP;
        characterImages[characterNumber].sprite = characterStat.characterImage;
    }

    public void ToggleWindow(int windowNumber)
    {
        UpdateMainStats();

        int i = 0;
        foreach (GameObject window in menuWindows)
        {
            if (i == windowNumber)
            {
                menuButtons[i].ToggleButton(!window.activeInHierarchy);

                window.SetActive(!window.activeInHierarchy);
            }
            else
            {
                window.SetActive(false);
                menuButtons[i].ToggleButton(false);
            }

            i++;
        }

        itemCharacterSelectionMenu.SetActive(false);
    }

    public void CloseMenu()
    {
        // Close all the windows
        int i = 0;
        foreach (GameObject window in menuWindows)
        {
            window.SetActive(false);
            menuButtons[i].ToggleButton(false);

            i++;
        }

        // Close the menu
        menu.SetActive(false);
        itemCharacterSelectionMenu.SetActive(false);

        // Activate walking again
        GameManager.instance.gameMenuOpen = false;
    }

    public void QuitGame()
    {
        StartCoroutine(FadeAndQuit());
    }

    private IEnumerator FadeAndQuit()
    {
        UIFade.instance.CallFadeOut();
        AudioManager.instance.StopMusic();

        yield return new WaitForSeconds(2.5f);
        UIFade.instance.QuitGame();

        Destroy(GameManager.instance.gameObject);
        Destroy(FindObjectOfType<PlayerController>().gameObject);
        Destroy(AudioManager.instance.gameObject);

        SceneLoader.LoadSceneByName(mainMenuName);

        yield return null;

        Destroy(gameObject);
    }

    public void QuitAfterFade()
    {
        SceneLoader.LoadSceneByName(mainMenuName);
    }

    public void OpenStatus()
    {
        // Update any new stats (i.e. drank potion from inventory)
        UpdateMainStats();

        // Update the information shown for P1
        ShowPlayerStats(0);

        int i = 0;
        foreach (GameObject statsButton in statsButtons)
        {
            // Deactivate the players that aren't available
            statsButton.SetActive(characterStats[i].gameObject.activeInHierarchy);

            statsButton.GetComponentInChildren<Text>().text = characterStats[i].characterName;

            i++;
        }
    }

    public void OpenQuests()
    {
        List<string> quests = QuestManager.instance.questMarkerNames;
        List<bool> questsComplete = QuestManager.instance.questMarkersComplete;

        int n = 1;
        if (quests.Count > questTexts.Length)
        {
            n = (quests.Count - questTexts.Length );
        }

        bool questFound = false;

        int i = 0;
        foreach (Text questText in questTexts)
        {
            if (quests.Count > i + n && quests[i + n] != "")
            {
                questFound = true;

                questText.text = "- " + quests[i + n];

                if (!questsComplete[i + n])
                {
                    questText.color = defaultQuestTextColor;
                }
                else
                {
                    questText.text += " - Completed!";
                    questText.color = completedQuestTextColor; ;
                }
            }
            else
            {
                questText.text = "";
                questText.color = defaultQuestTextColor;
            }

            i++;
        }

        if (!questFound)
        {
            questTexts[0].text = "No quests found! Go find some.";
            questTexts[0].color = defaultQuestTextColor;
        }
    }

    public void ShowPlayerStats(int playerNumber)
    {
        if (characterStats[playerNumber].equippedWeapon == "")
        {
            unequipWeaponButton.GetComponent<Image>().color = deactiveButtonColor;

            Color textColor = unequipWeaponButton.GetComponentInChildren<Text>().color;
            textColor = new Color(textColor.r, textColor.g, textColor.b, deactiveButtonColor.a);
        }
        else
        {
            unequipWeaponButton.GetComponent<Image>().color = defaultButtonColor;

            Color textColor = unequipWeaponButton.GetComponentInChildren<Text>().color;
            textColor = new Color(textColor.r, textColor.g, textColor.b, 1f);
        }

        if (characterStats[playerNumber].equippedArmor == "")
        {
            unequipArmorButton.GetComponent<Image>().color = deactiveButtonColor;

            Color textColor = unequipArmorButton.GetComponentInChildren<Text>().color;
            textColor = new Color(textColor.r, textColor.g, textColor.b, deactiveButtonColor.a);
        }
        else
        {
            unequipArmorButton.GetComponent<Image>().color = defaultButtonColor;

            Color textColor = unequipArmorButton.GetComponentInChildren<Text>().color;
            textColor = new Color(textColor.r, textColor.g, textColor.b, 1f);
        }

        activePlayer = playerNumber;

        int i = 0;
        foreach (ButtonToggle button in statsButtonToggles)
        {
            if (characterStats[i].gameObject.activeInHierarchy)
            {
                button.ToggleButton(i == playerNumber);
            }

            i++;
        }

        statsImage.sprite = characterStats[playerNumber].characterImage;
        statsName.text = "" + characterStats[playerNumber].characterName;

        statsHP.text = "" + characterStats[playerNumber].currentHP + "/" + characterStats[playerNumber].maxHP;
        statsMP.text = "" + characterStats[playerNumber].currentMP + "/" + characterStats[playerNumber].maxMP;
        statsStrength.text = characterStats[playerNumber].strength.ToString();
        statsDefense.text = characterStats[playerNumber].defense.ToString();

        if (characterStats[playerNumber].equippedWeapon != "")
        {
            statsWeapon.text = characterStats[playerNumber].equippedWeapon;
        }
        else
        {
            statsWeapon.text = "None";
        }
        statsWeaponPower.text = characterStats[playerNumber].weaponPower.ToString();

        if (characterStats[playerNumber].equippedArmor != "")
        {
            statsArmor.text = characterStats[playerNumber].equippedArmor;
        }
        else
        {
            statsArmor.text = "None";
        }
        statsArmorPower.text = characterStats[playerNumber].armorPower.ToString();

        statsEXP.text = (characterStats[playerNumber].expToNextLevel[characterStats[playerNumber].playerLevel] - characterStats[playerNumber].currentEXP).ToString();

        lvlText.text = "Lvl: " + characterStats[playerNumber].playerLevel;
    }

    public void Unequip(bool isWeapon)
    {
        CharacterStats selectedCharacter = characterStats[activePlayer];
        string message = "";

        if (isWeapon)
        {
            if (selectedCharacter.equippedWeapon != "")
            {
                string weaponName = selectedCharacter.equippedWeapon;
                GameManager.instance.AddItem(weaponName);

                selectedCharacter.equippedWeapon = "";
                selectedCharacter.weaponPower = 0;

                message = selectedCharacter.characterName + " unequipped the " + weaponName + ".";
            }
        }
        else
        {
            if (selectedCharacter.equippedArmor != "")
            {
                string armorName = selectedCharacter.equippedArmor;
                GameManager.instance.AddItem(armorName);

                selectedCharacter.equippedArmor = "";
                selectedCharacter.armorPower = 0;

                message = selectedCharacter.characterName + " unequipped the " + armorName + ".";
            }
        }

        StartCoroutine(ShowNotification(message, itemMessageTime, itemMessageColor));

        UpdateMainStats();
        ShowPlayerStats(activePlayer);
    }

    public void ShowItems()
    {
        GameManager.instance.SortItems();

        int i = 0;
        foreach (ItemButton itemButton in itemButtons)
        {
            itemButton.buttonValue = i;

            string currentItem = GameManager.instance.itemsHeld[i];
            int currentItemAmount = GameManager.instance.numberOfItems[i];

            if (currentItem == "" || currentItemAmount == 0)
            {
                itemButton.buttonImage.gameObject.SetActive(false);
                itemButton.amountText.text = "";
                itemButton.gameObject.SetActive(false);
            }
            else
            {
                itemButton.gameObject.SetActive(true);
                itemButton.buttonImage.gameObject.SetActive(true);

                itemButton.buttonImage.sprite = GameManager.instance.GetItemDetails(currentItem).itemSprite;
                itemButton.amountText.text = currentItemAmount.ToString();
            }

            i++;
        }

        isInventoryEmpty = CheckIfInventoryIsEmpty();
        if (isInventoryEmpty)
        {
            itemName.text = "Nothing";
            itemDescription.text = "Your inventory is empty! Go find something to keep.";

            useButton.SetActive(false);
            discardButton.SetActive(false);
        }
    }

    private bool CheckIfInventoryIsEmpty()
    {
        bool isInventoryEmpty = true;
        int[] itemNumbers = GameManager.instance.numberOfItems;

        foreach (int number in itemNumbers)
        {
            if (number > 0)
            {
                isInventoryEmpty = false;
            }
        }

        return isInventoryEmpty;
    }

    public void SelectFirstItem()
    {
        // Select the first item
        string firstItemName = GameManager.instance.itemsHeld[0];
        int firstItemNumber = GameManager.instance.numberOfItems[0];
        if (firstItemName != "" && firstItemNumber != 0)
        {
            Item firstItem = GameManager.instance.GetItemDetails(firstItemName);
            SelectItem(firstItem);
        }
    }

    public void SelectItem(Item selectedItem)
    {
        // Highlight selected item, un-highlight all others
        string selectedItemString = selectedItem.itemName;
        int i = 0;
        foreach(ItemButton itemButton in itemButtons)
        {
            string currentItemString = GameManager.instance.itemsHeld[i];

            itemButtonToggles[i].ToggleButton(currentItemString == selectedItemString);

            i++;
        }

        activeItem = selectedItem;

        if (activeItem.isItem)
        {
            useButtonText.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmor)
        {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
    }

    public void AddItem()
    {
        if (isInventoryEmpty)
        {
            useButton.SetActive(true);
            discardButton.SetActive(true);

            ShowItems();
            SelectFirstItem();
        }
    }

    public void DiscardItem()
    {
        if (activeItem != null)
        {
            GameManager.instance.RemoveItem(activeItem.itemName);
        }
    }

    public void OpenItemPlayerChoicePanel()
    {
        itemCharacterSelectionMenu.SetActive(true);

        for (int i = 0; i < itemCharacterNames.Length; i++)
        {
            itemCharacterNames[i].text = GameManager.instance.playerStatsArray[i].characterName;
            itemCharacterNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStatsArray[i].gameObject.activeInHierarchy); // Deactivate buttons if player not active
        }
    }

    public void CloseItemPlayerChoicePanel()
    {
        itemCharacterSelectionMenu.SetActive(false);
    }

    public void UseItem(int selectedCharacter)
    {
        string message = activeItem.UseItem(selectedCharacter);
        StartCoroutine(ShowNotification(message, itemMessageTime, itemMessageColor));

        CloseItemPlayerChoicePanel();
    }

    public bool isPaused()
    {
        return menu.activeInHierarchy;
    }

    public void SaveGame()
    {
        PlayerPrefs.DeleteAll();

        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();

        StartCoroutine(ShowNotification("Game Saved.", saveLoadNotificationActiveTime, saveLoadNotificationColor));

        CheckLoadButton();
    }

    public void LoadGame()
    {
        StartCoroutine(ShowNotification("Game Loaded.", saveLoadNotificationActiveTime, saveLoadNotificationColor));

        StartCoroutine(FadeInLoadAndFadeOut());
    }

    private IEnumerator FadeInLoadAndFadeOut()
    {
        GameManager.instance.fadingScreen = true;
        UIFade.instance.CallFadeOut();
        AudioManager.instance.StopMusic();

        yield return new WaitForSeconds(2f);

        CloseMenu();

        Destroy(FindObjectOfType<PlayerController>().gameObject);
        Destroy(GameManager.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
        SceneLoader.LoadSceneByName("Loading Scene");
    }

    public void PlayButtonSound()
    {
        AudioManager.instance.PlaySFX(menuButtonsSound);
    }

    public void PlaySlotClickSound()
    {
        AudioManager.instance.PlaySFX(itemSlotSound);
    }

    public IEnumerator ShowNotification(string message, float activeTime, Color messageColor)
    {
        notificationText.text = message;
        notificationText.color = messageColor;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        notificationText.gameObject.SetActive(false);
    }
}
