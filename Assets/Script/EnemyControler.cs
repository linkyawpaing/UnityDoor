using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControler : MonoBehaviour
{
    public LayerMask WhatIsPlayer;
    public Transform AttackPosition;
    public float AttackRange;
    public int damage;
    RaycastHit2D RayHit;
    public Animator EnemyAni;
    public float AttackInterval;
    public float AttackTimeIntervalCount;

    public Transform[] PatrolPoints;
    public float speed;
    public int currentPoint;
    public float TotalWaitTime;
    float WaitTime;
    // Start is called before the first frame update
    void Start()
    {
        AttackTimeIntervalCount = AttackInterval;
        transform.position = PatrolPoints[0].position;
        transform.rotation = PatrolPoints[0].rotation;
        WaitTime = TotalWaitTime;
    }

    // Update is called once per frame
    void Update()
    {
        RayHit = Physics2D.Raycast(AttackPosition.position, Vector2.left * (transform.localScale.x * 1.5f), 2, WhatIsPlayer);
        Debug.DrawRay(AttackPosition.position, Vector2.left * (transform.localScale.x * 1.5f));
        if(RayHit.collider && AttackTimeIntervalCount < 0)
        {
            EnemyAni.SetBool("IsAttacking", true);
            Collider2D[] EnemyDamage = Physics2D.OverlapCircleAll(AttackPosition.position, AttackRange, WhatIsPlayer);
            for(int i = 0; i < EnemyDamage.Length; i++)
            {
                EnemyDamage[i].GetComponent<HealthSystem>().TakeDamage(damage);
            }
            AttackTimeIntervalCount = AttackInterval;
        }
        else
        {
            EnemyAni.SetBool("IsAttacking", false);
            AttackTimeIntervalCount -= Time.deltaTime;
        }
        transform.position = Vector2.MoveTowards(transform.position, PatrolPoints[currentPoint].position, speed * Time.deltaTime);
        
        if(transform.position == PatrolPoints[currentPoint].position)
        {
            if (WaitTime <= 0)
            {
                transform.localScale = transform.localScale * new Vector2(-1, 1);
                EnemyAni.SetBool("IsRunning", true);
                if (currentPoint + 1 < PatrolPoints.Length)
                {
                    currentPoint++;
                }
                else
                {
                    currentPoint = 0;
                    
                }
                WaitTime = TotalWaitTime;   
            }
            else
            {
                WaitTime -= Time.deltaTime;
                EnemyAni.SetBool("IsRunning", false);
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(AttackPosition.position, AttackRange);
    }
}
