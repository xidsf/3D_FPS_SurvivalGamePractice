using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float applySpeed;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce;


    //상태 변수
    private bool isRun = false;
    private bool isGround = true;
    private bool isCrouch = false;

    //앉았을 때 얼마나 앉았을 지 결정하는 변수
    [SerializeField]
    private float crouchPoxY;
    private float originPoxY;
    private float applyCrouchPoxY;


    //카메라 민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 회전 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    //필요 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private GunController theGunController;
    private CapsuleCollider capsuleCollider;
    private Rigidbody myRigid;
    

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        applySpeed = walkSpeed;
        originPoxY = theCamera.transform.localPosition.y;
        applyCrouchPoxY = originPoxY;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        tryJump();
        tryRun();
        tryCrouch();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void tryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;

        if(isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPoxY = crouchPoxY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPoxY = originPoxY;
        }

        StartCoroutine("CrouchCoroutine");
        

    }

    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPoxY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPoxY, 0.2f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if(count > 15)
            {
                break;
            }
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPoxY, 0);
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void tryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isCrouch)
            Crouch();
        myRigid.velocity = transform.up * jumpForce;
    }

    private void tryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        if (isCrouch)
            Crouch();
        isRun = true;
        theGunController.CancelFineSight();
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        Vector3 velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        //상하 카메라 회전

        float _xRotation = Input.GetAxisRaw("Mouse Y");
        
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0, 0);
    }

    private void CharacterRotation()
    {
        //좌우 케릭터 회전

        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0, _yRotation, 0) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }
}
