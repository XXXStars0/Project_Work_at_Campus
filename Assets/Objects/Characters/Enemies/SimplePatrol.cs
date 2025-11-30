using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class SimplePatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float speed = 2f;

    private int currentWaypointIndex = 0;
    private Enemy _e;

    void Awake()
    {
        _e = GetComponent<Enemy>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerItemManager player = collision.gameObject.GetComponent<PlayerItemManager>();
        if (player)
        {
            _e.TriggerThreat(player);

            Vector3 pushDirection = (player.transform.position - transform.position).normalized;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb)
            {
                playerRb.AddForce(pushDirection * 5f, ForceMode.Impulse);
            }
        }
    }
}