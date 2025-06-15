using UnityEngine;

using Unity.Cinemachine;

public class CameraShake2 : MonoBehaviour
{
    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField]private float shakeTimer;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeAmplitude = 2f;
    public float shakeFrequency = 2f;

    void Awake()
    {
        virtualCam = GetComponent<CinemachineCamera>();

        if (virtualCam != null)
        {
            noise = virtualCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        if (noise == null)
        {
            Debug.LogWarning("BasicMultiChannelPerlin component not found on this CinemachineVirtualCamera.");
        }
    }

    void Update()
    {
        if (noise == null) return;

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                StopShake();
            }
        }
    }

    /// <summary>
    /// Call this to trigger a camera shake.
    /// </summary>
    public void Shake()
    {
        if (noise == null) return;

        noise.AmplitudeGain = shakeAmplitude;
        noise.FrequencyGain = shakeFrequency;
        shakeTimer = shakeDuration;
    }
    public void ShakeFromScript(float amp, float frec, float dur)
    {
        if (noise == null) return;

        noise.AmplitudeGain = amp;
        noise.FrequencyGain = frec;
        shakeTimer = dur;
    }
    /// <summary>
    /// Stops the camera shake instantly.
    /// </summary>
    public void StopShake()
    {
        if (noise == null) return;

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
}
