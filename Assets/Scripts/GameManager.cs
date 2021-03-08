using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Config Parameters
    [SerializeField] public CharacterStats[] playerStatsArray = null;

    // Cached References
    PlayerController player = null;

    // State Variables
    [HideInInspector] public bool gameMenuOpen = false;
    [HideInInspector] public bool dialogueActive = false;
    [HideInInspector] public bool fadingScreen = false;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpen || dialogueActive || fadingScreen)
        {
            player.CanMove(false);
        }
        else
        {
            player.CanMove(true);
        }
    }
}
