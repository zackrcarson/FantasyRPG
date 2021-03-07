using UnityEngine.UI;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // Config Parameters
    [Header("Global Character Info")]
    [SerializeField] string characterName = "Quentin";
    [SerializeField] Sprite characterImage = null;

    [Header("Player Leveling Info")]
    [SerializeField] int playerLevel = 1;
    [SerializeField] int currentEXP = 0;
    [SerializeField] int maxLevel = 99;
    public int[] expToNextLevel = null;
    [SerializeField] int baseEXP = 1000;
    [SerializeField] float exponentialLevelCurveFactor = 1.05f;
    [SerializeField] float exponentialHPCurveFactor = 1.05f;
    [SerializeField] float exponentialMPCurveFactor = 1.05f;

    [Header("Health and Mana")]
    [SerializeField] int currentHP = 100;
    [SerializeField] int currentMP = 30;
    [SerializeField] int maxHP = 100;
    [SerializeField] int maxMP = 30;

    [Header("Other Skills")]
    [SerializeField] int strength = 1;
    [SerializeField] int defense = 1;
    [SerializeField] int weaponPower = 0;
    [SerializeField] int armorPower = 0;

    [Header("Equipped Items")]
    [SerializeField] string equippedWeapon = null;
    [SerializeField] string equippedArmor = null;

    private void Start()
    {
        SetUpLevelEXPs();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddEXP(1000);
        }
    }

    private void SetUpLevelEXPs()
    {
        expToNextLevel = new int[maxLevel];

        expToNextLevel[0] = 0;
        expToNextLevel[1] = baseEXP;

        for (int i = 2; i < expToNextLevel.Length; i++)
        {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i - 1] * exponentialLevelCurveFactor);
        }
    }

    public void AddEXP(int expToAdd)
    {
        currentEXP += expToAdd;

        if (playerLevel >= maxLevel) { return; }

        if (currentEXP >= expToNextLevel[playerLevel])
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentEXP -= expToNextLevel[playerLevel];

        playerLevel++;

        // odd level gets defense+, even level gets strength+
        if (playerLevel % 2 == 0)
        {
            strength++;
        }
        else
        {
            defense++;
        }

        maxHP = Mathf.FloorToInt(maxHP * exponentialHPCurveFactor);
        maxMP = Mathf.FloorToInt(maxMP * exponentialMPCurveFactor);

        currentHP = maxHP;
        currentMP = maxMP;
    }

    public string GetName()
    {
        return characterName;
    }
}
