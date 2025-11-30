using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Persuasion Settings")]
    public float maxPersuasion = 100f;
    public bool movesAside = false;
    public Transform moveAsideTarget;

    [Header("UI References")]
    public GameObject persuasionSliderPrefab;
    private Slider _sliderInstance;

    [Header("Flash on Hit")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    [Header("Threat Settings")]
    public bool forcesDrop = true;

    private float currentPersuasion = 0f;
    private Actor myActor;

    private Renderer[] myRenderers;
    private List<Color> originalColors = new List<Color>();
    private MaterialPropertyBlock propBlock;

    private bool isFlashing = false;

    void Awake()
    {
        myActor = GetComponent<Actor>();
        if (myActor != null) myActor.currentState = ActorState.Enemy;

        propBlock = new MaterialPropertyBlock();

        myRenderers = GetComponentsInChildren<Renderer>();

        originalColors.Clear();
        foreach (var renderer in myRenderers)
        {
            if (renderer.material.HasProperty("_Color"))
            {
                originalColors.Add(renderer.material.GetColor("_Color"));
            }
            else
            {
                originalColors.Add(Color.clear);
            }
        }

        InstantiateSlider();
    }

    public void ApplyPersuasion(float amount)
    {
        if (currentPersuasion >= maxPersuasion) return;

        /*if (_sliderInstance == null && persuasionSliderPrefab != null)
        {
            InstantiateSlider();
        }*/

        currentPersuasion = Mathf.Clamp(currentPersuasion + amount, 0, maxPersuasion);
        if (_sliderInstance != null)
        {
            _sliderInstance.value = currentPersuasion;
        }

        if (!isFlashing)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentPersuasion >= maxPersuasion)
        {
            OnPersuaded();
        }
    }

    private IEnumerator FlashEffect()
    {
        isFlashing = true;

        for (int i = 0; i < myRenderers.Length; i++)
        {
            myRenderers[i].GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", flashColor);
            myRenderers[i].SetPropertyBlock(propBlock);
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < myRenderers.Length; i++)
        {
            if (originalColors[i] != Color.clear)
            {
                myRenderers[i].GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", originalColors[i]);
                myRenderers[i].SetPropertyBlock(propBlock);
            }
        }

        isFlashing = false;
    }


    private void OnPersuaded()
    {
        Debug.Log($"{name} has been persuaded!");
        GetComponent<Collider>().enabled = false;
        if (_sliderInstance != null)
        {
            Destroy(_sliderInstance.gameObject);
        }
        if (movesAside && moveAsideTarget != null)
        {
            // Move aside logic
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }
    }

    public void TriggerThreat(PlayerItemManager player)
    {
        // Threat logic
    }

    private void InstantiateSlider()
    {
        GameObject canvasObj = GameObject.Find("UI_Bubble");
        if (canvasObj == null)
        {
            return;
        }

        GameObject sliderGO = Instantiate(persuasionSliderPrefab);

        sliderGO.transform.SetParent(canvasObj.transform, false);

        _sliderInstance = sliderGO.GetComponent<Slider>();
        if (_sliderInstance != null)
        {
            _sliderInstance.maxValue = maxPersuasion;
            _sliderInstance.value = currentPersuasion;
        }

        FollowTargetUI follower = sliderGO.GetComponent<FollowTargetUI>();
        if (follower != null)
        {
            follower.target = this.transform;
        }
    }

    private void OnDestroy()
    {
        if (_sliderInstance != null)
        {
            Destroy(_sliderInstance.gameObject);
        }
    }
}