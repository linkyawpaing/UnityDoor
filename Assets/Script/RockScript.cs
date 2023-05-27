using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    public Transform parent;
    public Transform stick;

    // Update is called once per frame
    private void Awake()
    {
        //stick = (parent.Find("ShootingPoint")).Find("ShootPoint");
        stick =( parent.Find("ShootingPoint")).Find("ShootPoint");
        
    }
    public void Shoot(GameObject ShootRock,Transform stick)
    {
        ShootRock.GetComponent<Rigidbody2D>().AddForce(stick.forward * 200 * Time.deltaTime, ForceMode2D.Impulse);
    }
   
   
}
