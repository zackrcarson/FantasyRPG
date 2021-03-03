using UnityEngine;

public class PortalExit : MonoBehaviour
{
    // Config Params
    [SerializeField] string portalName = null;
    [SerializeField][Tooltip("Scene Description - Portal Number")] string sceneToLoad = null;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            SceneLoader.LoadSceneByName(sceneToLoad);

            FindObjectOfType<PlayerController>().portalName = portalName;
        }
    }
}
