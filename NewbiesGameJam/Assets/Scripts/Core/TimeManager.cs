using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private float _slowdownFactor = 0.05f;
    [SerializeField] private float _slowdownLength = 2f;

    public float SlowdownFactor => _slowdownFactor;
    public float SlowdownLength => _slowdownLength;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null && Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        ResetTimeScale();
    }

    public void ResetTimeScale()
    {
        Time.timeScale += (1f / _slowdownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }

    public void DoSlowmotion()
    {
        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}