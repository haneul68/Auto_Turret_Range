using UnityEngine;

public class Target_Drone_Mover : MonoBehaviour
{
    [SerializeField]
    private Transform center_Target;

    [SerializeField] 
    private float radius = 6f;
    [SerializeField]
    private float rotate_Speed = 40f;

    [SerializeField]
    private float base_Height = 1.5f;
    [SerializeField] 
    private float up_Down_Range = 0.5f;
    [SerializeField]
    private float up_Down_Speed = 2f;

    private float angle;

    private void Update()
    {
        if (center_Target == null)
            return;

        angle += rotate_Speed * Time.deltaTime;

        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;

        float y = base_Height + Mathf.Sin(Time.time * up_Down_Speed) * up_Down_Range;

        Vector3 pos = new Vector3(x, y, z);

        transform.position = center_Target.position + pos;
    }
}