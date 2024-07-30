using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] float minimumVisBar;
    [SerializeField] float lightRange;

    
    private delegate void LightRaycast();
    [SerializeField] private LightRaycast lightRaycast;
    private LightRaycast debugCast;


    private void InitializeVariables()
    {
        if (currentType == LightTypesSO.LightType.SpotLight)
        {
            lightRaycast = SpotLightRadiusCheck;
            lightRange = light.range;

        }
        else if (currentType == LightTypesSO.LightType.PointLight)
        {
            lightRaycast = PointLightRadiusCheck;
            lightRange = light.range;
        }
    }

    private void Start()
    {
       InitializeVariables();
    }



    private void Update()
    {
        RaycastHitChecker();
        //DebugRay();
    }



    private void DebugRay()
    {
        if (Time.frameCount % 25 == 0)
        {
            if(currentType == LightTypesSO.LightType.SpotLight)
            {
                var isHit = RotaryHeart.Lib.PhysicsExtension.Physics.SphereCast(transform.position, (light.spotAngle / 10f) * (1 + 0.3f), transform.forward, out RaycastHit hit, light.range, characters, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
            }
            else if(currentType == LightTypesSO.LightType.PointLight)
            {
                var isHit = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapSphere(transform.position, light.intensity, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
            }
        }
        
    }

    private void RaycastHitChecker()
    {
        if (Time.frameCount % 25 == 0)
        {
            lightRaycast();
        }
    }

    private void SpotLightRadiusCheck()
    {
        var radius = (light.spotAngle / 10f) * (1 + 0.3f);
        var offset = 2f;
        var ray = RotaryHeart.Lib.PhysicsExtension.Physics.SphereCastAll(transform.position, radius + offset, transform.forward, light.range + offset, characters, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
        for (int i = 0; i < ray.Length; i++)
        {
            HitCheck(ray[i].transform);
        }

    }

    private void PointLightRadiusCheck()
    {
        var ray = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapSphere(transform.position, light.range, characters, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None);
        for(int i = 0; i < ray.Length; i++)
        {
            HitCheck(ray[i].transform);
        }
    }


    private void HitCheck(Transform target)
    {
        var direction = target.position - transform.position;
        var canHit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position, direction + Vector3.up * 0.15f, out RaycastHit hit);
        if (hit.transform == target.transform)
        {
            if(hit.transform.TryGetComponent(out EntityVisibilityController vis))
            {
                var distance = Vector3.Distance(hit.transform.position, transform.position);
                if(distance < light.range)
                {
                    vis.OnLightSourceHit(transform, light.intensity, lightRange);
                }
                else
                {
                    vis.OnLightSourceLeave();
                }
                
            }
        }
    }
}
