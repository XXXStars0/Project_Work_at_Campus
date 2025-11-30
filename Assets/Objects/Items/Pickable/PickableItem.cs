using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemSize
{
    Light, 
    Medium, 
    Heavy,
    Tool 
}

[ExecuteAlways]
[RequireComponent(typeof(Collider))]
public class PickableItem : MonoBehaviour, IInteractable
{
    [Header("Identification")]
    public string itemID;
    public ItemSize itemSize = ItemSize.Light;

    [Header("Grid Settings")]
    public bool snapToGrid = true;
    public float gridSize = 1f;
    public bool snapRotation = true;
    public bool showGizmo = true;
    public Color gizmoColor = Color.yellow;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private Renderer[] renderers;
    private List<Material> materials = new List<Material>();
    private List<Color> originalBaseColors = new List<Color>();
    private List<Color> originalLineColors = new List<Color>();

    private Rigidbody rb;
    private Collider col;

    private Collider[] colliders;


    [HideInInspector] public PlayerItemManager holder;
    [HideInInspector] public bool isHeld = false;

    void Awake()
    {
        CacheMaterials();

        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        colliders = GetComponentsInChildren<Collider>();
    }

    void OnValidate()
    {
        if (snapToGrid)
            SnapPosition();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying && snapToGrid && transform.position != lastPosition)
            SnapPosition();
    }
#endif

    private void CacheMaterials()
    {
        renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        materials.Clear();
        originalBaseColors.Clear();
        originalLineColors.Clear();

        foreach (var r in renderers)
        {
            if (r == null || r.sharedMaterial == null)
                continue;

            Material mat = Application.isPlaying ? r.material : r.sharedMaterial;
            materials.Add(mat);

            if (mat.HasProperty("_BaseColor"))
                originalBaseColors.Add(mat.GetColor("_BaseColor"));
            else
                originalBaseColors.Add(Color.white);

            if (mat.HasProperty("_LineColor"))
                originalLineColors.Add(mat.GetColor("_LineColor"));
            else
                originalLineColors.Add(Color.black);
        }
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

    public virtual void OnPickUp(PlayerItemManager picker)
    {
        holder = picker;
        if (col) col.enabled = false;

        SetCollidersActive(false);

        isHeld = true;

        //Debug.Log($"{name} picked up by {picker.name}");
    }

    public virtual void OnDrop()
    {
        holder = null;
        if (col) col.enabled = true;

        transform.SetParent(null);

        SetCollidersActive(true);

        isHeld = false;

        //Debug.Log($"{name} dropped");
    }
    public virtual void OnInteract(PlayerItemManager picker)
    {
       // Debug.Log($"{picker.name} interacted with {name}");
    }

    private void SetCollidersActive(bool value)
    {
        foreach (var c in colliders)
            c.enabled = value;
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

        if (!string.IsNullOrEmpty(itemID))
        {
            Handles.Label(transform.position + Vector3.up * 0.5f, $"ID: {itemID}");
        }
    }
#endif
}
