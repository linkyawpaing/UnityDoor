using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    public GameObject hook;
    public GameObject linkprefab;
    public GameObject[] links;
    public int linkcount;
    public Transform player;
    public PlayerController pc;
    public bool collided;
    void Awake()
    {
        GenerateLink();
        pc = player.GetComponent<PlayerController>();
        collided = false;
    }
    public void GenerateLink()
    {
        links = new GameObject[linkcount+1];

        GameObject previous = hook;
        links[0] = previous;
        for (int i = 0; i < linkcount; i++)
        {

            GameObject link = Instantiate(linkprefab, transform);
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = new Vector2(0, 4.2f);
            if (previous == hook)
            {
                joint.connectedAnchor = new Vector2(0, 0);
            }
            else
            {
                joint.connectedAnchor = new Vector2(0, (link.GetComponent<SpriteRenderer>().bounds.size.y) * -1);
            }
            joint.connectedBody = previous.GetComponent<Rigidbody2D>();
            link.GetComponent<LinkNumber>().LinkNo = i + 1;
            links[i + 1] = link;
            previous = link;
        }
    }
    public void Update()
    {
        
        if ((pc.IsAttach || pc.collided) && pc.parentname != null)
        {
            for(int i = 1; i < linkcount; i++)
            {
                links[i].GetComponent<PlatformEffector2D>().enabled = true;
                pc.parentname.GetComponent<RopeScript>().links[i].GetComponent<CapsuleCollider2D>().usedByEffector = true;
                if (pc.facingRight)
                {
                    pc.parentname.GetComponent<RopeScript>().links[i].GetComponent<PlatformEffector2D>().rotationalOffset = 270;
                }
                else
                {
                    pc.parentname.GetComponent<RopeScript>().links[i].GetComponent<PlatformEffector2D>().rotationalOffset = 90;
                }
            }
        }
        else
        {
            for (int i = 1; i < linkcount; i++)
            {
                links[i].GetComponent<CapsuleCollider2D>().usedByEffector = false;
               // links[i].GetComponent<PlatformEffector2D>().enabled = false;
            }
        }
        
        //StartCoroutine(FreezeRotation());
    }
   /* IEnumerator FreezeRotation()
    {
        yield return new WaitForSeconds(10f);
       
            for (int i = 0; i < linkcount; i++)
            {
                links[i].GetComponent<Rigidbody2D>().freezeRotation = false;
            }
        
    }*/
    


}
