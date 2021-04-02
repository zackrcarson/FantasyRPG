using UnityEngine;

public class Dialogue : MonoBehaviour
{
    // Config Parameters
    [SerializeField] string NPCName = null;
    [TextArea] [SerializeField] string[] dialogueLines = null;
    //[SerializeField] bool shouldActivateQuest = false;
    [SerializeField] string questToMark = null;
    [SerializeField] bool markQuestComplete = false;
    [SerializeField] string addNewQuest = "";

    // State variables
    bool canActivate = false;

    private void Update()
    {
        if (GameMenu.instance.isPaused()) { return; }

        if (!LevelUp.instance.isShowingRewards && canActivate && !DialogueManager.instance.dialogueBox.activeInHierarchy)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
            {
                DialogueManager.instance.ShowNewDialogue(dialogueLines, NPCName);
                DialogueManager.instance.ShouldActivateQuestAtEnd(questToMark, markQuestComplete);
                DialogueManager.instance.ShouldAddQuestAtEnd(addNewQuest);
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
