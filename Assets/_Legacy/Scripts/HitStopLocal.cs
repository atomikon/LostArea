using UnityEngine;

/// <summary>
/// Local-only hitstop helper (no networking).
/// Use: Play(0.06f, 0.0f) for freeze; Play(0.08f, 0.2f) for slow-mo.
/// </summary>
public class HitStopLocal : MonoBehaviour
{
    public bool affectTimeScale = true;

    private float _timer;
    private float _restoreTimeScale = 1f;

    public void Play(float duration, float timeScale)
    {
        duration = Mathf.Max(0f, duration);
        timeScale = Mathf.Clamp(timeScale, 0f, 1f);

        // If hitstop starts fresh, capture the current scale ONCE.
        // Otherwise repeated calls during slowmo would "restore" back to slowmo forever.
        if (_timer <= 0f && affectTimeScale)
            _restoreTimeScale = Time.timeScale;

        _timer = Mathf.Max(_timer, duration);

        if (affectTimeScale)
            Time.timeScale = timeScale;
    }

    private void Update()
    {
        if (_timer <= 0f) return;
        _timer -= Time.unscaledDeltaTime;
        if (_timer <= 0f && affectTimeScale)
            Time.timeScale = _restoreTimeScale;
    }

    private void OnDisable()
    {
        // Safety: never leave the game slowed if this component gets disabled mid-hitstop.
        if (_timer > 0f && affectTimeScale)
            Time.timeScale = _restoreTimeScale;
        _timer = 0f;
    }

    private void OnDestroy()
    {
        if (_timer > 0f && affectTimeScale)
            Time.timeScale = _restoreTimeScale;
    }
}
