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
        if (GameManager.instance.itemsHeld[buttonValue] != "")
        {
            string selectedItemName = GameManager.instance.itemsHeld[buttonValue];
            Item selectedItem = GameManager.instance.GetItemDetails(selectedItemName);

            GameMenu.instance.SelectItem(selectedItem);
        }
    }
}
