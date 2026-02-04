
using System;
using UnityEngine;

public class ActivityFlag : MonoBehaviour
{
    public State idleState;
    public State busyState;

    public GameObject root;

    [Serializable]
    public class State
    {
        public GameObject root;
    }
}