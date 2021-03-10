using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static GameMenu instance;

    // Config Parameters
    [Header("Configs")]
    [SerializeField] GameObject menu = null;
    [SerializeField] GameObject[] characterStatHolders = null;
    [SerializeField] GameObject[] menuWindows = null;
    [SerializeField] ButtonToggle[] menuButtons = null;

    [Header("Items Panel Config")]
    [SerializeField] ItemButton[] itemButtons = null;
    ButtonToggle[] itemButtonToggles = null;
    [SerializeField] Text itemName = null;
    [SerializeField] Text itemDescription = null;
    [SerializeField] Text useButtonText = null;

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

    // State Variables
    string selectedItem = null;
    Item activeItem = null;

    // Cached References
    CharacterStats[] characterStats = null;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        itemButtonToggles = new ButtonToggle[itemButtons.Length];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtonToggles[i] = itemButtons[i].gameObject.GetComponent<ButtonToggle>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPauseButton();
    }

    private void CheckPauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        {
            if (menu.activeInHierarchy)
            {
                CloseMenu();
            }
            else
            {
                menu.SetActive(true);
                GameManager.instance.gameMenuOpen = true;

                UpdateMainStats();

            }
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

        // Activate walking again
        GameManager.instance.gameMenuOpen = false;
    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void ShowPlayerStats(int playerNumber)
    {
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

        statsHP.text = "" + characterStats[playerNumber].currentMP + "/" + characterStats[playerNumber].maxMP;
        statsMP.text = "" + characterStats[playerNumber].currentHP + "/" + characterStats[playerNumber].maxHP;
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
}