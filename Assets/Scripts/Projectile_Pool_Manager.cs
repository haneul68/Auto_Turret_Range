using UnityEngine;

public class Projectile_Pool_Manager : MonoBehaviour
{
    [SerializeField]
    private Projectile_Mover projectile_Prefab;

    [SerializeField]
    private int default_Count = 20;

    [SerializeField]
    private int max_Count = 40;

    private Local_Object_Pool<Projectile_Mover> projectile_Pool;

    private void Awake()
    {
        projectile_Pool =
            new Local_Object_Pool<Projectile_Mover>
            (
                projectile_Prefab,
                transform,
                default_Count,
                max_Count
            );
    }

    public Projectile_Mover Get_Projectile()
    {
        Projectile_Mover projectile = projectile_Pool.Get();

        projectile.Set_Pool(this);

        return projectile;
    }

    public void Return_Projectile(Projectile_Mover projectile)
    {
        projectile_Pool.Return(projectile);
    }
}