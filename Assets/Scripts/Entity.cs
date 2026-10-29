using UnityEngine;

public class Entity : MonoBehaviour
{
    public Rigidbody2D rb {get; private set;}
    public Animator anim {get; private set;}   
    protected StateMachine stateMachine;
    
    private bool _facingRight = true;
    public int facingDir { get; private set; } = 1;
    
    [Header("Collision detection")] 
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform priamryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    
    
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
    }



    protected virtual void Start()
    {
        
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }
    
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (!_facingRight && xVelocity > 0)
        {
            Flip();
        }
        else if (_facingRight && xVelocity < 0)
        {
            Flip();
        }
    }
    
    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
        facingDir *= -1;
    }

    private void HandleCollisionDetection()
    {
        
        // groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        // wallDetected = Physics2D.Raycast(priamryWallCheck.position, Vector2.right * facingDir, wallCheckDistance,
        //     whatIsGround)
        // && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance,
        //     whatIsGround);
        groundDetected = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            whatIsGround
        );
        RaycastHit2D hit1 = Physics2D.Raycast(
            priamryWallCheck.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround);
        
        RaycastHit2D hit2 = Physics2D.Raycast(
            secondaryWallCheck.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround);
        
        wallDetected = hit1 && hit2;
        
        // Debug.DrawRay(
        //     priamryWallCheck.position,
        //     Vector2.right * facingDir * wallCheckDistance,
        //     hit1 ? Color.green : Color.red);
        //
        // Debug.DrawRay(
        //     secondaryWallCheck.position,
        //     Vector2.right * facingDir * wallCheckDistance,
        //     hit2 ? Color.green : Color.red);
    

    }

   private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(priamryWallCheck.position, priamryWallCheck.position + new Vector3(wallCheckDistance * facingDir,0));
        Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir,0));
    }
}
