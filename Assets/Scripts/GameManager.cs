using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Config Parameters
    [SerializeField] public CharacterStats[] playerStatsArray = null;
    [SerializeField] Item[] referenceItems = null;
    [SerializeField] public string[] itemsHeld = null; // TODO: Remove serialize
    [SerializeField] public int[] numberOfItems = null; // TODO: Remove serialize

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
}
