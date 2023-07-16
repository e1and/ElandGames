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
    private Building building;

    private void Start()
    {
        building = Game.links.building;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!building.isConstructing)
        {
            ChangeColor(disableToBuildMaterial);
            building.isAbleToBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!building.isConstructing)
        {
            ChangeColor(ableToBuildMaterial);
            building.isAbleToBuild = true;
        }
    }

    void ChangeColor(Material material)
    {
        foreach (MeshRenderer mesh in parts)
        {
            mesh.material = material;
        }
    }
}
