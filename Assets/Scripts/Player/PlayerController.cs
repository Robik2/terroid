using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    [Header("Moving")] 
    [SerializeField] private float acceleration;
    [SerializeField] private float groundStopMultiplier; // 0 insta stop, 1 no stop
    [SerializeField] private float airStopMultiplier; // 0 insta stop, 1 no stop
    [SerializeField] private float maxSpeed;
    
    [Header("Jumping")] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float coyoteTimeLength;
    [SerializeField] private float jumpLength;
    [SerializeField] private float jumpCutMult;
    [SerializeField] private float jumpBufferTime;
    private float jumpLengthCounter, coyoteTimeCounter, currentJumpForce, jumpPressedTime;
    private bool jumpPressed, beginJumping, isGrounded, doubleJump;
    
    [Header("Dashing")] 
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashGravityMult;
    [SerializeField] private float dashTimeBetweenPresses;
    [SerializeField] private float dashCD;
    [SerializeField] private float dashDuration;
    private float dashPressCounterA, dashPressCounterD;
    private bool canDash, isDashing;

    
    [Header("Transforms")]
    [SerializeField] private Transform selectedItem;
    [SerializeField] private Transform groundCheck;
    
    [Header("Other")]
    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private LayerMask whatIsGround;
        
    private Vector2 lookDirection;
    private float lookAngle;
    
    private Rigidbody2D rb;
    private Camera cam;
    
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        canDash = true;
    }

    private void Update() {
        JumpInput();

        DashInput();
    }
    
    private void FixedUpdate() {
        CheckGround();

        if (!isDashing) { HorizontalMovement(); }
        
        LookAtMouse();

        if(beginJumping) {
            Jump();
            jumpLengthCounter += Time.fixedDeltaTime;
        }
    }

    private void HorizontalMovement() {
        //Moving and stoping
        if(Input.GetAxisRaw("Horizontal") == 0) {
            rb.linearVelocity = isGrounded ? new Vector2(rb.linearVelocity.x * groundStopMultiplier, rb.linearVelocity.y) : new Vector2(rb.linearVelocity.x * airStopMultiplier, rb.linearVelocity.y);
        } else {
            rb.AddForce(IsChangingDirection(Input.GetAxisRaw("Horizontal")) ? new Vector2(acceleration * 3f * Input.GetAxisRaw("Horizontal"), rb.linearVelocity.y) : new Vector2(acceleration * Input.GetAxisRaw("Horizontal"), rb.linearVelocity.y));
        }
        
        //speed cap
        if (rb.linearVelocity.x > maxSpeed) {
            rb.linearVelocity = new Vector2(maxSpeed, rb.linearVelocity.y);
        } else if (rb.linearVelocity.x < -maxSpeed) {
            rb.linearVelocity = new Vector2(-maxSpeed, rb.linearVelocity.y);
        }
    }

    private bool IsChangingDirection(float dir) {
        if (rb.linearVelocity.x == 0) { return false;}
        return (dir < 0 && rb.linearVelocity.x > 0.1f) || (dir > 0 && rb.linearVelocity.x < -0.1f);
    }
    
    private void JumpInput() {
        jumpPressed = Input.GetKey(KeyCode.Space);
        if (Input.GetKeyDown(KeyCode.Space)) { jumpPressedTime = Time.time;}
        
        if (Time.time - jumpPressedTime < jumpBufferTime && isGrounded) {
            currentJumpForce = jumpForce;
            beginJumping = true;
        } else if (Input.GetKeyDown(KeyCode.Space) && doubleJump) {
            currentJumpForce = doubleJumpForce;
            beginJumping = true;
            doubleJump = false;
        }
    }

    private void Jump() {
        if (jumpLength < jumpLengthCounter) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMult);
            beginJumping = false;
            jumpLengthCounter = 0;
            return;
        }
        
        if (jumpPressed) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
        } else {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMult);
            beginJumping = false;
            jumpLengthCounter = 0;
        }
    }

    private void DashInput() {
        if (!canDash) { return; }
        
        if (Input.GetKeyDown(KeyCode.A)) {
            if (dashPressCounterA <= 0) {
                dashPressCounterA = dashTimeBetweenPresses;
            } else {
                Dash(-1);
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            if (dashPressCounterD <= 0) {
                dashPressCounterD = dashTimeBetweenPresses;
            } else {
                Dash(1);
            }
        }
        
        dashPressCounterA -= dashPressCounterA > 0 ? Time.deltaTime : 0;
        dashPressCounterD -= dashPressCounterD > 0 ? Time.deltaTime : 0;
    }

    private void Dash(int dir) {
        print("dash");
        canDash = false;
        isDashing = true;
        StartCoroutine(nameof(EnableDash));

        rb.linearVelocity = new Vector2(dashSpeed * dir, rb.linearVelocity.y * dashGravityMult);
    }

    private IEnumerator EnableDash() {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCD - dashDuration);
        canDash = true;
    }

    private void LookAtMouse() {
        lookDirection = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        lookAngle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;

        rend.flipX = lookAngle < 0;
        
        selectedItem.rotation = Quaternion.Euler(new Vector3(0, 0, -lookAngle));
    }

    private void CheckGround() {
        if (Physics2D.OverlapBox(new Vector2(groundCheck.position.x, groundCheck.position.y), new Vector2(.98f, .2f), 0, whatIsGround)) {
            coyoteTimeCounter = coyoteTimeLength;
            doubleJump = true;
        } else {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
        
        isGrounded = coyoteTimeCounter > 0;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(groundCheck.position, new Vector3(.98f, .2f, 1));
    }
}
