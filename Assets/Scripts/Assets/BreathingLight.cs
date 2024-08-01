using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingLight : MonoBehaviour
{
    public float minIntensity = 0.5f; // Minimum intensity of the light
    public float maxIntensity = 2.0f; // Maximum intensity of the light
    public float speed = 1.0f;        // Speed of the breathing effect

    private Light pointLight;
    private float phase = 0.0f;

    void Start()
    {
        // Get the Light component attached to this GameObject
        pointLight = GetComponent<Light>();

        if (pointLight == null)
        {
            Debug.LogError("No Light component found on this GameObject.");
            enabled = false; // Disable the script if no Light component is found
        }
    }

    void Update()
    {
        // Calculate the new intensity using a sine wave
        phase += Time.deltaTime * speed;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(phase) + 1) / 2);

        // Apply the new intensity to the light
        pointLight.intensity = intensity;
    }
}
