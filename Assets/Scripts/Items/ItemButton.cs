using UnityEngine.UI;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    // Config Params
    [SerializeField] public Image buttonImage = null;
    [SerializeField] public Text amountText = null;

    // State Variables
    [HideInInspector] public int buttonValue = 0;

    public void PressButton()
    {
        if (GameMenu.instance.menu.activeInHierarchy)
        {
            if (GameManager.instance.itemsHeld[buttonValue] != "")
            {
                string selectedItemName = GameManager.instance.itemsHeld[buttonValue];
                Item selectedItem = GameManager.instance.GetItemDetails(selectedItemName);

                GameMenu.instance.SelectItem(selectedItem);
            }
        }
        
        if (Shop.instance.shopMenu.activeInHierarchy)
        {
            if (Shop.instance.buyMenu.activeInHierarchy)
            {
                string selectedItemName = Shop.instance.itemsForSale[buttonValue];
                Item selectedItem = GameManager.instance.GetItemDetails(selectedItemName);

                Shop.instance.SelectBuyItem(selectedItem);
            }
            else if (Shop.instance.sellMenu.activeInHierarchy)
            {
                string selectedItemName = GameManager.instance.itemsHeld[buttonValue];
                Item selectedItem = GameManager.instance.GetItemDetails(selectedItemName);

                Shop.instance.SelectSellItem(selectedItem);
            }
        }
    }
}
