using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject playerPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (!player)
        {
            Instantiate(playerPrefab);
        }
    }
}
