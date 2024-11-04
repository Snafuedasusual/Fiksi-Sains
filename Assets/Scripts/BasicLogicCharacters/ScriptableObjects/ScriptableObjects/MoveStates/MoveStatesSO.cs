using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveStatesSO", menuName = "States/MoveStates")]
public class MoveStatesSO : ScriptableObject
{
    public enum MoveStates
    {
        Idle,
        Walk,
        Run,
        Jump
    }
}
