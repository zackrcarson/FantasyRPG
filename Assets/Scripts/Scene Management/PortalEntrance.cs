using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public string portalFromName = null;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController player = GetPlayer();

        if (portalFromName == player.portalName)
        {
            player.transform.position = transform.position;
        }

        UIFade.instance.CallFadeIn();
        GameManager.instance.fadingScreen = false;
    }

    private PlayerController GetPlayer()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        PlayerController player = null;
        if (players.Length > 1)
        {
            if (players[0].portalName == "")
            {
                player = players[1];
            }
            else
            {
                player = players[0];
            }
        }
        else
        {
            player = players[0];
        }

        return player;
    }
}
