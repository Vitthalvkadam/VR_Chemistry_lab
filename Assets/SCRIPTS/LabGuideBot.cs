using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class LabGuideBot : MonoBehaviour
{
    public enum BotState { Idle, MovingToEntrance, AtEntrance }

    [System.Serializable]
    public class ExperimentData
    {
        public string experimentName = "Experiment 1";
        [TextArea(3, 5)] public string equipmentDetails = "• Beaker 1 (100ml)\n• Beaker 2 (100ml)\n• Beaker 3 (200ml)\n• Stir Rod\n• pH Meter";
        public Transform experimentAnchor; // Teleport position for this specific experiment
    }

    [Header("State Tracking")]
    [SerializeField] private BotState currentState = BotState.Idle;

    [Header("XR Rig Reference")]
    public Transform xrOrigin; // Assign your XR Origin transform here

    [Header("Movement & Navigation")]
    public NavMeshAgent agent;
    public float fallbackMoveSpeed = 2.5f;
    public float fallbackRotationSpeed = 6.0f;
    public float stoppingDistance = 0.2f;

    [Header("Waypoints")]
    public Transform labEntranceWaypoint;

    [Header("UI Canvas Panels")]
    public GameObject welcomePanel;
    public GameObject chooseExperimentPanel;
    public GameObject experimentInfoPanel;

    [Header("Dynamic Info Text")]
    public TextMeshProUGUI equipmentInfoText;

    [Header("Experiment Configuration")]
    [Tooltip("Configure data & teleport anchors for Experiments 1 through 8.")]
    public List<ExperimentData> experimentList = new List<ExperimentData>();

    [Header("Visual & Audio Effects")]
    public ParticleSystem tireSmokeLeft;
    public ParticleSystem tireSmokeRight;
    public AudioSource engineAudio;

    private Coroutine activeMovementCoroutine;
    private ExperimentData currentSelectedExperiment;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (welcomePanel != null) welcomePanel.SetActive(true);
        if (chooseExperimentPanel != null) chooseExperimentPanel.SetActive(false);
        if (experimentInfoPanel != null) experimentInfoPanel.SetActive(false);
    }

    // --- UI EVENT LISTENERS ---

    // 1. Attached to 'gotoexperimentselctionpanelbutton'
    public void OnGoToSelectionPressed()
    {
        if (welcomePanel != null) welcomePanel.SetActive(false);
        if (chooseExperimentPanel != null) chooseExperimentPanel.SetActive(true);
    }

    // 2. Attached to Experiment Buttons (Pass index 1 for Exp 1, 2 for Exp 2, etc.)
    public void OnExperimentSelected(int experimentNumber)
    {
        int arrayIndex = experimentNumber - 1;

        if (arrayIndex >= 0 && arrayIndex < experimentList.Count)
        {
            currentSelectedExperiment = experimentList[arrayIndex];

            // Switch Panels
            if (chooseExperimentPanel != null) chooseExperimentPanel.SetActive(false);
            if (experimentInfoPanel != null) experimentInfoPanel.SetActive(true);

            // Update Equipment Text
            if (equipmentInfoText != null)
            {
                equipmentInfoText.text = $"<b>Selected:</b> {currentSelectedExperiment.experimentName}\n\n<b>Equipment Needed:</b>\n{currentSelectedExperiment.equipmentDetails}";
            }

            // Auto-move bot to lab entrance
            MoveToDestination(labEntranceWaypoint, BotState.MovingToEntrance, OnReachedEntrance);
        }
        else
        {
            Debug.LogError($"[LabGuideBot] Experiment number {experimentNumber} is out of bounds!");
        }
    }

    // 3. Attached to 'enter lab button'
    public void OnEnterLabPressed()
    {
        if (currentSelectedExperiment == null)
        {
            Debug.LogError("[LabGuideBot] Cannot enter lab: No experiment selected!");
            return;
        }

        if (currentSelectedExperiment.experimentAnchor == null)
        {
            Debug.LogError($"[LabGuideBot] Teleport anchor missing for {currentSelectedExperiment.experimentName}!");
            return;
        }

        // Teleport the XR Origin to the selected experiment anchor
        if (xrOrigin != null)
        {
            xrOrigin.SetPositionAndRotation(
                currentSelectedExperiment.experimentAnchor.position, 
                currentSelectedExperiment.experimentAnchor.rotation
            );

            Debug.Log($"[LabGuideBot] Teleported user to {currentSelectedExperiment.experimentName}");
        }
        else
        {
            Debug.LogError("[LabGuideBot] XR Origin reference is missing!");
        }

        // Hide UI after entering
        if (experimentInfoPanel != null) experimentInfoPanel.SetActive(false);
    }

    // --- NAVIGATION LOGIC ---

    private void MoveToDestination(Transform target, BotState moveState, Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError($"[LabGuideBot] Waypoint missing for state: {moveState}");
            return;
        }

        currentState = moveState;
        SetSmokeEmitting(true);

        if (activeMovementCoroutine != null) StopCoroutine(activeMovementCoroutine);

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            activeMovementCoroutine = StartCoroutine(MoveWithNavMesh(target, onComplete));
        }
        else
        {
            activeMovementCoroutine = StartCoroutine(MoveWithTransform(target, onComplete));
        }
    }

    private IEnumerator MoveWithNavMesh(Transform target, Action onComplete)
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        while (agent.pathPending || agent.remainingDistance > stoppingDistance)
        {
            yield return null;
        }

        agent.isStopped = true;
        CompleteMovement(onComplete);
    }

    private IEnumerator MoveWithTransform(Transform target, Action onComplete)
    {
        while (Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, fallbackRotationSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, fallbackMoveSpeed * Time.deltaTime);
            yield return null;
        }

        CompleteMovement(onComplete);
    }

    private void CompleteMovement(Action onComplete)
    {
        SetSmokeEmitting(false);
        onComplete?.Invoke();
    }

    private void OnReachedEntrance()
    {
        currentState = BotState.AtEntrance;
    }

    private void SetSmokeEmitting(bool emit)
    {
        if (tireSmokeLeft != null)
        {
            var eLeft = tireSmokeLeft.emission;
            eLeft.enabled = emit;
        }

        if (tireSmokeRight != null)
        {
            var eRight = tireSmokeRight.emission;
            eRight.enabled = emit;
        }

        if (engineAudio != null)
        {
            if (emit && !engineAudio.isPlaying) engineAudio.Play();
            else if (!emit && engineAudio.isPlaying) engineAudio.Stop();
        }
    }
}