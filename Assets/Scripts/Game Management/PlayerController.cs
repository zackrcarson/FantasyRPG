using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float verticalWalkSpeed = 1f;
    [SerializeField] float horizontalWalkSpeed = 1f;
    [SerializeField] float xBoundaryOffset = 0.5f;
    [SerializeField] float yBoundaryOffset = 0.5f;
    [SerializeField] float footStepPitchMin = 0.8f;
    [SerializeField] float footStepPitchMax = 1.2f;

    // State Variables
    [HideInInspector] public string portalName = null;
    [HideInInspector] public bool canMove = true;

    // Cached References
    Rigidbody2D rigidBody = null;
    Animator animator = null;
    AudioSource audioSource = null;
    Vector3 bottomLeftBound = new Vector3(0f, 0f, 0f);
    Vector3 topRightBound = new Vector3(0f, 0f, 0f);

    private void Awake()
    {
        SingletonPattern();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

        ClampPlayerInBounds();
    }

    private void MovePlayer()
    {
        float xThrow = Input.GetAxisRaw("Horizontal");
        float yThrow = Input.GetAxisRaw("Vertical");

        if (canMove && !LevelUp.instance.isShowingRewards)
        {
            float xSpeed = xThrow * horizontalWalkSpeed;
            float ySpeed = yThrow * verticalWalkSpeed;

            if (xSpeed != 0 && ySpeed != 0)
            {
                xSpeed /= Mathf.Sqrt(2f);
                ySpeed /= Mathf.Sqrt(2f);
            }

            rigidBody.velocity = new Vector2(xSpeed, ySpeed);

            animator.SetFloat("moveX", xThrow);
            animator.SetFloat("moveY", yThrow);
        }
        else
        {
            rigidBody.velocity = Vector2.zero;

            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 0f);
        }


        if (xThrow == 1 || xThrow == -1 || yThrow == 1 || yThrow == -1)
        {
            if (canMove && !LevelUp.instance.isShowingRewards)
            {
                animator.SetFloat("lastMoveX", xThrow);
                animator.SetFloat("lastMoveY", yThrow);
            }
        }
    }

    private void ClampPlayerInBounds()
    {
        float xPos = Mathf.Clamp(transform.position.x, bottomLeftBound.x, topRightBound.x);
        float yPos = Mathf.Clamp(transform.position.y, bottomLeftBound.y, topRightBound.y);
        float zPos = transform.position.z;

        transform.position = new Vector3(xPos, yPos, zPos);
    }

    public void SetBounds(Vector3 bottomLeft, Vector3 topRight)
    {
        Vector3 offsetVector = new Vector3(xBoundaryOffset, yBoundaryOffset, 0f);

        bottomLeftBound = bottomLeft + offsetVector;
        topRightBound = topRight - offsetVector;
    }

    public void CanMove(bool movementAllowed)
    {
        canMove = movementAllowed;
    }

    public Vector2 GetIdleDirection()
    {
        return new Vector2(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"));
    }

    public void SetIdleDirection(Vector2 idleDirection)
    {
        animator.SetFloat("lastMoveX", idleDirection.x);
        animator.SetFloat("lastMoveY", idleDirection.y);
    }

    public void FootStep()
    {
        float pitch = Random.Range(footStepPitchMin, footStepPitchMax);

        audioSource.pitch = pitch;
        audioSource.Play();
    }
}
