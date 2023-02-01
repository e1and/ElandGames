using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
    public class CameraDistance : MonoBehaviour
    {
        Cinemachine3rdPersonFollow cinemachine;

        void Start()
        {
            cinemachine = FindObjectOfType<Cinemachine3rdPersonFollow>();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            { cinemachine.CameraDistance++; }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            { cinemachine.CameraDistance--; }
        }
    }
}
