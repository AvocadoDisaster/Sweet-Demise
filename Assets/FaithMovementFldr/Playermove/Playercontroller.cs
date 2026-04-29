using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playercontroller : MonoBehaviour
{
   public GameObject Door;
   

    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Player Settings")] //equivalent to gamemaker variables
    [SerializeField] float speed;
    [SerializeField] float horizontalBounce;
    [SerializeField] float nHorizontalBounce;
    [SerializeField] float jumpingPower;
    [SerializeField] float bouncePower;
    [SerializeField] float nBouncePower;
    [SerializeField] float superJumpingPower;
    [SerializeField] float sprintSpeed;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public Transform groundCheck;

    [Header("LeftGrounding")] 
    [SerializeField] LayerMask leftGroundLayer;
    [SerializeField] LayerMask leftWallLayer;
    [SerializeField] public Transform leftGroundCheck;

    [Header("RightGrounding")]
    [SerializeField] LayerMask rightGroundLayer;
    [SerializeField] LayerMask rightWallLayer;
    [SerializeField] public Transform rightGroundCheck;

    [Header("UpGrounding")]
    [SerializeField] LayerMask upGroundLayer;
    [SerializeField] LayerMask roofLayer;
    [SerializeField] public Transform upGroundCheck;

    private int jumpCount = 2;
    private int keyCount = 0;
    private int jumpAllow = 0;
    public float moveSpeed;

    public Vector2 horizontal;

    private void Update()
    {
        rb.velocity = new Vector2(horizontal.x * speed, rb.velocity.y);

        bool isPressingUp = Input.GetKey(KeyCode.D);

        if (isPressingUp && IsUpGrounded() /* && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)*/ )
        {
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
        }

    }

    private void FixedUpdate() //allow movement
    {
       

       
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
        if (context.performed && jumpAllow != 0 /*&& IsGrounded()*/)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpAllow--;
        }
         
        if (IsGrounded())
        {
            jumpCount = 0;
            jumpAllow = 2;
        }
        if (context.performed && IsGrounded() && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift) /* && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)*/)
        {
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold down on the ground to bounce upwards
        }
        //GAMEPADREPEAT
        if (context.performed && IsGrounded() && Input.GetKey(KeyCode.Joystick1Button4) /*&& Input.GetKey(KeyCode.LeftShift) /* && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)*/)
        {
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold down on the ground to bounce upwards
        }
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = horizontalBounce;
            //hold left on a leftwall to bounce rightwards
        }
        //GAMEPADREPEAT
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.Joystick1Button4)/* && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift)*/)
        {
            horizontal.x = horizontalBounce;
            //hold left on a leftwall to bounce rightwards
        }
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = nHorizontalBounce;
            //hold right on the rightwall to bounce leftwards
        }
        //GAMEPADREPEAT
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.Joystick1Button4)/* && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift)*/)
        {
            horizontal.x = nHorizontalBounce;
            //hold right on the rightwall to bounce leftwards
        }
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = horizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold left and up on the leftwall to bounce upwards and rightwards 
        }
        //GAMEPADREPEAT sv
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.Joystick1Button4) && Input.GetKey(KeyCode.Joystick1Button5) /*&& !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift)*/)
        {
            horizontal.x = horizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold left and up on the leftwall to bounce upwards and rightwards 
        }
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = nHorizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold right and up on the rightwall to bounce upwards and leftwards 
        }
        //GAMEPADREPEAT sv
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.Joystick1Button4) && Input.GetKey(KeyCode.Joystick1Button5)/* && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift)*/)
        {
            horizontal.x = nHorizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold right and up on the rightwall to bounce upwards and leftwards 
        }
        if (context.performed && isLeftGrounded() && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = horizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
            //hold left and down on the leftwall to bounce downwards and rightwards 
        }
        if (context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift) || context.performed && isRightGrounded() && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.Joystick1Button4)) //left bumper
        {
            horizontal.x = nHorizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
            //hold right and down on the rightwall to bounce downwards and leftwards
        }
        if (context.performed && IsGrounded() && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = horizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold right and down on the ground to bounce UPWARDS and RIGHTWARDS (changed pattern)
        }
        if (context.performed && IsGrounded() && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = nHorizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, bouncePower);
            //hold left and down on the ground to bounce UPWARDS and LEFTWARDS (changed pattern)
        }
        if (context.performed && IsUpGrounded() && !Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow) &&  !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            horizontal.x = nHorizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
            //hold left and up on the ceiling to bounce DOWNWARDS and LEFTWARDS (changed pattern)
        }
        if (context.performed && IsUpGrounded() && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            horizontal.x = horizontalBounce;
            rb.velocity = new Vector2(rb.velocity.x, nBouncePower);
            //hold right and up on the ceiling to bounce DOWNWARDS and RIGHTWARDS (changed pattern)
        }
    }

    #region PLAYER_CONTROLS
    public void Move(InputAction.CallbackContext context) //code for moving back and forth
    {
        horizontal.x = context.ReadValue<Vector2>().x;
    }

    public void Descend(InputAction.CallbackContext context) //code for moving back and forth
    {
        rb.velocity = new Vector2(rb.velocity.x, -100);

    }

    public void Bounce(InputAction.CallbackContext context) //bouncing you have to hold the correct direction for the bounce to work
    {
       
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

    

    

public bool IsGrounded() //a capsule checks if the player is on the ground, capsule values here must match with scale of capsule i made
    {
        Debug.LogWarning("activated");
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(4.4f, 0.14f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    private bool IsUpGrounded() //a capsule checks if the player is touching the ceiling, capsule values here must match with scale of capsule i made
    {

        //return Physics2D.OverlapCapsule(upGroundCheck.position, new Vector2(4.4f, 0.14f), CapsuleDirection2D.Horizontal, 0, groundLayer);
        return Physics2D.OverlapCapsule(upGroundCheck.position, new Vector2(4.4f, 0.14f), CapsuleDirection2D.Horizontal, 0, roofLayer);

    }

    private bool isLeftGrounded() //a capsule checks if the player is touching an left wall, capsule values here must match with scale of capsule i made
    {
        Debug.LogWarning("leftwall");
      //  return Physics2D.OverlapCapsule(leftGroundCheck.position, new Vector2(0.12f, 1.95f), CapsuleDirection2D.Horizontal, 0, groundLayer);
      return Physics2D.OverlapCapsule(leftGroundCheck.position, new Vector2(0.12f, 1.95f), CapsuleDirection2D.Horizontal, 0, leftWallLayer);
    }

    private bool isRightGrounded() //a capsule checks if the player is touching an left wall, capsule values here must match with scale of capsule i made
    {

       // return Physics2D.OverlapCapsule(rightGroundCheck.position, new Vector2(0.12f, 1.95f), CapsuleDirection2D.Horizontal, 0, groundLayer);
        return Physics2D.OverlapCapsule(rightGroundCheck.position, new Vector2(0.12f, 1.95f), CapsuleDirection2D.Horizontal, 0, rightWallLayer);
    }
    //KEYCODE
   // public void GravityControl(InputAction.CallbackContext context) //trying to make character stick to ceilings
   // {
   //     if (context.performed /*&& IsUpGrounded()*/)
   //     {
   //         Physics2D.gravity = new Vector2(0, 9f);
   //         gameObject.GetComponent<Rigidbody2D>().constraints = /*RigidbodyConstraints2D.FreezePositionX |*/ RigidbodyConstraints2D.FreezePositionY;
   //     }

   // }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("key"))
        {
            Physics2D.gravity = new Vector2(0, 9f);
            //keytimer--;
            // Destroy(gameObject);
            // Destroy(Door);
            // gameObject.SetActive(true);
            // Door.SetActive(false);

            //gameObject.SetActive(false);

        }
        /* if (keyCount == 3)
          {
              Debug.Log("keycount 1");
              Destroy(Door);
          }*/
    }

    // public void CupStick

}
#endregion

