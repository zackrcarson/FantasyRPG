using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject playerPrefab = null;
    [SerializeField] GameObject UIScreenPrefab = null;
    [SerializeField] GameManager gameManagerPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!UIFade.instance)
        {
            Instantiate(UIScreenPrefab);
        }

        if (!GameManager.instance)
        {
            Instantiate(gameManagerPrefab);
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (!player)
        {
            Instantiate(playerPrefab);
        }
    }
}
