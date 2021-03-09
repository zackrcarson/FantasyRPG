using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Color deactivatedColor, activatedColor;

    // Cached References
    Image image = null;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ToggleButton(bool isOn)
    {
        if (!image) { image = GetComponent<Image>(); }

        if (isOn)
        {
            image.color = activatedColor;
        }
        else
        {
            image.color = deactivatedColor;
        }
    }
}
