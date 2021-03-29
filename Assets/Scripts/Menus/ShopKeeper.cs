using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    // Config Parameters
    [SerializeField] string[] itemsForSale = new string[40];
    [SerializeField] float depreciationFactor = 0.6f;

    // Cached References
    PlayerController player = null;

    // State Variables
    bool canOpenShop = false;

    private void Update()
    {
        if (!LevelUp.instance.isShowingRewards && canOpenShop && !Shop.instance.shopMenu.activeInHierarchy)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
            {
                if (!player) { player = FindObjectOfType<PlayerController>(); }

                if (player.canMove)
                {
                    Shop.instance.itemsForSale = itemsForSale;
                    Shop.instance.depreciationFactor = depreciationFactor;

                    Shop.instance.OpenShop();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canOpenShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canOpenShop = false;
        }
    }
}
