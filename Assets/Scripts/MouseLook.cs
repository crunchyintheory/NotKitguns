using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField, Range(0, 200)] float _sensitivityX = 100;
    [SerializeField, Range(0, 200)] float _sensitivityY = 100;

    [SerializeField] Transform _playerBody;

    float _xRotation;

    private void Update()
    {
        float x = Input.GetAxis("Mouse X") * _sensitivityX * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * _sensitivityY * Time.deltaTime;

        _xRotation -= y;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(_xRotation, 0, transform.localRotation.eulerAngles.z);
        _playerBody.Rotate(Vector3.up * x);
    }
}
