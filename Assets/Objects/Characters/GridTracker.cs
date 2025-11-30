using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTracker : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;

    [Header("Tracking Configuration")]
    public Transform trackingTarget; 

    public Vector3 trackingDirection = Vector3.down; 

    [Header("Highlight Indicator")]
    public GameObject highlightIndicator;

    [Header("Detection Settings")]
    public LayerMask blockLayer;

    [field: SerializeField] public Vector3Int CurrentCell { get; private set; }
    [field: SerializeField] public Block CurrentBlock { get; private set; }

    void Start()
    {
        if (trackingTarget == null)
        {
            trackingTarget = this.transform;
        }

        if (highlightIndicator != null)
        {
            highlightIndicator.SetActive(false);
            highlightIndicator.transform.rotation = Quaternion.identity;
        }
    }

    void Update()
    {
        TrackPosition();
    }

    void TrackPosition()
    {
        Vector3 targetPosition = trackingTarget.position;
        Vector3 nextCellCenter = targetPosition + trackingDirection.normalized * gridSize;

        Vector3Int newCell = new Vector3Int(
            Mathf.RoundToInt(nextCellCenter.x / gridSize),
            Mathf.RoundToInt((transform.position.y - (gridSize * 0.5f)) / gridSize),
            Mathf.RoundToInt(nextCellCenter.z / gridSize)
        );

        if (newCell != CurrentCell)
        {
            CurrentCell = newCell;

            RaycastHit hit;
            Vector3 cellCenter = new Vector3(CurrentCell.x, CurrentCell.y, CurrentCell.z) * gridSize;
            Vector3 rayStart = cellCenter - trackingDirection.normalized * gridSize;

            if (Physics.Raycast(rayStart, trackingDirection, out hit, gridSize * 2, blockLayer))
            {
                CurrentBlock = hit.collider.GetComponent<Block>();
                if (highlightIndicator != null)
                {
                    Vector3 _newHit = new Vector3(hit.point.x, (float)(hit.point.y - 0.5 * gridSize), hit.point.z);
                    highlightIndicator.transform.position = _newHit;
                    highlightIndicator.SetActive(true);
                }
            }
            else
            {
                CurrentBlock = null;
                if (highlightIndicator != null)
                {
                    highlightIndicator.SetActive(false);
                }
            }
        }
    }
}