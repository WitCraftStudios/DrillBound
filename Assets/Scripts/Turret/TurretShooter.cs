using UnityEngine;
using System.Collections;

public class TurretShooter : MonoBehaviour
{
    public ParticleSystem bulletParticle; // Assign in Inspector
    public float burstInterval = 1.5f;    // Time between bursts
    public float shotDelay = 0.1f;        // Delay between each bullet in a burst
    public int bulletsPerBurst = 3;

    private float timer = 0f;
    private bool isBursting = false;
    public GameObject interactionPrompt;
    public GameObject smokeParticle;
    public bool isBroken = false;

    void Start()
    {
        if (smokeParticle != null) smokeParticle.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (isBroken) return;
        timer += Time.deltaTime;
        if (timer >= burstInterval && !isBursting)
        {
            StartCoroutine(BurstFire());
            timer = 0f;
        }
    }
    
    IEnumerator BurstFire()
    {
        isBursting = true;
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            if (bulletParticle != null)
                bulletParticle.Play();
            yield return new WaitForSeconds(shotDelay);
        }
        isBursting = false;
    }

    public void BreakDown()
    {
        isBroken = true;
        if (smokeParticle != null) smokeParticle.SetActive(true);
        Debug.Log("TurretShooter damaged by asteroid!");
    }

    public void Repair()
    {
        isBroken = false;
        if (smokeParticle != null) smokeParticle.SetActive(false);
        Debug.Log("TurretShooter repaired!");
    }
}