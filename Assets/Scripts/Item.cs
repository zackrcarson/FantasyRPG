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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
