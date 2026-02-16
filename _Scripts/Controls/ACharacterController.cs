using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Controls
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(IGroundDetector))]
    public class ACharacterController : MonoBehaviour,ICharacterController
    {
        // Start is called before the first frame update
        [Header("Locomotion")]
        [SerializeField] protected float MaxSpeed = 8;
        [SerializeField] protected float Acceleration = 200;
        [SerializeField] protected AnimationCurve AccelerationFactorFromDot;
        [SerializeField] protected float MaxAccelForce = 150;
        [SerializeField] protected AnimationCurve MaxAccelerationForceFactorFromDot;
        [SerializeField] protected Vector3 ForceScale = new Vector3(1, 0, 1);
        [SerializeField] float GravityScaleDrop = 10f;
        [SerializeField] float rotationSpeed = 20f;

        [Header("Jumping")]
        [SerializeField] float jumpHeight = 3f;
        protected float JumpUpVel;
        float gravity;
        [SerializeField] float maxJumpTime = .5f;
        [SerializeField] float heaviness = 3f;
        [Header("Jumping Logic")]
        [SerializeField]float JumpBufferTime = 0.25f;
        protected float _jumpBufferTimer = 0.0f;
           
        ///PRIVATE VARS
        protected Transform cameraTransform;
        protected IGroundDetector _groundDetector;
        protected Vector3 m_GoalVel= Vector3.zero;    
        protected Vector3 _input=Vector3.zero;

        bool _isJumpPressed=false;
        protected bool _isJumping = false;

        //test


        
        public Vector3 MoveDirection {
            get { return (new Vector3(cameraTransform.forward.x, 0,cameraTransform.forward.z) * _input.z + new Vector3(cameraTransform.right.x,0, cameraTransform.right.z) * _input.x).normalized; }
        }
        public bool IsGrounded
        {
            get
            {
                return _groundDetector.IsGrounded;
            }
        }
        public bool IsMoving
        {
            get {return _input.magnitude > 0&&!_isJumping; }
        }
        public bool IsJumping
        {
            get { return _isJumping; }
        }
        public bool IsJumpPressed
        {
            get { return _isJumpPressed; }
        }
        public bool IsFalling { get { return _rb.velocity.y < -9f; } }// si la velocidad en el eje y(altura) es negativa esta cayendo, es de cajon tonto de los cojones
        public Vector2 inputDir { get { return new Vector2(_input.x,_input.z); } }
        protected Rigidbody _rb;
        protected void Start()
        {
            cameraTransform=Camera.main.transform;
            _rb = GetComponent<Rigidbody>();
            _groundDetector=GetComponent<IGroundDetector>();
            //Jumping
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
            JumpUpVel = (2 * jumpHeight) / timeToApex;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void onMove(InputAction.CallbackContext ctx)
        {
            _input=new Vector3(ctx.ReadValue<Vector2>().x,0, ctx.ReadValue<Vector2>().y);
        }

        public void onJump(InputAction.CallbackContext ctx)
        {
            _isJumpPressed = ctx.ReadValueAsButton();
            if(ctx.performed)
            _jumpBufferTimer = JumpBufferTime;
        }
        public void FixedUpdate()
        {
            MovePlayer();
            JumpPlayer();
            HandleGravity();
        }

        protected void HandleGravity()
        {
            if (!_groundDetector.IsGrounded)
            {
                if (_isJumpPressed)
                {
                    _rb.AddForce(gravity * _rb.mass * Vector3.up);
                }
                else
                {
                   
                     _rb.AddForce((gravity * _rb.mass * Vector3.up)*heaviness);
                }
            }
        }

        protected void JumpPlayer()
        {
            if (_jumpBufferTimer>0 && _groundDetector.IsGrounded&& !_isJumping)
            {
               _isJumping = true;

                //Debug.Log("[PLAYER] SALTANDO");
                _rb.velocity=new Vector3(_rb.velocity.x,JumpUpVel,_rb.velocity.z);
            }
            else if( _groundDetector.IsGrounded && _isJumping)
            {
                _isJumping = false;
            }
            if(_jumpBufferTimer>0) 
            _jumpBufferTimer -= Time.fixedDeltaTime;
        }

        protected virtual void MovePlayer()
        {
            Vector3 moveDirection = (cameraTransform.forward.normalized * _input.z + cameraTransform.right.normalized * _input.x).normalized;
            Vector3 unitVel = moveDirection.normalized;

            //Calculate new goal vel...
            float velDot = Vector3.Dot(m_GoalVel, unitVel);
            float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = moveDirection * MaxSpeed;
            m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accel * Time.deltaTime);
            //Actual force...

            Vector3 neededAccel = (m_GoalVel - _rb.velocity) / Time.deltaTime;
            float maxAccel = MaxAccelForce + MaxAccelerationForceFactorFromDot.Evaluate(velDot);
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, ForceScale));

        }
    }
}
