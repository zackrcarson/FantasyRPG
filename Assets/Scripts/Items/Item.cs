using UnityEngine;

public class Item : MonoBehaviour
{
    // Config Params
    [Header("Item Type")]
    [SerializeField] public bool isItem = false;
    [SerializeField] public bool isWeapon = false;
    [SerializeField] public bool isArmor = false;

    [Header("Item Descriptors")]
    [SerializeField] public string itemName = null;
    [SerializeField] public string description = null;
    [SerializeField] public Sprite itemSprite = null;

    [Header("Item Details")]
    [SerializeField] public int cost = 0;

    [SerializeField] public int amountToChange = 0;
    [SerializeField] public bool affectHP = false;
    [SerializeField] public bool affectMP = false;
    [SerializeField] public bool affectStrength = false;
    [SerializeField] public bool affectDefense = false;

    [Header("Weapon/Armor Stats")]
    [SerializeField] public int weaponStrength = 0;
    [SerializeField] public int armorStrength = 0;

    public void UseItem(int characterToUseOn)
    {
        // Dummy holders - won't ever be used in the alternative situation
        int activeBattler = 0;
        CharacterStats selectedCharacter = GameManager.instance.playerStatsArray[0]; 

        bool isBattleActive = BattleManager.instance.isBattleActive;

        if (isBattleActive)
        {
            activeBattler = characterToUseOn;
        }
        else
        {
            selectedCharacter = GameManager.instance.playerStatsArray[characterToUseOn];
        }

        if (isItem)
        {
            if (affectHP)
            {
                if (isBattleActive)
                {
                    BattleManager.instance.DisplayItemBoost(activeBattler, amountToChange, "Health");

                    BattleManager.instance.activeBattlers[activeBattler].currentHP += amountToChange;

                    if (BattleManager.instance.activeBattlers[activeBattler].currentHP > BattleManager.instance.activeBattlers[activeBattler].maxHP)
                    {
                        BattleManager.instance.activeBattlers[activeBattler].currentHP = BattleManager.instance.activeBattlers[activeBattler].maxHP;
                    }
                }
                else
                {
                    selectedCharacter.currentHP += amountToChange;

                    if (selectedCharacter.currentHP > selectedCharacter.maxHP)
                    {
                        selectedCharacter.currentHP = selectedCharacter.maxHP;
                    }
                }
            }
            
            if (affectMP)
            {
                if (isBattleActive)
                {
                    BattleManager.instance.DisplayItemBoost(activeBattler, amountToChange, "Mana");

                    BattleManager.instance.activeBattlers[activeBattler].currentMP += amountToChange;

                    if (BattleManager.instance.activeBattlers[activeBattler].currentMP > BattleManager.instance.activeBattlers[activeBattler].maxMP)
                    {
                        BattleManager.instance.activeBattlers[activeBattler].currentMP = BattleManager.instance.activeBattlers[activeBattler].maxMP;
                    }
                }
                else
                {
                    selectedCharacter.currentMP += amountToChange;

                    if (selectedCharacter.currentMP > selectedCharacter.maxMP)
                    {
                        selectedCharacter.currentMP = selectedCharacter.maxMP;
                    }
                }
            }
            
            if (affectStrength)
            {
                if (isBattleActive)
                {
                    BattleManager.instance.DisplayItemBoost(activeBattler, amountToChange, "Strength");

                    BattleManager.instance.activeBattlers[activeBattler].strength += amountToChange;
                }
                else
                {
                    selectedCharacter.strength += amountToChange;
                }
            }
            
            if (affectDefense)
            {
                if (isBattleActive)
                {
                    BattleManager.instance.DisplayItemBoost(activeBattler, amountToChange, "Defense");

                    BattleManager.instance.activeBattlers[activeBattler].defense += amountToChange;
                }
                else
                {
                    selectedCharacter.defense += amountToChange;
                }
            }
        }

        if (!isBattleActive)
        {
            if (isWeapon)
            {
                if (selectedCharacter.equippedWeapon != "")
                {
                    GameManager.instance.AddItem(selectedCharacter.equippedWeapon);
                }

                selectedCharacter.equippedWeapon = itemName;
                selectedCharacter.weaponPower = weaponStrength;
            }

            if (isArmor)
            {
                if (selectedCharacter.equippedArmor != "")
                {
                    GameManager.instance.AddItem(selectedCharacter.equippedArmor);
                }

                selectedCharacter.equippedArmor = itemName;
                selectedCharacter.armorPower = armorStrength;
            }
        }

        GameManager.instance.RemoveItem(itemName);
    }
}
