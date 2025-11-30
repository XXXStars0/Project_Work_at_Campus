using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Added for easy access to all objects

[ExecuteAlways]
public class Grid3D : MonoBehaviour
{
    [Header("Grid Visualization Settings")]
    [Tooltip("The size of one grid cell.")]
    public float cellSize = 1f;
    [Tooltip("The dimensions of the visual guide drawn in the editor.")]
    public Vector3Int visualGridSize = new Vector3Int(20, 3, 20); 
    public bool showGizmos = true;

    private Dictionary<Vector3Int, GameObject> gridObjects = new Dictionary<Vector3Int, GameObject>();
    public IEnumerable<GameObject> AllGridObjects => gridObjects.Values;

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return new Vector3(cell.x * cellSize, cell.y * cellSize, cell.z * cellSize) + transform.position;
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return new Vector3Int(
            Mathf.RoundToInt(localPos.x / cellSize),
            Mathf.RoundToInt(localPos.y / cellSize),
            Mathf.RoundToInt(localPos.z / cellSize)
        );
    }

    public bool IsOccupied(Vector3Int cell)
    {
        return gridObjects.ContainsKey(cell);
    }

    public GameObject GetObjectAt(Vector3Int cell)
    {
        gridObjects.TryGetValue(cell, out GameObject obj);
        return obj;
    }

    public void RegisterObject(Vector3Int cell, GameObject obj)
    {
        if (gridObjects.ContainsKey(cell) && gridObjects[cell] != null)
        {
            if (Application.isPlaying) Destroy(gridObjects[cell]);
            else DestroyImmediate(gridObjects[cell]);
        }
        gridObjects[cell] = obj;
    }

    public void RemoveObject(Vector3Int cell)
    {
        if (gridObjects.TryGetValue(cell, out GameObject objToDestroy))
        {
            if (objToDestroy != null)
            {
                if (Application.isPlaying) Destroy(objToDestroy);
                else DestroyImmediate(objToDestroy);
            }
            gridObjects.Remove(cell);
        }
    }

    public void ClearGrid()
    {
        foreach (var obj in gridObjects.Values)
        {
            if (obj != null)
            {
                if (Application.isPlaying) Destroy(obj);
                else DestroyImmediate(obj);
            }
        }
        gridObjects.Clear();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.gray;
        Vector3 origin = transform.position - (Vector3.one * cellSize * 0.5f);

        for (int y = 0; y < visualGridSize.y; y++)
        {
            for (int z = 0; z < visualGridSize.z; z++)
            {
                for (int x = 0; x < visualGridSize.x; x++)
                {
                    Vector3 pos = CellToWorld(new Vector3Int(x, y, z));
                    Gizmos.DrawWireCube(pos, Vector3.one * cellSize);
                }
            }
        }
    }
#endif
}