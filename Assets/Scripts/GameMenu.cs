using UnityEngine;

public class GameMenu : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject menu = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        {
            bool menuOpen = menu.activeInHierarchy;

            menu.SetActive(!menuOpen);

            GameManager.instance.gameMenuOpen = !menuOpen;
        }
    }
}
