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
    // The Vector2 input field stores the controller’s current input values for x and y at any
    // point in time.Negatives mean the controls are going left(-x) or down(-y), and positives
    // mean right(x) or up(y).
    private Vector2 input;
    // The last three private fields cache references to the SpriteRenderer, Rigidbody2D and
    // Animator components.
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;

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
    }
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
    }
}
