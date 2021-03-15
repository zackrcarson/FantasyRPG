using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float waitToLoad = 1f;

    private void Update()
    {
        if (waitToLoad > 0)
        {
            waitToLoad -= Time.deltaTime;

            if (waitToLoad <= 0)
            {
                GameManager.instance.LoadData();
                QuestManager.instance.LoadQuestData();
            }
        }
    }
}
