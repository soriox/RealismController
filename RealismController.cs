using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Avalix
{

    public enum CONTROLLER_TYPES
    {
        FIRST_PERSON = 0,
        THIRD_PERSON_1 = 1,
        THIRD_PERSON_2 = 2,
        PROP_HUNT = 3
    }

    [RequireComponent(typeof(CharacterController))]
    public class RealismController : MonoBehaviour
    {   
        public CONTROLLER_TYPES ct;
        public int controllerType;
        
        // Allows the option for gamepads
        public bool useGamePad;

        // Allow the ability to toggle sprint or hold it down to run
        public bool toggleSprint;

        // This will disable the ability to sprint entirely
        public bool disableSprinting;

        // This will disable the ability to jog entirely
        public bool disableJogging;

        // Assign player transform for rotation
        public Transform cameraTransform;

        // PlayerBody to be used for animations
        public GameObject playerBody;

        // If there is now playerBody set, the animator
        // will be gotten from the this object
        Animator animator;

        // Set the Character Controller
        CharacterController playerController;
        Vector3 playerVelocity;
        public float gravity = -10.0f;
        public bool disableJumping;
        public float jumpHeight = 3.0f;
        public float jogJumpHeight = 3.0f;
        public float runningJumpHeight = 3.0f;
        public float jumpDuration = 0.5f;
        public float jumpWaitTimer = 0.5f;
        public float standingJumpDuration = 0.5f;
        bool ignoreGrounded;

        // Animation States
        bool grounded;
        bool walking;
        bool jogging;
        bool running;
        bool jumping;

        // Movement Direction and Speeds --------------
        float targetRotation;
        Vector2 forward;

        public float walkSpeed;
        public float jogSpeed;
        public float runSpeed;

        // Speed smoothing
        public float speedSmoothTime;
        public float jogSmoothTime;
        float speedSmoothVelocity;

        // Turn/Rotation smoothing
        float turnSmoothVelocity;
        public float turnSmoothTime;
        bool rotationLock;

        // Desired movement speed
        float movementSpeed;

        // Speed we are currently moving
        float currentSpeed;
        
        // Gamepad input -----------------------------
        float inputMagnitude;
        Vector2 inputXY;

        // IK Variables ------------------------------

        // Head Rotation
        public bool enableHeadRotation;
        public Transform headLookAt;
        float headRotationY;
        public float headRotationSpeed;
        Quaternion headRotation;
        Vector3 headLookVelocity;

        // Start is called before the first frame update
        void Start()
        {  
            // Select the animator from the playerbody or this object
            if(playerBody) {
                animator = playerBody.GetComponent<Animator>();
            } else {
                animator = GetComponent<Animator>();
            }

            // Set the player controller
            playerController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {   
            // Use horizontal and vertical axes for the controller movement
            // This will allow for smooth movement with gamepads.
            // This will technically work with a keyboard but will not allow for in-between states
            // such as jogging. Those are driven by other controls
            if(useGamePad) {
                inputXY = new Vector2(Input.GetAxis("JoystickHorizontal"), Input.GetAxis("JoystickVertical") * -1);
            } else {
                inputXY = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }

            // Set the input magnitude globally to be used in fixed update as well
            inputMagnitude = inputXY.magnitude;

            // Set and check if the CharacterController is grounded
            grounded = playerController.isGrounded; 

            // If we are grounded then we can move via input
            if(grounded) {

                if(!ignoreGrounded) {
                    jumping = false;
                }

                if(playerVelocity.y < 0) {
                    playerVelocity.y = -2.0f;
                }

                if(useGamePad) { // Controls and more info can be found in the documentation

                    // Update walking state based on input magnitude.
                    if(inputMagnitude > 0) {
                        walking = true;
                    } else {
                        walking = false;
                    }
                    
                    // If we did NOT disable jogging
                    if(!disableJogging) {
                        // Update jogging state based on input magnitude and current movement
                        if(inputMagnitude >= 1.0f && currentSpeed > walkSpeed - 0.5f) {
                            jogging = true;
                        } else {
                            jogging = false;
                        }
                    }
                
                } else { // Without a gamepad the player will use W, A, S, and D to move
                    
                    // Update walking state based on input magnitude.
                    if(inputMagnitude > 0) {
                        walking = true;
                    } else {
                        walking = false;
                    }

                    // If we did NOT disable jogging
                    if(!disableJogging) {
                        if(Input.GetKey(KeyCode.LeftControl)) {
                            jogging = true;
                        } else {
                            jogging = false;
                        }
                    } 

                }

                // Set movement speed based on current state
                // Running will overcome all states
                if(walking) {
                    movementSpeed = walkSpeed;
                    if(jogging&&!disableJogging) {
                        movementSpeed = jogSpeed;
                    }
                }
            
            }

                    if(useGamePad) {
                        if(!disableSprinting) {
                            if(toggleSprint) {
                                // Press sprint button once to toggle sprint
                                if(Input.GetButtonUp("JoystickSprint")) {
                                    running = !running;
                                }
                            } else {
                                // Hold sprint button down to sprint
                                if(Input.GetButton("JoystickSprint")) {
                                    running = true;
                                } else {
                                    running = false;
                                }
                            }
                        }
                    } else {
                        // If we did NOT disable sprinting
                        if(!disableSprinting) {
                            if(toggleSprint) {
                                // Press sprint button once to toggle sprint
                                if(Input.GetKeyUp(KeyCode.LeftShift)) {
                                    running = !running;
                                }
                            } else {
                                // Hold sprint button down to sprint
                                if(Input.GetKey(KeyCode.LeftShift)) {
                                    running = true;
                                } else {
                                    running = false;
                                }
                            }
                        }
                    }

            // Update the animator states every frame
            if(animator) {
                animator.SetBool("Grounded", grounded);
                animator.SetBool("Walking", walking);
                animator.SetBool("Jogging", jogging);
                animator.SetBool("Running", running);
                animator.SetBool("Jumping", jumping);
            }

        }

        // Here we are using the LateUpdate function to update the position and rotation
        // of our character. Using the LateUpdate will allow the character and camera to stay
        // on par with each other and compensate for framerate fluctuations. Choppy, jittery, or shakey looking
        // movement would be seen between the camera and player.
        void LateUpdate() {
                // Set the targetSpeed as the movementSpeed
                // We will make tweaks where necessary
                float targetSpeed = movementSpeed;

                // If we did NOT disable sprinting
                if(!disableSprinting) {
                    // If we are sprinting use the runSpeed, otherwise, use the default movementSpeed previously set in the frame
                    targetSpeed = ((running) ? runSpeed : movementSpeed);
                }

                // If we are not moving then set the desired speed to 0
                // so that we do drift forward. A very small but crucial few lines
                if(inputMagnitude <= 0.0f)
                    movementSpeed = 0;

                // Set the current speed to be the targetSpeed, smoothing it as it changes.
                // The smooth time will be selected based on the state that we are in.
                // If we are jogging the jogSmoothTime will be used. Otherwise, the normal speedSmoothTime
                currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, currentSpeed > walkSpeed ? jogSmoothTime : speedSmoothTime);

                // Using the SimpleMove function in the CharacterController we can move the player forward by our set currentSpeed.
                if(controllerType==1) {
                    float targetRotation = Mathf.Atan2 (inputXY.x, inputXY.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                    if(currentSpeed > targetSpeed / 4 && inputMagnitude > 0.3f) {
                        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
                    }
                    playerController.Move(transform.TransformDirection(Vector3.forward) * currentSpeed * Time.deltaTime);
                }

                if(controllerType==0 || controllerType==2 || controllerType==3) {

                    if(!rotationLock) {
                        float targetRotation = cameraTransform.eulerAngles.y;
                        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
                    }

                    if(rotationLock&&inputMagnitude>0) {
                        rotationLock = false;
                    }

                    Vector3 moveDirectionForward = transform.forward * inputXY.y;
                    Vector3 moveDirectionSide = transform.right * inputXY.x;

                    float movSpeed = 0.0f;

                    if(inputMagnitude <= 0.85f&&!running) {
                        movSpeed = Mathf.SmoothDamp (movSpeed, walkSpeed, ref speedSmoothVelocity, speedSmoothTime);
                    }

                    if(inputMagnitude > 0.85f&&!running) {
                        if(disableJogging) {
                            movSpeed = Mathf.SmoothDamp (movSpeed, walkSpeed, ref speedSmoothVelocity, speedSmoothTime);
                        } else {
                            movSpeed = Mathf.SmoothDamp (movSpeed, jogSpeed, ref speedSmoothVelocity, speedSmoothTime);  
                        }
                    }
                        
                    if(inputMagnitude > 0&&running) {
                        movSpeed = Mathf.SmoothDamp (movSpeed, runSpeed, ref speedSmoothVelocity, speedSmoothTime);
                    }

                    //find the direction
                    Vector3 direction = (moveDirectionForward + moveDirectionSide).normalized;
                    //find the distance
                    Vector3 distance = direction * movSpeed * Time.deltaTime;

                    // Apply Movement to Player
                    playerController.Move (distance);
                }

                if(controllerType==3) {
                    if(Input.GetButtonDown("LockRotation")) {
                        Debug.Log("LockPostion");
                        rotationLock = true;
                    }
                }

                if(Input.GetButtonDown("Jump") && grounded && !jumping && !disableJumping) {
                    if(!walking && !jogging && !running) {
                        StartCoroutine(standingJump());
                    } else {
                        StartCoroutine(jump());
                    }
                }

                playerVelocity.y += gravity * Time.deltaTime;

                playerController.Move(playerVelocity * Time.deltaTime);

                if(jumping) {
                    if(walking) {
                        playerController.Move(transform.TransformDirection(Vector3.forward) * jogSpeed * Time.deltaTime);
                    } else {
                        playerController.Move(transform.TransformDirection(Vector3.forward) * currentSpeed * Time.deltaTime);
                    }
                }

                // If head rotation is enabled in the inspector,
                // we can go ahead and do the math before applying the rotation
                // in the OnAnimatorIK function
                if(enableHeadRotation) {
                    solveHeadRotation();
                }
        }

        // Set the headRotation variable to be used in the OnAnimatorIK
        void solveHeadRotation() {
            
            if(animator) {
                // Get the head transform from the animator if set
                Transform head = animator.GetBoneTransform(HumanBodyBones.Head);

                // Find the desired rotation of the head based on the head's current position and rotation, and the head look transform's position.
                headRotation = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation((headLookAt.position - head.position).normalized, Vector3.Cross((headLookAt.position - head.position).normalized, transform.right));

                // Clamp the head rotation so that we can't unaturally spin the head around,
                // only allowing you to look over your left or right shoulder.
                headRotationY = headRotation.eulerAngles.y;
                if(headRotationY >= 75.0f && headRotationY < 180.0f) {
                    headRotationY = 75.0f;
                }
                if(headRotationY >= 180.0f && headRotationY <= 285.0f) {
                    headRotationY = 285.0f;
                }
                
                // Set the headRotation eulerAngles
                headRotation.eulerAngles = new Vector3 (headRotation.eulerAngles.x, headRotationY, 0);
            }

            
        }

        IEnumerator jump() {
            jumping = true;
            if(running) {
                playerVelocity.y += runningJumpHeight;
            } else if(jogging) {
                playerVelocity.y += jogJumpHeight;
            } else {
                playerVelocity.y += jumpHeight;
            }
            yield return new WaitForSeconds(jumpDuration);
            jumping = false;
        }

        IEnumerator standingJump() {
            ignoreGrounded = true;
            jumping = true;
            animator.CrossFade("Base Layer.Standing Jump", 0.05f, 0, 0.0f, 0.0f);
            yield return new WaitForSeconds(jumpWaitTimer);
            playerVelocity.y += jumpHeight;
            ignoreGrounded = false;
            yield return new WaitForSeconds(jumpDuration);
            jumping = false;
        }

        private void OnAnimatorIK (int layerIndex) {
            // If there is an animator, apply the gotten headRotation
            if(animator) {
                animator.SetBoneLocalRotation(HumanBodyBones.Head, headRotation);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(RealismController))]
    [CanEditMultipleObjects]
    class RealismControllerEditor : Editor {

        SerializedProperty controllerType;

        SerializedProperty useGamePad;
        SerializedProperty disableSprinting;
        SerializedProperty toggleSprint;
        SerializedProperty disableJogging;

        SerializedProperty cameraTransform;
        SerializedProperty playerBody;

        SerializedProperty walkSpeed;
        SerializedProperty jogSpeed;
        SerializedProperty runSpeed;

        SerializedProperty turnSmoothTime;
        SerializedProperty speedSmoothTime;
        SerializedProperty jogSmoothTime;

        SerializedProperty enableHeadRotation;
        SerializedProperty headLookAt;

        SerializedProperty gravity;
        SerializedProperty disableJumping;
        SerializedProperty jumpHeight;
        SerializedProperty jogJumpHeight;
        SerializedProperty runningJumpHeight;
        SerializedProperty jumpDuration;
        SerializedProperty standingJumpDuration;
        SerializedProperty jumpWaitTimer;

        public string ct;

        bool showGamepadOptions;
        bool showMovementOptions;
        bool showJumpOptions;
        bool showSmoothingOptions;
        bool showAnimatorIK;

        void OnEnable() {

            controllerType = serializedObject.FindProperty ("controllerType");

            useGamePad = serializedObject.FindProperty ("useGamePad");
            disableSprinting = serializedObject.FindProperty ("disableSprinting");
            toggleSprint = serializedObject.FindProperty ("toggleSprint");
            disableJogging = serializedObject.FindProperty ("disableJogging");

            cameraTransform = serializedObject.FindProperty ("cameraTransform");
            playerBody = serializedObject.FindProperty ("playerBody");
            
            walkSpeed = serializedObject.FindProperty ("walkSpeed");
            jogSpeed = serializedObject.FindProperty ("jogSpeed");
            runSpeed = serializedObject.FindProperty ("runSpeed");

            turnSmoothTime = serializedObject.FindProperty ("turnSmoothTime");
            speedSmoothTime = serializedObject.FindProperty ("speedSmoothTime");
            jogSmoothTime = serializedObject.FindProperty ("jogSmoothTime");

            enableHeadRotation = serializedObject.FindProperty ("enableHeadRotation");
            headLookAt = serializedObject.FindProperty ("headLookAt");
            
            gravity = serializedObject.FindProperty ("gravity");
            disableJumping = serializedObject.FindProperty ("disableJumping");
            jumpHeight = serializedObject.FindProperty ("jumpHeight");
            jogJumpHeight = serializedObject.FindProperty ("jogJumpHeight");
            runningJumpHeight = serializedObject.FindProperty ("runningJumpHeight");
            jumpDuration = serializedObject.FindProperty ("jumpDuration");
            standingJumpDuration = serializedObject.FindProperty ("standingJumpDuration");
            jumpWaitTimer = serializedObject.FindProperty ("jumpWaitTimer");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update ();

            switch(controllerType.intValue) {
                case 0:
                    ct = "First Person Controller";
                    break;
                case 1:
                    ct = "Third Person Forward Based";
                    break;
                case 2:
                    ct = "Third Person WSAD";
                    break;
                case 3:
                    ct = "Prop Hunt";
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.IntSlider (controllerType, 0, 3, new GUIContent ("Controller Type"));
            ProgressBar (controllerType.intValue / 3.0f, ct);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Realism Controller v0.3", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.ObjectField(cameraTransform, typeof(Transform));

            EditorGUILayout.Space();

            showGamepadOptions = EditorGUILayout.Foldout(showGamepadOptions, "Gamepads and Controllers");

            if(showGamepadOptions) {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useGamePad, new GUIContent("Enable Controllers"));
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            showMovementOptions = EditorGUILayout.Foldout(showMovementOptions, "Player Movement");

            if(showMovementOptions) {
                EditorGUILayout.Space();
                EditorGUILayout.ObjectField(playerBody, typeof(GameObject));

                EditorGUILayout.Space();
                EditorGUILayout.Slider (gravity, -100.0f, 100.0f, new GUIContent ("Gravity"));
                ProgressBar (gravity.floatValue / 100.0f, "");
                EditorGUILayout.Space();
                
                EditorGUILayout.Space();
                EditorGUILayout.Slider (walkSpeed, 0.0f, 10.0f, new GUIContent ("Walk Speed"));
                ProgressBar (walkSpeed.floatValue / 10.0f, "");
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(disableSprinting, new GUIContent("Disable Sprinting"));
                EditorGUILayout.Space();

                if(!disableSprinting.boolValue) {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(toggleSprint, new GUIContent("'Toggle-Sprint' Mode"));

                    EditorGUILayout.Space();
                    EditorGUILayout.Slider (runSpeed, 0.0f, 10.0f, new GUIContent ("Sprint Speed"));
                    ProgressBar (runSpeed.floatValue / 10.0f, "");
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(disableJogging, new GUIContent("Disable Jogging"));

                if(!disableJogging.boolValue) {
                    EditorGUILayout.Space();
                    EditorGUILayout.Slider (jogSpeed, 0.0f, 10.0f, new GUIContent ("Jog Speed"));
                    ProgressBar (jogSpeed.floatValue / 10.0f, "");
                }

            }

            EditorGUILayout.Space();

            showJumpOptions = EditorGUILayout.Foldout(showJumpOptions, "Jumping");

            if(showJumpOptions) {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(disableJumping, new GUIContent("Disable Jumping"));
                EditorGUILayout.Space();

                if(!disableJumping.boolValue) {
                    EditorGUILayout.Slider (jumpWaitTimer, 0.0f, 5.0f, new GUIContent ("Jump Wait Timer"));
                    ProgressBar (jumpWaitTimer.floatValue / 5.0f, "");
                    EditorGUILayout.Space();

                    EditorGUILayout.Slider (jumpHeight, -100.0f, 100.0f, new GUIContent ("Jump Height"));
                    ProgressBar (jumpHeight.floatValue / 100.0f, "");
                    EditorGUILayout.Space();

                    EditorGUILayout.Slider (jogJumpHeight, -100.0f, 100.0f, new GUIContent ("Jog Jump Height"));
                    ProgressBar (jogJumpHeight.floatValue / 100.0f, "");
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.Slider (runningJumpHeight, -100.0f, 100.0f, new GUIContent ("Running Jump Height"));
                    ProgressBar (runningJumpHeight.floatValue / 100.0f, "");
                    EditorGUILayout.Space();

                    EditorGUILayout.Slider (standingJumpDuration, 0.0f, 2.0f, new GUIContent ("Standing Jump Duration"));
                    ProgressBar (standingJumpDuration.floatValue / 2.0f, "");
                    EditorGUILayout.Space();

                    EditorGUILayout.Slider (jumpDuration, 0.0f, 2.0f, new GUIContent ("Jump Duration"));
                    ProgressBar (jumpDuration.floatValue / 2.0f, "");
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.Space();

            showSmoothingOptions = EditorGUILayout.Foldout(showSmoothingOptions, "Player Movement Smoothing");

            if(showSmoothingOptions) {
                EditorGUILayout.Space();
                EditorGUILayout.Slider (speedSmoothTime, 0.0f, 5.0f, new GUIContent ("Walking Speed Smoothing"));
                ProgressBar (speedSmoothTime.floatValue / 5.0f, "");
                EditorGUILayout.Space();
                EditorGUILayout.Slider (jogSmoothTime, 0.0f, 5.0f, new GUIContent ("Jog/Run Speed Smoothing"));
                ProgressBar (jogSmoothTime.floatValue / 5.0f, "");
                EditorGUILayout.Space();
                EditorGUILayout.Slider (turnSmoothTime, 0.0f, 5.0f, new GUIContent ("Turn/Rotation Smoothing"));
                ProgressBar (turnSmoothTime.floatValue / 5.0f, "");
            }

            EditorGUILayout.Space();

            showAnimatorIK = EditorGUILayout.Foldout(showAnimatorIK, "Animator IK");

            if(showAnimatorIK) {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(enableHeadRotation, new GUIContent("Enable Head Rotation"));
                EditorGUILayout.Space();

                if(enableHeadRotation.boolValue) {
                    EditorGUILayout.ObjectField(headLookAt, typeof(Transform));
                    // EditorGUILayout.Space();
                    // EditorGUILayout.Slider (headRotationSpeed, 0.0f, 10.0f, new GUIContent ("Head Look Speed"));
                    // ProgressBar (headRotationSpeed.floatValue / 10.0f, "");
                }
            }
            
            serializedObject.ApplyModifiedProperties ();
        }

        void ProgressBar (float value, string label) {
            Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
            EditorGUI.ProgressBar (rect, value, label);
            EditorGUILayout.Space ();
        }

    }
    #endif

}