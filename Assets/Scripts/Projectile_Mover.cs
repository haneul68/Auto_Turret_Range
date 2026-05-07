using System.Collections;
using UnityEngine;

public class Projectile_Mover : MonoBehaviour
{
    [SerializeField] private float projectile_Speed = 12f;
    [SerializeField] private float life_Time = 3f;

    private Projectile_Pool_Manager pool_Manager;
    private Coroutine life_Coroutine;

    private bool is_Returned;

    public void Set_Pool(Projectile_Pool_Manager pool)
    {
        pool_Manager = pool;
    }

    private void OnEnable()
    {
        is_Returned = false;
    }

    private void Update()
    {
        transform.position +=
            transform.forward *
            projectile_Speed *
            Time.deltaTime;
    }

    public void Fire(Vector3 start_Pos, Quaternion start_Rot)
    {
        transform.position = start_Pos;
        transform.rotation = start_Rot;

        is_Returned = false;

        if (life_Coroutine != null)
        {
            StopCoroutine(life_Coroutine);
            life_Coroutine = null;
        }

        life_Coroutine = StartCoroutine(Return_After_Time());
    }

    private IEnumerator Return_After_Time()
    {
        yield return new WaitForSeconds(life_Time);

        Return_To_Pool();
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyLinearMover enemy =
            other.GetComponentInParent<EnemyLinearMover>();

        if (enemy == null)
            return;

        enemy.Return_To_Pool();

        Return_To_Pool();
    }

    private void Return_To_Pool()
    {
        if (is_Returned)
            return;

        is_Returned = true;

        if (pool_Manager != null)
        {
            pool_Manager.Return_Projectile(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (life_Coroutine != null)
        {
            StopCoroutine(life_Coroutine);
            life_Coroutine = null;
        }
    }
}