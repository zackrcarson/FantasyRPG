using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    // Config Parameters
    [Header("Configs")]
    [SerializeField] GameObject menu = null;
    [SerializeField] GameObject[] characterStatHolders = null;

    [Header("Character Stat Arrays")]
    [SerializeField] Text[] nameTexts = null;
    [SerializeField] Text[] HPTexts = null;
    [SerializeField] Text[] MPTexts = null;
    [SerializeField] Text[] LVLTexts = null;
    [SerializeField] Text[] EXPTexts = null;
    [SerializeField] Slider[] EXPSliders = null;
    [SerializeField] Image[] characterImages = null;
    

    // Cached References
    CharacterStats[] characterStats = null;

    // Update is called once per frame
    void Update()
    {
        CheckPauseButton();
    }

    private void CheckPauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        {
            bool menuOpen = menu.activeInHierarchy;

            menu.SetActive(!menuOpen);

            if (!menuOpen) { UpdateMainStats(); }

            GameManager.instance.gameMenuOpen = !menuOpen;
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
}
