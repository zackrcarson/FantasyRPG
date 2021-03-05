using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject playerPrefab = null;
    [SerializeField] GameObject UIScreenPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!UIFade.instance)
        {
            Instantiate(UIScreenPrefab);
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (!player)
        {
            Instantiate(playerPrefab);
        }
    }
}
