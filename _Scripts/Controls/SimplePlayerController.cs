using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float rotSpeed = 20f;
    private Vector2 inputDir = Vector2.zero;
    private Vector2 inputLook = Vector2.zero;

    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        inputDir = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        inputLook = ctx.ReadValue<Vector2>();
    }
    private void Update()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
        _smoothedMovementInput,
        inputDir,
        ref _movementInputSmoothVelocity,
        0.1f);

        rb.velocity =new Vector3( _smoothedMovementInput.x * speed,0, _smoothedMovementInput.y * speed);

        transform.rotation = Quaternion.LookRotation(new Vector3(inputLook.x,inputLook.y,0));
    }

}
