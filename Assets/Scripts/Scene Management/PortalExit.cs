using UnityEngine;

public class PortalExit : MonoBehaviour
{
    // Config Params
    [SerializeField] string portalName = null;
    [SerializeField][Tooltip("Scene Description - Portal Number")] string sceneToLoad = null;
    [SerializeField] float loadDelayTimer = 1f;
    [SerializeField] bool shouldLoadAfterFade = false;
    [SerializeField] bool shouldPlaySound = false;
    [SerializeField] int soundToPlay = 32;

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
            if (shouldPlaySound)
            {
                AudioManager.instance.PlaySFX(soundToPlay);
            }

            shouldLoadAfterFade = true;

            GameManager.instance.fadingScreen = true;
            UIFade.instance.CallFadeOut();

            FindObjectOfType<PlayerController>().portalName = portalName;
        }
    }
}
