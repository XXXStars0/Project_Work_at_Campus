using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Block))]
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public string requiredItemID;
    public ReadableItem hint;

    private bool isLocked = true;
    private Block _b;

    void Awake()
    {
        _b = GetComponent<Block>();

        if (_b.blockType != BlockType.Door)
        {
            _b.blockType = BlockType.Door;
        }

        hint = GetComponent<ReadableItem>();

        if (hint)
        {
            hint.title = "Door";
            hint.text = $"Key Require: {requiredItemID}";
        }
    }

    public void TryUnlock(PickableItem keyUsed)
    {
        if (!isLocked)
        {
            //Debug.Log("Door is already unlocked.");
            return;
        }

        if (keyUsed != null && keyUsed.itemID == requiredItemID)
        {
            isLocked = false;
            //Debug.Log($"Door '{name}' unlocked with '{keyUsed.itemID}'!");
            if (hint)
            {
                hint.text = $"Successfully unlocked with {keyUsed.itemID}!";
            }
            this.gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("Wrong key or no key used!");
        }
    }
}