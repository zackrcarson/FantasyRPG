using UnityEngine;

public class PortalExit : MonoBehaviour
{
    // Config Params
    [SerializeField] string portalName = null;
    [SerializeField][Tooltip("Scene Description - Portal Number")] string sceneToLoad = null;
    [SerializeField] float loadDelayTimer = 1f;
    [SerializeField] bool shouldLoadAfterFade = false;

    private void Update()
    {
        LoadTimer();
    }

    private void LoadTimer()
    {
        if (shouldLoadAfterFade)
        {
            loadDelayTimer -= Time.deltaTime;

            if (loadDelayTimer <= 0)
            {
                SceneLoader.LoadSceneByName(sceneToLoad);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            shouldLoadAfterFade = true;
            UIFade.instance.CallFadeOut();

            FindObjectOfType<PlayerController>().portalName = portalName;
        }
    }
}
