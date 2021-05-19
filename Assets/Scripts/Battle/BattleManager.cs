using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    // Config Parameters
    [Header("Battle Scene Loading and Fading In")]
    [SerializeField] GameObject battleScene = null;
    [SerializeField] SpriteRenderer battleBackground = null;
    [SerializeField] GameObject battleUI = null;
    [SerializeField] Sprite[] backgroundImages = null;
    [SerializeField] float backgroundFadeTime = 1f;
    [SerializeField] float battleEndDelay1 = 1.5f;
    [SerializeField] float battleEndDelay2 = 0.5f;
    [SerializeField] float battleEndDelay3 = 1.5f;

    [Header("Battle Settings")]
    [SerializeField] int percentChanceToFlee = 35;
    [SerializeField] float enemyAttackDelay = 1.25f;
    [SerializeField] BattleMove[] movesList = null;
    [SerializeField] float damageRandomFactorMinimum = 0.9f;
    [SerializeField] float damageRandomFactorMaximum = 1.1f;

    [Header("Effects")]
    [SerializeField] GameObject effectsParent = null;
    [SerializeField] GameObject enemyAttackEffect = null;
    [SerializeField] GameObject bossAttackEffect = null;
    [SerializeField] GameObject playerItemEffectHP = null;
    [SerializeField] GameObject playerItemEffectMP = null;
    [SerializeField] DamageDisplay damageDisplay = null;
    [SerializeField] DamageDisplay dodgeDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;
    [SerializeField] ItemDisplayEffect itemDisplay = null;

    [Header("Battle Character Positions and Prefabs")]
    [SerializeField] Transform[] playerPositions = null;
    [SerializeField] Transform[] enemyPositions = null;
    [SerializeField] BattleCharacter[] playerPrefabs = null;
    [SerializeField] BattleCharacter[] enemyPrefabs = null;

    [Header("Audio")]
    [SerializeField] int beepSound = 5;
    [SerializeField] int itemSlotSound = 8;
    [SerializeField] int battleIntro = 33;
    [SerializeField] int battleMusic = 0;
    [SerializeField] int gameOverMusic = 8;
    [SerializeField] int victorySound = 9;
    [SerializeField] int fleeSound = 10;
    [SerializeField] int fleeSuccessSound = 41;
    [SerializeField] int failToFleeSound = 39;
    [SerializeField] int errorSound = 21;
    [SerializeField] int gameOverSound = 40;
    [SerializeField] int dodgeSound = 43;
    [SerializeField] public int enemyDeathSound = 44;
    [SerializeField] public int playerDeathSound = 45;

    [Header("UI")]
    [Header("Misc. UI")]
    [SerializeField] GameObject uiButtons = null;
    [SerializeField] public BattleNotification battleNotification = null;

    [Header("Stats Menu")]
    [SerializeField] Text[] playerNames = null;
    [SerializeField] Text[] playerHPs = null;
    [SerializeField] Text[] playerMPs = null;
    [SerializeField] Color deadPlayerTextColor;

    [Header("Items Menu")]
    [SerializeField] public GameObject itemsMenu = null;
    [SerializeField] ItemButton[] itemButtons = null;
    ButtonToggle[] itemButtonToggles = null;
    [SerializeField] Text itemName = null;
    [SerializeField] Text itemDescription = null;
    [SerializeField] GameObject useButton = null;
    [SerializeField] GameObject itemCharacterSelectionMenu = null;
    [SerializeField] Text[] itemCharacterNames = null;

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
    Color defaultNameColor;
    Color defaultHPColor;
    Color defaultMPColor;

    // State Variables
    [HideInInspector] public bool isBattleActive = false;
    [HideInInspector] public List<BattleCharacter> activeBattlers = new List<BattleCharacter>();
    [HideInInspector] public int currentTurn = 0;
    [HideInInspector] public bool isCastingSpell = false;
    [HideInInspector] public int currentSpellCost = 0;
    [HideInInspector] public int selectedCharacterTurnSlot = 0;
    List<int> activePlayerBattlerSlots = new List<int>();
    Item activeItem = null;
    bool turnWaiting = false;
    bool isInventoryEmpty = false;
    bool battleEnding = false;
    bool cannotFlee = false;
    bool isBoss = false;

    [HideInInspector] public int battleExp = 0;
    [HideInInspector] public int battleGold = 0;
    [HideInInspector] public string[] battleRewards = null;
    [HideInInspector] public int[] battleRewardNumbers = null;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        enoughManaNameColor = battleMagicButtons[0].nameText.color;
        enoughManaCostColor = battleMagicButtons[0].costText.color;
        defaultNameColor = playerNames[0].color;
        defaultHPColor = playerHPs[0].color;
        defaultMPColor = playerMPs[0].color;

        itemButtonToggles = new ButtonToggle[itemButtons.Length];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtonToggles[i] = itemButtons[i].gameObject.GetComponent<ButtonToggle>();
        }
    }

    private void Update()
    {
        if (isBattleActive)
        {
            CurrentTurn();
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee, bool boss)
    {
        if (isBattleActive) { return; }

        GameMenu.instance.DeactivateIcons();

        cannotFlee = setCannotFlee;
        isBoss = boss;

        LoadBackground();

        battleEnding = false;
        isBattleActive = true;
        GameManager.instance.isBattleActive = true;

        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.x);

        StartCoroutine(FadeInBattleScene(enemiesToSpawn));
    }

    private void LoadBackground()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Countryside")
        {
            battleBackground.sprite = backgroundImages[0];
        }
        else if (sceneName == "Cave")
        {
            battleBackground.sprite = backgroundImages[1];
        }
        else if (sceneName == "Town")
        {
            battleBackground.sprite = backgroundImages[2];
        }
        else
        {
            battleBackground.sprite = backgroundImages[3];
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
    }

    private void NextTurn()
    {
        activeBattlers[currentTurn].activeBattlerParticles.Stop();

        currentTurn++;

        if (currentTurn >= activeBattlers.Count)
        {
            currentTurn = 0;
        }

        if (activeBattlers[currentTurn].currentHP <= 0)
        {
            activeBattlers[currentTurn].activeBattlerParticles.Stop();
        }
        else
        {
            activeBattlers[currentTurn].activeBattlerParticles.Play();
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
                activeBattlers[currentTurn].activeBattlerParticles.Stop();
            }

            if (battler.currentHP > 0)
            {
                if (battler.isPlayer)
                {
                    battler.Alive();
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
            EndBattle(allEnemiesDead, false);
        }
        else
        {
            while(activeBattlers[currentTurn].currentHP == 0)
            {
                activeBattlers[currentTurn].activeBattlerParticles.Stop();

                currentTurn++;
                if (currentTurn >= activeBattlers.Count)
                {
                    currentTurn = 0;
                }
            }
                activeBattlers[currentTurn].activeBattlerParticles.Play();
        }
    }

    private IEnumerator FadeInBattleScene(string[] enemiesToSpawn)
    {
        AudioManager.instance.PlaySFX(battleIntro);

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

    private void EndBattle(bool isVictorious, bool didFlee)
    {
        if (isVictorious)
        {
            if (!battleEnding)
            {
                StartCoroutine(FadeOutBattleScreenVictory(didFlee));
            }
        }
        else
        {
            AudioManager.instance.PlaySFX(gameOverSound);
            StartCoroutine(FadeOutBattleScreenGameOver());
        }

        cannotFlee = false;
        isBoss = false;
    }

    private IEnumerator FadeOutBattleScreenVictory(bool didFlee)
    {
        battleEnding = true;

        if (didFlee)
        {
            AudioManager.instance.PlaySFX(fleeSound);
        }
        else
        {
            yield return new WaitForSeconds(battleEndDelay1);
            AudioManager.instance.PlaySFX(victorySound);
        }
        
        AudioManager.instance.PlayMusic(FindObjectOfType<CameraController>().musicToPlay);

        uiButtons.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemsMenu.SetActive(false);
        battleUI.SetActive(false);

        yield return new WaitForSeconds(battleEndDelay2);

        UIFade.instance.CallFadeOut();

        yield return new WaitForSeconds(battleEndDelay3);

        RefreshGameManagerStats();
        ResetBattleManager(didFlee);

        UIFade.instance.CallFadeIn();
    }

    private IEnumerator FadeOutBattleScreenGameOver()
    {
        isBattleActive = false;
        uiButtons.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemsMenu.SetActive(false);
        battleUI.SetActive(false);

        yield return new WaitForSeconds(battleEndDelay1);
        
        UIFade.instance.CallFadeOut();
        AudioManager.instance.PlayMusic(gameOverMusic);

        yield return new WaitForSeconds(battleEndDelay3);

        battleScene.SetActive(false);

        SceneLoader.LoadSceneByName("Game Over");
        UIFade.instance.CallFadeIn();
    }

    public void ResetBattleManager(bool isFleeing)
    {
        foreach (BattleCharacter battler in activeBattlers)
        {
            Destroy(battler.gameObject);
        }

        battleBackground.color = new Color(battleBackground.color.r, battleBackground.color.g, battleBackground.color.b, 0f);
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;
        isCastingSpell = false;
        currentSpellCost = 0;
        selectedCharacterTurnSlot = 0;
        activePlayerBattlerSlots.Clear();
        activeItem = null;
        turnWaiting = false;
        isInventoryEmpty = false;

        if (isFleeing)
        {
            GameManager.instance.isBattleActive = false;
            isBattleActive = false;
            GameMenu.instance.ActivateIcons();
        }
        else
        {
            BattleReward.instance.OpenRewardScreen(battleExp, battleGold, battleRewards, battleRewardNumbers);
        }
    }

    private void RefreshGameManagerStats()
    {
        foreach (BattleCharacter battler in activeBattlers)
        {
            if (battler.isPlayer)
            {
                foreach (CharacterStats playerStat in GameManager.instance.playerStatsArray)
                {
                    if (battler.characterName == playerStat.characterName)
                    {
                        if (battler.currentHP <= 0)
                        {
                            playerStat.currentHP = 1;
                        }
                        else
                        {
                            playerStat.currentHP = battler.currentHP;
                        }

                        playerStat.currentMP = battler.currentMP;
                        playerStat.maxHP = battler.maxHP;
                        playerStat.maxMP = battler.maxMP;
                        playerStat.defense = battler.defense;
                        playerStat.strength = battler.strength;
                        playerStat.armorPower = battler.armorPower;
                        playerStat.weaponPower = battler.weaponPower;
                    }
                }
            }
        }
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

        activeBattlers[currentTurn].activeBattlerParticles.Play();
    }

    private IEnumerator DelayedEnemyAttack()
    {
        turnWaiting = false;
        
        yield return new WaitForSeconds(enemyAttackDelay);

        if (activeBattlers[currentTurn].currentHP > 0)
        {
            EnemyAttack();
        }

        yield return new WaitForSeconds(enemyAttackDelay);

        NextTurn();
    }

    private void EnemyAttack()
    {
        int selectedTarget = ChooseRandomPlayer();

        bool canDodge = CanDodge(selectedTarget);

        if (canDodge)
        {
            AudioManager.instance.PlaySFX(dodgeSound);

            if (isBoss)
            {
                Instantiate(bossAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);
            }
            else
            {
                Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);
            }

            Instantiate(enemyAttackEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);

            activeBattlers[currentTurn].Attack();
            activeBattlers[selectedTarget].Dodge();

            if (isBoss && !activeBattlers[selectedTarget].isPlayer)
            {
                Vector3 displacementVector = new Vector3(-2f, 0f, 0f);
                Instantiate(dodgeDisplay, activeBattlers[selectedTarget].transform.position + displacementVector, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
            }
            else
            {
                Instantiate(dodgeDisplay, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
            }
        }
        else
        {
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

            activeBattlers[currentTurn].Attack();
            if (isBoss)
            {
                Instantiate(bossAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);
            }
            else
            {
                Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);
            }

            DealDamage(selectedTarget, movePower);
        }

    }

    private bool CanDodge(int selectedTarget)
    {
        int percentChanceToDodge = activeBattlers[selectedTarget].dodgeChance;
        int dodgeRoll = UnityEngine.Random.Range(0, 100);

        if (dodgeRoll <= percentChanceToDodge)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        int damageToGive = 0;
        if (GameManager.instance.godMode && activeBattlers[targetNumber].isPlayer)
        {
            damageToGive = 0;
        }
        else
        {
            float offensivePower = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].weaponPower;
            float defensivePower = activeBattlers[targetNumber].defense + activeBattlers[targetNumber].armorPower;

            float totalDamage = (offensivePower / defensivePower) * movePower * UnityEngine.Random.Range(damageRandomFactorMinimum, damageRandomFactorMaximum);

            damageToGive = Mathf.RoundToInt(totalDamage);
        }

        activeBattlers[targetNumber].ProcessHit(damageToGive);

        
        if (isBoss && !activeBattlers[targetNumber].isPlayer)
        {
            Vector3 displacementVector = new Vector3(-2f, 0f, 0f);

            Instantiate(damageDisplay, activeBattlers[targetNumber].transform.position + displacementVector, activeBattlers[targetNumber].transform.rotation, effectsParent.transform).SetDamage(damageToGive);
        }
        else
        {
            Instantiate(damageDisplay, activeBattlers[targetNumber].transform.position, activeBattlers[targetNumber].transform.rotation, effectsParent.transform).SetDamage(damageToGive);
        }

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

                if (playerData.currentHP <= 0)
                {
                    playerNames[i].color = deadPlayerTextColor;
                    playerHPs[i].color = deadPlayerTextColor;
                    playerMPs[i].color = deadPlayerTextColor;
                }
                else
                {
                    playerNames[i].color = defaultNameColor;
                    playerHPs[i].color = defaultHPColor;
                    playerMPs[i].color = defaultMPColor;
                }
            }
            else
            {
                playerNames[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        bool canDodge = CanDodge(selectedTarget);

        if (canDodge)
        {
            AudioManager.instance.PlaySFX(dodgeSound);

            Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);

            if (isBoss)
            {
                Instantiate(bossAttackEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
            }
            else
            {
                Instantiate(enemyAttackEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
            }

            activeBattlers[currentTurn].Attack();
            activeBattlers[selectedTarget].Dodge();

            if (isBoss && !activeBattlers[selectedTarget].isPlayer)
            {
                Vector3 displacementVector = new Vector3(-2f, 0f, 0f);
                Instantiate(dodgeDisplay, activeBattlers[selectedTarget].transform.position + displacementVector, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
            }
            else
            {
                Instantiate(dodgeDisplay, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
            }
        }
        else
        {
            int movePower = 0;
            foreach (BattleMove move in movesList)
            {
                if (move.moveName == moveName)
                {
                    if (isBoss)
                    {
                        Vector3 displacementVector = new Vector3(-2f, 0f, 0f);

                        Instantiate(move.effect, activeBattlers[selectedTarget].transform.position + displacementVector, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
                    }
                    else
                    {
                        Instantiate(move.effect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation, effectsParent.transform);
                    }
                    movePower = move.movePower;
                }
            }

            activeBattlers[currentTurn].Attack();
            Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);

            if (isCastingSpell)
            {
                Instantiate(manaDisplay, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform).SetMana(currentSpellCost);

                isCastingSpell = false;
                currentSpellCost = 0;
            }

            DealDamage(selectedTarget, movePower);


        }
        
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

        Dictionary<string, int> enemyNames = new Dictionary<string, int>();
        i = 0;
        foreach (BattleTargetButton targetButton in battleTargetButtons)
        {
            targetButton.gameObject.SetActive(enemies.Count > i);

            if (enemies.Count > i && activeBattlers[enemies[i]].currentHP > 0)
            {
                targetButton.moveName = moveName;
                targetButton.activeBattlerTarget = enemies[i];

                string currentName = activeBattlers[enemies[i]].characterName;
                if (enemyNames.ContainsKey(currentName))
                {
                    enemyNames[currentName]++;
                    targetButton.targetName.text = activeBattlers[enemies[i]].characterName + " " + enemyNames[currentName];
                    activeBattlers[enemies[i]].ShowEnemyNumber(enemyNames[currentName]);
                }
                else
                {
                    enemyNames.Add(currentName, 1);
                    targetButton.targetName.text = activeBattlers[enemies[i]].characterName;
                }
            }
            else
            {
                targetButton.gameObject.SetActive(false);
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
    public void PlayItemButtonBeep()
    {
        AudioManager.instance.PlaySFX(itemSlotSound);
    }

    public void FleeBattle()
    {
        if (cannotFlee)
        {
            AudioManager.instance.PlaySFX(errorSound);
            battleNotification.notificationMessage.text = "You can't even attempt to escape this battle, you would get eaten!";
            battleNotification.Activate();
        }
        else
        {
            StartCoroutine(FleeBattleWithDelay());
        }
    }

    private IEnumerator FleeBattleWithDelay()
    {
        int fleeRoll = UnityEngine.Random.Range(0, 100);

        if (fleeRoll <= percentChanceToFlee)
        {
            battleNotification.notificationMessage.text = "You successfully escaped the battle!";
            battleNotification.Activate();
            AudioManager.instance.PlaySFX(fleeSuccessSound);

            yield return new WaitForSeconds(battleNotification.awakeTime);

            EndBattle(true, true);
        }
        else
        {
            AudioManager.instance.PlaySFX(failToFleeSound);
            battleNotification.notificationMessage.text = "You failed to flee!";
            battleNotification.Activate();

            yield return new WaitForSeconds(battleNotification.awakeTime);
            NextTurn();
        }
    }

    public void OpenItemsMenu()
    {
        itemsMenu.SetActive(true);

        ShowItems();
        SelectFirstItem();
    }

    public void CloseItemsMenu()
    {
        itemsMenu.SetActive(false);
        itemCharacterSelectionMenu.SetActive(false);
    }

    public void ShowItems()
    {
        GameManager.instance.SortItems();

        int i = 0;
        foreach (ItemButton itemButton in itemButtons)
        {
            itemButton.buttonValue = i;

            string currentItem = GameManager.instance.itemsHeld[i];
            int currentItemAmount = GameManager.instance.numberOfItems[i];

            if (currentItem == "" || currentItemAmount == 0 || !GameManager.instance.GetItemDetails(currentItem).isItem)
            {
                itemButton.buttonImage.gameObject.SetActive(false);
                itemButton.amountText.text = "";
                itemButton.gameObject.SetActive(false);
            }
            else
            {
                itemButton.gameObject.SetActive(true);
                itemButton.buttonImage.gameObject.SetActive(true);

                itemButton.buttonImage.sprite = GameManager.instance.GetItemDetails(currentItem).itemSprite;
                itemButton.amountText.text = currentItemAmount.ToString();
            }

            i++;
        }

        isInventoryEmpty = CheckIfBattleInventoryIsEmpty();
        if (isInventoryEmpty)
        {
            itemName.text = "Nothing";
            itemDescription.text = "Your inventory doesn't have anything you can use right now!";

            useButton.SetActive(false);
        }
    }

    private bool CheckIfBattleInventoryIsEmpty()
    {
        bool isInvEmpty = true;
        int[] itemNumbers = GameManager.instance.numberOfItems;
        string[] itemNames = GameManager.instance.itemsHeld;

        int i = 0;
        foreach (int number in itemNumbers)
        {
            Item currentItem = GameManager.instance.GetItemDetails(itemNames[i]);

            if (number > 0 && currentItem.isItem)
            {
                isInvEmpty = false;
            }

            i++;
        }

        return isInvEmpty;
    }

    public void SelectItem(Item selectedItem)
    {
        if (!isInventoryEmpty)
        {
            itemCharacterSelectionMenu.SetActive(false);

            // Highlight selected item, un-highlight all others
            string selectedItemString = selectedItem.itemName;
            int i = 0;
            foreach (ItemButton itemButton in itemButtons)
            {
                string currentItemString = GameManager.instance.itemsHeld[i];

                itemButtonToggles[i].ToggleButton(currentItemString == selectedItemString);

                i++;
            }

            activeItem = selectedItem;

            if (activeItem.isItem)
            {
                useButton.gameObject.SetActive(true);
            }

            if (activeItem.isWeapon || activeItem.isArmor)
            {
                useButton.gameObject.SetActive(false);
            }

            itemName.text = activeItem.itemName;
            itemDescription.text = activeItem.description;
        }
        else
        {
            itemName.text = "Nothing";
            itemDescription.text = "Your inventory doesn't have anything you can use right now!";

            useButton.SetActive(false);
        }
    }

    public void SelectFirstItem()
    {
        // Select the first item
        string firstItemName = GameManager.instance.itemsHeld[0];
        int firstItemNumber = GameManager.instance.numberOfItems[0];
        if (firstItemName != "" && firstItemNumber != 0)
        {
            Item firstItem = GameManager.instance.GetItemDetails(firstItemName);
            SelectItem(firstItem);
        }
    }

    public void OpenItemPlayerChoicePanel()
    {
        itemCharacterSelectionMenu.SetActive(true);

        List<BattleCharacter> activePlayers = new List<BattleCharacter>();
        activePlayerBattlerSlots = new List<int>();

        int j = 0;
        foreach (BattleCharacter battler in activeBattlers)
        {
            if (battler.isPlayer)
            {
                activePlayers.Add(battler);
                activePlayerBattlerSlots.Add(j);
            }

            j++;
        }

        int i = 0;
        foreach (Text playerButton in itemCharacterNames)
        {
            if (i >= activePlayers.Count || activePlayers[i].currentHP <= 0)
            {
                itemCharacterNames[i].transform.parent.gameObject.SetActive(false);
            }
            else
            {
                itemCharacterNames[i].transform.parent.gameObject.SetActive(true);
                itemCharacterNames[i].text = activePlayers[i].characterName;
            }

            i++;
        }
    }

    public void CloseItemPlayerChoicePanel()
    {
        itemCharacterSelectionMenu.SetActive(false);
    }

    public void UseItem(int selectedCharacter)
    {
        string result = activeItem.UseItem(activePlayerBattlerSlots[selectedCharacter]);

        if (result == "True")
        {
            activeBattlers[activePlayerBattlerSlots[selectedCharacter]].ProcessHit(0);
            UpdateUIStats();

            CloseItemPlayerChoicePanel();
            CloseItemsMenu();

            NextTurn();
        }
        else
        {
            if (result == "False-HP")
            {
                battleNotification.notificationMessage.text = activeBattlers[activePlayerBattlerSlots[selectedCharacter]].characterName + " is at full HP! You can't heal them any more.";
                battleNotification.Activate();
            }
            else if (result == "False-MP")
            {
                battleNotification.notificationMessage.text = activeBattlers[activePlayerBattlerSlots[selectedCharacter]].characterName + " is at full MP! You can't restore any more.";
                battleNotification.Activate();
            }
        }
    }

    public void DisplayItemBoost(int selectedCharacter, int amount, string type)
    {
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation, effectsParent.transform);

        GameObject effectToUse = playerItemEffectHP;

        if (type == "Health")
        {
            effectToUse = playerItemEffectHP;
        }
        else if (type == "Mana")
        {
            effectToUse = playerItemEffectMP;
        }

        Instantiate(effectToUse, activeBattlers[selectedCharacter].transform.position, activeBattlers[selectedCharacter].transform.rotation, effectsParent.transform);

        Instantiate(itemDisplay, activeBattlers[selectedCharacter].transform.position, activeBattlers[selectedCharacter].transform.rotation, effectsParent.transform).SetBoost(amount, type);
    }
}
