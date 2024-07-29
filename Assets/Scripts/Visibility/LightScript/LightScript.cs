using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    [SerializeField] private Light light;


    private void Update()
    {
        var isHit = RotaryHeart.Lib.PhysicsExtension.Physics.SphereCast(transform.position, (light.spotAngle / 10f) * 0.8f, transform.forward, out RaycastHit hit, light.range, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
    }

}
