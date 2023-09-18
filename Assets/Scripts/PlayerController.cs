using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [FormerlySerializedAs("WalkingSpeed")]
    public float walkingSpeed = 5.5f;
    
    [FormerlySerializedAs("RunningSpeed")]
    public float runningSpeed = 10.0f;
    
    [FormerlySerializedAs("JumpSpeed")]
    public float jumpSpeed = 6.0f;
    
    [FormerlySerializedAs("Gravity")]
    public float gravity = 20.0f;
    
    [FormerlySerializedAs("LookSpeed")]
    public float lookSpeed = 8.5f;
    
    [FormerlySerializedAs("LookXLimit")]
    public float lookXLimit = 120.0f;
    
    public Transform playerCamera;

    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>().transform;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        
        // Press Left Shift to run
        var isRunning = Input.GetKey(KeyCode.LeftShift);
        
        var curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        
        var movementDirectionY = _moveDirection.y;
        
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
        {
            _moveDirection.y = jumpSpeed;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (_characterController.isGrounded is false)
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove is false)
        {
            return;
        }
        
        _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}