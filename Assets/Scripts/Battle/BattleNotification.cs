using UnityEngine;
using UnityEngine.UI;

public class BattleNotification : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public float awakeTime = 1f;

    // Cached References
    [SerializeField] public Text notificationMessage = null;

    // State Variables
    float awakeCounter = 0f;

    // Update is called once per frame
    void Update()
    {
        if (awakeCounter > 0)
        {
            awakeCounter -= Time.deltaTime;

            if (awakeCounter <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        awakeCounter = awakeTime;
    }
}
