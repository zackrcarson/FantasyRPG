using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    // Config Parameters
    [Header("Administrative Parameters")]
    [SerializeField] bool isPlayer = false;
    [SerializeField] string characterName = null;
    [SerializeField] string[] movesAvailable = null;

    [Header("Character Stats")]
    [SerializeField] int currentHP = 0;
    [SerializeField] int maxHP = 0;
    [SerializeField] int currentMP = 0;
    [SerializeField] int maxMP = 0;
    [SerializeField] int strength = 0;
    [SerializeField] int defense = 0;
    [SerializeField] int weaponPower = 0;
    [SerializeField] int armorPower = 0;

    // State Variables
    bool isDead = false;
}
