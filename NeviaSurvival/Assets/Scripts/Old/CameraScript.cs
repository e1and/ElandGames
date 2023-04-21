using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] SkinnedMeshRenderer mesh;
    [SerializeField] SkinnedMeshRenderer[] meshs;
    [SerializeField] Material[] originalShader;
    [SerializeField] Texture[] originalTexture;
    [SerializeField] Material transparentShader;
    [SerializeField] float distance;
    [SerializeField] float alpha;
    Color col;

    private void Awake()
    {
        meshs = player.GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    void Start()
    {
        
        for (int i = 1; i < meshs.Length; i++)
        {
            originalShader[i] = meshs[i].material;
            originalTexture[i] = meshs[i].material.mainTexture;
        }
    }


        // Update is called once per frame
        void Update()
        {
            distance = Vector3.Distance(transform.position, player.transform.position);

            for (int i = 1; i < meshs.Length; i++)
            {
                col = meshs[i].material.color;
                if (distance < 1.6f)
                {
                    meshs[i].material = new Material(transparentShader);
                    meshs[i].material.mainTexture = originalTexture[i];
                    //mesh.material.shader = Shader.Find("Standart");
                    col.a = (distance - 1.4f) * 1f;
                    alpha = col.a;
                    meshs[i].material.color = col;
                }
                else
                {
                    //mesh.material.shader = Shader.Find("Fantasy Forest/StandartNoCulling");

                    meshs[i].material = new Material(originalShader[i]);
                    meshs[i].material.mainTexture = originalTexture[i];
                    col.a = 1f;
                    meshs[i].material.color = col;
                }
            }
        }
    }
