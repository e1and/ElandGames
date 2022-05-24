using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    public float raycastLength = 500;
    public GameObject Player;
    public int stickLimit;
    public GameObject Target;
    private Animator Animator;
    private Vector3 lookDirection;

    private void Start()
    {
        Animator = Player.GetComponent<Animator>();
    }

    void Update()
    {
        GameObject Target = GameObject.Find("Target");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, raycastLength))
        {
            Debug.Log(hit.collider.tag);

            if (hit.collider.tag == "Stick")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (Player.GetComponent<StickInvent>().stick < 3)
                    {
                        Player.GetComponent<StickInvent>().stick += 1;
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Debug.Log("New hit " + hit.collider.tag);
                    //Target.transform.rotation = new Quaternion(45, 100, 45, 0);
                    Target.GetComponent<MeshRenderer>().enabled = true;
                    Target.transform.position = hit.point;
                 }
            }

            Debug.DrawRay(ray.origin, ray.direction * raycastLength, Color.yellow);


        }
        if (Target.GetComponent<MeshRenderer>().enabled == true)
        {
            if (hit.transform != null)
            {
                Animator.SetFloat("Speed", 2);
                lookDirection = new Vector3(Target.transform.position.x, Player.transform.position.y, Target.transform.position.z);
                transform.LookAt(lookDirection);
                Player.transform.position = Vector3.MoveTowards(Player.transform.position, Target.transform.position, 2 * Player.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed * Time.deltaTime); 
                if (Vector3.Distance(Player.transform.position, Target.transform.position) < 1f || GetComponent<StarterAssets.StarterAssetsInputs>().v2m)
                { Target.GetComponent<MeshRenderer>().enabled = false;
                    Animator.SetFloat("Speed", 0);
                }
            }
        }
    }



}
