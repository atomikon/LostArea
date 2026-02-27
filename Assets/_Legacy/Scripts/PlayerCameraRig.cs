using UnityEngine;

public class PlayerCameraRig : MonoBehaviour
{
    public Transform yawPivot;
    public Transform pitchPivot;
    public Camera cam;

    [Header("Look")]
    public float sensitivity = 2.0f;
    public float pitchMin = -80f;
    public float pitchMax = 80f;

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    private bool _active = true;

    private void Awake()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();
        if (yawPivot == null) yawPivot = transform;
        if (pitchPivot == null && cam != null) pitchPivot = cam.transform;
        Yaw = yawPivot.eulerAngles.y;
        Pitch = pitchPivot.localEulerAngles.x;
    }

    public void SetActive(bool active)
    {
        _active = active;
        if (cam != null) cam.enabled = active;
        var al = GetComponent<AudioListener>();
        if (al != null) al.enabled = active;
    }

    private void Update()
    {
        if (!_active) return;

        float mx = Input.GetAxis("Mouse X") * sensitivity;
        float my = Input.GetAxis("Mouse Y") * sensitivity;

        Yaw += mx;
        Pitch -= my;

        Pitch = ClampAngle(Pitch, pitchMin, pitchMax);

        if (yawPivot != null)
            yawPivot.rotation = Quaternion.Euler(0f, Yaw, 0f);

        if (pitchPivot != null)
            pitchPivot.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        if (min <= max)
            return Mathf.Clamp(angle, min, max);

        // wrap-around case (rare)
        if (angle > max && angle < min)
            return (Mathf.Abs(angle - min) < Mathf.Abs(angle - max)) ? min : max;

        return angle;
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
