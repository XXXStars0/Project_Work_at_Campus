// Bullet.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public float lifetime = 5f;

    private Enemy owner;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetOwner(Enemy _e)
    {
        owner = _e;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        PlayerItemManager player = other.GetComponentInParent<PlayerItemManager>();
        if (player)
        {
             //owner.TriggerThreat(player);

            Vector3 pushDirection = transform.forward;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb)
            {
                playerRb.AddForce(pushDirection * 10.0f, ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
        else if(!other.GetComponentInParent<Shooter>())
        {
            Destroy(gameObject);
        }

    }
}