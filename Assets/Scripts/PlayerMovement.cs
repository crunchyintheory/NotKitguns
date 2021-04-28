using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _ladderMask;

    [SerializeField] private float _speed = 12;
    [SerializeField] private float _jumpHeight = 3;

    [SerializeField] private float _groundDistance = 0.4f;

    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _landingsfx;

    [SerializeField] new private Camera camera;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private LayerMask interactableLayer;

    public Vector3 _velocity;
    private bool _isGrounded;
    private bool _isClimbing;
    private byte _dashState;

    public bool _locked;

    private GameObject pickup;
    private GameObject onCursor;
    private bool pickupOnCursor;

    public CharacterController Controller { get { return _controller; } }

    private IEnumerator PickupDetection()
    {
        while (true)
        {
            if (pickup != null) yield return null;
            if (Physics.Raycast(
                    origin: camera.transform.position,
                    direction: camera.transform.forward,
                    maxDistance: 10,
                    layerMask: interactableLayer,
                    hitInfo: out RaycastHit hit))
            {
                pickupOnCursor = (pickupLayer.value & (1 << hit.collider.gameObject.layer)) > 0;
                onCursor = hit.collider.gameObject;
            }
            else onCursor = null;
            for (int i = 0; i < 10; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(PickupDetection());
    }

    private void Interact()
    {
        if (pickup == null)
        {
            if (onCursor != null)
                if (pickupOnCursor)
                    Pickup(onCursor);
                else
                    Interact(onCursor);
        }
        else
            Drop();
    }

    private void Interact(GameObject interactable)
    {
        IInteractable c = interactable.GetComponent<IInteractable>();
        if(c != null) c.Interact();
    }

    private void Pickup(GameObject pickup)
    {
        this.pickup = pickup;
        Rigidbody rb = pickup.GetComponent<Rigidbody>();
        if (rb != null)
            rb.useGravity = false;
        onCursor = null;
    }

    private void Drop()
    {
        Rigidbody rb = pickup.GetComponent<Rigidbody>();
        if (rb != null)
            rb.useGravity = true;
        pickup = null;
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

        if (Input.GetKeyUp(KeyCode.E))
            Interact();

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

        if(pickup != null)
        {
            pickup.transform.position = camera.transform.position + (camera.transform.forward * 4);
            pickup.transform.rotation = camera.transform.rotation;
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
