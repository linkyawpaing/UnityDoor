using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]

    Rigidbody2D rb;
    float MoveInput;
    float Speed;
    float JumpForce;
    int JumpCountValue;
    int JumpCount;
    float JumpTime;
    float JumpTimeCounter;
    bool IsGrounded;
    bool IsAir;
    SpriteRenderer sp;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsBox;
    public LayerMask WhatIsRope;
    float fullGravity = 2f;
    public Animator ani;

    public CapsuleCollider2D bc;

    RaycastHit2D RayHit;
    RaycastHit2D WallRayHit1;
    RaycastHit2D WallRayHit2;
    RaycastHit2D BoxHit;
    RaycastHit2D RopeHit;
    RaycastHit2D FrontHit;
    [SerializeField]
    internal Weapon wp;
    public GameObject player;
    public GameObject box;


    /* string idle = "PlayerIdel";
     string walk = "PlayerWalk";
     string jumpup = "PlayerJumpUp";
     string jumpdown = "PlayerJumpDown";*/
    public bool facingRight;
    bool IsPushing = false;
    // Start is called before the first frame update

    //float DashSpeed = 1000f;
    private float DashTime;
    float StartDashTime = 0.3f;
    bool IsDashing = false;
    float DashCoolDown;
    float StartDashCoolDown;
    bool AfterDash;

    public float WeaponCoolDown;
    public float WeaponCoolDownStart;
    public static bool AfterShoot;

    internal bool IsAttach;
    float PushForce;
    Transform AttachedTo;

    GameObject PullySelected;
    HingeJoint2D hj;
    GameObject[] links;
    internal bool collided;
    internal Transform parentname;
    float RightRotation;
    float LeftRotation;


    float AirTime;
    float AirTimeStart = 0;

    public ParticleSystem DustJump;
    public ParticleSystem DashEffect;

    public bool IsTouchingFront;
    public bool IsTouchingBack;
    public Transform FrontCheck;
    public bool WallSliding;
    public float WallSlidingSpeed;
    bool IsWallJumping;
    float XWallForce;
    float YWallForce;
    void Awake()
    {
        bc = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        hj = GetComponent<HingeJoint2D>();
        //box.GetComponent<FixedJoint2D>().enabled = false;
        Speed = 500f;
        JumpCount = JumpCountValue;

        wp.enabled = false;
        DashTime = StartDashTime;
        AfterShoot = true;
        AfterDash = true;
        IsAttach = false;
        collided = false;
        //AirTimeStart = 0.5f;
        AirTime = AirTimeStart;
        JumpForce = 18f;
        IsAir = false;
        JumpTime = 0.4f;
        IsTouchingFront = false;
        IsTouchingBack = false;
        WallSlidingSpeed = 20f;



        /*RightRotation = Mathf.Clamp(RightRotation, 90, -90);
        LeftRotation = Mathf.Clamp(RightRotation, 90, -90);*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        WallJumping();
        Wepon();
        PushPull();
        Dash();
        AirControl();
        RopeClime();

    }
    public void RopeClime()
    {
        CheckKeyBoardInput();
    }
    public void CheckKeyBoardInput()
    {
        RopeHit = Physics2D.Raycast(transform.position, Vector2.right * (transform.localScale.x * 0.5f), 0.5f, WhatIsRope);

        if (Input.GetKey("a") && IsAttach)
        {
            //rb.AddForce(new Vector3(-1,0,0)* PushForce);
            RopeHit.collider.gameObject.GetComponent<Rigidbody2D>();
            rb.AddRelativeForce(new Vector3(-1, 0, 0) * 100, ForceMode2D.Force);
        }
        if (Input.GetKey("d") && IsAttach)
        {
            RopeHit.collider.gameObject.GetComponent<Rigidbody2D>();
            rb.AddRelativeForce(new Vector3(1, 0, 0) * 100, ForceMode2D.Force);
        }
        else if (Input.GetKeyDown("w") && IsAttach)
        {
            Slide(1);

        }
        else if (Input.GetKeyUp("w"))
        {

            ani.SetBool("IsClimbUp", false);
        }
        else if (Input.GetKeyDown("s") && IsAttach)
        {
            Slide(-1);

        }
        else if (Input.GetKeyUp("s"))
        {

            ani.SetBool("IsClimbDown", false);

        }
        else if (RopeHit.collider && !IsAttach && Input.GetKeyDown(KeyCode.E))
        {
            Attach();

        }
        else if (IsAttach && Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
            IsAttach = false;
        }
        else if (IsGrounded)
        {
            parentname = null;
        }

    }
    public void Slide(int i)
    {

        if (i == 1)
        {
            // Debug.Log(hj.connectedBody.GetComponent<LinkNumber>().LinkNo);
            transform.position = RopeHit.collider.gameObject.GetComponentInParent<RopeScript>().links[hj.connectedBody.GetComponent<LinkNumber>().LinkNo - 1].transform.position;
            hj.connectedBody = RopeHit.collider.gameObject.GetComponentInParent<RopeScript>().links[hj.connectedBody.GetComponent<LinkNumber>().LinkNo - 1].GetComponent<Rigidbody2D>();
            ani.SetBool("IsClimbUp", true);
            ani.SetBool("IsClimbDown", false);
        }
        if (i == -1)
        {
            //Debug.Log(hj.connectedBody.GetComponent<LinkNumber>().LinkNo);
            transform.position = RopeHit.collider.gameObject.GetComponentInParent<RopeScript>().links[hj.connectedBody.GetComponent<LinkNumber>().LinkNo + 1].transform.position;
            hj.connectedBody = RopeHit.collider.gameObject.GetComponentInParent<RopeScript>().links[hj.connectedBody.GetComponent<LinkNumber>().LinkNo + 1].GetComponent<Rigidbody2D>();
            ani.SetBool("IsClimbUp", false);
            ani.SetBool("IsClimbDown", true);
        }


    }
    public void Attach()
    {
        hj.enabled = true;
        hj.connectedBody = RopeHit.collider.gameObject.GetComponent<Rigidbody2D>();
        parentname = RopeHit.collider.gameObject.gameObject.transform.parent;
        if (facingRight)
        {
            hj.connectedAnchor = new Vector2(-0.8f, -0.7f);
        }
        else
        {
            hj.connectedAnchor = new Vector2(0.89f, -0.7f);
        }
        ani.SetBool("IsClimbHold", true);
        IsAttach = true;
        rb.freezeRotation = false;

    }
    public void Detach()
    {
        hj.enabled = false;
        hj.connectedBody = null;
        IsAttach = false;
        ani.SetBool("IsClimbHold", false);
        rb.freezeRotation = true;
    }
    public void Dash()
    {
        if (AfterDash && DashCoolDown > 0)
        {
            DashCoolDown -= Time.deltaTime;
        }

        if (DashTime > 0 && DashCoolDown <= 0)
        {
            if (facingRight && Input.GetKeyDown(KeyCode.C) && IsGrounded && !IsDashing)
            {
                IsDashing = true;
                ani.SetBool("IsDashing", true);
                //rb.AddForce(Vector2.right *200f, ForceMode2D.Impulse);
                rb.velocity = Vector2.right * 50f;
                CreatDashEffect();
            }
            else if (!facingRight && Input.GetKeyDown(KeyCode.C) && IsGrounded && !IsDashing)
            {
                IsDashing = true;
                ani.SetBool("IsDashing", true);
                // rb.AddForce(Vector2.left*200f, ForceMode2D.Impulse);
                rb.velocity = Vector2.left * 50f;
                CreatDashEffect();
            }
            if (facingRight && Input.GetKeyDown(KeyCode.C) && MoveInput > 0 && IsGrounded && !IsDashing)
            {
                IsDashing = true;
                ani.SetBool("IsDashing", true);
                //rb.AddForce(Vector2.right *200f, ForceMode2D.Impulse);
                rb.velocity = Vector2.right * (50f + Speed) * Time.deltaTime;
                CreatDashEffect();
            }
            else if (!facingRight && Input.GetKeyDown(KeyCode.C) && MoveInput < 0 && IsGrounded && !IsDashing)
            {
                IsDashing = true;
                ani.SetBool("IsDashing", true);
                // rb.AddForce(Vector2.left*200f, ForceMode2D.Impulse);
                rb.velocity = Vector2.left * (50f + Speed) * Time.deltaTime;
                CreatDashEffect();
            }
            if (IsDashing)
            {
                DashTime -= Time.deltaTime;

                AfterDash = true;
            }


        }
        else if (DashTime <= 0)
        {
            // StartCoroutine("AfterDashStant");
            DashTime = StartDashTime;
            IsDashing = false;
            rb.velocity = Vector2.zero;
            ani.SetBool("IsDashing", false);
            DashCoolDown = StartDashCoolDown;
        }
        IEnumerator AfterDashStant()
        {
            yield return new WaitForSeconds(3f);
            rb.velocity = new Vector2(0, 0);
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rope"))
        {
            if (collision.gameObject.transform.parent == parentname)
            {
                collided = true;
            }
        }
        else
        {
            collided = false;
        }
        //Debug.Log(collision.gameObject.transform.parent + "+" + parentname);
    }
    /*private void OnCollisionExit2D(Collision2D collision)
    {
      
        if (collision.gameObject.transform.parent == null)
        {
            collided = false;
        }
        Debug.Log(collision.gameObject.transform.parent + "+" + parentname);
        
    }*/

    public void Recoil(bool fac)
    {

        if (fac)
        {
            rb.velocity = new Vector2(-10f, 0);
            // rb.AddForce(new Vector2(200, 0), ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = new Vector2(10f, 0);
            // rb.AddForce(new Vector2(200, 0), ForceMode2D.Impulse);
        }
    }
    public void WallJumping()
    {
        WallRayHit1 = Physics2D.Raycast(transform.position, Vector2.right * (transform.localScale.x * 0.5f), 0.5f, WhatIsGround);
        WallRayHit2 = Physics2D.Raycast(transform.position, Vector2.left * (transform.localScale.x * 0.5f), 4f, WhatIsGround);
        IsTouchingFront = WallRayHit1.collider;
        IsTouchingBack = WallRayHit2.collider;
        if (IsTouchingFront && !IsGrounded)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fullGravity-4f) * Time.deltaTime;
        }
     
        
    }
    public void Movement()
    {


        RayHit = Physics2D.BoxCast(bc.bounds.center, new Vector2(1f,2.3f), 0f, Vector2.down, 0.1f, WhatIsGround);
        MoveInput = Input.GetAxis("Horizontal");
        IsGrounded = RayHit.collider;
        if ((MoveInput < 0 || MoveInput > 0) && !IsAttach && IsGrounded && !IsDashing && WeaponCoolDown <0 && !IsAir)
        {
             rb.velocity = new Vector2(MoveInput * Speed * Time.fixedDeltaTime, rb.velocity.y * Time.fixedDeltaTime);
           // rb.AddForce(new Vector2(MoveInput * 400 * Time.deltaTime,0f),ForceMode2D.Impulse);
           // this.transform.Translate(MoveInput * Speed * Time.deltaTime, 0f,0f);
            ani.SetBool("Walking", true);
        }
        else if (MoveInput == 0 && !IsAttach && !IsDashing && IsGrounded && WeaponCoolDown < 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y * Time.deltaTime);
            ani.SetBool("Walking", false);
        }
        
       
        if (transform.localScale.x == -1)
        {
            facingRight = false;
        }
        if (transform.localScale.x == 1)
        {

            facingRight = true;
        }


        if (facingRight == false && MoveInput > 0 && !IsPushing  && !IsDashing && !IsAttach)
        {

            Flip();
        }
        else if (facingRight == true && MoveInput < 0 && !IsPushing  && !IsDashing && !IsAttach)
        {

            Flip();
        }


        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded && !IsTouchingFront) && !IsAir)
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            //rb.AddForce(Vector2.up * JumpForce * Time.deltaTime, ForceMode2D.Force);
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            IsAir = true;

        }
        if (Input.GetKeyDown(KeyCode.Space) && (IsTouchingFront))
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            //rb.AddForce(Vector2.up * JumpForce * Time.deltaTime, ForceMode2D.Force);
            JumpTimeCounter = JumpTime;
            rb.velocity = Vector2.up * 1200 * Time.fixedDeltaTime;
        }

         if ((rb.velocity.y > 0.2 && IsAir) && !IsTouchingFront || IsTouchingBack && IsAir)
        {
            if (JumpTimeCounter > 0)
            {
                //rb.velocity = Vector2.up * 1400 * Time.fixedDeltaTime;
                JumpTimeCounter -= Time.deltaTime;
                float jumpMultiplier = 3;
                float t = JumpTimeCounter / JumpTime;
                float currentJumpM = jumpMultiplier;

                if(t>0.5f)
                {
                    currentJumpM = jumpMultiplier * (1 - t);
                }
                rb.velocity = Vector2.up * 1400 * Time.deltaTime;
               
            }
            else
            {
                IsAir = false;
            }
        }

         if (Input.GetKeyUp(KeyCode.Space))
        {
            IsAir = false;
            JumpTimeCounter = 0;
            if (rb.velocity.y > 0.2)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f * Time.fixedDeltaTime);
            }

        }

        //if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        //    IsAir = true;
        //    Debug.Log("jump");
        //}


        //if(rb.velocity.y>0 && IsAir)
        //{
        //    JumpTimeCounter += Time.deltaTime;
        //    if (JumpTimeCounter > JumpTime) IsAir = false;
        //    float jumpMultiplier = 3;
        //    float t = JumpTimeCounter / JumpTime;
        //    float currentJumpM = jumpMultiplier;

        //    if(t>0.5f)
        //    {
        //        currentJumpM = jumpMultiplier * (1 - t);
        //    }
        //    rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y) * currentJumpM * Time.deltaTime;

        //}
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    IsAir = false;
        //    JumpTimeCounter = 0;
        //    if (rb.velocity.y > 0.2)
        //    {
        //        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f * Time.fixedDeltaTime);
        //    }
        //}
        Debug.Log(rb.velocity);
        /* if (Input.GetKey(KeyCode.Space) && !IsAttach)
         {
             rb.velocity = Vector2.up * 30;
             AirTime -= Time.deltaTime;
             //rb.AddForce(new Vector2(0f, 100f));
         }
         else if (Input.GetKeyUp(KeyCode.Space))
         {
             if (rb.velocity.y > 0.2)
             {
                 rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f * Time.deltaTime);
                 //rb.AddForce(new Vector2(0f, 1000f * 0.5f));
             }
         }*/
        ani.SetBool("IsGrounded", IsGrounded);
        if (IsGrounded && (MoveInput > 0 || MoveInput < 0))
        {
            AirTime = AirTimeStart;
            CreatDust();
        }

        if (rb.velocity.y < 0.2 && !IsAttach)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fullGravity + 2) * Time.deltaTime;
            ani.SetBool("Up", false);
        }
        if (rb.velocity.y > 0.2 && !IsAttach)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fullGravity) * Time.deltaTime;
            ani.SetBool("Up", true);
        }
        
        if (!IsAttach)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (rb.velocity.y < -Mathf.Abs(50))
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -Mathf.Abs(50), Mathf.Infinity));
        }
        /* else if(IsAttach && facingRight)
         {
            // Rotation = Mathf.Clamp(Rotation, 15, -40);
             transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, RightRotation));
         }
         else if (IsAttach && !facingRight)
         {

             transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, RightRotation));
         }*/
        

    }
    private void AirControl()
    {

       /* if (!IsGrounded && facingRight && MoveInput > 0 && !IsAttach)
        {
            rb.velocity = new Vector2(1 * 1000 * Time.deltaTime, rb.velocity.y);
        }
        if (!IsGrounded && !facingRight && MoveInput > 0 && !IsAttach)
        {
            rb.velocity = new Vector2(1 * 150 * Time.deltaTime, rb.velocity.y);
        }
        if (!IsGrounded && !facingRight && MoveInput < 0 && !IsAttach)
        {
                rb.velocity = new Vector2(-1 * 1000 * Time.deltaTime, rb.velocity.y);
        }
        if (!IsGrounded && facingRight && MoveInput < 0 && !IsAttach)
        {
            rb.velocity = new Vector2(-1 * 150 * Time.deltaTime, rb.velocity.y);
        }*/
       if(!IsGrounded && (MoveInput > 0 || MoveInput < 0) && !IsAttach)
        {
            rb.velocity = new Vector2(MoveInput * Speed * Time.fixedDeltaTime, rb.velocity.y);
        }
        if (IsAttach && MoveInput > 0 && Input.GetKeyDown(KeyCode.Space) && facingRight)
        {
            rb.velocity = new Vector2(MoveInput * Speed * Time.fixedDeltaTime, rb.velocity.y);
            //rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        
            Detach();
        }
        if (IsAttach && MoveInput > 0 && Input.GetKeyDown(KeyCode.Space) && !facingRight)
        {
            rb.velocity = new Vector2(MoveInput * Speed * Time.fixedDeltaTime, rb.velocity.y);
            //rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
           
            //Flip();
            Detach();
        }
        if (IsAttach && MoveInput < 0 && Input.GetKeyDown(KeyCode.Space) && !facingRight)
        {
            rb.velocity = new Vector2(MoveInput * Speed * Time.fixedDeltaTime, rb.velocity.y);
            //rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
           
            Detach();
        }
        if (IsAttach && MoveInput < 0 && Input.GetKeyDown(KeyCode.Space) && facingRight)
        {
            rb.velocity = new Vector2(MoveInput * Speed * Time.fixedDeltaTime, rb.velocity.y);
            //rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
           
            //Flip();
            Detach();
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * (transform.localScale.x * 0.5f));
    }
    public void Wepon()
    {

        if (WeaponCoolDown < 0 && Input.GetMouseButton(1) && IsGrounded && !IsPushing && !Input.GetKey(KeyCode.Space))
        {
            AfterShoot = false;
            wp.enabled = true;
            rb.velocity = new Vector2(0, rb.velocity.y);

        }
        else if (Input.GetMouseButtonUp(1))
        {
            wp.enabled = false;
        }


        if (WeaponCoolDown > 0)
        {
            WeaponCoolDown -= Time.deltaTime;
        }
        else if (!AfterShoot)
        {
            WeaponCoolDown = WeaponCoolDownStart;
        }

    }
    public void PushPull()
    {
        BoxHit = Physics2D.Raycast(transform.position, Vector2.right * (transform.localScale.x * 0.5f), 0.5f, WhatIsBox);
        if (BoxHit.collider && !IsPushing && Input.GetKeyDown(KeyCode.E) && IsGrounded)
        {

            IsPushing = true;
            box = BoxHit.collider.gameObject;
            box.GetComponent<FixedJoint2D>().enabled = true;
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            ani.SetBool("IsPushHold", true);
        }
        else if (BoxHit.collider && IsPushing && Input.GetKeyDown(KeyCode.E) && MoveInput == 0)
        {
            IsPushing = false;
            box.GetComponent<FixedJoint2D>().enabled = false;
            ani.SetBool("IsPushHold", false);
        }
        if (facingRight && MoveInput > 0 && IsPushing)
        {
            ani.SetBool("IsPushing", true);
            ani.SetBool("isPulling", false);

        }
        else if (facingRight && MoveInput < 0 && IsPushing)
        {

            ani.SetBool("IsPulling", true);
            ani.SetBool("IsPushing", false);


        }
        else if (facingRight && MoveInput == 0 && IsPushing)
        {
            ani.SetBool("IsPushing", false);
            ani.SetBool("IsPulling", false);

        }
        else if (!facingRight && MoveInput < 0 && IsPushing)
        {
            ani.SetBool("IsPushing", true);
            ani.SetBool("IsPulling", false);

        }
        else if (!facingRight && MoveInput > 0 && IsPushing)
        {
            
            ani.SetBool("IsPushing", false);
            ani.SetBool("IsPulling", true);

        }
        else if (!facingRight && MoveInput == 0 && IsPushing)
        {
            ani.SetBool("IsPushing", false);
            ani.SetBool("IsPulling", false);

        }


    }

    /*  void ChangeAnimationState(string newstate)
      {
          if (newstate == currentstate) return;
          ani.Play(newstate);
          currentstate = newstate;
      }
     void FlipBody(float MoveInput)
      {

         if(!sp.flipX && MoveInput < 0)
          {
              sp.flipX = true;

          }
         else if(sp.flipX && MoveInput > 0)
          {
              sp.flipX = false;
          }

      }
     */
    public void Flip()
    {
        facingRight = !facingRight;
        transform.Find("ShootingPoint").localScale = transform.Find("ShootingPoint").localScale * new Vector2(-1, 1);
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        CreatDust();
    }
    void CreatDust()
    {
        DustJump.Play();
    }
    void CreatDashEffect()
    {
        DashEffect.Play();
    }

}
