using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowstickWeapon : Weapon
{
    [Header("Glowstick Settings")]
    public float persuasionAmount = 35f;
    public float range = 2f;

    [Header("Detection Settings")]
    public LayerMask enemyLayer;

    protected override void OnAttack(PlayerItemManager user)
    {
        //Debug.Log("Used Glowstick!");
        Vector3 attackCenter = user.transform.position + user.transform.forward * (range / 2);
        Collider[] hits = Physics.OverlapSphere(attackCenter, range, enemyLayer);

        foreach (var hit in hits)
        {
            Debug.Log(hit);
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ApplyPersuasion(persuasionAmount);
                break;
            }
        }
    }

    protected override void DrawAttackShape(LineRenderer lineRenderer, PlayerItemManager user)
    {
        if (lineRenderer == null) return;

        lineRenderer.startColor = new Color(1, 1, 0, 0.5f);
        lineRenderer.endColor = new Color(1, 1, 0, 0.1f);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;

        Vector3 attackCenter = user.transform.position + user.transform.forward * (range / 2);

        lineRenderer.transform.position = attackCenter;
        lineRenderer.transform.rotation = Quaternion.identity;

        int segments = 32; 
        lineRenderer.positionCount = segments + 1;
        for (int i = 0; i < segments + 1; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            float x = Mathf.Sin(angle) * range;
            float y = Mathf.Cos(angle) * range;

            lineRenderer.SetPosition(i, new Vector3(x, 0, y)); 
        }
        // 为了更好地表示球体，可以再画两个垂直的圆
        // lineRenderer.positionCount = segments * 3 + 1; // 需要更多点
        // DrawCircle(lineRenderer, segments, range, Vector3.up);
        // DrawCircle(lineRenderer, segments, range, Vector3.right);
        // DrawCircle(lineRenderer, segments, range, Vector3.forward);
    }
}