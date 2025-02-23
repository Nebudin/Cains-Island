using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class playerMovement_Test : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] private float horizontal;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float accSpeed = 0.1f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashTime;
    [SerializeField] private int dashCount = 0;
    [SerializeField] private int dashMax = 1;
    [SerializeField] private float fallingGravityMult = 1.5f;

    private bool isFacingRight = true;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int maxJump = 2;

    //KAMERA
    private CameraFollowObject _cameraFollowObject;
    [SerializeField] private GameObject _cameraFollowGO;
    private float _fallSpeedYDampingChangeThreshold;

    private float originalGravity;

    private void Start()
    {
        currentSpeed = baseSpeed;
        originalGravity = rb.gravityScale;

        //KAMERA
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>(); 
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        if (isDashing)
        {
            if (Time.time > dashTime)
            {
                isDashing = false;
                currentSpeed = maxSpeed; //bevara momentum
                rb.gravityScale = originalGravity;
            }
        }
        else
        {
            if (horizontal != 0)
            {
                currentSpeed = Mathf.Min(currentSpeed + accSpeed, maxSpeed);
            }
            else
            {
                currentSpeed = baseSpeed;
            }

        }

        if (rb.linearVelocity.y < 0.25f && !isDashing) //extra gravitation under fall
        {
            rb.gravityScale = originalGravity * fallingGravityMult; 
        }
        else if (!isDashing)
        {
            rb.gravityScale = originalGravity;
        }

        rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);

        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0)
        {
            Flip();
        }

        if (isGrounded())
        {
            jumpCount = 0;
            dashCount = 0;
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
        if (context.performed && !isDashing && dashCount < dashMax)
        {
            isDashing = true;
            dashTime = Time.time + dashDuration;
            currentSpeed = dashSpeed;
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            dashCount++;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void Flip() //ny flip som funkar med kameran
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        _cameraFollowObject.CallTurn();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

}
