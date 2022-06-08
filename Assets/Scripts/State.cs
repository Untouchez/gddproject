using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public StateManager myStateManager;
    public Transform me;

    public abstract void MyUpdate();

    public abstract void MyStart();
}
