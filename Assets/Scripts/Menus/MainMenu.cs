using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Config Parameters
    [SerializeField] string newGameScene = "Countryside";
    [SerializeField] string loadGameScene = "Loading Scene";
    [SerializeField] GameObject newGamePanel = null;
    [SerializeField] GameObject continueButton = null;
    [SerializeField] Color deactiveButtonColor;
    [SerializeField] Image fadeImage = null;
    [SerializeField] float fadeSpeed = 1f;

    // State Variables
    bool shouldFadeOut = false;
    bool isContinuing = false;

    private void Start()
    {
        // Gray out Continue button if no save data found
        if (!PlayerPrefs.HasKey("Current_Scene"))
        {
            Button button = continueButton.GetComponent<Button>();
            Image buttonImage = continueButton.GetComponent<Image>();
            Text buttonText = continueButton.GetComponentInChildren<Text>();

            button.enabled = false;
            buttonImage.color = deactiveButtonColor;
            buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, deactiveButtonColor.a);
        }
    }

    void Update()
    {
        if (shouldFadeOut)
        {
            FadeOut();
        }
    }

    public void ContinueGame()
    {
        isContinuing = true;

        shouldFadeOut = true;
    }

    public void NewGame()
    {
        newGamePanel.SetActive(true);
    }

    public void BackOutNewGamePanel()
    {
        newGamePanel.SetActive(false);
    }

    public void StartNewGame()
    {
        isContinuing = false;

        shouldFadeOut = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void FadeOut()
    {
        StartCoroutine(FindObjectOfType<MainMenuAudio>().FadeOut());

        float newAlpha = Mathf.MoveTowards(fadeImage.color.a, 1f, fadeSpeed * Time.deltaTime);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);

        if (fadeImage.color.a == 1f)
        {
            shouldFadeOut = false;

            if (isContinuing)
            {
                SceneLoader.LoadSceneByName(loadGameScene);
            }
            else
            {
                PlayerPrefs.DeleteAll();

                SceneLoader.LoadSceneByName(newGameScene);
            }
        }
    }
}
