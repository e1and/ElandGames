using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildData", menuName = "Build")]
public class BuildData : ScriptableObject
{
    public string Name;
    public string id;
    public GameObject Prefab;
}
