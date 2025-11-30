using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is an abstract base class for any "persuasion tool".
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float cooldown = 1f;

    private float lastUseTime = -999f;

    [Header("Visual Feedback")]
    public GameObject rangeVisualizerPrefab;
    private GameObject _currentVisualizerInstance;
    public float visualizerDuration = 0.5f;

    public void UseWeapon(PlayerItemManager user)
    {
        if (Time.time >= lastUseTime + cooldown)
        {
            lastUseTime = Time.time;
            OnAttack(user);

            StartCoroutine(VisualizeAttackRangeCo(user));
        }
        else
        {
            //Debug.Log($"{gameObject.name} is on cooldown.");
        }
    }

    protected abstract void OnAttack(PlayerItemManager user);

    private IEnumerator VisualizeAttackRangeCo(PlayerItemManager user)
    {
        if (rangeVisualizerPrefab == null) yield break; 

        if (_currentVisualizerInstance != null)
        {
            Destroy(_currentVisualizerInstance);
        }

        _currentVisualizerInstance = Instantiate(rangeVisualizerPrefab, user.transform.position, user.transform.rotation);
        _currentVisualizerInstance.transform.parent = user.transform;

        DrawAttackShape(_currentVisualizerInstance.GetComponent<LineRenderer>(), user);

        yield return new WaitForSeconds(visualizerDuration);

        if (_currentVisualizerInstance != null)
        {
            Destroy(_currentVisualizerInstance);
            _currentVisualizerInstance = null; 
        }
    }

    protected abstract void DrawAttackShape(LineRenderer lineRenderer, PlayerItemManager user);

    private void OnDestroy()
    {
        if (_currentVisualizerInstance != null)
        {
            Destroy(_currentVisualizerInstance);
        }
    }
}