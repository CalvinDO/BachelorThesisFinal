using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWLimb : MonoBehaviour {

    [HideInInspector]
    public CWLimb parent;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public List<CWLimb> childLimbs;
    [HideInInspector]
    public Transform endCapCenter;
}