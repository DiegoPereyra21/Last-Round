using UnityEngine;

public class EnemyRandom : MonoBehaviour
{
    public Transform firePoint;
    public GameObject enemyProjectile;
    public float fireRate = 2f;
    private Transform transformPlayer;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float minChangeTime = 1f;
    [SerializeField] private float maxChangeTime = 3f;
    [SerializeField] private float projectileDamage = 12f;

    [SerializeField] private Transform player;

    [SerializeField] private float startDelay = 1.5f;//para que no disparen apenas aparecen
    private float nextFire;

    private Vector3 moveDir;
    private float nextChangeTime;
    private Animator anim;

    //para el sonido de la bola de fuego
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] fireballSounds;//f1,f2,f3, uno al azar por disparo para que no suene repetitivo
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponentInChildren<Animator>();//el animator esta en el dragon hijo
        //seguridad x si nos olvidamos algo
        if (player == null || firePoint == null || enemyProjectile == null || anim == null)
        {
            Debug.LogWarning(name + " faltan referencias");
            enabled = false;
            return;
        }
        transformPlayer = player.transform;
        PickNewDirection();
        SetNextChangeTime();
        nextFire = Time.time + startDelay;

        anim.SetBool("isWalking", true);//siempre se esta moviendo
    }
    // Update is called once per frame
    void Update()
    {
        if (player == null) return;//si el player murio y se destruyo, que no tire error buscandolo

        transform.LookAt(transformPlayer);

        //cada tanto (random) cambia de lado, el mas impredecible de los 4
        if (Time.time >= nextChangeTime)
        {
            PickNewDirection();
            SetNextChangeTime();
        }
        transform.position += moveDir * speed * Time.deltaTime;

        if (Time.time >= nextFire)
        {
            GameObject Projectile = Pool.Spawn(enemyProjectile, firePoint.position, firePoint.rotation);//CAMBIO A POOL
            anim.SetTrigger("shoot");
            //para la consigna de que tengan mas daño segun el nivel, hasta ahora solo cambiaba el firerate pero no esta en la consigna
            Projectile.GetComponent<ProjectileEnemy>().SetDamage(projectileDamage);
            audioSource.PlayOneShot(fireballSounds[Random.Range(0, fireballSounds.Length)]);//sonido de la bola de fuego
            nextFire = Time.time + fireRate;
        }
    }

    void SetNextChangeTime()
    {
        //tira el numero random para saber cuanto tarda en volver a cambiar
        nextChangeTime = Time.time + Random.Range(minChangeTime, maxChangeTime);
    }

    void PickNewDirection()
    {
        //angulo random en 360, para que no sea solo izq/der sino cualquier lado
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        moveDir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
    }
}