using UnityEngine;

public class ActivateOnPlay : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    
    void Start()
    {
        meshRenderer.enabled = true;
    }
}
