using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantAngularVelocity : MonoBehaviour
{
    public float AngularVelocity;
    Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _rb.angularVelocity = AngularVelocity;
    }
}
