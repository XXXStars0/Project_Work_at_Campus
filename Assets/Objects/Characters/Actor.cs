using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorState
{
    Normal,
    Pushing,
    Carrying,
    Enemy,
    NPC
}

[RequireComponent(typeof(GridTracker))]
public class Actor : MonoBehaviour
{
    public ActorState currentState = ActorState.Normal;
}
