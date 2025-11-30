using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetUI : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0, 1.5f, 0);

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target != null && mainCamera != null)
        {
            Vector3 worldPos = target.position + offset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                transform.position = screenPos;
            }
        }
    }
}