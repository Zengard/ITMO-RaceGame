using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    //Componentss
    protected InputMaster carInputMaster;
    private Animator _carAnimator;
    Rigidbody carRB;

    //movement
    private Vector2 inputVector;
    private Vector3 moveDirection;

    //jump
    private bool isJumpPressed = false;
    private bool inAir = false;


    public GameObject baseModel;
    public GameObject cameraPosition;

    [SerializeField] private Transform originCameraPosition;
    [SerializeField] private Transform LimitCameraPosition;

    [Header("Camera")]
    public CameraMovement camera;

    [Header("UI")]
    public TextMeshProUGUI speedometr;
    public float timeStart;
    public TextMeshProUGUI timerText;
    private bool startTime = false;

    //ParticleSystem
    public ParticleSystem hitParticle;
    public ParticleSystem speedParticle;
    public ParticleSystem jumpParticle;

    [Header("Movement")]
    public float moveSpeed = 5;
    public float speedCap = 50;
    public float jumpForce;
    public float Jumpindicator;
    [SerializeField] private float speedRotation = 5;
    private bool isReachSpeedLimit = false;
    public float currentSpeed;

    //Animations
    private int jump2;
    private int jump3;


    //Audio
    public AudioClip getHit;
    public AudioClip boosterSound;
    public AudioClip jumpSound;
    public AudioClip fullSpeedSound;
    public AudioClip windSound;
    private AudioSource carAS;

    public bool startMove = false;

    private void Awake()
    {
        carInputMaster = new InputMaster();
        _carAnimator = GetComponent<Animator>();
        carAS = GetComponent<AudioSource>();
        jump2 = Animator.StringToHash("Jump2");
        jump3 = Animator.StringToHash("Jump3");

        carInputMaster.Car.Enable();
        //carInputMaster.Car.Jump.started += Jump;
        //carInputMaster.Car.Jump.canceled += Jump;
    }

    private void Start()
    {
        carRB = GetComponent<Rigidbody>();
        carRB.freezeRotation = true;
        timerText.text = timeStart.ToString();
        startTime = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(startMove == false)
            {
                startMove = true;
            }
        }

        currentSpeed = carRB.velocity.magnitude;

        if (Input.GetKeyDown(KeyCode.Space) && Jumpindicator > 0)
        {
            ExecuteJump();
        }
        if (startTime == true)
        {
            timeStart += Time.deltaTime;
        }
        timerText.text = "Time: " + timeStart.ToString();

        if (inputVector.x == 0)
            transform.rotation = Quaternion.Lerp(baseModel.transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0), Time.deltaTime * speedRotation);

        if (inputVector.x < 0)
        {
            transform.rotation = Quaternion.Lerp(baseModel.transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 8), Time.deltaTime * speedRotation);
        }
        else if
            (inputVector.x > 0)
        {
            transform.rotation = Quaternion.Lerp(baseModel.transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, -8), Time.deltaTime * speedRotation);
        }

        if (speedCap < 25)
        {
            speedCap = 25;
        }

        if (carRB.velocity.magnitude >= 200)
        {
            isReachSpeedLimit = true;
        }
        else
        {
            isReachSpeedLimit = false;
        }

        if (isReachSpeedLimit == true)
        {
            speedParticle.Play();
            //windSound.Play();
            carAS.PlayOneShot(windSound);
            carAS.PlayOneShot(fullSpeedSound);
            cameraPosition.transform.position = Vector3.Lerp(cameraPosition.transform.position, LimitCameraPosition.position, Time.deltaTime * 2);
        }
        else
        {
            speedParticle.Stop();
            //windSound.Stop();
            cameraPosition.transform.position = Vector3.Lerp(cameraPosition.transform.position, originCameraPosition.position, Time.deltaTime * speedRotation);
        }      

    }

    private void FixedUpdate()
    {
        MoveCar();
        
        SpeedControl();
    }

    private void MoveCar()
    {
        inputVector = carInputMaster.Car.Movement.ReadValue<Vector2>();
        moveDirection = Vector3.forward + Vector3.right * inputVector.x;

        if(startMove == true)
        {
            carRB.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        speedometr.text = "Speed: " + ((int)carRB.velocity.magnitude) + "";
    }

    private void Jump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    private void ExecuteJump()
    {      
        if(Jumpindicator == 1)
        {
            carRB.velocity = new Vector3(carRB.velocity.x, jumpForce *1.3f, carRB.velocity.z);
            jumpParticle.Play();
            carAS.PlayOneShot(jumpSound);
            Jumpindicator--;
        }
        else
        {
            carRB.velocity = new Vector3(carRB.velocity.x, jumpForce, carRB.velocity.z);
            Jumpindicator--;
        }
        //carRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(carRB.velocity.x, 0f, carRB.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > speedCap)
        {
            Vector3 limitedVel = flatVel.normalized * speedCap;
            carRB.velocity = new Vector3(limitedVel.x, carRB.velocity.y, limitedVel.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Booster")
        {
            speedCap += 25;
            carAS.PlayOneShot(boosterSound);
            //moveSpeed += 2;
        }

        if (other.gameObject.tag == "Rock")
        {
            speedCap /= 2;
            //moveSpeed -= 2;
            hitParticle.Play();
            carAS.PlayOneShot(getHit);
        }

        if (other.gameObject.tag == "Finish")
        {
            //startTime = false;
            speedCap = 0;
            //moveSpeed = 0;
            isReachSpeedLimit = false;
            carInputMaster.Car.Disable();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            camera.onSand = false;
            jumpForce = 7;
            Jumpindicator = 2;
        }

        if (collision.gameObject.tag == "Sand")
        {
            camera.onSand = true;
        }

        if (collision.gameObject.tag == "JumpPad")
        {
            //Jumpindicator = 3;
            jumpForce = 30;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "JumpPad")
        {          
            //jumpForce = 7;
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        //if (collision.gameObject.tag == "Ground")
        //{
        //    camera.onSand = false;
        //    jumpForce = 7;
        //    Jumpindicator = 1;
        //}
    }

}
