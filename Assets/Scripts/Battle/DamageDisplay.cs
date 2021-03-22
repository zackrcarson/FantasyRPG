using UnityEngine;
using UnityEngine.UI;

public class DamageDisplay : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Text damageText = null;
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

    public void SetDamage(int damage)
    {
        damageText.text = "-" + damage + " hp";
        transform.position += new Vector3(Random.Range(-placementJitter, +placementJitter), Random.Range(-placementJitter, +placementJitter), 0f);
    }
}
