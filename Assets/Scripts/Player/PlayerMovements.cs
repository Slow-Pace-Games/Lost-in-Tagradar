using Unity.Mathematics;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Movements")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    private float maxSpeed;
    private float currentSpeed = 0f;
    private float acceleration = 65f;
    private Vector3 moveDirection;
    private Vector2 inputMovement;
    private Vector3 momentum;
    private float speedMultiplier = 1f;
    private float jumpMultiplier = 1f;
    public float SpeedMultiplier { set => speedMultiplier = value; }
    public float JumpMultiplier { set => jumpMultiplier = value; }

    [Header("Physics")]
    [SerializeField] float gravity;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float fallDamageMultiplier;
    [SerializeField] float fallDamageThreshold;
    private bool isGroundedLastFrame = false;
    private float lastGroundedYPos;
    private bool isGrounded;

    [Header("Crouch")]
    [SerializeField] float crouchSpeed;
    private bool isCrouching = false;

    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float slideMaxCooldown;
    private bool isSliding = false;
    private bool canSlide = true;
    private float slideDuration = 0.75f;
    private float slideTimer = 0f;
    private float slideCooldown = 0f;

    [Header("Height Modifiers")]
    float standHeight;
    float crouchHeight;
    float slideHeight;

    [Header("Camera")]
    [SerializeField] Transform virtualCamera;

    [Header("Sound")]
    [SerializeField] PlayerSound sound;

    [Header("Particle")]
    [SerializeField] GameObject walkTrail;
    [SerializeField] Transform particleContainer;
    [SerializeField] float nbWalkParticle;

    [Header("Animation")]
    [SerializeField] Animator legAnimation;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get horizontal and vertical axis
        inputMovement = PlayerInputManager.Instance.GetZQSDMovementsValue();

        // Inertia
        if (inputMovement.magnitude == 0 && momentum.magnitude == 0)
        {
            momentum = new Vector3(moveDirection.x, 0f, moveDirection.z);
        }

        if (inputMovement.magnitude == 0 && !isGrounded)
        {
            momentum = Vector3.Slerp(momentum, Vector3.zero, TimeScale.deltaTime);
            characterController.Move(momentum * TimeScale.deltaTime);
        }
        else
        {
            momentum = Vector3.zero;
        }
        //

        ZQSDMovements();
        Jump();
        Crouch();
        Slide();
        IsGrounded();
        ChangeHeights();

        // Apply gravity while in the air
        if (!isGrounded)
        {
            moveDirection.y -= gravity * TimeScale.deltaTime;
        }

        // Apply Movement
        characterController.Move(moveDirection * TimeScale.deltaTime);

        FallDamage();

        legAnimation.SetLayerWeight(1,math.abs(inputMovement.x)/15f);
        //
    }

    private void LateUpdate()
    {
        isGroundedLastFrame = isGrounded;
    }

    private void ZQSDMovements()
    {
        if (!isSliding)
        {
            // Changes speed based on current state
            maxSpeed = isCrouching ? crouchSpeed : PlayerInputManager.Instance.RunIsPressed() ? runSpeed * speedMultiplier : walkSpeed;
            if (PlayerInputManager.Instance.RunIsPressed())
            {
                legAnimation.SetBool("IsRunning", true);
                legAnimation.SetBool("IsWalking", false);
            }
            else
            {
                legAnimation.SetBool("IsRunning", false);
                legAnimation.SetBool("IsWalking", true);
            }
            if (inputMovement.magnitude > 0f)
            {
                currentSpeed += acceleration * TimeScale.deltaTime;
            }
            else 
            {
                currentSpeed -= acceleration * TimeScale.deltaTime;
            }
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            if (currentSpeed <= 0)
            {
                legAnimation.SetBool("IsRunning", false);
                legAnimation.SetBool("IsWalking", false);
            }
            inputMovement *= currentSpeed;

            float MoveDirectionY = moveDirection.y;
            moveDirection = (transform.TransformDirection(Vector3.forward) * inputMovement.y) + (transform.TransformDirection(Vector3.right) * inputMovement.x);
            moveDirection.y = MoveDirectionY;

            // Sounds
            if (inputMovement.magnitude != 0 && isGrounded)
            {
                sound.PlayRandomSoundInPlayerFootStep(currentSpeed >= runSpeed * speedMultiplier);
            }
        }
    }

    private void Jump()
    {
        if (PlayerInputManager.Instance.JumpIsPressed() && isGrounded && !isCrouching && !isSliding)
        {
            moveDirection.y = jumpForce * jumpMultiplier;
        }
    }

    private void Crouch()
    {
        if (PlayerInputManager.Instance.CrouchIsPressed() && isGrounded && !isCrouching)
        {
            // Slide
            if (PlayerInputManager.Instance.RunIsPressed() && inputMovement.y > 0.3f && canSlide && GetGroundSteepness() >= 0)
            {
                StartSlide();
            }
            // Crouch
            else if (!PlayerInputManager.Instance.RunIsPressed())
            {
                virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x, crouchHeight, virtualCamera.transform.position.z);
                isCrouching = true;
            }
        }

        // Uncrouch
        if (!PlayerInputManager.Instance.CrouchIsPressed() && isCrouching && CanStand())
        {
            virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x, standHeight, virtualCamera.transform.position.z);
            isCrouching = false;
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x, slideHeight, virtualCamera.transform.position.z);
    }

    private void Slide()
    {
        if (isSliding)
        {
            Vector3 direction = transform.TransformDirection(Vector3.forward) * inputMovement.y;

            moveDirection += direction.normalized * slideForce;
            slideTimer += TimeScale.deltaTime;

            float groundSteepness = GetGroundSteepness();
            if (PlayerInputManager.Instance.CrouchIsPressed() && groundSteepness > 0 && groundSteepness < 10f)
            {
                slideTimer = 0f;
            }

            if (slideTimer >= slideDuration || !PlayerInputManager.Instance.CrouchIsPressed())
            {
                StopSlide();
            }
        }
        if (!isSliding && !canSlide)
        {
            slideCooldown += TimeScale.deltaTime;
            if (slideCooldown >= slideMaxCooldown)
            {
                slideCooldown = 0f;
                canSlide = true;
            }
        }
    }

    private void StopSlide()
    {
        isSliding = false;
        slideTimer = 0f;
        canSlide = false;
        if (CanStand())
        {
            virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x, standHeight, virtualCamera.transform.position.z);
        }
        else
        {
            virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x, crouchHeight, virtualCamera.transform.position.z);
            isCrouching = true;
        }
    }

    private bool CanStand()
    {
        return !Physics.Raycast(transform.position, transform.up, 2f);
    }

    private void IsGrounded()
    {
        float maxDistance;
        if (isCrouching)
        {
            maxDistance = 0.6f;
        }
        else
        {
            maxDistance = 1.25f;
        }

        float steepness = GetSteepness();
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistance) && !hit.collider.isTrigger && steepness >= 0.5f)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private float GetSteepness()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.25f))
        {
            return hit.normal.y;
        }

        return 0f;
    }

    private float GetGroundSteepness()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.25f) && hit.transform.tag == "Ground")
        {
            return math.dot(transform.forward, hit.normal);
        }

        return 145789f;
    }

    private void FallDamage()
    {
        if (isGroundedLastFrame && !isGrounded)
        {
            lastGroundedYPos = transform.position.y;
        }

        if (!isGrounded && transform.position.y > lastGroundedYPos)
        {
            lastGroundedYPos = transform.position.y;
        }

        if (!isGroundedLastFrame && isGrounded)
        {
            int fallDistance = (int)lastGroundedYPos - (int)transform.position.y;

            if (fallDistance > fallDamageThreshold)
            {
                Player.Instance.HitPlayer((int)(fallDistance * fallDamageMultiplier));
            }
        }
    }

    private void ChangeHeights()
    {
        standHeight = transform.position.y + 0.75f;
        crouchHeight = transform.position.y;
        slideHeight = transform.position.y - 0.5f;
    }
}