using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLight : MonoBehaviour
{
    
    void OnMouseEnter()
    {

        if (tag == "Enemy")
        {
            GetComponent<Renderer>().material.color = Color.yellow;
            GetComponent<Outline>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
            GetComponent<Outline>().enabled = true;
        }
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;
        GetComponent<Outline>().enabled = false;
    }
}
