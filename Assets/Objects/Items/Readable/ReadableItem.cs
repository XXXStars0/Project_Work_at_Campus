using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider))]
public class ReadableItem : MonoBehaviour,IInteractable
{
    [Header("Identification")]
    public string ID;
    public string title;
    [TextArea(5, 15)]
    public string text;

    [Header("Grid Settings")]
    public bool snapToGrid = true;
    public float gridSize = 1f;
    public bool snapRotation = true;
    public bool showGizmo = true;
    public Color gizmoColor = Color.yellow;


    private Rigidbody rb;
    private Collider col;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private Collider[] colliders;


    void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        colliders = GetComponentsInChildren<Collider>();
    }


        void Start()
    {
        
    }

    void Update()
    {
        if (!Application.isPlaying && snapToGrid && transform.position != lastPosition)
            SnapPosition();
    }
    private void SnapPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
        pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
        transform.position = pos;
        lastPosition = pos;

        if (snapRotation)
        {
            Vector3 rot = transform.eulerAngles;
            rot.y = Mathf.Round(rot.y / 90f) * 90f;
            transform.eulerAngles = rot;
            lastRotation = transform.rotation;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = gizmoColor;
        Collider c = GetComponent<Collider>();
        if (c != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            var bounds = c.bounds;
            Vector3 localCenter = transform.InverseTransformPoint(bounds.center);
            Vector3 localSize = transform.InverseTransformVector(bounds.size);
            Gizmos.DrawWireCube(localCenter, localSize);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }

        /*if (!string.IsNullOrEmpty(ID))
        {
            Handles.Label(transform.position + Vector3.up * 0.5f, $"ID: {ID}");
        }*/
    }
#endif
}
