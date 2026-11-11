using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 2f;
    private Pool pool;

    void OutPool(Pool origin)
    {
        pool = origin;
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Health otherHealth = other.GetComponent<Health>();
        if (otherHealth != null)
        {
            otherHealth.TakeDamage(damage);
        }
        ReturnToPool();
    }

    void ReturnToPool()
    {
        CancelInvoke();
        if (pool != null) pool.Return(gameObject);
        else Destroy(gameObject);
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }
}