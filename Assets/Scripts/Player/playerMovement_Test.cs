using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementEnumBased : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        Dashing,
        Walking
    }

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] private float horizontal;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float accSpeed = 0.1f;
    [SerializeField] public float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashTime;
    [SerializeField] public int dashCount = 0;
    [SerializeField] private int dashMax = 1;
    [SerializeField] private float fallingGravityMult = 1.5f;

    private bool isFacingRight = true;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int maxJump = 2;

    private float originalGravity;

    private PlayerState currentState = PlayerState.Idle;

    //Kamera
    private CameraFollowObject _cameraFollowObject;
    [SerializeField] private GameObject _cameraFollowGO;
    private float _fallSpeedYDampingChangeThreshold;

    private void Start()
    {
        currentSpeed = baseSpeed;
        originalGravity = rb.gravityScale;
       
        //Kamera
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        UpdateState();
        ApplyState();
        HandleMovement();
        HandleFlip();

        if (isGrounded())
        {
            jumpCount = 0;
            dashCount = 0;
        }
    }

    private void UpdateState()
    {
        if (currentState == PlayerState.Dashing)
        {
            if (Time.time > dashTime)
            {
                currentState = PlayerState.Running;
                currentSpeed = maxSpeed;
                rb.gravityScale = originalGravity;
            }
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            currentState = PlayerState.Falling;
        }
        else if (!Mathf.Approximately(rb.linearVelocity.y, 0f))
        {
            currentState = PlayerState.Jumping;
        }
        else if (Mathf.Abs(horizontal) > 0.01f)
        {
            currentState = PlayerState.Running;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void ApplyState()
    {
        switch (currentState)
        {
            case PlayerState.Falling:
                rb.gravityScale = originalGravity * fallingGravityMult;
                break;
            case PlayerState.Dashing:
                rb.gravityScale = 0;
                break;
            default:
                rb.gravityScale = originalGravity;
                break;
        }
    }

    private void HandleMovement()
    {
        if (currentState != PlayerState.Dashing)
        {
            if (horizontal != 0)
            {
                currentSpeed = Mathf.Min(currentSpeed + accSpeed, maxSpeed);
            }
            else
            {
                currentSpeed = baseSpeed;
            }

            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpCount < maxJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }

        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && currentState != PlayerState.Dashing && dashCount < dashMax)
        {
            if (horizontal != 0)
            {
                currentState = PlayerState.Dashing;
                dashTime = Time.time + dashDuration;
                currentSpeed = dashSpeed;
                rb.linearVelocity = new Vector2(Mathf.Sign(horizontal) * dashSpeed, 0);
                dashCount++;
            }
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void HandleFlip()
    {
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0)
        {
            Flip();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        _cameraFollowObject.CallTurn();
    }
   
}
