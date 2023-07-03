using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BlueprintLogic : MonoBehaviour
{
    public MeshRenderer[] parts;
    public Material ableToBuildMaterial;
    public Material disableToBuildMaterial;
    public Building building;

    private void OnTriggerEnter(Collider other)
    {
        ChangeColor(disableToBuildMaterial);
        building.isAbleToBuild = false;
    }

    private void OnTriggerExit(Collider other)
    {
        ChangeColor(ableToBuildMaterial);
        building.isAbleToBuild = true;
    }

    void ChangeColor(Material material)
    {
        foreach (MeshRenderer mesh in parts)
        {
            mesh.material = material;
        }
    }
}
