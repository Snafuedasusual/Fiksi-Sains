using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationHashSO : ScriptableObject 
{
    public readonly List<int> animationHash = new List<int>();
    public int numtest = 0;

    public virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {
        
    }
}
