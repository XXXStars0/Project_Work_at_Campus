using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpHintUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI IDText;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public float smoothSpeed = 8f;

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

    public void AttachToItem(Transform itemTransform, string message, string key, string ID)
    {
        targetItem = itemTransform;
        messageText.text = message;


        bool keyVisible = !string.IsNullOrEmpty(key);
        if (keyText != null)
        {
            //keyText.text = keyVisible ? $"[{key}]" : "";
            //keyText.gameObject.SetActive(keyVisible);
            messageText.text = (keyVisible ? $"[{key}] " : "") + message;
        }

        bool IDVisible = !string.IsNullOrEmpty(ID);
        if (IDText != null)
        {
            //IDText.text = IDVisible ? ID : "";
            //IDText.gameObject.SetActive(IDVisible);
            messageText.text += IDVisible ? " "+ID : "";
        }

    }
    public void Hide()
    {
        targetItem = null;
        canvasGroup.alpha = 0f;
    }
}