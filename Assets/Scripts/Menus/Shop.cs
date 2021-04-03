using UnityEngine.UI;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    // Config Variables
    [Header("Main Button Toggles")]
    [SerializeField] ButtonToggle buyButtonToggle = null;
    [SerializeField] ButtonToggle sellButtonToggle = null;
    [SerializeField] GameObject buyButton = null;
    [SerializeField] GameObject sellButton = null;
    [SerializeField] GameObject buyValueString = null;
    [SerializeField] GameObject sellValueString = null;

    [Header("Menu Objects")]
    [SerializeField] public GameObject shopMenu = null;
    [SerializeField] public GameObject buyMenu = null;
    [SerializeField] public GameObject sellMenu = null;
    [SerializeField] Text goldText = null;

    [Header("Item Buttons")]
    [SerializeField] ItemButton[] buyItemButtons = null;
    [SerializeField] ItemButton[] sellItemButtons = null;
    ButtonToggle[] buyItemButtonToggles = null;
    ButtonToggle[] sellItemButtonToggles = null;

    [Header("Selected Item Descriptions")]
    [SerializeField] Text buyItemName = null;
    [SerializeField] Text buyItemDescription = null;
    [SerializeField] Text buyItemValue = null;

    [SerializeField] Text sellItemName = null;
    [SerializeField] Text sellItemDescription = null;
    [SerializeField] Text sellItemValue = null;

    [SerializeField] Text buyValueLabel = null;   
    [SerializeField] Color notEnoughMoneyColor;
    [SerializeField] Color notEnoughMoneyButtonColor;

    [Header("SFX")]
    [SerializeField] int shopSound = 29;
    [SerializeField] int[] coinSounds = new int[] { 11, 12, 13};
    [SerializeField] int errorSound = 21;

    // Cached References
    Color enoughMoneyColor;
    Color enoughMoneyButtonColor;

    // State Variables
    [HideInInspector] public string[] itemsForSale = null;
    [HideInInspector] public float depreciationFactor = 0.6f;
    Item selectedItem = null;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        enoughMoneyColor = buyItemValue.color;
        enoughMoneyButtonColor = buyButton.GetComponent<Image>().color;

        buyItemButtonToggles = new ButtonToggle[buyItemButtons.Length];
        for (int i = 0; i < buyItemButtons.Length; i++)
        {
            buyItemButtonToggles[i] = buyItemButtons[i].gameObject.GetComponent<ButtonToggle>();
        }

        sellItemButtonToggles = new ButtonToggle[sellItemButtons.Length];
        for (int i = 0; i < sellItemButtons.Length; i++)
        {
            sellItemButtonToggles[i] = sellItemButtons[i].gameObject.GetComponent<ButtonToggle>();
        }
    }

    public void OpenShop()
    {
        AudioManager.instance.PlaySFX(shopSound);

        shopMenu.SetActive(true);
        OpenBuyMenu();

        GameManager.instance.shopActive = true;

        goldText.text = GameManager.instance.currentGold.ToString();
    }

    public void CloseShop()
    {
        AudioManager.instance.PlaySFX(shopSound);

        shopMenu.SetActive(false);

        GameManager.instance.shopActive = false;
    }

    public void OpenBuyMenu()
    {
        buyButton.SetActive(true);
        buyValueString.SetActive(true);

        buyMenu.SetActive(true);
        sellMenu.SetActive(false);

        if (itemsForSale[buyItemButtons[0].buttonValue] == "")
        {
            SelectBuyItemNothing();
        }
        else
        {
            buyItemButtons[0].PressButton();
        }

        buyButtonToggle.ToggleButton(true);
        sellButtonToggle.ToggleButton(false);

        ShowBuyItems();
    }

    public void OpenSellMenu()
    {
        sellButton.SetActive(true);
        sellValueString.SetActive(true);

        buyMenu.SetActive(false);
        sellMenu.SetActive(true);

        if (GameManager.instance.itemsHeld[sellItemButtons[0].buttonValue] == "")
        {
            SelectSellItemNothing();
        }
        else
        {
            sellItemButtons[0].PressButton();
        }

        buyButtonToggle.ToggleButton(false);
        sellButtonToggle.ToggleButton(true);

        ShowSellItems();
    }

    private void ShowBuyItems()
    {
        int i = 0;
        foreach (ItemButton itemButton in buyItemButtons)
        {
            itemButton.buttonValue = i;

            string currentItem = itemsForSale[i];

            if (currentItem == "")
            {
                itemButton.buttonImage.gameObject.SetActive(false);
                itemButton.amountText.text = "";
                itemButton.gameObject.SetActive(false);
            }
            else
            {
                itemButton.gameObject.SetActive(true);
                itemButton.buttonImage.gameObject.SetActive(true);

                itemButton.buttonImage.sprite = GameManager.instance.GetItemDetails(currentItem).itemSprite;
                itemButton.amountText.text = "\u221E";
            }

            i++;
        }
    }

    private void ShowSellItems()
    {
        GameManager.instance.SortItems();

        int i = 0;
        foreach (ItemButton itemButton in sellItemButtons)
        {
            itemButton.buttonValue = i;

            string currentItem = GameManager.instance.itemsHeld[i];
            int currentItemAmount = GameManager.instance.numberOfItems[i];

            if (currentItem == "" || currentItemAmount == 0)
            {
                itemButton.buttonImage.gameObject.SetActive(false);
                itemButton.amountText.text = "";
                itemButton.gameObject.SetActive(false);
            }
            else
            {
                itemButton.gameObject.SetActive(true);
                itemButton.buttonImage.gameObject.SetActive(true);

                itemButton.buttonImage.sprite = GameManager.instance.GetItemDetails(currentItem).itemSprite;
                itemButton.amountText.text = currentItemAmount.ToString();
            }

            i++;
        }
    }

    public bool IsShopping()
    {
        return shopMenu.activeInHierarchy;
    }

    public void SelectBuyItem(Item buyItem)
    {
        selectedItem = buyItem;

        // Highlight selected item, un-highlight all others
        string selectedItemString = selectedItem.itemName;
        int i = 0;
        foreach (ItemButton itemButton in buyItemButtons)
        {
            string currentItemString = itemsForSale[i];

            buyItemButtonToggles[i].ToggleButton(currentItemString == selectedItemString);

            i++;
        }

        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.description;
        buyItemValue.text = selectedItem.cost.ToString() + "g";

        if (selectedItem.cost > GameManager.instance.currentGold)
        {
            buyItemValue.color = notEnoughMoneyColor;
            buyValueLabel.color = notEnoughMoneyColor;

            buyButton.GetComponent<Image>().color = notEnoughMoneyButtonColor;

            Color textColor = buyButton.GetComponentInChildren<Text>().color;
            textColor = new Color(textColor.r, textColor.g, textColor.b, notEnoughMoneyButtonColor.a);
        }
        else
        {
            buyItemValue.color = enoughMoneyColor;
            buyValueLabel.color = enoughMoneyColor;

            buyButton.GetComponent<Image>().color = enoughMoneyButtonColor;

            Color textColor = buyButton.GetComponentInChildren<Text>().color;
            textColor = new Color(textColor.r, textColor.g, textColor.b, 1f);
        }
    }

    public void SelectSellItem(Item sellItem)
    {
        selectedItem = sellItem;

        // Highlight selected item, un-highlight all others
        string selectedItemString = selectedItem.itemName;
        int i = 0;
        foreach (ItemButton itemButton in sellItemButtons)
        {
            string currentItemString = GameManager.instance.itemsHeld[i];

            sellItemButtonToggles[i].ToggleButton(currentItemString == selectedItemString);

            i++;
        }

        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.description;
        sellItemValue.text = Mathf.FloorToInt(selectedItem.cost * depreciationFactor).ToString() + "g";
    }

    public void SelectBuyItemNothing()
    {
        buyItemName.text = "Nothing";
        buyItemDescription.text = "The shop is out of stock! Come back later.";
        buyItemValue.text = "";

        buyButton.SetActive(false);
        buyValueString.SetActive(false);
    }

    public void SelectSellItemNothing()
    { 
        sellItemName.text = "Nothing";
        sellItemDescription.text = "Your inventory is empty! Go find something to keep.";
        sellItemValue.text = "";

        sellButton.SetActive(false);
        sellValueString.SetActive(false);
    }

    public void BuyItem()
    {
        if (selectedItem == null) { return; }

        if (GameManager.instance.currentGold >= selectedItem.cost)
        {
            GameManager.instance.currentGold -= selectedItem.cost;

            GameManager.instance.AddItem(selectedItem.itemName);

            SelectBuyItem(selectedItem);

            AudioManager.instance.PlayRandomSFX(coinSounds);
        }
        else
        {
            AudioManager.instance.PlaySFX(errorSound);
        }

        goldText.text = GameManager.instance.currentGold.ToString() +"g";
    }

    public void SellItem()
    {
        if (selectedItem == null) { return; }
        
        AudioManager.instance.PlayRandomSFX(coinSounds);

        GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.cost * depreciationFactor);

        GameManager.instance.RemoveItem(selectedItem.itemName);

        // Check if we depleted the item. If so, select the first item again. If the first item is empty, we have an empty inventory!
        if (GameManager.instance.FindSelectedItemAmount(selectedItem.itemName) <= 0)
        {
            GameManager.instance.SortItems();

            if (GameManager.instance.itemsHeld[sellItemButtons[0].buttonValue] == "")
            {
                SelectSellItemNothing();
            }
            else
            {
                sellItemButtons[0].PressButton();
            }
        }

        // Re-display the gold amount, and all of the tiles.
        goldText.text = GameManager.instance.currentGold.ToString() +"g";
        ShowSellItemsAfterSelling();
    }

    private void ShowSellItemsAfterSelling()
    {
        GameManager.instance.SortItems();

        if (GameManager.instance.itemsHeld[sellItemButtons[0].buttonValue] == "")
        {
            SelectSellItemNothing();
        }

        int i = 0;
        foreach (ItemButton itemButton in sellItemButtons)
        {
            itemButton.buttonValue = i;

            string currentItem = GameManager.instance.itemsHeld[i];
            int currentItemAmount = GameManager.instance.numberOfItems[i];

            if (currentItem == "" || currentItemAmount == 0)
            {
                itemButton.buttonImage.gameObject.SetActive(false);
                itemButton.amountText.text = "";
                itemButton.gameObject.SetActive(false);
            }
            else
            {
                itemButton.gameObject.SetActive(true);
                itemButton.buttonImage.gameObject.SetActive(true);

                itemButton.buttonImage.sprite = GameManager.instance.GetItemDetails(currentItem).itemSprite;
                itemButton.amountText.text = currentItemAmount.ToString();
            }

            i++;
        }
    }
}
