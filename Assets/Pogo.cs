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
    public float maxPower = 5;
    public float minPower = 1;
    public float jumpWindow = 0.1f;
    public LayerMask groundMask;
    
    private RaycastHit2D[] _results = new RaycastHit2D[1];
    private float _rayOffset = 0f;
    private Rigidbody2D _body;
    private float _lastJumpInput;
    private bool _allowBounce;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _rayOffset = (rayTransformation.position - minSpringTransformation.position).magnitude;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            _lastJumpInput = Time.time;
        }
    }

    private void FixedUpdate() {
        BounceDetection();
    }

    private void BounceDetection() {
        if (TryGetGroundDistance(out var hit)) {
            var distanceToGround = hit.distance - _rayOffset;
            if (distanceToGround > springLength) {
                _allowBounce = true;
                return;
            }

            if (!_allowBounce) {
                return;
            }

            if (distanceToGround <= 0) {
                Bounce(hit.normal, minPower);
            }
            else if(Time.time - _lastJumpInput <= jumpWindow) {
                var springCenter = springLength / 2;
                var distanceToCenter = Mathf.Abs(distanceToGround - springCenter);
                var normDistanceToCenter = distanceToCenter / springCenter;
                var power = Mathf.Lerp(minPower, maxPower, 1 - normDistanceToCenter);
                Bounce(hit.normal, power);
            }
        }
    }

    private void Bounce(Vector2 normal, float power) {
        Debug.Log("bounced");
        var reflection = Vector2.Reflect(_body.velocity, normal);
        reflection *= bounciness;

        Vector2 pogoForce = transform.up * power;

        _body.velocity = pogoForce + reflection;
        _allowBounce = false;
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
