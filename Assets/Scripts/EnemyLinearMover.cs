using UnityEngine;

public class EnemyLinearMover : MonoBehaviour
{
    [SerializeField] private float moveSpeedUnitsPerSecond = 5f;
    [SerializeField] private float lifeTimeSeconds = 10f;

    [Header("Up Down")]
    [SerializeField] private float up_Down_Range = 0.5f;
    [SerializeField] private float up_Down_Speed = 2f;

    private Enemy_Spawner pool_Manager;

    private float remainingLifeSeconds;
    private float base_Y;
    private bool is_Returned;

    public void Set_Pool(Enemy_Spawner pool)
    {
        pool_Manager = pool;
    }

    public void Initialize(float speedUnitsPerSecond, float lifeTime)
    {
        moveSpeedUnitsPerSecond = speedUnitsPerSecond;
        lifeTimeSeconds = lifeTime;
        remainingLifeSeconds = lifeTimeSeconds;

        base_Y = transform.position.y;
        is_Returned = false;
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        pos += transform.forward * moveSpeedUnitsPerSecond * Time.deltaTime;

        pos.y = base_Y + Mathf.Sin(Time.time * up_Down_Speed) * up_Down_Range;

        transform.position = pos;

        remainingLifeSeconds -= Time.deltaTime;

        if (remainingLifeSeconds <= 0f)
        {
            Return_To_Pool();
        }
    }

    public void Return_To_Pool()
    {
        if (is_Returned)
            return;

        is_Returned = true;

        if (pool_Manager != null)
        {
            pool_Manager.Return_Enemy(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}