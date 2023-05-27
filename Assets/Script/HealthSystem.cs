using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public  int Health;
    public  int NumOfHearth;

    public Image[] Hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;
    public Animator PlayerAni;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Health > NumOfHearth)
        {
            Health = NumOfHearth;
        }
        for(int i = 0; i < Hearts.Length; i++)
        {
            if(i < Health)
            {
                Hearts[i].sprite = FullHeart;
            }
            else
            {
                Hearts[i].sprite = EmptyHeart;
            }
            if(i < NumOfHearth)
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }
        }
        if(Health == 0)
        {
            PlayerAni.SetBool("IsHeartRemain", false);
        }
        else
        {
            PlayerAni.SetBool("IsHeartRemain", true);
        }
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            NumOfHearth -= 1;
            Debug.Log("Hit with Enemy");
        }
        
    }
    public void TakeDamage(int Damage)
    {
        NumOfHearth -= Damage;
        Debug.Log("Taking Damage");
    }
    /* if(collision.collider.tag == "Enemy")
     {
         NumOfHearth -= 1;
         Debug.Log("Hit with Enemy");
     }*/

}
