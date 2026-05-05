using System.Collections;
using UnityEngine;

public class Projectile_Mover : MonoBehaviour
{
    [SerializeField] 
    private float projectile_Speed = 12f;
    [SerializeField]
    private float life_Time = 3f;

    private Coroutine life_Coroutine;

    private void Update()
    {
        transform.position += transform.forward * projectile_Speed * Time.deltaTime;
    }

    public void Fire(Vector3 start_Pos, Quaternion start_Rot)
    {
        transform.position = start_Pos;
        transform.rotation = start_Rot;

        gameObject.SetActive(true);

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

        gameObject.SetActive(false);
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