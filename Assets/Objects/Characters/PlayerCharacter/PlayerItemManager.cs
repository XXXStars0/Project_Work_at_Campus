using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public enum HandType
{
    Left,
    Right,
    Both
}

public class PlayerItemManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public Actor actor;

    [Header("Hint UI")]
    public PickUpHintUI pickUpHintUI;
    public InteractableHintUI interactableHintUI;
    public ItemHUD_UI itemHUD;

    [Header("Hand Transforms")]
    public Transform leftHandPoint;
    public Transform rightHandPoint;
    public Transform bothHandPoint;

    [Header("Held Items")]
    public PickableItem leftHandItem;
    public PickableItem rightHandItem;
    public PickableItem doubleHandItem;

    [Header("Settings")]
    public float pickDistance = 1.2f;
    public float placeCheckRadius = 0.3f;
    public LayerMask placeBlockLayer;
    public float itemHoldScale = 0.5f;
    public float gridSize = 1f;


    private void Start()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (actor == null)
            actor = GetComponent<Actor>();

        if(pickUpHintUI != null)
        {
            itemHUD.setHandType(UI_HandType.Default);
        }

        HidePickUpHint();

        string leftKey = playerController.keyLeftItem.ToString().ToUpper();
        string rightKey = playerController.keyRightItem.ToString().ToUpper();
        string bothKeys = leftKey + "/" + rightKey;

        itemHUD.resetPanelText(HandType.Left,leftKey);
        itemHUD.resetPanelText(HandType.Right,rightKey);
        itemHUD.resetPanelText(HandType.Both, bothKeys);
    }

    private void Update()
    {
        HandleItemInput();

        PickableItem detectedItem = GetDetectedItem();
        if (detectedItem != null && !detectedItem.isHeld)
        {
            ShowPickUpHint(detectedItem);
        }
        else
        {
            HidePickUpHint();
        }

        ReadableItem readableItem = GetReadableItem();
        //Debug.Log(readableItem);
        if (readableItem != null)
        {
            ShowReadHintUI(readableItem);
        }
        else
        {
            HideReadHintUI();
        }

        HandleItemUseInput();

    }

    void HandleItemInput()
    {
        PickableItem target = GetDetectedItem();
        bool hasTarget = target != null;

        bool pressLeft = Input.GetKeyDown(playerController.keyLeftItem);
        bool pressRight = Input.GetKeyDown(playerController.keyRightItem);

        if (doubleHandItem != null)
        {
            if (pressLeft || pressRight)
            {
                TryPlaceOrDrop(doubleHandItem);
                return;
            }
        }

        if (pressLeft)
            HandleHandInput(HandType.Left, hasTarget, target);

        if (pressRight)
            HandleHandInput(HandType.Right, hasTarget, target);

        if (Input.GetKeyDown(playerController.keyInteraction) && target != null)
            TryInteract(target);
    }


    void HandleHandInput(HandType hand, bool hasTarget, PickableItem target)
    {
        PickableItem currentItem = GetItemInHand(hand);

        if (currentItem != null)
        {
            TryPlaceOrDrop(currentItem);
            return;
        }

        if (hasTarget)
            TryPick(target, hand);
    }


    PickableItem GetItemInHand(HandType hand)
    {
        switch (hand)
        {
            case HandType.Left: return leftHandItem;
            case HandType.Right: return rightHandItem;
            case HandType.Both: return doubleHandItem;
            default: return null;
        }
    }

    PickableItem GetDetectedItem()
    {
        if (playerController == null) return null;

        return playerController.detectItem;
    }

    ReadableItem GetReadableItem()
    {
        if (playerController == null) return null;

        return playerController.detectedReadable;
    }

    Block GetFrontBlock()
    {
        if (playerController == null) return null;

        return playerController.detectedBlock;
    }

    void TryPick(PickableItem item, HandType hand)
    {
        if (item == null) return;

        switch (item.itemSize)
        {
            case ItemSize.Heavy:
                // TO DO: Cart Interface
                ShowHint("Cannot pick up (Too Heavy)");
                return;

            case ItemSize.Medium:
                // Medium items both hands free
                if (AnyHandOccupied())
                {
                    ShowHint("Need both hands free");
                    return;
                }
                EquipItem(item, HandType.Both);
                return;

            case ItemSize.Tool:
            case ItemSize.Light:
                // Light items one hand, but not if holding a double-hand item
                if (doubleHandItem != null)
                {
                    ShowHint("Hands occupied (Holding both-hand item)");
                    return;
                }

                if (hand == HandType.Left && leftHandItem == null)
                    EquipItem(item, HandType.Left);
                else if (hand == HandType.Right && rightHandItem == null)
                    EquipItem(item, HandType.Right);
                else
                    ShowHint("Hand not free");
                return;
        }
    }

    bool AnyHandOccupied()
    {
        return leftHandItem != null || rightHandItem != null || doubleHandItem != null;
    }


    void EquipItem(PickableItem item, HandType hand)
    {
        item.OnPickUp(this);

        Transform parent = null;
        string leftKey = GetFriendlyKeyName(playerController.keyLeftItem);
        string rightKey = GetFriendlyKeyName(playerController.keyRightItem);
        string leftKey_use = GetFriendlyKeyName(playerController.keyLeftUse);
        string rightKey_use = GetFriendlyKeyName(playerController.keyRightUse);
        string bothKeys = leftKey + "/" + rightKey;

        switch (hand)
        {
            case HandType.Left:
                parent = leftHandPoint;
                leftHandItem = item;
                itemHUD.setHandType(UI_HandType.Default);
                itemHUD.setPanelText(HandType.Left, item.itemID, leftKey, leftKey_use, item.itemSize);
                break;

            case HandType.Right:
                parent = rightHandPoint;
                rightHandItem = item;
                itemHUD.setHandType(UI_HandType.Default);
                itemHUD.setPanelText(HandType.Right, item.itemID, rightKey, rightKey_use, item.itemSize);
                break;

            case HandType.Both:
                parent = bothHandPoint;
                doubleHandItem = item;
                leftHandItem = null; 
                rightHandItem = null;
                itemHUD.setHandType(UI_HandType.Both);
                itemHUD.setPanelText(HandType.Both, item.itemID, bothKeys, "" ,item.itemSize);
                break;
        }

        item.transform.SetParent(parent);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale *= itemHoldScale;

        actor.currentState = ActorState.Carrying;
        ShowHint($"Picked up {item.name}");
    }

    private string GetFriendlyKeyName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Mouse0:
                return "Left Mouse Button";
            case KeyCode.Mouse1:
                return "Right Mouse Button";
            case KeyCode.Mouse2:
                return "Middle Mouse Button";

            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                return "Ctrl";
            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                return "Shift";

            default:
                return key.ToString().ToUpper();
        }
    }

        void TryPlaceOrDrop(PickableItem item)
    {
        Vector3 placePos = transform.position + transform.forward * gridSize;
        placePos = SnapToGrid(placePos);

        bool blocked = Physics.CheckSphere(placePos, placeCheckRadius, placeBlockLayer);
        if (blocked)
        {
            ShowHint("Cannot place here (Blocked)");
            return;
        }

        Drop(item, placePos);
    }

    void Drop(PickableItem item, Vector3? forcedPos = null)
    {
        if (item == null) return;

        item.OnDrop();

        string leftKey = playerController.keyLeftItem.ToString().ToUpper();
        string rightKey = playerController.keyRightItem.ToString().ToUpper();
        string bothKeys = leftKey + "/" + rightKey;

        if (item == leftHandItem) {
            itemHUD.resetPanelText(HandType.Left, leftKey);
            leftHandItem = null;
        }
        if (item == rightHandItem) {
            itemHUD.resetPanelText(HandType.Right, rightKey);
            rightHandItem = null;
        }
        if (item == doubleHandItem) {
            itemHUD.resetPanelText(HandType.Both, bothKeys);
            doubleHandItem = null;
        } 

        item.transform.SetParent(null);
        item.transform.localScale = Vector3.one;

        Vector3 dropPos = forcedPos ?? (transform.position + transform.forward * 0.8f + Vector3.up * 0.5f);
        item.transform.position = dropPos;

        if (leftHandItem == null && rightHandItem == null && doubleHandItem == null)
            actor.currentState = ActorState.Normal;

        itemHUD.setHandType(UI_HandType.Default);
        ShowHint($"Dropped {item.name}");
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
        pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
        return pos;
    }

    void TryInteract(PickableItem item)
    {
        item.OnInteract(this);
    }

    public void ShowPickUpHint(PickableItem item)
    {
        if (item == null || pickUpHintUI == null) return;

        string message = "";
        string key = "";
        string showID = item.itemID;

        bool leftFree = (leftHandItem == null && doubleHandItem == null);
        bool rightFree = (rightHandItem == null && doubleHandItem == null);

        string leftKey = playerController.keyLeftItem.ToString().ToUpper();
        string rightKey = playerController.keyRightItem.ToString().ToUpper();
        string bothKeys = leftKey + "/" + rightKey;

        switch (item.itemSize)
        {
            case ItemSize.Heavy:
                message = "Too Heavy (Need Cart)";
                key = "";
                break;

            case ItemSize.Medium:
                if (AnyHandOccupied())
                {
                    message = "Hands Occupied";
                    key = "";
                }
                else
                {
                    message = "Pick Up";
                    key = bothKeys;
                }
                break;

            case ItemSize.Tool:
            case ItemSize.Light:
                if (doubleHandItem != null || (leftHandItem != null && rightHandItem != null))
                {
                    message = "Hands Full";
                    key = "";
                }
                else if (leftFree && rightFree)
                {
                    message = "Pick Up";
                    key = bothKeys;
                }
                else if (leftFree)
                {
                    message = "Pick Up";
                    key = leftKey;
                }
                else if (rightFree)
                {
                    message = "Pick Up";
                    key = rightKey;
                }
                break;
        }

        pickUpHintUI.AttachToItem(item.transform, message, key, showID);
    }

    public void HidePickUpHint()
    {
        if (pickUpHintUI != null)
        {
            pickUpHintUI.Hide();
        }
    }

    public void ShowReadHintUI(ReadableItem item)
    {
        interactableHintUI.AttachToItem(item.transform, item.title, item.text);
    }

    public void HideReadHintUI()
    {
        if (interactableHintUI != null)
        {
            interactableHintUI.Hide();
        }
    }

    void ShowHint(string msg)
    {
        //Debug.Log($"Hint: {msg}"); 
    }

    private void HandleItemUseInput()
    {
        if (Input.GetKeyDown(playerController.keyLeftUse))
        {
            if (leftHandItem != null)
            {
                AttemptInteraction(leftHandItem);
            }
        }

        if (Input.GetKeyDown(playerController.keyRightUse))
        {
            if (rightHandItem != null)
            {
                AttemptInteraction(rightHandItem);
            }
        }
    }
    private void AttemptInteraction(PickableItem itemInHand)
    {
        Weapon weapon = itemInHand.GetComponent<Weapon>();
        if (weapon != null)
        {
            weapon.UseWeapon(this);
        }

        Block targetBlock = GetFrontBlock(); 
        if (targetBlock == null) return; 

        Door door = targetBlock.GetComponent<Door>();
        if (door != null)
        {
            door.TryUnlock(itemInHand);
            return; 
        }

        //TO DO: More Useable Items
        // else if (targetBlock.GetComponent<XXX>() != null) { ... }
    }
}