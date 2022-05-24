using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float seeDistance = 5f;
    public float attackDistance = 2f;
    public float speed = 3f;
    private Transform target;
    private Animator Animator;
    private Vector3 targetXZ;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        Animator = GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < seeDistance)
        {

            if (Vector3.Distance(transform.position, target.transform.position) > attackDistance)
            {
                targetXZ = new Vector3(target.transform.position.x, 0, target.transform.position.z);

                transform.LookAt(targetXZ);              
                
                transform.position = Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);
                Animator.SetInteger("Move", 1);
               

            }
            else
            {
                Animator.SetInteger("Move", 0);
            }
        }

    }
}