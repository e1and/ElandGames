using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swimming : MonoBehaviour
{
    private Player player;
    private Animator animator;
    public float waterY;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        animator = GetComponentInParent<Animator>();
    }

    public void SwimmingStateSwitcher(bool swim)
    {
        player.isSwim = swim;
        if (swim) player.links.personController.isFallDamage = false;
        animator.SetBool("isSwimming", swim);
    }


}
