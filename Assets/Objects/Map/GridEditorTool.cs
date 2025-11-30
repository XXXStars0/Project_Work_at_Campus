using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GridEditorTool : MonoBehaviour
{
    [Header("Prefabs")]

    [Header("Current Type")]
    public BlockType currentType = BlockType.Floor;

    private Grid3D grid;


    void OnEnable()
    {
        grid = FindObjectOfType<Grid3D>();
        if (grid == null)
        {
            Debug.LogWarning("No Grid3D found in scene. Please add one to your Map object.");
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying && grid != null)
        {
            HandleMouseInput();
        }
    }

    void HandleMouseInput()
    {
        if (Event.current == null) return;
        if (Event.current.type != EventType.MouseDown) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3Int cell = grid.WorldToCell(hit.point);

            if (Event.current.button == 0) 
            {
                if (!grid.IsOccupied(cell))
                {
                    GameObject prefab = GetPrefabByType(currentType);
                    if (prefab != null)
                    {
                        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        obj.transform.position = grid.CellToWorld(cell);
                        obj.transform.SetParent(grid.transform);
                        grid.RegisterObject(cell, obj);
                    }
                }
            }
            else if (Event.current.button == 1)
            {
                if (grid.IsOccupied(cell))
                {
                    grid.RemoveObject(cell);
                }
            }

            Event.current.Use(); 
        }
    }

    GameObject GetPrefabByType(BlockType type)
    {
        switch (type)
        {
            default: return null;
        }
    }
#endif
}
