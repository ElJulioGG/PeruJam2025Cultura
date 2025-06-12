using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public float attackTime = 0.05f;
    public float sustainTime = 2.6f;
    public float decayTime = 0.3f;

    public void Shake(float intensity = 1.0f)
    {
        if (impulseSource != null)
        {
            var def = impulseSource.ImpulseDefinition;
            def.TimeEnvelope.AttackTime = attackTime;
            def.TimeEnvelope.SustainTime = sustainTime;
            def.TimeEnvelope.DecayTime = decayTime;

            impulseSource.GenerateImpulse(intensity);
        }
    }
}
