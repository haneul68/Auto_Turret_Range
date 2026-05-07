using UnityEngine;

public class Auto_Turret_Controller : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField]
    private Transform yaw_Pivot;
    [SerializeField]
    private Transform pitch_Pivot;
    [SerializeField]
    private Transform muzzle_Point;
    [SerializeField]
    private Projectile_Pool_Manager projectile_Pool_Manager;

    [Header("Target")]
    [SerializeField]
    private float detect_Range = 20f;
    [SerializeField]
    private LayerMask target_Layer;

    private Transform target;
    private Collider target_Collider;
    private Transform previous_Target;

    [Header("Yaw")]
    [SerializeField]
    private float yaw_Smooth = 10f;

    [Header("Pitch")]
    [SerializeField]
    private float pitch_Smooth = 10f;
    [SerializeField]
    private float min_Pitch = -45f;
    [SerializeField]
    private float max_Pitch = 30f;

    [Header("Fire")]
    [SerializeField]
    private float fire_Interval = 0.5f;
    [SerializeField]
    private float ray_Distance = 20f;

    private float fire_Timer;

    private void Start()
    {
        fire_Timer = fire_Interval;
    }

    private void Update()
    {
        Find_Closest_Target();

        if (target == null)
        {
            previous_Target = null;
            fire_Timer = fire_Interval;
            return;
        }

        if (previous_Target != target)
        {
            fire_Timer = fire_Interval;
            previous_Target = target;
        }

        Rotate_Yaw();
        Rotate_Pitch();

        Fire_Check();
    }

    private void Find_Closest_Target()
    {
        if (target != null && !target.gameObject.activeInHierarchy)
        {
            target = null;
            target_Collider = null;
        }

        if (target != null && target.gameObject.activeInHierarchy)
        {
            float sqrDistance =
                (target.position - transform.position).sqrMagnitude;

            if (sqrDistance <= detect_Range * detect_Range)
            {
                return;
            }
        }

        target = null;
        target_Collider = null;

        Collider[] hits = Physics.OverlapSphere
        (
            transform.position,
            detect_Range,
            target_Layer
        );

        float closestDistance = float.MaxValue;
        Transform closestTarget = null;
        Collider closestCollider = null;

        foreach (Collider hit in hits)
        {
            EnemyLinearMover enemy =
                hit.GetComponentInParent<EnemyLinearMover>();

            if (enemy == null)
                continue;

            if (!enemy.gameObject.activeInHierarchy)
                continue;

            float distance = Vector3.SqrMagnitude
            (
                enemy.transform.position - transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = enemy.transform;
                closestCollider = hit;
            }
        }

        target = closestTarget;
        target_Collider = closestCollider;
    }

    private void Rotate_Yaw()
    {
        Vector3 aimPos = Get_Target_Aim_Position();

        Vector3 dir = aimPos - yaw_Pivot.position;
        dir.y = 0f;

        if (dir.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

        float t = 1f - Mathf.Exp(-yaw_Smooth * Time.deltaTime);

        yaw_Pivot.rotation = Quaternion.Slerp
        (
            yaw_Pivot.rotation,
            targetRotation,
            t
        );
    }

    private void Rotate_Pitch()
    {
        Vector3 aimPos = Get_Target_Aim_Position();

        Vector3 dir = aimPos - pitch_Pivot.position;

        Vector3 localDir = yaw_Pivot.InverseTransformDirection(dir.normalized);

        float targetAngle = Mathf.Atan2
        (
            -localDir.y,
            localDir.z
        ) * Mathf.Rad2Deg;

        targetAngle = Mathf.Clamp(targetAngle, min_Pitch, max_Pitch);

        float currentAngle = Normalize_Angle(pitch_Pivot.localEulerAngles.x);

        float t = 1f - Mathf.Exp(-pitch_Smooth * Time.deltaTime);

        float smoothAngle = Mathf.LerpAngle
        (
            currentAngle,
            targetAngle,
            t
        );

        pitch_Pivot.localRotation = Quaternion.Euler(smoothAngle, 0f, 0f);
    }

    private void Fire_Check()
    {
        fire_Timer += Time.deltaTime;

        Debug.DrawRay
        (
            muzzle_Point.position,
            muzzle_Point.forward * ray_Distance,
            Color.red
        );
        if (Physics.Raycast(muzzle_Point.position, muzzle_Point.forward, out RaycastHit hit, ray_Distance, target_Layer))
        {
            EnemyLinearMover enemy =
                hit.collider.GetComponentInParent<EnemyLinearMover>();

            if (enemy == null)
                return;

            target = enemy.transform;
            target_Collider = hit.collider;

            if (fire_Timer >= fire_Interval)
            {
                Fire();
                fire_Timer = 0f;
            }
        }
    }

    private void Fire()
    {
        if (projectile_Pool_Manager == null)
        {
            Debug.LogWarning("projectile_Pool_Manager == null");
            return;
        }

        Projectile_Mover projectile = projectile_Pool_Manager.Get_Projectile();

        if (projectile == null)
        {
            Debug.LogWarning("projectile == null");
            return;
        }

        projectile.Fire
        (
            muzzle_Point.position,
            muzzle_Point.rotation
        );
    }

    private Vector3 Get_Target_Aim_Position()
    {
        if (target_Collider != null)
            return target_Collider.bounds.center;

        return target.position;
    }

    private float Normalize_Angle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detect_Range);
    }
}