using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float verticalWalkSpeed = 1f;
    [SerializeField] float horizontalWalkSpeed = 1f;

    // State Variables
    [HideInInspector] public string portalName = null;

    // Cached References
    Rigidbody2D rigidBody = null;
    Animator animator = null;

    private void Awake()
    {
        SingletonPattern();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void SingletonPattern()
    {
        int numPlayers = FindObjectsOfType<PlayerController>().Length;

        if (numPlayers > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float xThrow = Input.GetAxisRaw("Horizontal");
        float yThrow = Input.GetAxisRaw("Vertical");

        float xSpeed = xThrow * horizontalWalkSpeed;
        float ySpeed = yThrow * verticalWalkSpeed;

        rigidBody.velocity = new Vector2(xSpeed, ySpeed);

        animator.SetFloat("moveX", xThrow);
        animator.SetFloat("moveY", yThrow);

        if (xThrow == 1 || xThrow == -1 || yThrow == 1 || yThrow == -1)
        {
            animator.SetFloat("lastMoveX", xThrow);
            animator.SetFloat("lastMoveY", yThrow);
        }
    }
}
