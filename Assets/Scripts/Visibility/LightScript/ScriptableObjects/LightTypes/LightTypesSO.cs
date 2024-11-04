using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LightTypesSO", menuName = "Lights/LightTypes")]
public class LightTypesSO : ScriptableObject
{
    public enum LightType
    {
        None,
        SpotLight,
        PointLight
    }

    public LightType currentType;
}
