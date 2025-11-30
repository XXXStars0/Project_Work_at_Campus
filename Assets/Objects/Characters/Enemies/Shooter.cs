using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(SphereCollider))] 
public class Shooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;

    private float fireTimer;
    private Transform playerTarget;
    private Enemy _e;

    void Awake()
    {
        _e = GetComponent<Enemy>();
        GetComponent<SphereCollider>().isTrigger = true;
    }

    void Update()
    {
        if (playerTarget)
        {
            transform.LookAt(playerTarget);
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                fireTimer = 0;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        /*if (bullet)
        {
            bullet.SetOwner(_e);
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>())
        {
            playerTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == playerTarget)
        {
            playerTarget = null;
        }
    }
}