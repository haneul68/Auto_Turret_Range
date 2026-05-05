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
    private Transform target;
    [SerializeField] 
    private Projectile_Pool_Manager projectile_Pool_Manager;

    [Header("Yaw")]
    [SerializeField]
    private float yaw_Rotate_Speed = 120f;

    [Header("Pitch")]
    [SerializeField]
    private float pitch_Rotate_Speed = 120f;
    [SerializeField]
    private float min_Pitch = -45f;
    [SerializeField]
    private float max_Pitch = 30f;

    [Header("Fire")]
    [SerializeField]
    private float fire_Interval = 0.5f;
    [SerializeField]
    private float ray_Distance = 20f;
    [SerializeField]
    private LayerMask target_Layer;

    private float fire_Timer;

    private void Update()
    {
        if (target == null)
            return;

        Rotate_Yaw();
        Rotate_Pitch();
        Fire_Check();
    }

    private void Rotate_Yaw()
    {
        Vector3 dir = target.position - yaw_Pivot.position;
        dir.y = 0f;

        if (dir.sqrMagnitude <= 0.001f)
            return;

        Quaternion target_Rotation = Quaternion.LookRotation(dir);
        yaw_Pivot.rotation = Quaternion.RotateTowards(yaw_Pivot.rotation, target_Rotation, yaw_Rotate_Speed * Time.deltaTime);
    }

    private void Rotate_Pitch()
    {
        Vector3 local_Target_Pos = pitch_Pivot.InverseTransformPoint(target.position);

        float angle = Mathf.Atan2(-local_Target_Pos.y, local_Target_Pos.z) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, min_Pitch, max_Pitch);

        Quaternion target_Local_Rotation = Quaternion.Euler(angle, 0f, 0f);
        pitch_Pivot.localRotation = Quaternion.RotateTowards(pitch_Pivot.localRotation, target_Local_Rotation, pitch_Rotate_Speed * Time.deltaTime);
    }

    private void Fire_Check()
    {
        fire_Timer += Time.deltaTime;

        Debug.DrawRay(muzzle_Point.position, muzzle_Point.forward * ray_Distance, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(muzzle_Point.position, muzzle_Point.forward, out hit, ray_Distance, target_Layer))
        {
            if (hit.transform == target && fire_Timer >= fire_Interval)
            {
                Fire();
                fire_Timer = 0f;
            }
        }
    }

    private void Fire()
    {
        if (projectile_Pool_Manager == null)
            return;

        Projectile_Mover projectile = projectile_Pool_Manager.Get_Projectile();

        if (projectile == null)
            return;

        projectile.Fire(muzzle_Point.position, muzzle_Point.rotation);
    }
}