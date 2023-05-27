using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform Player;
    public Rigidbody2D rb;
    public Transform Right;
    public GameObject Rock;
    public GameObject wep;
    public GameObject ShootRock;
    public Transform Stick;
    public float offset;
    public SpriteRenderer sp;
    public Animator ani;
    Vector3 tem;
    SpriteRenderer psp;
    public PlayerController pc;
    
    public float starttime = 1f;
    public float aimtime = 1f;
    private bool facingRight;
    private bool aftershoot;

    public float CoolDown;
    public float StartCoolDown;
    // Update is called once per frame
    void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
        transform.position = Right.position;
        ani = Player.GetComponent<Animator>();
        pc = Player.GetComponent<PlayerController>();
        
        CoolDown = pc.WeaponCoolDown;
        StartCoolDown = pc.WeaponCoolDownStart;
    }
   
    void Update()
    {
      //  Player.rotation = transform.rotation = Quaternion.Euler(0, 0, 0);
        psp = Player.GetComponent<SpriteRenderer>();
        var mousePos = Input.mousePosition;
        mousePos.z = 7;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);//Camera.main.ScreenToWorldPoint(Input.mousePosition);
       // worldPos.Normalize();
        //worldPos.z = 0;
        Vector2 dir = worldPos - transform.position;
        Vector2 plydir = (worldPos - Player.transform.position);
       // Debug.Log(plydir);
        if (Player.localScale.x == -1)
        {
            facingRight = false;
        }
        if (Player.localScale.x == 1)
        {
            facingRight = true;
        }
        // Debug.Log(Vector3.Angle(transform.right, dir));
        if (Vector3.Angle(Player.transform.up , plydir) > 20f && Vector3.Angle(-Player.transform.up,plydir) > 60f)
        {
            
            if (Vector3.Angle(Player.transform.right, plydir) > 90 && facingRight)
            {
               
                transform.position = Right.position;
                /* if (pc.facingRight)
                     transform.localScale = transform.localScale * new Vector2(-1, 1);
                 else
                 {

                 }*/
                // sp.flipX = true;
                pc.Flip();
            }
            else if (Vector3.Angle(Player.transform.right, plydir) < 90 && (!facingRight))
            {
     
                transform.position = Right.position;
               /* if(pc.facingRight)
                transform.localScale = transform.localScale * new Vector2(-1, 1);
                else
                {
                    transform.localScale = transform.localScale * new Vector2(-1, 1);
                }*/
                //  sp.flipX = false;
                pc.Flip();
             //  transform.localScale = transform.localScale * new Vector2(-1, 1);
            }
          
                transform.right = plydir;
        }
        if (Input.GetMouseButton(1))
        {
            pc.enabled = false;
          
            ani.SetBool("BeforeShoot",true);
            ani.SetBool("AfterShoot", false);
            transform.GetComponent<SpriteRenderer>().enabled = true;
           /* if (Input.GetKeyDown(KeyCode.W) == true)
            {
              //  ani.SetBool("IsShooting", false);
                ani.SetBool("BeforeShoot", false);
                pc.enabled = true;
                this.GetComponent<Weapon>().enabled = false;
            }*/
            
            aimtime -= Time.deltaTime;
            PlayerController.AfterShoot = false;
        }
        if (Input.GetMouseButtonUp(1) && aimtime <= 0f )
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            ani.SetBool("AfterShoot", true);
            ani.SetBool("BeforeShoot", false);
           ShootRock =  Instantiate(Rock, Stick.position, Stick.rotation);
            ShootRock.GetComponent<Rigidbody2D>().AddForce(Stick.right * 200);
            GetComponent<Weapon>().enabled = false;
            pc.enabled = true;
            aimtime = starttime;
            PlayerController.AfterShoot = true;
            pc.Recoil(facingRight);
        }
        
        if((aimtime > 0 && aimtime <= starttime )&& Input.GetMouseButtonUp(1))
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            ani.SetBool("BeforeShoot", false);
            //ani.SetBool("AfterShoot", false);
            GetComponent<Weapon>().enabled = false;
            aimtime = starttime;
            pc.enabled = true;
            PlayerController.AfterShoot = true;
           
        }
        if (rb.velocity.y < 0.2 )
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2f + 4) * Time.deltaTime;
            ani.SetBool("Up", false);
        }
        if (rb.velocity.y > 0.2  )
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2f) * Time.deltaTime;
            ani.SetBool("Up", true);
        }
        if(rb.velocity.y < -Mathf.Abs(50))
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -Mathf.Abs(50), Mathf.Infinity));
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        Player.transform.localScale = Player.transform.localScale * new Vector2(-1, 1);
        transform.localScale = transform.localScale * new Vector2(-1, 1);
    }
    
}
