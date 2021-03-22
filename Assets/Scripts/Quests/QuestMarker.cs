using UnityEngine;

public class QuestMarker : MonoBehaviour
{
    // Config Parameters
    [SerializeField] string questToMark = null;
    [SerializeField] bool markQuestComplete = true;
    [SerializeField] bool markOnEnter = true;
    [SerializeField] bool deactivateOnMarking = true;

    // State variables
    private bool canMark = false;

    private void Update()
    {
        if (canMark)
        {
            //If we are in the zone, and we need to click to activate the quest (i.e. !markOnEnter)
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
            {
                canMark = false;
                MarkQuest();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            if (markOnEnter)
            {
                // If markOnEnter, immediately mark the quest (as complete or incomplete) when we enter the zone. Else, allow a click in update
                MarkQuest();
            }
            else
            {
                canMark = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            canMark = false;
        }
    }

    private void MarkQuest()
    {
        // Mark the quest as completed or incomplete when we've completed the task (enter the area, or click button in area)
        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToMark);
        }
        else
        {
            QuestManager.instance.MarkQuestIncomplete(questToMark);
        }

        // If we say so, deactivate this quest marker once we mark it once
        gameObject.SetActive(!deactivateOnMarking);
    }
}
