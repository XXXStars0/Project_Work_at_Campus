using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaphoneWeapon : Weapon
{
    [Header("Megaphone Settings")]
    public float persuasionAmount = 20f;

    public float range = 5f;
    public float angle = 90f;
    
    [Header("Detection Settings")]
    public LayerMask enemyLayer;

    protected override void OnAttack(PlayerItemManager user)
    {
        //Debug.Log("Used Megaphone!");
        
        Collider[] hitsInSphere = Physics.OverlapSphere(user.transform.position, range, enemyLayer);

        foreach (var hit in hitsInSphere)
        {
            Vector3 directionToTarget = (hit.transform.position - user.transform.position).normalized;
            
            if (Vector3.Angle(user.transform.forward, directionToTarget) < angle / 2)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ApplyPersuasion(persuasionAmount);
                }
            }
        }
    }


    protected override void DrawAttackShape(LineRenderer lineRenderer, PlayerItemManager user)
    {
        if (lineRenderer == null) return;

        lineRenderer.startColor = new Color(0, 1, 1, 0.5f); 
        lineRenderer.endColor = new Color(0, 1, 1, 0.1f);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;

        lineRenderer.transform.position = user.transform.position;
        lineRenderer.transform.rotation = user.transform.rotation;
        
        int segments = 20;
        float halfAngleRad = angle * 0.5f * Mathf.Deg2Rad; 
        
        lineRenderer.positionCount = segments + 2; 

        lineRenderer.SetPosition(0, Vector3.zero);


        for (int i = 0; i <= segments; i++)
        {

            float currentAngle = -halfAngleRad + (i * (angle * Mathf.Deg2Rad) / segments);
            
            float x = Mathf.Sin(currentAngle) * range;
            float z = Mathf.Cos(currentAngle) * range;
            
            lineRenderer.SetPosition(i + 1, new Vector3(x, 0, z));
        }

        lineRenderer.loop = true;
    }
}