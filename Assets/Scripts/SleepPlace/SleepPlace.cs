using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SleepPlace : MonoBehaviour, ISleepPlace
{
    public float sleepRestoreAmount = 100f;
    public float eatRestoreAmount = 50f;
    public bool isBroken = false;
    public float repairTime = 10f;
    public Image repairCircle; // Assign in Inspector
    private bool playerNearby = false;
    private bool isRepairing = false;
    private float repairTimer = 0f;
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
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
        Debug.Log("SleepPlace repaired!");
    }

    public void Sleep(PlayerEnergy playerEnergy)
    {
        if (isBroken) {
            Debug.Log("SleepPlace is broken! Repair it first.");
            return;
        }
        playerEnergy.RestoreEnergy(sleepRestoreAmount);
        Debug.Log("Player slept and restored energy!");
    }

    public void Eat(PlayerEnergy playerEnergy)
    {
        if (isBroken) {
            Debug.Log("SleepPlace is broken! Repair it first.");
            return;
        }
        playerEnergy.RestoreEnergy(eatRestoreAmount);
        Debug.Log("Player ate and restored some energy!");
    }

    public void BreakDown()
    {
        isBroken = true;
        Debug.Log("SleepPlace damaged by asteroid!");
        // Add additional damage logic here
    }

    public void Repair()
    {
        if (!isBroken || isRepairing) return;
        StartCoroutine(RepairRoutine());
    }
}
