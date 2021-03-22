using UnityEngine.UI;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // Config Parameters
    [Header("Global Character Info")]
    [SerializeField] public string characterName = "Quentin";
    [SerializeField] public Sprite characterImage = null;

    [Header("Player Leveling Info")]
    [SerializeField] public int playerLevel = 1;
    [SerializeField] public int currentEXP = 0;
    [SerializeField] public int maxLevel = 99;
    [HideInInspector] public int[] expToNextLevel = null;
    [SerializeField] int baseEXP = 1000;
    [SerializeField] float exponentialLevelCurveFactor = 1.05f;
    [SerializeField] float exponentialHPCurveFactor = 1.05f;
    [SerializeField] float exponentialMPCurveFactor = 1.05f;

    [Header("Health and Mana")]
    [SerializeField] public int currentHP = 100;
    [SerializeField] public int currentMP = 30;
    [SerializeField] public int maxHP = 100;
    [SerializeField] public int maxMP = 30;

    [Header("Other Skills")]
    [SerializeField] public int strength = 1;
    [SerializeField] public int defense = 1;
    [SerializeField] public int weaponPower = 0;
    [SerializeField] public int armorPower = 0;

    [Header("Equipped Items")]
    [SerializeField] public string equippedWeapon = null;
    [SerializeField] public string equippedArmor = null;

    private void Start()
    {
        SetUpLevelEXPs();
    }

    public void SetUpLevelEXPs()
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
