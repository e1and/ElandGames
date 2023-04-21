using UnityEngine;

public class CampfireWarmTrigger : MonoBehaviour
{
    Campfire campfire;

    void Start()
    {
        campfire = GetComponentInParent<Campfire>();
    }

    private void OnTriggerEnter(Collider other)
    {
        campfire.TriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        campfire.TriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        campfire.TriggerExit(other);
    }
}
