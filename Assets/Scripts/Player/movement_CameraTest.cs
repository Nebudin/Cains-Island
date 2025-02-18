using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    //M�STE KVAR DESSA F�R KAMERAN
    private CameraFollowObject _cameraFollowObject;
    [SerializeField] private GameObject _cameraFollowGO;
    private float _fallSpeedYDampingChangeThreshold;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>(); //SPARA

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold; //SPARA
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

       

        
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        //Detta roterar karakt�ren, m�ste sparas f�r kameran
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

        if(rb.linearVelocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if(rb.linearVelocity.y >= 0 && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpFromPlayerFalling)
        {
            CameraManager.instance.LerpFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }


    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");

       
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

       
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

}

