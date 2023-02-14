using System.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 Target;
    public float Speed = 100;

    private void Start()
    {
        AutoDestruct();
    }
    void Update()
    {
        if (Target != null)
        {
            //var targetLocation = Target;
            var step = Speed * Time.deltaTime;
            transform.LookAt(Target);
            transform.Translate(Vector3.forward * step);
            if (transform.position.x == Target.x && transform.position.z == Target.z)
            {
                Destroy(gameObject);
            }
        }
    }
    private async void AutoDestruct()
    {
        await Task.Delay(200);
        Destroy(gameObject);
    }
}
