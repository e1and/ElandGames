using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gradients
{

    [ExecuteInEditMode]

    public class Daytime : MonoBehaviour
    {
        [SerializeField] Gradient directionalLightGradient;
        [SerializeField] Gradient ambientLightGradient;

        [SerializeField, Range(1, 3600)] float timeDayInSeconds = 60;
        [SerializeField, Range(0f, 1f)] float timeProgress;

        [SerializeField] Light dirLight;
        Vector3 defaultAngles;

        void Start()
        { defaultAngles = dirLight.transform.localEulerAngles; }

        void Update()
        {
            if (Application.isPlaying)
                timeProgress += Time.deltaTime / timeDayInSeconds;

            if (timeProgress > 1f)
                timeProgress = 0f;

            dirLight.color = directionalLightGradient.Evaluate(timeProgress);
            RenderSettings.ambientLight = ambientLightGradient.Evaluate(timeProgress);

            dirLight.transform.localEulerAngles = new Vector3(x: 360f * timeProgress - 90, y: defaultAngles.x, defaultAngles.z);

        }
    }
}




