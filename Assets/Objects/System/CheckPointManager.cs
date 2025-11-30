using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField]private List<CheckPoint> checkpoints;
    public bool AreAllTasksComplete => checkpoints.All(cp => cp.flag == true);
    public int TotalTasks => checkpoints.Count;

    public int CompletedTasks => checkpoints.Count(cp => cp.flag == true);
    void Awake()
    {
        checkpoints = FindObjectsOfType<CheckPoint>().ToList();
        //Debug.Log($"Found {checkpoints.Count} checkpoints in this level.");
    }
}