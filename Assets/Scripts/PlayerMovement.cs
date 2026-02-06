using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 Movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        context.ReadValue<Vector2>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
