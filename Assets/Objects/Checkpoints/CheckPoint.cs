using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class CheckPoint : MonoBehaviour
{
    [Header("Target Settings")]
    public string targetItemID;
    public bool flag = false;

    [Header("UI Settings")]
    public CheckPointUI checkPointUIPrefab;
    public bool alwaysShowUI = true;

    private CheckPointUI myCheckPointUIInstance;

    [Header("Grid Settings")]
    public float gridSize = 1f;
    public bool snapRotation = true;
    public Color gizmoColor = Color.cyan;

    [Header("Detection Settings")]
    public LayerMask detectionMask;
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public float checkInterval = 0.1f;

    [Header("Detection Settings")]

    private float checkTimer = 0f;
    private bool lastFlag = false; 

    void Start()
    {

        if (!Application.isPlaying)
        {
            return;
        }
        if (checkPointUIPrefab != null)
        {
            Transform canvasParent = null;
            GameObject canvasObj = GameObject.Find("UI_Bubble");

            if (canvasObj != null)
            {
                canvasParent = canvasObj.transform;
            }

            myCheckPointUIInstance = Instantiate(checkPointUIPrefab, canvasParent);

            if (myCheckPointUIInstance != null)
            {
                myCheckPointUIInstance.SetTarget(transform);
            }
        }

        UpdateUIState(flag);
    }

    void OnDestroy()
    {
        if (myCheckPointUIInstance != null)
        {
            if (Application.isPlaying)
            {
                Destroy(myCheckPointUIInstance.gameObject);
            }
            else
            {
                DestroyImmediate(myCheckPointUIInstance.gameObject);
            }
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SnapPosition();
            return; 
        }
#endif

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            DetectItems();
        }
    }

    void OnValidate()
    {
        if (gridSize > 0)
        {
            SnapPosition();
        }
    }

   
    void DetectItems()
    {
        if (!Application.isPlaying) return; 

        Collider[] hits = Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, detectionMask);
        bool hasTarget = false;

        foreach (var hit in hits)
        {
            PickableItem item = hit.GetComponentInParent<PickableItem>();
            //Debug.Log(hit);
            if (item == null || item.isHeld)
                continue;

            if (item.itemID == targetItemID)
            {
                hasTarget = true;
                break;
            }
        }

        if (flag != hasTarget)
        {
            flag = hasTarget;
            UpdateUIState(flag);

            //Debug.Log($"{name}: {targetItemID} {(flag ? "entered" : "left")} detection zone!");
        }
    }

    private void UpdateUIState(bool isComplete)
    {
        if (myCheckPointUIInstance == null) return;

        if (isComplete)
        {
            myCheckPointUIInstance.SetState(transform, targetItemID, true);
        }
        else
        {
            if (alwaysShowUI)
            {
                myCheckPointUIInstance.SetState(transform, targetItemID, false);
            }
            else
            {
                myCheckPointUIInstance.Hide();
            }
        }
    }

    private void SnapPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
        pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
        transform.position = pos;

        if (snapRotation)
        {
            Vector3 rot = transform.eulerAngles;
            rot.y = Mathf.Round(rot.y / 90f) * 90f;
            transform.eulerAngles = rot;
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = flag ? Color.green : gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
#endif
}