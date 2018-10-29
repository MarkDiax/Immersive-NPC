using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Settings 
    [System.Serializable]
    public class MoveSettings {
        public float forwardVelocity = 12 , rotateVelocity = 100, jumpVelocity = 25, distanceToGrounded = 0.1f;
        public LayerMask ground;
    }

    [System.Serializable]
    public class PhysicsSettings {
        public float downAcceleration = 0.75f;
    }

    [System.Serializable]
    public class InputSettings {
        public float inputDelay = 0.1f;
        public string forward_Axis = "Vertical", turn_Axis = "Horizontal", jump_Axis = "Jump";  
    }
    #endregion


    public MoveSettings moveSettings = new MoveSettings();
    public PhysicsSettings physicsSettings = new PhysicsSettings();
    public InputSettings inputSettings = new InputSettings();

    private Vector3 _velocity = Vector3.zero;
    private Quaternion _targetRotation;
    private Rigidbody _rigidbody;
    private float _forwardInput, _turnInput, _jumpInput;

    private Animator _animator;

    // Use this for initialization
    void Start () {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _targetRotation = transform.rotation;
        _forwardInput = _turnInput = _jumpInput = 0;
    }

    // Update is called once per frame
    void Update() {
        GetInput();
        Turn();
    }

    void FixedUpdate() {
        Run();
        Jump();

        _rigidbody.velocity = transform.InverseTransformDirection(_velocity);
    }



    private void Run() {
        if(Mathf.Abs(_forwardInput) > inputSettings.inputDelay) {
            //_rigidbody.velocity = transform.forward * _forwardInput * moveSettings.forwardVelocity;
            _velocity.z = moveSettings.forwardVelocity * _forwardInput;
        } else {
            //_rigidbody.velocity = Vector3.zero;
            _velocity.z = 0;
        }

    }
    private void Turn() {
        if(Mathf.Abs(_turnInput) > inputSettings.inputDelay) {
            _targetRotation *= Quaternion.AngleAxis(moveSettings.rotateVelocity * _turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = _targetRotation;
    }

    private void Jump() {
        if(_jumpInput > 0 && Grounded()) {//jump 
            _velocity.y = moveSettings.jumpVelocity;

        } else if(_jumpInput ==0 && Grounded()) {//zero out our velocity
            _velocity.y = 0;

        } else { //decrease velocity
            _velocity.y -= physicsSettings.downAcceleration;
        }
    }

    public bool Grounded() {
        return Physics.Raycast(transform.position, Vector3.down, moveSettings.distanceToGrounded, moveSettings.ground);
    }

    private void GetInput() {
        _forwardInput = Input.GetAxis(inputSettings.forward_Axis);
        _turnInput = Input.GetAxis(inputSettings.turn_Axis);
        _jumpInput = Input.GetAxisRaw(inputSettings.jump_Axis);
    }

    public Quaternion TargetRotation {
        get { return _targetRotation; }
    }

}
