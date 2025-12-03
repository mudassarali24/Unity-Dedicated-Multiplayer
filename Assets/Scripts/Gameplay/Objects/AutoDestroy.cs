using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifeTime = 2f;
    private ObjectPooler pooler;
    ParticleSystem ps;

    private void Start()
    {
        pooler = GetComponentInParent<ObjectPooler>();
        TryGetComponent<ParticleSystem>(out ps);
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (ps) ps.Play();
        Invoke(nameof(Destroy), lifeTime);
    }

    private void Destroy()
    {
        pooler.AddToPool(gameObject);
    }
}