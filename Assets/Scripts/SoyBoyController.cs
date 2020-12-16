using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D),
typeof(Animator))]

public class SoyBoyController : MonoBehaviour
{
    // The speed and accel floats hold pre-defined values to use when calculating how much
    // force to apply to Super Soy Boy’s Rigidbody.
    public float speed = 14f;
    public float accel = 6f;
    public bool isJumping;
    public float jumpSpeed = 8f;
    public float jumpDurationThreshold = 0.25f;
    public float airAccel = 3f;
    public float jump = 14f;
    // The Vector2 input field stores the controller’s current input values for x and y at any
    // point in time.Negatives mean the controls are going left(-x) or down(-y), and positives
    // mean right(x) or up(y).
    private Vector2 input;
    // The last three private fields cache references to the SpriteRenderer, Rigidbody2D and
    // Animator components.
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    private float rayCastLengthCheck = 0.005f;
    private float width;
    private float height;
    private float jumpDuration;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 1. Input.GetAxis() gets X and Y values from the built-in Unity control axes named Horizontal and Jump.
        // These values will either be - 1, 0 or 1, depending on whether the controls are left, right, up, down or
        // neutral.The value is held in the x and y properties of the input variable, which is a Vector2.
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");

        animator.SetFloat("Speed", Mathf.Abs(input.x));

        // 2. If input.x is greater than 0, then the player is facing right, so the sprite gets flipped on the X - axis.
        // Otherwise, the player must be facing left, so the sprite is set back to “not flipped”.
        if (input.x > 0f)
        {
            sr.flipX = false;
        }
        else if (input.x < 0f)
        {
            sr.flipX = true;
        }

        if (input.y >= 1f)
        {
            jumpDuration += Time.deltaTime;
            animator.SetBool("IsJumping", true);
        }
        else
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
            jumpDuration = 0f;
        }

        // Determines if player is touching the ground.
        if (PlayerIsOnGround() && isJumping == false)
        {
            if (input.y > 0f)
            {
                isJumping = true;
            }
            animator.SetBool("IsOnWall", false);
        }

        if (jumpDuration > jumpDurationThreshold) input.y = 0f;
    }

    // Locates the specified component types on the GameObject upon
    // which SoyBoyController.cs sits and assigns them to the three fields. With this, you
    // reap the benefits of using the RequireComponent() class attribute.
    // To bring it full circle, consider this: If you didn’t enforce required component types and
    // the GameObject didn’t have one or more of these components, then Awake() would fail
    // when it tried to locate the missing component(s)!
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
        height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;
    }

    void FixedUpdate()
    {
        var acceleration = 0f;
        if (PlayerIsOnGround())
        {
            acceleration = accel;
        }
        else
        {
            acceleration = airAccel;
        }

        // 1. Assign the value of accel — the public float field — to a private variable named
        // acceleration.This might seem redundant right now, but later you’ll see
        // acceleration used to hold ground and air acceleration values, so it’s important to define it now.
        //var acceleration = accel;
        var xVelocity = 0f;
        // 2. If horizontal axis controls are neutral, then xVelocity is set to 0, otherwise
        // xVelocity is set to the current x velocity of the rb(aka Rigidbody2D) component.
        if (PlayerIsOnGround() && input.x == 0)
        {
            xVelocity = 0f;
        }
        else
        {
            xVelocity = rb.velocity.x;
        }

        var yVelocity = 0f;
        if (PlayerIsTouchingGroundOrWall() && input.y == 1)
        {
            yVelocity = jump;
        }
        else
        {
            yVelocity = rb.velocity.y;
        }

        // 3. Force is added to rb by calculating the current value of the horizontal axis controls
        // multiplied by speed, which is in turn multiplied by acceleration. Both speed and
        // acceleration values are pre - defined values from the public float fields declared at
        // the top of the script. 0 is used for the Y component, as jumping is not yet being
        // taken into account.
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x)
        * acceleration, 0));
        // 4. Velocity is reset on rb so it can stop Super Soy Boy from moving left or right when
        // controls are in a neutral state.Otherwise, velocity is set to exactly what it’s
        // currently at.This has the effect of stopping Super Soy Boy quickly, even from a full
        // run.
        rb.velocity = new Vector2(xVelocity, yVelocity);

        if (IsWallToLeftOrRight() && !PlayerIsOnGround() && input.y == 1)
        {
            rb.velocity = new Vector2(-GetWallDirection()
            * speed * 0.75f, rb.velocity.y);
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        else if (!IsWallToLeftOrRight())
        {
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        if (IsWallToLeftOrRight() && !PlayerIsOnGround())
        {
            animator.SetBool("IsOnWall", true);
        }

        if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    public bool PlayerIsOnGround()
    {
        // 1. The first ground check performs a Raycast directly below the center of the the
        // character(Transform.position.x), using a length equal to the value of
        // rayCastLengthCheck which is defaulted to 0.005f — a very short raycast is therefore
        // sent down from the bottom of the SoyBoy sprite. The other two ground checks do
        // exactly the same thing, but slightly to the left and right of center.These three
        // raycast checks allow for accurate ground detection.
        bool groundCheck1 = Physics2D.Raycast(new Vector2(
        transform.position.x, transform.position.y - height),
        -Vector2.up, rayCastLengthCheck);
        bool groundCheck2 = Physics2D.Raycast(new Vector2(
        transform.position.x + (width - 0.2f),
        transform.position.y - height), -Vector2.up,
        rayCastLengthCheck);
        bool groundCheck3 = Physics2D.Raycast(new Vector2(
        transform.position.x - (width - 0.2f),
        transform.position.y - height), -Vector2.up,
        rayCastLengthCheck);
        // 2. If any of the ground checks come back as true, then this method returns true to the
        // caller.Otherwise, it will return false.
        if (groundCheck1 || groundCheck2 || groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWallToLeftOrRight()
    {
        // 1. Again you’re using the implicit bool conversion check of the Physics2D.Raycast()
        // method to see if either of two raycasts sent out to the left(-Vector2.right) and to
        // the right(Vector2.right) of the character sprite hit anything. (Remember that
        // rayCastLengthCheck has a small value, so the raycast only goes out a very short
        // distance to the sides of the sprite to check for walls.)
            bool wallOnleft = Physics2D.Raycast(new Vector2(
                 transform.position.x - width, transform.position.y),
                     -Vector2.right, rayCastLengthCheck);
        bool wallOnRight = Physics2D.Raycast(new Vector2(
            transform.position.x + width, transform.position.y),
              Vector2.right, rayCastLengthCheck);

        // 2. If either of these raycasts hit anything, the method returns true — otherwise, false.
        if (wallOnleft || wallOnRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerIsTouchingGroundOrWall()
    {
        if (PlayerIsOnGround() || IsWallToLeftOrRight())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetWallDirection()
    {
        bool isWallLeft = Physics2D.Raycast(new Vector2(
        transform.position.x - width, transform.position.y),
        -Vector2.right, rayCastLengthCheck);
        bool isWallRight = Physics2D.Raycast(new Vector2(
        transform.position.x + width, transform.position.y),
        Vector2.right, rayCastLengthCheck);
        if (isWallLeft)
        {
            return -1;
        }
        else if (isWallRight)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}