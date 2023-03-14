using System.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 Target;
    public float Speed = 100;

    void Update()
    {
        if (Target != null)
        {
            var step = Speed * Time.deltaTime;
            if((transform.position - Target).magnitude < step)
            {
                step = (transform.position - Target).magnitude;
            }
            transform.LookAt(Target);
            transform.Translate(Vector3.forward * step);
            if (transform.position.x == Target.x && transform.position.z == Target.z)
            {
                Destroy(gameObject);
            }
        }
    }
}
