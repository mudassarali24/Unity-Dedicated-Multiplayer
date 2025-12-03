using UnityEngine;

public class TracerObject : MonoBehaviour
{
    public float lifeTime = 5f;
    public float speed = 5f;
    public GameObject hitImpact;
    Vector3 targetPos;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void SetTarget(Vector3 target)
    {
        targetPos = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(hitImpact, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}