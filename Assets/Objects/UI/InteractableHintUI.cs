using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableHintUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI titleText;
    public Image imageBackground;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 0, 0);
    public float smoothSpeed = 8f;

    [Header("Others")]
    public CanvasGroup canvasGroup;

    private Transform targetItem;
    private Camera mainCamera;
    private RectTransform rectTransform;


    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        Hide();
    }

    void LateUpdate()
    {
        if (targetItem != null && mainCamera != null)
        {
            Vector3 worldPos = targetItem.position + offset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                if (smoothSpeed > 0 && Application.isPlaying)
                {
                    Vector3 currentPos = transform.position;
                    Vector3 targetPos = screenPos;
                    transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * smoothSpeed);
                }
                else
                {
                    transform.position = screenPos;
                }

                canvasGroup.alpha = 1f;
            }
            else
            {
                canvasGroup.alpha = 0f;
            }
        }
    }

    public void AttachToItem(Transform itemTransform, string title, string message)
    {
        targetItem = itemTransform;
        messageText.text = message;
        titleText.text = title;
    }

    public void Hide()
    {
        targetItem = null;
        canvasGroup.alpha = 0f;
    }
}
