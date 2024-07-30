using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    [Header("ScriptableObjects")]
    [SerializeField] LightTypesSO lightTypesSO;

    [Header("Components")]
    [SerializeField] private Light light;

    [Header("Variables")]
    [SerializeField] LayerMask characters;
    [SerializeField] RaycastHit[] lightCollides = new RaycastHit[10];
    [SerializeField] LightTypesSO.LightType currentType;


    private void Update()
    {
        Debug();
    }


    private void Debug()
    {
        if (Time.frameCount % 25 == 0)
        {
            if(currentType == LightTypesSO.LightType.SpotLight)
            {
                var isHit = RotaryHeart.Lib.PhysicsExtension.Physics.SphereCast(transform.position, (light.spotAngle / 10f), transform.forward, out RaycastHit hit, light.range, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
            }
            else if(currentType == LightTypesSO.LightType.PointLight)
            {
                var isHit = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapSphere(transform.position, light.intensity, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
            }
        }
        
    }

    private void LightRadiusCheck()
    {
        if(Time.frameCount % 25 == 0)
        {
            lightCollides = RotaryHeart.Lib.PhysicsExtension.Physics.SphereCastAll(transform.position, (light.spotAngle / 10f), transform.forward, light.range);
            for (int i = 0; i < lightCollides.Length; i++)
            {

            }
        } 
    }

    private void HitCheck(Transform target)
    {
        var direction = target.position - transform.position;
        var canHit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position, direction + Vector3.up * 0.15f, out RaycastHit hit);
        if (canHit)
        {

        }
    }


}
