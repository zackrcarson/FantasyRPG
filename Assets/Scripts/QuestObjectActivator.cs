using UnityEngine;

public class QuestObjectActivator : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject objectToActivate = null;
    [SerializeField] string questToCheck = null;
    [SerializeField] bool activeIfComplete = true;

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
            objectToActivate.SetActive(activeIfComplete);
        }
    }
}
