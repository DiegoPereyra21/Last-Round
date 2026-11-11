using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 2f;
    private Pool pool;

    //lo llama el Pool cada vez que la bala sale (Start/Awake solo corren una vez, por eso el Invoke va aca)
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
        CancelInvoke();//si choco antes de los 2s, que el Invoke viejo no la devuelva en medio de otro uso
        if (pool != null) pool.Return(gameObject);
        else Destroy(gameObject);//por si alguna bala esta puesta a mano en la escena
    }
}