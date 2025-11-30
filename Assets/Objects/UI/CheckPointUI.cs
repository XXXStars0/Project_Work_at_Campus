using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; 

public class CheckPointUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI requirementText; 
    public GameObject successIcon;         
    public CanvasGroup canvasGroup;        

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 1.0f, 0);

    private Transform targetCheckPoint;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (successIcon != null) successIcon.SetActive(false);
        //Hide();
    }

    void LateUpdate()
    {
        if (targetCheckPoint != null && mainCamera != null)
        {
            Vector3 worldPos = targetCheckPoint.position + offset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                transform.position = screenPos;
                canvasGroup.alpha = 1f;
            }
            else
            {
                canvasGroup.alpha = 0f;
            }
        }
    }

    public void SetState(Transform cpTransform, string targetId, bool isComplete)
    {
        targetCheckPoint = cpTransform;

        if (isComplete)
        {
            if (requirementText != null) requirementText.gameObject.SetActive(false);
            if (successIcon != null) successIcon.SetActive(true);
        }
        else
        {
            if (requirementText != null)
            {
                requirementText.gameObject.SetActive(true);

                requirementText.text = $"Require {targetId}";
            }
            if (successIcon != null) successIcon.SetActive(false);
        }

    }

    public void SetTarget(Transform target)
    {
        targetCheckPoint = target;
    }

    public void Hide()
    {
        targetCheckPoint = null;
        canvasGroup.alpha = 0f;
    }
}