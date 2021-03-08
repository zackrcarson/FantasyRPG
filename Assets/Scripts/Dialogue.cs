using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    // Config Parameters
    [SerializeField] string NPCName = null;
    [TextArea] [SerializeField] string[] dialogueLines = null;

    // State variables
    bool canActivate = false;

    private void Update()
    {
        if (canActivate && !DialogueManager.instance.dialogueBox.activeInHierarchy)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
            {
                DialogueManager.instance.ShowNewDialogue(dialogueLines, NPCName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canActivate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canActivate = false;
        }
    }
}
