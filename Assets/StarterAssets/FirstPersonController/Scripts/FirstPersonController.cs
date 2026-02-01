using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        //[Tooltip("Sprint speed of the character in m/s")]
        //public float SprintSpeed = 6.0f;
        [Tooltip("Max speed multiplier on platforms")]
        public float MaxSpeedMultiplier;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Abilities")]
        public float DashSpeed = 10f;
        public float DashFalloff = 0.85f;
        [System.NonSerialized] public float DashMod;
        [System.NonSerialized] public Vector3 DashDirection;
        [System.NonSerialized] public bool CanDash;

        [Header("Boost Ramp Settings")]
        public float BoostDuration = 0.3f;  // How long at full power before decay starts
        public float BoostFalloff = 0.75f;  // How quickly boost fades (0-1, lower = faster fade)
        
        [Header("Boost Debug (Read-Only at Runtime)")]
        [SerializeField] private Vector3 _currentBoostDirection;  // Shows current boost direction
        [SerializeField] private float _currentBoostPower;        // Shows current boost power
        
        // Actual boost values used internally
        [HideInInspector] public Vector3 BoostDirection;
        [HideInInspector] public float BoostMod;
        private float _boostTimer = 0f;
        private bool _isBoosting = false;
        private bool _boostPending = false;  // Set by BoostRamp to trigger a new boost
        
        // Called by BoostRamp to trigger a new boost
        public void TriggerBoost(float boostPower, Vector3 direction)
        {
            BoostMod = boostPower;
            BoostDirection = direction;
            _boostPending = true;
        }
        [System.NonSerialized] public bool CanDoubleJump = false;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [Header("Wallrunning")]
        [Tooltip("Run along wall")]
        public bool WallRun = false;
        [Tooltip("WallRunningSpeed")]
        public float WallRunningSpeed = 10.0f;
        [Tooltip("ControlFactor against wall")]
        public float WallControlFactor = 0.5f;
        [Tooltip("How much is dragged towards wall.")]
        [SerializeField] public float WallDragPower = 25;
        [Tooltip("Wall jump force.")]
        [SerializeField] public float WallJumpForce = 10.0f;
        [Tooltip("Wall jump duration.")]
        [SerializeField] public float WallJumpDuration = 0.2f;
        [Header("Wall Run Camera")]
        public float WallRunCameraRoll = 20.0f;
        public float WallRunRollSpeed = 2.0f;
        [HideInInspector]
        public int WallSide = 0; // -1 = left, +1 = right

        private float _currentCameraRoll;
        private AnimateCharacter character;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        [HideInInspector]
        public CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.0001f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            character = transform.GetChild(0).GetComponent<AnimateCharacter>();
        }

        private void Update()
        {
            if (!WallRun)
            {
                JumpAndGravity();
                GroundedCheck();
                Move();
            } 
            else
            {
                //ability resets
                CanDash = true;
                CanDoubleJump = true;
            }

            Ability();
            Dash();
        }

        private void LateUpdate()
        {
            CameraRotation();
            float targetRoll = 0.0f;

            if (WallRun)
            {
                targetRoll = WallRunCameraRoll * WallSide;
            }

            _currentCameraRoll = Mathf.Lerp(_currentCameraRoll, targetRoll, Time.deltaTime * WallRunRollSpeed * _speed);

            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, _currentCameraRoll);
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float maxSpeedMod = 1.0f;
            if (Grounded || WallRun) 
            {
                maxSpeedMod = MaxSpeedMultiplier;
            }
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            //float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            float targetSpeed = MoveSpeed * maxSpeedMod;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            //if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            float inputScale = WallRun ? 0.25f : 1f;

            // normalise input direction
            Vector3 inputDirection = (transform.forward + transform.right * _input.move.x) * inputScale;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            //if (_input.move != Vector2.zero)
            //{
            //	// move
            //	inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            //}

            //inputDirection = transform.forward + transform.right * _input.move.x;

            // Handle boost timer and control lock - only start when BoostRamp triggers it
            if (_boostPending && !_isBoosting)
            {
                _isBoosting = true;
                _boostPending = false;
                _boostTimer = BoostDuration;
                _verticalVelocity = 0f; // Reset gravity so upward boost works
                Debug.Log($"FPC: Boost STARTED! BoostMod: {BoostMod}, BoostDirection: {BoostDirection}, Duration: {BoostDuration}");
            }

            if (_boostTimer > 0)
            {
                _boostTimer -= Time.deltaTime;
            }
            else if (_isBoosting)
            {
                _isBoosting = false;
                Debug.Log("FPC: Boost ENDED - timer ran out");
            }

            // Only apply boost falloff AFTER the boost duration ends
            float previousBoostMod = BoostMod;
            if (!_isBoosting && BoostMod > 0)
            {
                // Apply falloff only after boost phase ends
                BoostMod *= Mathf.Pow(BoostFalloff, Time.deltaTime * 60f);
                if (BoostMod < 0.5f) BoostMod = 0f;
            }

            // Debug boost values while boosting
            if (_isBoosting || BoostMod > 0)
            {
                Vector3 boostVector = BoostDirection * BoostMod;
                Debug.Log($"FPC: BoostMod: {previousBoostMod:F2} -> {BoostMod:F2}, BoostVector: {boostVector}, isBoosting: {_isBoosting}, deltaTime: {Time.deltaTime:F4}");
            }
            
            // Update debug values for Inspector
            _currentBoostDirection = BoostDirection;
            _currentBoostPower = BoostMod;

            // During boost, reduce player input control
            // Calculate how much boost is affecting movement (0 = no boost, 1 = full boost)
            float boostInfluence = Mathf.Clamp01(BoostMod / (DashSpeed * 2f)); // Normalize based on typical boost power
            
            // Blend between boost movement and player movement
            Vector3 playerMovement = inputDirection.normalized * _speed * (1f - boostInfluence);

            // During active boost phase, apply minimal gravity; otherwise normal gravity
            float verticalComponent = _isBoosting ? _verticalVelocity * 0.1f : _verticalVelocity;

            // Calculate final move vector - player movement is now blended with boost
            Vector3 finalMoveVector = (playerMovement + new Vector3(0.0f, verticalComponent, 0.0f) + DashDirection * DashMod + BoostDirection * BoostMod) * Time.deltaTime;
            
            // Debug: Log position and move vector during boost
            if (_isBoosting)
            {
                Vector3 posBefore = transform.position;
                _controller.Move(finalMoveVector);
                Vector3 posAfter = transform.position;
                Vector3 actualMovement = posAfter - posBefore;
                Debug.Log($"FPC MOVE DEBUG: MoveVector: {finalMoveVector}, PosBefore: {posBefore}, PosAfter: {posAfter}, ActualMove: {actualMovement}, Grounded: {Grounded}");
            }
            else
            {
                _controller.Move(finalMoveVector);
            }
        }

        private void Jump()
        {
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                //ability resets
                CanDash = true;
                CanDoubleJump = true;

                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    Jump();
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                if (CanDoubleJump && Keyboard.current.spaceKey.wasPressedThisFrame && GameManager.Singleton.currentColor == GameManager.colors.GREEN)
                {
                    CanDoubleJump = false;
                    Jump();
                }
                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        public void Ability()
        {
            if (_input.ability)
            {
                if (GameManager.Singleton.currentColor == GameManager.colors.RED)
                {
                    Camera camera = CinemachineCameraTarget.GetComponent<Camera>();
                    RaycastHit hit;
                    Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    character.Shoot();
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.GetComponent<ShootingTarget>())
                        {
                            hit.collider.GetComponent<ShootingTarget>().OnHit();
                        }
                    }
                }
                else if (GameManager.Singleton.currentColor == GameManager.colors.BLUE && CanDash && !Grounded)
                {
                    Camera camera = CinemachineCameraTarget.GetComponent<Camera>();

                    DashDirection = CinemachineCameraTarget.transform.forward;
                    DashMod = DashSpeed;
                    CanDash = false;
                }
                _input.ability = false;
            }
        }

        public void Dash()
        {
            if (DashMod > 0)
            {
                DashMod *= DashFalloff;
                if (DashMod <= 0.01f)
                {
                    DashMod = 0;
                }
            }

            // BoostMod falloff is now handled in Move() - removed duplicate falloff here
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}