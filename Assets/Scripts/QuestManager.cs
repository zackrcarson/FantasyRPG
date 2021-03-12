using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // Config Parameters
    [SerializeField] string[] questMarkerNames = null;
    
    // State Variables
    public bool[] questMarkersComplete = null; // TODO: Remove public for testing

    private void Awake()
    {
        instance = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        questMarkersComplete = new bool[questMarkerNames.Length];
    }

    private int GetQuestNumber(string questToFind)
    {
        int i = 0;
        foreach (string quest in questMarkerNames)
        {
            if (quest == questToFind)
            {
                return i;
            }

            i++;
        }

        // Always leave the first quest blank, so a 0 does not correspond to a quest 
        Debug.LogError("Quest " + questToFind + " does not exist.");
        return 0;
    }

    public bool CheckIfComplete(string questToCheck)
    {
        int questNumber = GetQuestNumber(questToCheck);

        if (questNumber != 0)
        {
            return questMarkersComplete[questNumber];
        }
        else
        {
            return false;
        }
    }

    public void MarkQuestComplete(string questToMark)
    {
        int questNumber = GetQuestNumber(questToMark);

        questMarkersComplete[questNumber] = true;

        UpdateLocalQuestObjects();
    }

    public void MarkQuestIncomplete(string questToMark)
    {
        int questNumber = GetQuestNumber(questToMark);

        questMarkersComplete[questNumber] = false;

        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects()
    {
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();

        if (questObjects.Length == 0) { return; }

        foreach(QuestObjectActivator questObject in questObjects)
        {
            questObject.CheckCompletion();
        }
    }
}
