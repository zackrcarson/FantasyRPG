using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    // Config Parameters
    [SerializeField] BattleType[] potentialBattles = null;
    [SerializeField] bool activateOnEnter = false;
    [SerializeField] bool activateOnStay = true;
    [SerializeField] bool activateOnExit = false;
    [SerializeField] bool deactivateAfterStarting = false;
    [SerializeField] float randomTimeMin = 2f;
    [SerializeField] float randomTimeMax = 10f;

    // State Variables   
    bool isInBattleZone = false;
    float battleCounter = 10f;

    // Cached References
    PlayerController player = null;

    private void Start()
    {
        battleCounter = Random.Range(randomTimeMin, randomTimeMax);
    }

    private void Update()
    {
        if (!player) { player = FindObjectOfType<PlayerController>(); }

        if (isInBattleZone && player.canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                battleCounter -= Time.deltaTime;

                if (battleCounter <= 0)
                {
                    battleCounter = Random.Range(randomTimeMin, randomTimeMax);

                    StartBattle();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            if (activateOnEnter)
            {
                StartBattle();
            }
            else
            {
                isInBattleZone = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            if (activateOnExit)
            {
                StartBattle();
            }
            else
            {
                isInBattleZone = false;
            }
        }
    }

    private void StartBattle()
    {
        int selectedBattle = Random.Range(0, potentialBattles.Length);
        BattleType battle = potentialBattles[selectedBattle];

        BattleManager.instance.battleExp = battle.rewardExp;
        BattleManager.instance.battleGold = battle.rewardGold;
        BattleManager.instance.battleRewards = battle.rewardItems;
        BattleManager.instance.battleRewardNumbers = battle.rewardItemNumbers;

        BattleManager.instance.BattleStart(battle.enemies);

        if (deactivateAfterStarting)
        {
            gameObject.SetActive(false);
        }
    }
}
