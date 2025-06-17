using UnityEngine;
using UnityEngine.Playables;

public class RitualCinematicTrigger : MonoBehaviour
{
    [Header("Referencias a los 4 rituales")]
    public CanvasRitual ritual1;
    public CanvasRitual ritual2;
    public CanvasRitual ritual3;
    public CanvasRitual ritual4;

    [Header("Timeline a reproducir")]
    public PlayableDirector director;

    private bool cinematicPlayed = false;

    void Update()
    {
        if (!cinematicPlayed && TodosLosRitualesCompletos())
        {
            director.Play();
            cinematicPlayed = true;
            Debug.Log("¡Todos los rituales completados! Cinemática iniciada.");
        }
    }

    bool TodosLosRitualesCompletos()
    {
        return ritual1.ritualComplete &&
               ritual2.ritualComplete &&
               ritual3.ritualComplete &&
               ritual4.ritualComplete;
    }
}
