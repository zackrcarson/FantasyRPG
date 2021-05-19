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

    public string UseItem(int characterToUseOn)
    {
        int potionSound = GameMenu.instance.potionSound;
        int powerUpSound = GameMenu.instance.powerUpSound;
        int errorSound = GameMenu.instance.errorSound;
        int[] equipSounds = GameMenu.instance.equipSounds;
        int[] swordSounds = GameMenu.instance.swordSounds;

        string message = "";

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
                    if (BattleManager.instance.activeBattlers[activeBattler].currentHP >= BattleManager.instance.activeBattlers[activeBattler].maxHP)
                    {
                        AudioManager.instance.PlaySFX(errorSound);

                        BattleManager.instance.activeBattlers[activeBattler].currentHP = BattleManager.instance.activeBattlers[activeBattler].maxHP;

                        return "False-HP";
                    }
                    else
                    {
                        AudioManager.instance.PlayTwoSFXExternal(potionSound, powerUpSound, 0.3f);

                        int amountToDisplay = amountToChange;

                        BattleManager.instance.activeBattlers[activeBattler].currentHP += amountToChange;

                        if (BattleManager.instance.activeBattlers[activeBattler].currentHP > BattleManager.instance.activeBattlers[activeBattler].maxHP)
                        {
                            amountToDisplay -= (BattleManager.instance.activeBattlers[activeBattler].currentHP - BattleManager.instance.activeBattlers[activeBattler].maxHP);
                            BattleManager.instance.activeBattlers[activeBattler].currentHP = BattleManager.instance.activeBattlers[activeBattler].maxHP;
                        }
                        
                        BattleManager.instance.DisplayItemBoost(activeBattler, amountToDisplay, "Health");
                        GameManager.instance.RemoveItem(itemName);

                        return "True";
                    }
                }
                else
                {
                    if (selectedCharacter.currentHP >= selectedCharacter.maxHP)
                    {
                        AudioManager.instance.PlaySFX(errorSound);
                        selectedCharacter.currentHP = selectedCharacter.maxHP;
                        
                        return selectedCharacter.characterName + "'s HP is already full!";
                    }

                    selectedCharacter.currentHP += amountToChange;

                    int tempAmountToChange = amountToChange;
                    if (selectedCharacter.currentHP > selectedCharacter.maxHP)
                    {
                        tempAmountToChange = amountToChange - (selectedCharacter.currentHP - selectedCharacter.maxHP);

                        selectedCharacter.currentHP = selectedCharacter.maxHP;
                    }

                    message = "Healed " + selectedCharacter.characterName + " with the " + itemName + " for " + tempAmountToChange + " HP. New HP: " + selectedCharacter.currentHP + "/" + selectedCharacter.maxHP + ".";
                }
            }
            
            if (affectMP)
            {
                if (isBattleActive)
                {
                    if (BattleManager.instance.activeBattlers[activeBattler].currentMP >= BattleManager.instance.activeBattlers[activeBattler].maxMP)
                    {
                        AudioManager.instance.PlaySFX(errorSound);
                        BattleManager.instance.activeBattlers[activeBattler].currentMP = BattleManager.instance.activeBattlers[activeBattler].maxMP;

                        return "False-MP";
                    }
                    else
                    {
                        AudioManager.instance.PlayTwoSFXExternal(potionSound, powerUpSound, 0.3f);

                        int amountToDisplay = amountToChange;

                        BattleManager.instance.activeBattlers[activeBattler].currentMP += amountToChange;

                        if (BattleManager.instance.activeBattlers[activeBattler].currentMP > BattleManager.instance.activeBattlers[activeBattler].maxMP)
                        {
                            amountToDisplay -= (BattleManager.instance.activeBattlers[activeBattler].currentMP - BattleManager.instance.activeBattlers[activeBattler].maxMP);
                            BattleManager.instance.activeBattlers[activeBattler].currentMP = BattleManager.instance.activeBattlers[activeBattler].maxMP;
                        }

                        BattleManager.instance.DisplayItemBoost(activeBattler, amountToDisplay, "Mana");

                        GameManager.instance.RemoveItem(itemName);
                        return "True";
                    }
                }
                else
                {
                    if (selectedCharacter.currentMP >= selectedCharacter.maxMP)
                    {
                        AudioManager.instance.PlaySFX(errorSound);
                        selectedCharacter.currentMP = selectedCharacter.maxMP;

                        return selectedCharacter.characterName + "'s MP is already full!";
                    }

                    selectedCharacter.currentMP += amountToChange;

                    int tempAmountToChange = amountToChange;
                    if (selectedCharacter.currentMP > selectedCharacter.maxMP)
                    {
                        tempAmountToChange = amountToChange - (selectedCharacter.currentMP - selectedCharacter.maxMP);

                        selectedCharacter.currentMP = selectedCharacter.maxMP;
                    }

                    message = "Restored " + selectedCharacter.characterName + "'s MP by " + tempAmountToChange + " with the " + itemName + ". New MP: " + selectedCharacter.currentMP + "/" + selectedCharacter.maxMP + ".";
                }
            }
            
            if (affectStrength)
            {
                if (isBattleActive)
                {
                    AudioManager.instance.PlayTwoSFXExternal(potionSound, powerUpSound, 0.3f);

                    BattleManager.instance.DisplayItemBoost(activeBattler, amountToChange, "Strength");

                    BattleManager.instance.activeBattlers[activeBattler].strength += amountToChange;

                    GameManager.instance.RemoveItem(itemName);
                    return "True";
                }
                else
                {
                    selectedCharacter.strength += amountToChange;

                    message = "Boosted " + selectedCharacter.characterName + "'s strength by " + amountToChange + " with the " + itemName + ". New strength: " + selectedCharacter.strength + ".";
                }
            }
            
            if (affectDefense)
            {
                if (isBattleActive)
                {
                    AudioManager.instance.PlayTwoSFXExternal(potionSound, powerUpSound, 0.3f);

                    BattleManager.instance.DisplayItemBoost(activeBattler, amountToChange, "Defense");

                    BattleManager.instance.activeBattlers[activeBattler].defense += amountToChange;

                    GameManager.instance.RemoveItem(itemName);
                    return "True";
                }
                else
                {
                    selectedCharacter.defense += amountToChange;

                    message = "Boosted " + selectedCharacter.characterName + "'s defense by " + amountToChange + " with the " + itemName + ". New defense: " + selectedCharacter.defense + ".";
                }
            }

            AudioManager.instance.PlayTwoSFXExternal(potionSound, powerUpSound, 0.3f);
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

                message = selectedCharacter.characterName + " equipped " + itemName + ". Weapon strength: " + weaponStrength + ".";

                AudioManager.instance.PlayRandomSFX(swordSounds);
            }

            if (isArmor)
            {
                if (selectedCharacter.equippedArmor != "")
                {
                    GameManager.instance.AddItem(selectedCharacter.equippedArmor);
                }

                selectedCharacter.equippedArmor = itemName;
                selectedCharacter.armorPower = armorStrength;

                message = selectedCharacter.characterName + " equipped " + itemName + ". Armor strength: " + armorStrength + ".";

                AudioManager.instance.PlayRandomSFX(equipSounds);
            }
        }

        GameManager.instance.RemoveItem(itemName);

        return message;
    }
}
