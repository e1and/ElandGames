using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] SkinnedMeshRenderer mesh;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color col = mesh.material.color;
        if (Vector3.Distance(transform.position, player.transform.position) < 3)
        {
            //mesh.material = new Material(Shader.Find("Standart"));
            //mesh.material.shader = Shader.Find("Standart");
            col.a = 0.1f;
            mesh.material.color = col;
        }
        else
        {
            //mesh.material.shader = Shader.Find("StandartNoCulling");
            col.a = 1f;
            mesh.material.color = col;
        }
    }
}
