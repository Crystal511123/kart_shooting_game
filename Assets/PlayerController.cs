using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    //Player Property Variable
    private Rigidbody _rb;
    private WirelessMotionController _controller;

    //Forward Speed Variables
    private float _currentSpeed = 0f;
    [Tooltip("The maximum forward speed the kart can reach.")]
    public float MaxSpeed;
    [Tooltip("The maximum backward speed the kart can reach.")]
    public float MinSpeed;
    [Tooltip("The maximum forward boosted speed the kart can reach.")]
    public float BoostSpeed;
    [Tooltip("The acceleration.")]
    public float Acceleration;


    //Rotate Speed Variables
    private Vector3 _turnAngle;
    [Tooltip("Max angular speed for rotations.")]
    public float MaxRotationAngle;
    [Tooltip("Rotation Speed.")]
    public float RotationSpeed;
    [Tooltip("Hand use to show rotations.")]
    public Transform Hands;
    [Tooltip("Front left wheel.")]
    public Transform FrontLeftWheel;
    [Tooltip("Front right wheel.")]
    public Transform FrontRightWheel;
    [Tooltip("Main Camera")]
    public Camera MainCamera;

    //Drift and Boost Variables
    private float _outwardsDriftForce = 12500;
    private bool _driftLeft = false;
    private bool _driftRight = false;
    private bool _isBoosting = false;
    private float _driftTime = 0f;
    private float _boostTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _controller = GetComponent<WirelessMotionController>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {   MainCamera = Camera.main;
        MainCamera.fieldOfView = (_controller.p/32 + 43);
        ControllerSteer();
        ControllerMove();

        //Steer();
        //Move();
        Drift();
        Boost();
        
    }
    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "gun"){
            Debug.Log("on collision");
            BoxCollider[] boxCollider;
            boxCollider = collision.gameObject.GetComponents<BoxCollider>();
            for(int i=0;i<2;i++) boxCollider[i].isTrigger = true;
        }
    }
    // Move the player forward
    private void Move()
    {
        // Calculate current speed
        if (_driftLeft || _driftRight)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.fixedDeltaTime * Acceleration * 3f);
        }
        else if (Input.GetKey(KeyCode.W)) //Move forward when press W
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, MaxSpeed, Time.fixedDeltaTime * Acceleration * 1f);
        }
        else if (Input.GetKey(KeyCode.S)) //Move backward when press S
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, MinSpeed, Time.fixedDeltaTime * Acceleration * 2f);
        }
        else //Slow down when no key's pressed
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.fixedDeltaTime * Acceleration * 6f);
        }
        

        if (_driftLeft)
        {
            _rb.AddForce(transform.right * _outwardsDriftForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else if (_driftRight)
        {
            _rb.AddForce(transform.right * -_outwardsDriftForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        RotateRigidbody();

        Vector3 vel = transform.forward * _currentSpeed;
        vel.y = _rb.velocity.y; //Keep the gravity
        _rb.velocity = vel;//Apply the speed to the rigidbody

    }

    //Steer the kart's front wheels
    private void Steer()
    {
        if (Input.GetKey(KeyCode.A)) //Rotate left when press A
        {
            RotateVisual(MaxRotationAngle, RotationSpeed * 1f);
        }
        else if (Input.GetKey(KeyCode.D)) //Rotate right when press D
        {
            RotateVisual(-MaxRotationAngle, RotationSpeed * 1f);
        }
        else //Rotate back when no key's pressed
        {
            RotateVisual(0, RotationSpeed * 3f);
        }
    }
    //Turn the kart's rigidbody's direction according to frontwheel
    private void RotateRigidbody()
    {
        _turnAngle = FrontLeftWheel.eulerAngles - transform.eulerAngles;
        _turnAngle.y = RegularizeAngle(_turnAngle.y);

        Quaternion deltaRotation = Quaternion.Euler(Mathf.Sign(_currentSpeed) * _turnAngle * Time.fixedDeltaTime * 2);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
    }

    //Regularize angle to -180-180
    private float RegularizeAngle(float angle)
    {
        //equivalent to angle = (angle + 540) % 360 -180;
        angle = (angle > 180) ? angle - 360 : angle;
        angle = (angle < -180) ? angle + 360 : angle;
        return angle;
    }
    //Rotate the kart's visual to target angle
    private void RotateVisual(float targetAngle, float rotateSpeed)
    {
        float handAngle = RegularizeAngle(Hands.localRotation.eulerAngles.z);
        float wheelAngle = RegularizeAngle(FrontLeftWheel.localRotation.eulerAngles.y);
        Hands.Rotate(0, 0, (targetAngle - handAngle) * Time.fixedDeltaTime * rotateSpeed, Space.Self);
        FrontLeftWheel.Rotate(0, (-targetAngle - wheelAngle) * Time.fixedDeltaTime * rotateSpeed, 0, Space.Self);
        FrontRightWheel.Rotate(0, (-targetAngle - wheelAngle) * Time.fixedDeltaTime * rotateSpeed, 0, Space.Self);
    }

    private void Drift()
    {
        if (!_controller.isTrigger && _controller.yaw > 30 && _controller.yaw < 180)
        {
            _driftTime += Time.fixedDeltaTime;
            _driftLeft = true;
            _driftRight = false;
            Debug.Log("drift left");
        } else if (!_controller.isTrigger && _controller.yaw > 180 && _controller.yaw < 330)
        {
            _driftTime += Time.fixedDeltaTime;
            _driftRight = true;
            _driftLeft = false;
             Debug.Log("drift right");
        } else if (!_isBoosting)
        {
            _driftLeft = false;
            _driftRight = false;
            _isBoosting = true;
            _boostTime = (_driftTime > 0.5f) ? 1f : 0f;
            _driftTime = 0f;
            
        }
    }

    private void Boost()
    {
        if (_boostTime > 0f)
        {
            _boostTime -= Time.fixedDeltaTime;
            MaxSpeed = BoostSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, MaxSpeed, Time.fixedDeltaTime);
        }
        else
        {
            MaxSpeed = BoostSpeed - 15;
            _isBoosting = false;
        }
    }

    private void ControllerSteer()
    {
        if (_controller.yaw < 180 && _controller.yaw > MaxRotationAngle)
            _controller.yaw = MaxRotationAngle;
        else if (_controller.yaw > 180 && _controller.yaw < 360 - MaxRotationAngle)
            _controller.yaw = 360 - MaxRotationAngle;

        RotateVisual(_controller.yaw, 1 / Time.fixedDeltaTime);
    }

    private void ControllerMove()
    {
        if (_controller.isTrigger)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, MaxSpeed, Time.fixedDeltaTime * Acceleration * 1f);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.fixedDeltaTime * Acceleration * 6f);
        }

        RotateRigidbody();

        Vector3 vel = transform.forward * _currentSpeed;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }
}