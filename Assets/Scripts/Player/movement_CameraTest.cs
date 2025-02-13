using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private CameraFollowObject _cameraFollowObject;
    [SerializeField] private GameObject _cameraFollowGO;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // Rotate player only when changing direction
        if (move > 0 && transform.eulerAngles.y != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _cameraFollowObject.CallTurn();
        }
        else if (move < 0 && transform.eulerAngles.y != 180)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _cameraFollowObject.CallTurn();
        }

        // Check if grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");

        // Move the player using Rigidbody in FixedUpdate
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Jump (checking in Update ensures responsiveness, applying in FixedUpdate ensures smooth physics)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

}

