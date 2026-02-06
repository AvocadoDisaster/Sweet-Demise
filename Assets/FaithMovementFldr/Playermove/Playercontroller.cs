using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playercontroller : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Player Settings")] //equivalent to gamemaker variables
    [SerializeField] float speed; 
    [SerializeField] float jumpingPower;
    [SerializeField] float bouncePower;
    [SerializeField] float nBouncePower;
    [SerializeField] float superJumpingPower;
    [SerializeField] float sprintSpeed;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    [Header("LeftGrounding")] 
    [SerializeField] LayerMask leftGroundLayer;
    [SerializeField] Transform leftGroundCheck;

    [Header("RightGrounding")]
    [SerializeField] LayerMask rightGroundLayer;
    [SerializeField] Transform rightGroundCheck;

    [Header("UpGrounding")]
    [SerializeField] LayerMask upGroundLayer;
    [SerializeField] Transform upGroundCheck;

    private int jumpCount = 2;


    private float horizontal;

    private void FixedUpdate() //allow movement
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    #region PLAYER_CONTROLS
    public void Move(InputAction.CallbackContext context) //code for moving back and forth
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Descend(InputAction.CallbackContext context) //code for moving back and forth
    {
        rb.velocity = new Vector2(rb.velocity.x, -20);

    }

    public void Bounce(InputAction.CallbackContext context) //bouncing you have to hold the correct direction for the bounce to work
    {
        if (context.performed && IsGrounded() && Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
        }
        if (context.performed && IsUpGrounded() && Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
        }
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = 10f;
        }
       if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = -10f;
        }
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = 10f;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
        }
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = -10f;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
        }
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = 10f;
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
        }
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            horizontal = -10f;
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
        }
    }

  

    /* public void Sprint(InputAction.CallbackContext context) //code for sprinting
      {

              speed += sprintSpeed;

      }*/
    public void Sprint(InputAction.CallbackContext context) //code for sprint
    {
        if (context.performed)
        {
            speed += sprintSpeed;
        }
        else
        { 
            speed = 10f;
        }

    }

    public void SuperJump(InputAction.CallbackContext context) //code for jumping
    {
        if (context.performed && jumpCount != 2)
        {
            rb.velocity = new Vector2(rb.velocity.x, superJumpingPower);
            jumpCount++;
        }

        if (IsGrounded())
        {
            jumpCount = 0;
        }
    }

    public void Jump(InputAction.CallbackContext context) //code for jumping
    {
        if(context.performed && jumpCount != 2)
        {
           rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpCount++;
        }
       if (IsGrounded())
            {
            jumpCount = 0;
        }
    }

    private bool IsGrounded() //a capsule checks if the player is on the ground, capsule values here must match with scale of capsule i made
    {
        
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.92f, 0.14f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    private bool IsUpGrounded() //a capsule checks if the player is touching the ceiling, capsule values here must match with scale of capsule i made
    {

        return Physics2D.OverlapCapsule(upGroundCheck.position, new Vector2(0.92f, 0.14f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    private bool isLeftGrounded() //a capsule checks if the player is touching an left wall, capsule values here must match with scale of capsule i made
    {

        return Physics2D.OverlapCapsule(leftGroundCheck.position, new Vector2(0.12f, 1.95f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    private bool isRightGrounded() //a capsule checks if the player is touching an left wall, capsule values here must match with scale of capsule i made
    {

        return Physics2D.OverlapCapsule(rightGroundCheck.position, new Vector2(0.12f, 1.95f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }


}
#endregion