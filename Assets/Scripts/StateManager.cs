using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State currentState;


    private void Start()
    {
        if(currentState)
            currentState.MyStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
            currentState.MyUpdate();
    }

    public void ChangeState(State newState)
    {
        if (newState == null)
            return;
        newState.MyStart();
        currentState = newState;
    }
}
