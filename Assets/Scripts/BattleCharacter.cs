using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    // Config Parameters
    [Header("Administrative Parameters")]
    [SerializeField] public bool isPlayer = false;
    [SerializeField] public string characterName = null;
    [SerializeField] public string[] movesAvailable = null;

    [Header("Character Stats")]
    [SerializeField] public int currentHP = 0;
    [SerializeField] public int maxHP = 0;
    [SerializeField] public int currentMP = 0;
    [SerializeField] public int maxMP = 0;
    [SerializeField] public int strength = 0;
    [SerializeField] public int defense = 0;
    [SerializeField] public int weaponPower = 0;
    [SerializeField] public int armorPower = 0;

    // State Variables
    bool isDead = false;
}
