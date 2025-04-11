using System.Collections;
using HealthAndStats;
using Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerController : MonoBehaviour {
        public static PlayerController instance;

        private void Awake() {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
        }

        [TabGroup("Moving")] [SerializeField] private float acceleration;
        [TabGroup("Moving")] [SerializeField] [Tooltip("0 = instant stop, 1 = no stop")] private float groundStopMultiplier;
        [TabGroup("Moving")] [SerializeField] [Tooltip("0 = instant stop, 1 = no stop")] private float airStopMultiplier;
        [TabGroup("Moving")] [SerializeField] private float maxSpeed;
        private float currentMaxSpeed, xDirection;

        [TabGroup("Jumping")] [SerializeField] private float jumpForce;
        [TabGroup("Jumping")] [SerializeField] private float doubleJumpForce;
        [TabGroup("Jumping")] [SerializeField] private float coyoteTimeLength;
        [TabGroup("Jumping")] [SerializeField] private float jumpLength;
        [TabGroup("Jumping")] [SerializeField] private float jumpCutMult;
        private float coyoteTimeCounter;
        private bool isJumping, isGrounded, doubleJump;
        private Coroutine jumpCoroutine;

        [TabGroup("Dashing")] [SerializeField] private float dashSpeed;
        [TabGroup("Dashing")] [SerializeField] private float dashGravityMult;
        [TabGroup("Dashing")] [SerializeField] private float dashTimeBetweenPresses;
        [TabGroup("Dashing")] [SerializeField] private float dashCD;
        [TabGroup("Dashing")] [SerializeField] private float dashDuration;
        private float previousDashDirection, dashPressCounter;
        private bool canDash, isDashing;


        [TabGroup("Transforms")] [SerializeField] private Transform selectedItem;
        [TabGroup("Transforms")] [SerializeField] private Transform groundCheck;

        [TabGroup("Other")] [SerializeField] private SpriteRenderer rend;
        [TabGroup("Other")] [SerializeField] private LayerMask whatIsGround;

        private Vector3 mousePos;
        private Vector2 lookDirection;
        private float lookAngle;
        private Quaternion toMouseRotation;
        public Quaternion ToMouseRotation => toMouseRotation;

        private Rigidbody2D rb;
        private Camera cam;

        private void Start() {
            rb = GetComponent<Rigidbody2D>();
            cam = Camera.main;
            canDash = true;
            currentMaxSpeed = ApplyMaxSpeedBonus();
        }

        private void FixedUpdate() {
            CheckGround();

            if (!isDashing) { HorizontalMovement(); }
        }

        private void Update() {
            LookAtMouse();
        }
        
    #region HorizontalMovement

        public void HorizontalMovementInput(InputAction.CallbackContext context) {
            xDirection = context.ReadValue<float>();

            if (context.performed && canDash == true) { // DASH INPUT
                if (Time.time - dashPressCounter < dashTimeBetweenPresses && previousDashDirection == xDirection) {
                    StartCoroutine(Dash(xDirection));
                } else {
                    previousDashDirection = xDirection;
                    dashPressCounter = Time.time;
                }
            }
        }
        
        private void HorizontalMovement() {
            //Moving and stopping
            if (xDirection == 0) { rb.linearVelocity = isGrounded ? 
                new Vector2(rb.linearVelocity.x * groundStopMultiplier, rb.linearVelocity.y) : 
                new Vector2(rb.linearVelocity.x * airStopMultiplier, rb.linearVelocity.y); } 
            else { rb.AddForce(IsChangingDirection(xDirection) ? 
                new Vector2(acceleration * 3f * xDirection, rb.linearVelocity.y) : 
                new Vector2(acceleration * xDirection, rb.linearVelocity.y)); }

            //speed cap
            if (rb.linearVelocity.x > currentMaxSpeed) { rb.linearVelocity = new Vector2(currentMaxSpeed, rb.linearVelocity.y); } 
            else if (rb.linearVelocity.x < -currentMaxSpeed) { rb.linearVelocity = new Vector2(-currentMaxSpeed, rb.linearVelocity.y); }
        }
        
        private bool IsChangingDirection(float dir) {
            if (rb.linearVelocity.x == 0) { return false; }

            return (dir < 0 && rb.linearVelocity.x > 0.1f) || (dir > 0 && rb.linearVelocity.x < -0.1f);
        }
        
        public float ApplyMaxSpeedBonus() {
            return maxSpeed + GetComponent<StatsManager>().MoveSpeedBonus;
        }
        
        private IEnumerator Dash(float dir) {
            canDash = false;
            isDashing = true;

            rb.linearVelocity = new Vector2(dashSpeed * dir, rb.linearVelocity.y * dashGravityMult);
            
            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
            
            yield return new WaitForSeconds(dashCD - dashDuration);
            canDash = true;
        }
    #endregion
    
    #region VerticalMovement
        public void JumpInput(InputAction.CallbackContext context) {
            if (context.performed) {
                if (isGrounded == true) {
                    jumpCoroutine = StartCoroutine(Jump(jumpForce));
                } else if (doubleJump) {
                    if(jumpCoroutine != null) StopCoroutine(jumpCoroutine);

                    doubleJump = false;
                    jumpCoroutine = StartCoroutine(Jump(doubleJumpForce));
                }
            }

            if (context.canceled && isJumping == true) {
                if(jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                
                JumpCut();
            }
        }

        private IEnumerator Jump(float force) {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
            
            
            yield return new WaitForSeconds(jumpLength);
            
            JumpCut();
        }

        private void JumpCut() {
            isJumping = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMult);
        }
        
        private void CheckGround() { // ITS A LITTLE BIG TO IMITATE JUMP BUFFER
            if (Physics2D.OverlapBox(new Vector2(groundCheck.position.x, groundCheck.position.y - .1f), new Vector2(.98f, .3f), 0, whatIsGround)) {
                coyoteTimeCounter = coyoteTimeLength;
                doubleJump = true;
            } else { coyoteTimeCounter -= Time.fixedDeltaTime; }

            isGrounded = coyoteTimeCounter > 0;
        }
    #endregion
    
    #region FacingDirectionAndItemRotation
        public Vector2 GetFacingDirection() {
            return new Vector2(Mathf.RoundToInt(transform.localScale.x), 1);
        }

        private void LookAtMouse() {
            lookDirection = mousePos - transform.position;
            
            lookAngle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;

            rend.flipX = lookAngle < 0;
            
            toMouseRotation = Quaternion.Euler(new Vector3(0, 0, -lookAngle + 90));
            
            selectedItem.rotation = Quaternion.Euler(new Vector3(0, 0, -lookAngle + 90));
        }
        
        public void MousePosition(InputAction.CallbackContext context) {
            mousePos = cam.ScreenToWorldPoint(context.ReadValue<Vector2>());
        }
    #endregion
    
        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(groundCheck.position - new Vector3(0, .1f, 0), new Vector3(.98f, .3f, 1));
        }
    }
}