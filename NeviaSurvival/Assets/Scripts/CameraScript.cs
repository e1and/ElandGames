using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] SkinnedMeshRenderer mesh;
    [SerializeField] Shader originalShader;
    [SerializeField] Texture originalTexture;
    [SerializeField] Shader transparentShader;
    [SerializeField] float distance;
    [SerializeField] float alpha;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color col = mesh.material.color;
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < 1.6f)
        {
            mesh.material = new Material(transparentShader);
            mesh.material.mainTexture = originalTexture;
            //mesh.material.shader = Shader.Find("Standart");
            col.a = (distance - 1.4f) * 1f;
            alpha = col.a;
            mesh.material.color = col;
        }
        else
        {
            //mesh.material.shader = Shader.Find("Fantasy Forest/StandartNoCulling");

            mesh.material = new Material(originalShader);
            mesh.material.mainTexture = originalTexture;
            col.a = 1f;
            mesh.material.color = col;
        }
    }
}
