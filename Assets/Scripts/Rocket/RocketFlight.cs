using UnityEngine;
using System;

public class RocketFlight : MonoBehaviour
{
    public float flightDuration = 3f; // Time to fly away and return
    public float flightDistance = 20f; // How far the rocket flies

    private Vector3 launchOrigin;
    private Vector3 targetPosition;
    private float timer = 0f;
    private bool inFlight = false;
    private bool returning = false;
    private Action onReturnCallback;

    // Call this to launch the rocket
    public void Launch(Vector3 direction, Action onReturn)
    {
        launchOrigin = transform.position;
        targetPosition = launchOrigin + direction.normalized * flightDistance;
        timer = 0f;
        inFlight = true;
        returning = false;
        onReturnCallback = onReturn;
    }

    void Update()
    {
        if (!inFlight) return;

        timer += Time.deltaTime;
        float halfDuration = flightDuration / 2f;
        float t = timer / halfDuration;

        if (!returning)
        {
            transform.position = Vector3.Lerp(launchOrigin, targetPosition, t);
            if (t >= 1f)
            {
                returning = true;
                timer = 0f;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(targetPosition, launchOrigin, t);
            if (t >= 1f)
            {
                inFlight = false;
                onReturnCallback?.Invoke();
                onReturnCallback = null;
                // Reset to original position
                transform.position = launchOrigin;
            }
        }
    }
}