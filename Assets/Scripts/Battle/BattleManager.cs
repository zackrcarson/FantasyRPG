using UnityEngine;
using UnityEngine.UI;
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

    [Header("Battle Settings")]
    [SerializeField] float enemyAttackDelay = 1.25f;
    [SerializeField] BattleMove[] movesList = null;
    [SerializeField] float damageRandomFactorMinimum = 0.9f;
    [SerializeField] float damageRandomFactorMaximum = 1.1f;

    [Header("Effects")]
    [SerializeField] GameObject effectsParent = null;
    [SerializeField] GameObject enemyAttackEffect = null;
    [SerializeField] DamageDisplay damageDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;

    [Header("Battle Character Positions and Prefabs")]
    [SerializeField] Transform[] playerPositions = null;
    [SerializeField] Transform[] enemyPositions = null;
    [SerializeField] BattleCharacter[] playerPrefabs = null;
    [SerializeField] BattleCharacter[] enemyPrefabs = null;

    [Header("UI")]
    [Header("Misc. UI")]
    [SerializeField] int beepSound = 5;
    [SerializeField] GameObject uiButtons = null;
    [SerializeField] public BattleNotification battleNotification = null;

    [Header("Stats Menu")]
    [SerializeField] Text[] playerNames = null;
    [SerializeField] Text[] playerHPs = null;
    [SerializeField] Text[] playerMPs = null;

    [Header("Target Menu")]
    [SerializeField] public GameObject targetMenu = null;
    [SerializeField] BattleTargetButton[] battleTargetButtons = null;

    [Header("Magic Menu")]
    [SerializeField] public GameObject magicMenu = null;
    [SerializeField] BattleMagicButton[] battleMagicButtons = null;
    [SerializeField] Color notEnoughManaNameColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] Color notEnoughManaCostColor = new Color(1f, 1f, 1f, 1f);

    // Cached References
    Color enoughManaNameColor = new Color(1f, 1f, 1f, 1f);
    Color enoughManaCostColor = new Color(1f, 1f, 1f, 1f);

    // State Variables
    [HideInInspector] public bool isBattleActive = false;
    [HideInInspector] public List<BattleCharacter> activeBattlers = new List<BattleCharacter>();
    [HideInInspector] public int currentTurn = 0;
    [HideInInspector] public bool isCastingSpell = false;
    [HideInInspector] public int currentSpellCost = 0;
    bool turnWaiting = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        enoughManaNameColor = battleMagicButtons[0].nameText.color;
        enoughManaCostColor = battleMagicButtons[0].costText.color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            BattleStart(new string[] { "Eyeball", "Spider", "Skeleton" , "Spider"});
        }

        if (isBattleActive)
        {
            CurrentTurn();
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
        UpdateUIStats();
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
        else
        {
            while(activeBattlers[currentTurn].currentHP == 0)
            {
                currentTurn++;
                if (currentTurn >= activeBattlers.Count)
                {
                    currentTurn = 0;
                }
            }
        }
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
        UpdateUIStats();
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

        int selectedAttack = UnityEngine.Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        int movePower = 0;
        foreach(BattleMove move in movesList)
        {
            if (move.moveName == activeBattlers[currentTurn].movesAvailable[selectedAttack])
            {
                Instantiate(move.effect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);

                movePower = move.movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);

        DealDamage(selectedTarget, movePower);
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

    private void DealDamage(int targetNumber, int movePower)
    {
        float offensivePower = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].weaponPower;
        float defensivePower = activeBattlers[targetNumber].defense + activeBattlers[targetNumber].armorPower;

        float totalDamage = (offensivePower / defensivePower) * movePower * UnityEngine.Random.Range(damageRandomFactorMinimum, damageRandomFactorMaximum);

        int damageToGive = Mathf.RoundToInt(totalDamage);

        activeBattlers[targetNumber].currentHP -= damageToGive;

        Instantiate(damageDisplay, activeBattlers[targetNumber].transform.position, activeBattlers[targetNumber].transform.rotation, effectsParent.transform).SetDamage(damageToGive);

        UpdateUIStats();
    }

    private void UpdateUIStats()
    {
        List<BattleCharacter> activePlayers = new List<BattleCharacter>();

        foreach (BattleCharacter battler in activeBattlers)
        {
            if (battler.isPlayer)
            {
                activePlayers.Add(battler);
            }
        }

        for (int i = 0; i < playerNames.Length; i++)
        {
            if (activePlayers.Count > i)
            {
                BattleCharacter playerData = activePlayers[i];

                playerNames[i].gameObject.SetActive(true);
                playerNames[i].text = playerData.characterName;
                playerHPs[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue).ToString() + "/" + playerData.maxHP;
                playerMPs[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue).ToString() + "/" + playerData.maxMP;
            }
            else
            {
                playerNames[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        int movePower = 0;
        foreach (BattleMove move in movesList)
        {
            if (move.moveName == moveName)
            {
                Instantiate(move.effect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
                movePower = move.movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);

        if (isCastingSpell)
        {
            Instantiate(manaDisplay, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform).SetMana(currentSpellCost);

            isCastingSpell = false;
            currentSpellCost = 0;
        }

        DealDamage(selectedTarget, movePower);

        uiButtons.SetActive(false);
        targetMenu.SetActive(false);

        NextTurn();
    }

    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);

        List<int> enemies = new List<int>();
        int i = 0;
        foreach (BattleCharacter battler in activeBattlers)
        {
            if (!battler.isPlayer)
            {
                enemies.Add(i);
            }

            i++;
        }

        i = 0;
        foreach (BattleTargetButton targetButton in battleTargetButtons)
        {
            targetButton.gameObject.SetActive(enemies.Count > i);

            if (enemies.Count > i)
            {
                targetButton.moveName = moveName;
                targetButton.activeBattlerTarget = enemies[i];
                targetButton.targetName.text = activeBattlers[enemies[i]].characterName;
            }

            i++;
        }
    }

    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        int i = 0;
        foreach (BattleMagicButton magicButton in battleMagicButtons)
        {
            if (activeBattlers[currentTurn].movesAvailable.Length > i)
            {
                magicButton.gameObject.SetActive(true);

                magicButton.spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButton.nameText.text = magicButton.spellName;

                foreach (BattleMove move in movesList)
                {
                    if (move.moveName == magicButton.spellName)
                    {
                        magicButton.spellCost = move.moveCost;
                        magicButton.costText.text = magicButton.spellCost.ToString() + "mp";

                        int currentMana = activeBattlers[currentTurn].currentMP;

                        // FIX AND TEST. Make available color different, more blue? deactive more gray?
                        if (magicButton.spellCost <= currentMana)
                        {
                            magicButton.nameText.color = enoughManaNameColor;
                            magicButton.costText.color = enoughManaCostColor;
                        }
                        else
                        {
                            magicButton.nameText.color = notEnoughManaNameColor;
                            magicButton.costText.color = notEnoughManaCostColor;
                        }
                    }
                }
            }
            else
            {
                magicButton.gameObject.SetActive(false);
            }

            i++;
        }
    }

    public void PlayButtonBeep()
    {
        AudioManager.instance.PlaySFX(beepSound);
    }
}
