using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _groundCheck;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _ladderMask;

    [SerializeField] float _speed = 12;
    [SerializeField] float _jumpHeight = 3;

    [SerializeField] float _groundDistance = 0.4f;

    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip _landingsfx;

    public Vector3 _velocity;
    bool _isGrounded;
    bool _isClimbing;
    byte _dashState;

    public bool _locked;

    public CharacterController Controller { get { return _controller; } }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Q))
            Application.Quit();

        //Ground check

        _isClimbing = Physics.CheckSphere(_groundCheck.position, _groundDistance, _ladderMask);

        bool wasGrounded = _isGrounded;
        _isGrounded = _isClimbing || Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if(_isGrounded)
        {
            if (!wasGrounded)
            {
                if (_locked) _velocity = Vector3.down * -2; //Hyperfriction landings
                _locked = false;
            }

            _dashState = 0;

            if(_velocity.y < 0)
            {
                _velocity.y = -2;
            }
        }

        //Move
        
        if(!_locked)
        {
            int multiplier = (Input.GetKey("left shift") && _isGrounded) ? 2 : 1; // Sprint

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            if (_isClimbing)
            {
                move += transform.up * z;
            }

            _controller.Move(move * _speed * Time.deltaTime * multiplier);
        }

        //Jump

        if(Input.GetButtonDown("Jump") && _isGrounded && !_locked)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2 * Physics.gravity.y);
        }/*

        if(Input.GetKeyDown("left shift") && !_isGrounded && _dashState == 0 && !_locked)
        {
            _dashState = 1;
            StartCoroutine(DashCoroutine());
        }*/

        //Gravity

        if(!_isClimbing)
        {
            _velocity += Physics.gravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime);
        }

        if (transform.position.y <= -100)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DashCoroutine()
    {
        Vector3 direction = transform.forward;
        float time = 0;
        while(time < 2 && _dashState == 1)
        {
            _controller.Move(direction * 15 * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        _dashState = 2;
    }
}
