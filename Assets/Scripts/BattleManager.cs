using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    // Config Parameters
    [Header("Battle Scene Loading and Fading In")]
    [SerializeField] GameObject battleScene = null;
    [SerializeField] SpriteRenderer battleBackground = null;
    [SerializeField] GameObject battleUI = null;
    [SerializeField] int battleMusic = 0;
    [SerializeField] float backgroundFadeTime = 1f;
    [SerializeField] float enemyAttackDelay = 1f;

    [Header("Battle Character Positions and Prefabs")]
    [SerializeField] Transform[] playerPositions = null;
    [SerializeField] Transform[] enemyPositions = null;

    [SerializeField] BattleCharacter[] playerPrefabs = null;
    [SerializeField] BattleCharacter[] enemyPrefabs = null;

    [Header("UI Buttons")]
    [SerializeField] GameObject uiButtons = null;

    // State Variables
    [HideInInspector] public bool isBattleActive = false;
    List<BattleCharacter> activeBattlers = new List<BattleCharacter>();
    int currentTurn = 0;
    bool turnWaiting = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            BattleStart(new string[] { "Eyeball", "Spider", "Skeleton" });
        }

        if (isBattleActive)
        {
            CurrentTurn();
        }
    }

    private void CurrentTurn()
    {
        if (turnWaiting)
        {
            uiButtons.SetActive(activeBattlers[currentTurn].isPlayer);

            if (!activeBattlers[currentTurn].isPlayer)
            {
                StartCoroutine(DelayedEnemyAttack());
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        currentTurn++;

        if (currentTurn >= activeBattlers.Count)
        {
            currentTurn = 0;
        }

        turnWaiting = true;

        UpdateBattle();
    }

    private void UpdateBattle()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        foreach(BattleCharacter battler in activeBattlers)
        {
            if (battler.currentHP <= 0)
            {
                battler.currentHP = 0;
            }

            if (battler.currentHP == 0)
            {
                // TODO: Handle Dead Battler
            }
            else
            {
                if (battler.isPlayer)
                {
                    allPlayersDead = false;
                }
                else
                {
                    allEnemiesDead = false;
                }
            }
        }

        if (allPlayersDead || allEnemiesDead)
        {
            if (allEnemiesDead)
            {
                // TODO: End Battle in Victory. Turn off remaining battlers before fading out
            }
            else
            {
                // TODO: End Battle in Failure. Turn off remaining battlers before fading out
            }

            StartCoroutine(FadeOutBattleScene());
        }
    }

    private void BattleStart(string[] enemiesToSpawn)
    {
        if (isBattleActive) { return; }

        isBattleActive = true;
        GameManager.instance.isBattleActive = true;

        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.x);

        StartCoroutine(FadeInBattleScene(enemiesToSpawn)); 
    }

    private IEnumerator FadeInBattleScene(string[] enemiesToSpawn)
    {
        battleScene.SetActive(true);
        AudioManager.instance.PlayMusic(battleMusic);

        while (true)
        {
            float newAlpha = Mathf.MoveTowards(battleBackground.color.a, 1f, (1f * Time.deltaTime) / backgroundFadeTime);
            battleBackground.color = new Color(battleBackground.color.r, battleBackground.color.g, battleBackground.color.b, newAlpha);

            if (battleBackground.color.a == 1f)
            {
                break;
            }

            yield return null;
        }

        LoadInPlayers();
        LoadInEnemies(enemiesToSpawn);
        StartTurns();
    }

    private IEnumerator FadeOutBattleScene()
    {
        AudioManager.instance.PlayMusic(FindObjectOfType<CameraController>().musicToPlay);

        while (true)
        {
            float newAlpha = Mathf.MoveTowards(battleBackground.color.a, 0f, (1f * Time.deltaTime) / backgroundFadeTime);
            battleBackground.color = new Color(battleBackground.color.r, battleBackground.color.g, battleBackground.color.b, newAlpha);

            if (battleBackground.color.a == 0f)
            {
                break;
            }

            yield return null;
        }

        battleScene.SetActive(false);
        GameManager.instance.isBattleActive = false;
        isBattleActive = false;
    }

    private void LoadInPlayers()
    {
        int i = 0;
        foreach (Transform playerPosition in playerPositions)
        {
            if (GameManager.instance.playerStatsArray[i].gameObject.activeInHierarchy)
            {
                foreach (BattleCharacter playerPrefab in playerPrefabs)
                {
                    if (playerPrefab.characterName == GameManager.instance.playerStatsArray[i].characterName)
                    {
                        BattleCharacter newPlayer = Instantiate(playerPrefab, playerPosition.position, playerPosition.rotation);
                        newPlayer.transform.parent = playerPosition;

                        activeBattlers.Add(newPlayer);

                        CharacterStats currentPlayer = GameManager.instance.playerStatsArray[i];
                        activeBattlers[i].currentHP = currentPlayer.currentHP;
                        activeBattlers[i].maxHP = currentPlayer.maxHP;
                        activeBattlers[i].currentMP = currentPlayer.currentMP;
                        activeBattlers[i].maxMP = currentPlayer.maxMP;
                        activeBattlers[i].strength = currentPlayer.strength;
                        activeBattlers[i].defense = currentPlayer.defense;
                        activeBattlers[i].weaponPower = currentPlayer.weaponPower;
                        activeBattlers[i].armorPower = currentPlayer.armorPower;
                    }
                }
            }

            i++;
        }
    }

    private void LoadInEnemies(string[] enemiesToSpawn)
    {
        int i = 0;
        foreach (string enemy in enemiesToSpawn)
        {
            if (enemy != "")
            {
                foreach (BattleCharacter enemyPrefab in enemyPrefabs)
                {
                    if (enemyPrefab.characterName == enemy)
                    {
                        BattleCharacter newEnemy = Instantiate(enemyPrefab, enemyPositions[i].position, enemyPositions[i].rotation);
                        newEnemy.transform.parent = enemyPositions[i];

                        activeBattlers.Add(newEnemy);
                    }
                }
            }

            i++;
        }
    }

    private void StartTurns()
    {
        turnWaiting = true;
        currentTurn = 0;

        // Randomly shuffle list of battlers
        activeBattlers = activeBattlers.OrderBy(i => Guid.NewGuid()).ToList();

        battleUI.SetActive(true);
        uiButtons.SetActive(activeBattlers[currentTurn].isPlayer);
    }

    private IEnumerator DelayedEnemyAttack()
    {
        turnWaiting = false;

        yield return new WaitForSeconds(enemyAttackDelay);

        EnemyAttack();

        yield return new WaitForSeconds(enemyAttackDelay);

        NextTurn();
    }

    private void EnemyAttack()
    {
        int selectedTarget = ChooseRandomPlayer();

        int enemyDamage = (activeBattlers[currentTurn].strength + activeBattlers[currentTurn].weaponPower) * 2;
        int playerDefense = activeBattlers[selectedTarget].defense + activeBattlers[selectedTarget].armorPower;

        activeBattlers[selectedTarget].currentHP -= (enemyDamage - playerDefense);
    }

    private int ChooseRandomPlayer()
    {
        List<int> players = new List<int>();

        int i = 0;
        foreach (BattleCharacter battler in activeBattlers)
        {
            if (battler.isPlayer && battler.currentHP > 0)
            {
                players.Add(i);
            }

            i++;
        }

        return players[UnityEngine.Random.Range(0, players.Count)]; // TODO: Throws index out or range error?? i.e. returns 5 for list of length 2........
    }
}
