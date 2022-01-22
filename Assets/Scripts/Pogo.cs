using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pogo : MonoBehaviour {
    [SerializeField] private Transform rayTransformation;
    [SerializeField] private Transform springHeadTransformation;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float springLength;
    
    [Header("Tweak Parameters")]
    [SerializeField] private float maxPower = 5;
    [SerializeField] private float minPower = 1;
    [SerializeField] private float maxTorque = 30;
    [SerializeField] private float bounciness = .8f;
    [SerializeField] private float jumpWindow = 0.3f;
    
    private RaycastHit2D[] _results = new RaycastHit2D[1];
    private float _rayOffset = 0f;
    private Rigidbody2D _body;
    private float _lastJumpInput;
    private bool _allowBounce;

    private void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _rayOffset = (rayTransformation.position - springHeadTransformation.position).magnitude;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            _lastJumpInput = Time.time;
        }
    }

    private void FixedUpdate() {
        BounceDetection();
        LookAtMouse();
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
                var timeElapsed = Time.time - _lastJumpInput;
                var timeElapsedNormalized = timeElapsed / jumpWindow;
                var power = Mathf.Lerp(minPower, maxPower, 1 - timeElapsedNormalized);
                Bounce(hit.normal, power);
            }
        }
    }
    
    private bool TryGetGroundDistance(out RaycastHit2D hit) {
        var hitCount = Physics2D.RaycastNonAlloc(
            rayTransformation.position,
            rayTransformation.up,
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
    
    private void Bounce(Vector2 normal, float power) {
        var reflection = Vector2.Reflect(_body.velocity, normal);
        reflection *= bounciness;

        Vector2 pogoForce = transform.up * power;

        _body.velocity = pogoForce + reflection;
        _allowBounce = false;
    }

    private void LookAtMouse() {
        var plane = new Plane(Vector3.back, Vector3.zero);
        var ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float distance);
        var pointedPos = ray.GetPoint(distance);
        var down = (pointedPos - transform.position).normalized;
        var targetRot = Quaternion.LookRotation(transform.forward, -down);
        LookAt(targetRot.eulerAngles.z);
    }

    private void LookAt(float targetAngle) {
        var angle = JustifyAngle(_body.rotation);
        var deltaAngle = targetAngle - angle;
        if (deltaAngle > 180f) {
            deltaAngle -= 360f;
        }
        else if (deltaAngle < -180) {
            deltaAngle += 360f;
        }
        var targetAngleVel = deltaAngle / Time.fixedDeltaTime;
        var deltaAngleVel = targetAngleVel - _body.angularVelocity;
        var torque = deltaAngleVel * _body.inertia;
        torque = Mathf.Clamp(torque, -maxTorque, +maxTorque);
        _body.AddTorque(torque, ForceMode2D.Force);
    }
    
    private float JustifyAngle(float angle) {
        return (angle % 360 + 360) % 360;
    }
}