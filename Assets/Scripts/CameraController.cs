using UnityEngine;

public class CameraController : MonoBehaviour
{
    // State Variables
    Transform target = null;

    void Start()
    {
        GetPlayer();
    }

    private void GetPlayer()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        PlayerController player = null;
        if (players.Length > 1)
        {
            player = players[players.Length - 1];
        }
        else
        {
            player = players[0];
        }

        target = player.transform;
    }

    // Late Update is called once per frame, after normal Update
    void LateUpdate()
    {
        if (!target) { GetPlayer(); }

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
