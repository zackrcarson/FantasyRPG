using UnityEngine;

public class QuestObjectActivator : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject objectToActivate = null;
    [SerializeField] string questToCheck = null;
    [SerializeField] bool activeIfComplete = true;
    [SerializeField] bool dontActivateIfQuestComplete = false;
    [SerializeField] string dontActivateIfQuestCompleteQuest = "";

    // State Variables
    bool initialCheckDone = false;

    // Update is called once per frame
    void Update()
    {
        if (!initialCheckDone)
        {
            initialCheckDone = true;

            CheckCompletion();
        }
    }

    public void CheckCompletion()
    {
        if (QuestManager.instance.CheckIfComplete(questToCheck))
        {
            if (dontActivateIfQuestComplete)
            {
                if (QuestManager.instance.CheckIfComplete(dontActivateIfQuestCompleteQuest))
                {
                    objectToActivate.SetActive(false);
                }
                else
                {
                    objectToActivate.SetActive(true);
                }
            }
            else
            {
                objectToActivate.SetActive(activeIfComplete);
            }
        }
    }
}
