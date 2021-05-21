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
    [SerializeField] int autoFillUnicode = 0;

    // State variables
    bool canActivate = false;

    private void Start()
    {
        if (autoFillUnicode != 0)
        {
            dialogueLines = GetUnicodeDialogueLines(autoFillUnicode);
        }
    }

    private string[] GetUnicodeDialogueLines(int number)
    {
        // UP : \u2191, DOWN:  \u2193, RIGHT: \u2192, LEFT: \u2190
        string[] newLines = new string[2];
        if (number == 1)
        {
            newLines[0] = " Caves: \u2191 \n Fountain: \u2192 \n Southern Fillory: \u2193";
            newLines[1] = " Northern Fillory: \u2191 \n Shop: \u2192";
        }
        else if (number == 2)
        {
            newLines[0] = " Caves: \u2191 \n Fountain: \u2192 \n Southern Fillory: \u2193";
            newLines[1] = " Northern Fillory: \u2191 \n Shop: \u2190";
        }
        else if (number == 3)
        {
            newLines[0] = " Caves: \u2191 \n Fountain: \u2192 \n Southern Fillory: \u2193";
            newLines[1] = " Northern Fillory: \u2191 \n Shop: \u2192";
        }

        return newLines;
    }

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
