using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkNumber : MonoBehaviour
{
    public int LinkNo;
    public int ParentNo;
    // Start is called before the first frame update
    void Start()
    {
        ParentNo = transform.gameObject.GetComponentInParent<RopeScript>().linkcount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
