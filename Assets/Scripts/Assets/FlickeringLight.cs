using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public float minIntensity = 0.5f; // Minimum intensity of the light
    public float maxIntensity = 2.0f; // Maximum intensity of the light
    public float flickerSpeed = 0.1f; // How often the light flickers (in seconds)

    private Light pointLight;
    private float nextFlickerTime = 0f;

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
        // Check if it's time to flicker
        if (Time.time >= nextFlickerTime)
        {
            // Set the next flicker time
            nextFlickerTime = Time.time + flickerSpeed;

            // Randomize the light intensity within the specified range
            pointLight.intensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}
