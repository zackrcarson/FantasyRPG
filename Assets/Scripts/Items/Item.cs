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
        CharacterStats selectedCharacter = GameManager.instance.playerStatsArray[characterToUseOn];

        if (isItem)
        {
            if (affectHP)
            {
                selectedCharacter.currentHP += amountToChange;

                if (selectedCharacter.currentHP > selectedCharacter.maxHP)
                {
                    selectedCharacter.currentHP = selectedCharacter.maxHP;
                }
            }
            
            if (affectMP)
            {
                selectedCharacter.currentMP += amountToChange;

                if (selectedCharacter.currentMP > selectedCharacter.maxMP)
                {
                    selectedCharacter.currentMP = selectedCharacter.maxMP;
                }
            }
            
            if (affectStrength)
            {
                selectedCharacter.strength += amountToChange;
            }
            
            if (affectDefense)
            {
                selectedCharacter.defense += amountToChange;
            }
        }

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

        GameManager.instance.RemoveItem(itemName);
    }
}
