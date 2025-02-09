using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{
    [SerializeField]

    public bool isJumping;
    public bool isReleasing;
    public bool isPressing;
    public float moveInput;
    public bool isInteract;
    // Start is called before the first frame update
    void Start()
    {
        isInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement input
        moveInput = Input.GetAxis("Horizontal");
        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressing = false;
            isReleasing = false;
            isJumping = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isPressing = false;
            isReleasing = true;
            isJumping = false;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            isPressing = true;
            isReleasing = false;
            isJumping = false;
        }
        if(Input.GetKeyDown(KeyCode.E) && !isInteract)
        {
            isInteract = true;
        }
        else if(isInteract && Input.GetKeyDown(KeyCode.E))
        {
            isInteract = false;
        }
    }
    public bool IsJumping
    {
        get { return isJumping; }
    }
    public bool IsReleaseing
    {
        get { return isReleasing; }
    }
    public bool IsPressing
    {
        get { return isPressing; }
    }
    public float MoveInput
    {
        get { return moveInput; }
    }
    public bool IsInteract
    {
        get { return isInteract;  }
    }

}
