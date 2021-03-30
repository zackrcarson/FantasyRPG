using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour
{
    // Config Parameters
    [Header("Administrative Parameters")]
    [SerializeField] public bool isPlayer = false;
    [SerializeField] public string characterName = null;
    [SerializeField] public string[] movesAvailable = null;
    [SerializeField] Sprite deadSprite = null;
    [SerializeField] RectTransform healthBar;
    [SerializeField] Text enemyNumber = null;
    [SerializeField] public ParticleSystem activeBattlerParticles = null;
    [SerializeField] float deathAnimationDelay = 0.8f;

    [Header("Character Stats")]
    [SerializeField] public int currentHP = 0;
    [SerializeField] public int maxHP = 0;
    [SerializeField] public int currentMP = 0;
    [SerializeField] public int maxMP = 0;
    [SerializeField] public int strength = 0;
    [SerializeField] public int defense = 0;
    [SerializeField] public int weaponPower = 0;
    [SerializeField] public int armorPower = 0;

    // Cached References
    Animator animator = null;
    SpriteRenderer spriteRenderer = null;
    Sprite aliveSprite = null;
    float healthBarSize;

    // State Variables
    //bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        aliveSprite = spriteRenderer.sprite;

        healthBarSize = healthBar.sizeDelta.x;
        healthBar.sizeDelta = new Vector2(((float)currentHP / (float)maxHP) * healthBarSize, healthBar.sizeDelta.y);
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
    }

    public IEnumerator Dead()
    {
        activeBattlerParticles.Stop();

        //isDead = true;
        animator.SetBool("isDead", true);

        yield return new WaitForSeconds(deathAnimationDelay);

        if (isPlayer)
        {
            spriteRenderer.sprite = deadSprite;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Alive()
    {
        spriteRenderer.sprite = aliveSprite;
    }

    public void ProcessHit(int damage)
    {
        currentHP -= damage;

        healthBar.sizeDelta = new Vector2(((float)currentHP / (float)maxHP) * healthBarSize, healthBar.sizeDelta.y);

        if (currentHP <= 0)
        {
            StartCoroutine(Dead());
        }
    }

    public void ShowEnemyNumber(int number)
    {
        enemyNumber.text = number.ToString();
        enemyNumber.gameObject.SetActive(true);
    }
}
