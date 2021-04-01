using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    // Config Parameters
    [SerializeField] string[] treasureList = null;
    [SerializeField] int[] treasureListNumbers = null;
    [SerializeField] int coins = 0;
    [SerializeField] float rewardPanelScreenTime = 6f;
    [SerializeField] Sprite openSprite = null;

    // State Variables
    bool canOpen = false;
    bool isOpen = false;

    private void Update()
    {
        if (!isOpen && canOpen)
        {
            if (!LevelUp.instance.isShowingRewards && !DialogueManager.instance.dialogueBox.activeInHierarchy)
            {
                if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
                {
                    OpenChest();
                }
            }
        }
    }

    private void OpenChest()
    {
        isOpen = true;
        GetComponent<Animator>().SetBool("chestOpen", true);

        StartCoroutine(ChestManager.instance.OpenChest(treasureList, treasureListNumbers, coins, rewardPanelScreenTime));

        GenerateGUID guid = GetComponent<GenerateGUID>();
        if (guid)
        {
            ChestManager.instance.ChestOpened(SceneLoader.GetCurrentScene(), guid.GUID);
        }
    }

    public void OpenChestOnSceneLoad()
    {
        isOpen = true;

        GetComponent<Animator>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = openSprite;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canOpen = false;
        }
    }
}
