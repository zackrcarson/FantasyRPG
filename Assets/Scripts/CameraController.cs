using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    // Config Params
    [SerializeField] Tilemap tilemap = null;

    // State Variables
    Transform target = null;

    // Cached References
    Vector3 bottomLeftBound = new Vector3(0f, 0f, 0f);
    Vector3 topRightBound = new Vector3(0f, 0f, 0f);

    void Start()
    {
        GetPlayer();

        GetTilemapBounds();
    }

    private void GetTilemapBounds()
    {
        float halfCameraHeight = Camera.main.orthographicSize;
        float halfCameraWidth = halfCameraHeight * Camera.main.aspect;

        Vector3 cameraOffset = new Vector3(halfCameraWidth, halfCameraHeight, 0f);

        bottomLeftBound = tilemap.localBounds.min + cameraOffset;
        topRightBound = tilemap.localBounds.max - cameraOffset;

        GetPlayer();
        target.GetComponent<PlayerController>().SetBounds(tilemap.localBounds.min, tilemap.localBounds.max);
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

        FollowTargetAndClampToBounds();
    }

    private void FollowTargetAndClampToBounds()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Keep the camera inside the bounds
        float xPos = Mathf.Clamp(transform.position.x, bottomLeftBound.x, topRightBound.x);
        float yPos = Mathf.Clamp(transform.position.y, bottomLeftBound.y, topRightBound.y);
        float zPos = transform.position.z;

        transform.position = new Vector3(xPos, yPos, zPos);
    }
}
