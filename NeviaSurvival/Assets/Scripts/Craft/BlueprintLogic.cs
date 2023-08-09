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
    private BuildingHandler _buildingHandler;

    private void Start()
    {
        _buildingHandler = Game.links.buildingHandler;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_buildingHandler.isConstructing)
        {
            ChangeColor(disableToBuildMaterial);
            _buildingHandler.isAbleToBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_buildingHandler.isConstructing)
        {
            ChangeColor(ableToBuildMaterial);
            _buildingHandler.isAbleToBuild = true;
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
