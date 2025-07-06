using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RocketCapsule : MonoBehaviour
{
    public bool isBroken = false;
    public float repairTime = 10f;
    public Image repairCircle; // Assign in Inspector
    private bool playerNearby = false;
    private bool isRepairing = false;
    private float repairTimer = 0f;
    public GameObject interactionPrompt;
    public GameObject smokeParticle;

    void Update()
    {
        if (isBroken && playerNearby && !isRepairing && UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartCoroutine(RepairRoutine());
        }
        if (isRepairing && repairCircle != null)
        {
            repairCircle.fillAmount = repairTimer / repairTime;
        }
        if (isBroken)
        {
            smokeParticle.SetActive(true);
        }
        else if (!isBroken)
        {
            smokeParticle.SetActive(false);
        }
        // No cooldown or currency logic here
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            interactionPrompt.SetActive(false);
        }
    }

    private IEnumerator RepairRoutine()
    {
        isRepairing = true;
        repairTimer = 0f;
        if (repairCircle != null)
        {
            repairCircle.gameObject.SetActive(true);
            repairCircle.fillAmount = 0f;
        }
        while (repairTimer < repairTime)
        {
            repairTimer += Time.deltaTime;
            if (repairCircle != null)
                repairCircle.fillAmount = repairTimer / repairTime;
            yield return null;
        }
        isBroken = false;
        isRepairing = false;
        if (repairCircle != null)
            repairCircle.gameObject.SetActive(false);
        Debug.Log("RocketCapsule repaired!");
    }

    public void BreakDown()
    {
        isBroken = true;
        Debug.Log("RocketCapsule damaged by asteroid!");
        // Add additional damage logic here
    }

    public void Repair()
    {
        if (!isBroken || isRepairing) return;
        StartCoroutine(RepairRoutine());
    }
}
