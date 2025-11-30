using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BlockType
{
    Floor,
    Wall,
    Door,
    Decoration,
    Stair,
    Slope,
    Other
}

[ExecuteAlways]
[RequireComponent(typeof(Collider))]
public class Block : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;
    public bool snapToGrid = true;
    public bool snapRotation = true;

    [Header("Block Info")]
    public BlockType blockType = BlockType.Floor;
    public Color gizmoColor = Color.green;

    [Header("Highlight")]
    public Color defaultColor = Color.white;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    // We don't need to track scale changes explicitly, OnValidate handles it.

    private Renderer[] renderers;
    private List<Material> materials = new List<Material>();
    private List<Color> originalBaseColors = new List<Color>();
    private List<Color> originalLineColors = new List<Color>();

    void Awake()
    {
        CacheMaterials();
    }

    void OnValidate()
    {
        if (snapToGrid)
        {
            SnapPosition();
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (snapToGrid && transform.position != lastPosition)
            {
                SnapPosition();
            }
        }
#endif
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
   // public void SetHighlight(bool active, Color color, bool topOnly = true) { }
    private void CacheMaterials()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        materials.Clear();
        originalBaseColors.Clear();
        originalLineColors.Clear();
        foreach (var r in renderers)
        {
            if (r == null || r.sharedMaterial == null) continue;
            Material mat = Application.isPlaying ? r.material : r.sharedMaterial;
            materials.Add(mat);
            if (mat.HasProperty("_BaseColor")) originalBaseColors.Add(mat.GetColor("_BaseColor"));
            else originalBaseColors.Add(Color.white);
            if (mat.HasProperty("_LineColor")) originalLineColors.Add(mat.GetColor("_LineColor"));
            else originalLineColors.Add(Color.black);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
    }
#endif
}