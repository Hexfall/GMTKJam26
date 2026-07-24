using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    private Vector3 target;

    [SerializeField] private float speed = 40f;


    public void Initialize(Vector3 destination)
    {
        target = destination;
    }


    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );


        if(Vector3.Distance(transform.position, target) < 0.05f)
        {
            Destroy(gameObject);
        }
    }
}