using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject continueButton = null;
    [SerializeField] Color deactiveButtonColor;
    [SerializeField] Image fadeImage = null;
    [SerializeField] float fadeSpeed = 1f;
    [SerializeField] int errorSound = 21;
    [SerializeField] int beepSound = 5;
    [SerializeField] int loadSound = 23;
    [SerializeField] int quitSound = 42;

    // State Variables
    bool shouldFadeOut = false;
    bool canLoad = false;

    private void Start()
    {
        FindObjectOfType<PlayerController>().GetComponentInChildren<SpriteRenderer>().enabled = false;

        // Gray out Continue button if no save data found
        if (!PlayerPrefs.HasKey("Current_Scene"))
        {
            canLoad = false;
            //Button button = continueButton.GetComponent<Button>();
            Image buttonImage = continueButton.GetComponent<Image>();
            Text buttonText = continueButton.GetComponentInChildren<Text>();

            //button.enabled = false;
            buttonImage.color = deactiveButtonColor;
            buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, deactiveButtonColor.a);
        }
        else
        {
            canLoad = true;
        }
    }

    void Update()
    {
        if (shouldFadeOut)
        {
            FadeOut();
        }
    }

    public void QuitToMain()
    {
        AudioManager.instance.PlaySFX(quitSound);
        StartCoroutine(FadeAndQuit());
    }

    private IEnumerator FadeAndQuit()
    {
        shouldFadeOut = true;
        UIFade.instance.CallFadeOut();

        yield return new WaitForSeconds(1.5f);

        Destroy(FindObjectOfType<PlayerController>().gameObject);
        Destroy(GameManager.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);

        SceneLoader.LoadSceneByName("Main Menu");
    }

    public void LoadLastSave()
    {
        if (canLoad)
        {
            AudioManager.instance.PlaySFX(loadSound);

            StartCoroutine(FadeInLoadAndFadeOut());
        }
        else
        {
            AudioManager.instance.PlaySFX(errorSound);
        }
    }

    private IEnumerator FadeInLoadAndFadeOut()
    {
        GameManager.instance.fadingScreen = true;
        shouldFadeOut = true;
        UIFade.instance.CallFadeOut();

        yield return new WaitForSeconds(1.5f);
        BattleManager.instance.ResetBattleManager(true);
        FindObjectOfType<PlayerController>().GetComponentInChildren<SpriteRenderer>().enabled = true;

        GameManager.instance.LoadData();
        QuestManager.instance.LoadQuestData();
    }

    private void FadeOut()
    {
        float newAlpha = Mathf.MoveTowards(fadeImage.color.a, 1f, fadeSpeed * Time.deltaTime);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);

        if (fadeImage.color.a == 1f)
        {
            shouldFadeOut = false;
        }
    }

    public void PlayButtonBeep()
    {
        AudioManager.instance.PlaySFX(beepSound);
    }
}
