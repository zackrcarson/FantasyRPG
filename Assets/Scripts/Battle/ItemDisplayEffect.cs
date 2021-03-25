using UnityEngine.UI;
using UnityEngine;

public class ItemDisplayEffect : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Text boostText = null;
    [SerializeField] Color healthColor, manaColor, defenseColor, strengthColor;
    [SerializeField] float lifeTime = 1f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float placementJitter = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime, 0f);
    }

    public void SetBoost(int boost, string type)
    {
        if (type == "Health")
        {
            boostText.text = "+" + boost + " hp";
            boostText.color = healthColor;
        }

        if (type == "Mana")
        {
            boostText.text = "+" + boost + " mp";
            boostText.color = manaColor;
        }

        if (type == "Defense")
        {
            boostText.text = "+" + boost + " def";
            boostText.color = defenseColor;
        }

        if (type == "Strength")
        {
            boostText.text = "+" + boost + " str";
            boostText.color = strengthColor;
        }

        transform.position += new Vector3(Random.Range(-placementJitter, +placementJitter), Random.Range(-placementJitter, +placementJitter), 0f);
    }
}
