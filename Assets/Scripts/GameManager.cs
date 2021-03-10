using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Config Parameters
    [SerializeField] public CharacterStats[] playerStatsArray = null;
    [SerializeField] Item[] referenceItems = null;
    [SerializeField] public string[] itemsHeld = null;
    [SerializeField] public int[] numberOfItems = null;

    // Cached References
    PlayerController player = null;

    // State Variables
    [HideInInspector] public bool gameMenuOpen = false;
    [HideInInspector] public bool dialogueActive = false;
    [HideInInspector] public bool fadingScreen = false;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        SortItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpen || dialogueActive || fadingScreen)
        {
            player.CanMove(false);
        }
        else
        {
            player.CanMove(true);
        }
    }

    public Item GetItemDetails(string itemToGrab)
    {
        foreach (Item item in referenceItems)
        {
            if (item.itemName == itemToGrab)
            {
                return item;
            }
        }

        return null;
    }

    public void SortItems()
    {
        bool itemAfterSpace = true;

        while (itemAfterSpace)
        {
            itemAfterSpace = false;

            for (int i = 0; i < itemsHeld.Length - 1; i++)
            {
                if (itemsHeld[i] == "" || numberOfItems[i] == 0)
                {
                    itemsHeld[i] = itemsHeld[i + 1];
                    itemsHeld[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1];
                    numberOfItems[i + 1] = 0;

                    if (itemsHeld[i] != "")
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddItem(string itemToAdd)
    {
        int newItemPosition = 0;
        bool foundSpaceForItem = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd)
            {
                newItemPosition = i;

                foundSpaceForItem = true;

                break;
            }
        }

        if (foundSpaceForItem)
        {
            bool itemExists = false;

            foreach (Item item in referenceItems)
            {
                if (item.itemName == itemToAdd)
                {
                    itemExists = true;

                    break;
                }
            }

            if (itemExists)
            {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;

                GameMenu.instance.AddItem();
            }
            else
            {
                Debug.LogError("Item " + itemToAdd + " does not exist!");
            }
        }

        GameMenu.instance.ShowItems();
    }

    public void RemoveItem(string itemToRemove)
    {
        int itemPosition = 0;
        bool foundItem = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == itemToRemove)
            {
                itemPosition = i;

                foundItem = true;

                break;
            }
        }

        if (foundItem)
        {
            numberOfItems[itemPosition]--;

            if (numberOfItems[itemPosition] <= 0)
            {
                itemsHeld[itemPosition] = "";
                numberOfItems[itemPosition] = 0;

                GameMenu.instance.ShowItems();
                GameMenu.instance.SelectFirstItem();
            }
            else
            {
                GameMenu.instance.ShowItems();
            }            
        }
        else
        {
            Debug.LogError("Couldn't find " + itemToRemove);
        }
    }
}
