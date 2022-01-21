using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pogo : MonoBehaviour {
    public Transform rayTransformation;
    public Transform minSpringTransformation;
    public float springLength;
    public float bounciness = .8f;
    public LayerMask groundMask;
    
    private RaycastHit2D[] _results = new RaycastHit2D[1];
    private float _rayOffset = 0f;
    private Rigidbody2D _body;
    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _rayOffset = (rayTransformation.position - minSpringTransformation.position).magnitude;
    }
    private void FixedUpdate() {
        if (TryGetGroundDistance(out var hit)) {
            var distanceToGround = hit.distance;
            if (distanceToGround < _rayOffset + springLength) {
                Bounce(hit.normal, 1f);
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (distanceToGround < _rayOffset + springLength) {
                    Bounce(hit.normal, 0f);
                }
            }
        }
        
    }
    private void Bounce(Vector2 normal, float power) {
        Debug.Log("bounced");
        var reflection = Vector2.Reflect(_body.velocity, normal);
        reflection *= bounciness;

        Vector2 pogoForce = transform.up * power;

        _body.velocity = pogoForce + reflection;
    }
    private bool TryGetGroundDistance(out RaycastHit2D hit) {
        var hitCount = Physics2D.RaycastNonAlloc(
            rayTransformation.position,
            rayTransformation.forward,
            _results,
            100f,
            groundMask.value);
        if (hitCount >= 1) {
            hit = _results[0];
            return true;
        }
        hit = default;
        return false;
    }
}
