using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public static UIFade instance;

    // Config Params
    [SerializeField] float fadeSpeed = 1f;

    // Cached References
    Image fadeImage = null;

    // State Variables
    public bool shouldFadeOut = false;
    public bool shouldFadeIn = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeImage = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFadeOut)
        {
            FadeOut();
        }

        if (shouldFadeIn)
        {
            FadeIn();
        }
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

    private void FadeIn()
    {
        float newAlpha = Mathf.MoveTowards(fadeImage.color.a, 0f, fadeSpeed * Time.deltaTime);

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);

        if (fadeImage.color.a == 0f)
        {
            shouldFadeIn = false;
        }
    }

    public void CallFadeOut()
    {
        shouldFadeOut = true;
        shouldFadeIn = false;
    }

    public void CallFadeIn()
    {
        shouldFadeOut = false;
        shouldFadeIn = true;
    }
}
