using UnityEngine;

public class DashJump : MonoBehaviour
{
    private playerMovement_Test playerMovement;
    private bool canDashAgain = false;

    [SerializeField] private Vector2 bounceBackForce = new Vector2(-5f, 10f);
    [SerializeField] private Vector2 bounceForwardForce = new Vector2(5f, 10f);
    [SerializeField] private Vector2 downwardDashForce = new Vector2(0, -15f);

    private void Start()
    {
        playerMovement = GetComponent<playerMovement_Test>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && playerMovement.isDashing)
        {

            playerMovement.dashCount = 1;

            if (!canDashAgain)
            {
                playerMovement.rb.linearVelocity = bounceBackForce;
                canDashAgain = true;
            }
            else
            {
                playerMovement.rb.linearVelocity = bounceForwardForce;
                canDashAgain = false;
            }
        }
    }
}
