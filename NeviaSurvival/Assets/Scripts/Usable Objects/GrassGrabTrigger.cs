using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGrabTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.GrassGrab(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.isLay || player.isSit || player.mousePoint.isCarry) player.GrassGrab(false);
            else player.GrassGrab(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.GrassGrab(false);
        }
    }
}
