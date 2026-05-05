using System.Collections.Generic;
using UnityEngine;

public class Projectile_Pool_Manager : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField]
    private Projectile_Mover projectile_Prefab;
    [SerializeField] 
    private int pool_Count = 20;

    private Queue<Projectile_Mover> projectile_Queue = new Queue<Projectile_Mover>();

    private void Awake()
    {
        Create_Pool();
    }

    private void Create_Pool()
    {
        for (int i = 0; i < pool_Count; i++)
        {
            Projectile_Mover projectile = Instantiate(projectile_Prefab, transform);
            projectile.gameObject.SetActive(false);
            projectile_Queue.Enqueue(projectile);
        }
    }

    public Projectile_Mover Get_Projectile()
    {
        Projectile_Mover projectile = projectile_Queue.Dequeue();

        projectile_Queue.Enqueue(projectile);

        return projectile;
    }
}