using UnityEngine;

public class PickupItem : MonoBehaviour
{
    // Cached References
    PlayerController player = null;

    // State Variables
    private bool canPickup = false;

    private void Update()
    {
        if (canPickup && (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit")))
        {
            if (!player) { player = FindObjectOfType<PlayerController>(); }

            if (player.canMove)
            {
                GameManager.instance.AddItem(GetComponent<Item>().itemName);

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canPickup = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canPickup = false;
        }
    }
}
