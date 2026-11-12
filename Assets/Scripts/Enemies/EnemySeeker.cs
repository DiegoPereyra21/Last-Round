using UnityEngine;
using UnityEngine.AI;

//ia para el movimiento y navmesh para q calcule caminos
[RequireComponent(typeof(NavMeshAgent))] //obliga a tener navmesh
public class EnemySeeker : MonoBehaviour
{
    public Transform firePoint;
    public GameObject enemyProjectile;
    [SerializeField] private float startDelay = 1.5f;//para que no disparen apenas aparecen
    public float fireRate = 2f;

    //lento porque la idea no es que nos golpee melee
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float stopDistance = 5f;//se frena a esta distancia para dispararte en vez de pegarse (luego ajustar a ojo)
    [SerializeField] private float projectileDamage = 12f;

    [SerializeField] private Transform player;
    private float nextFire;
    private NavMeshAgent agent;
    private Animator anim;

    //para el sonido de la bola de fuego
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] fireballSounds;//f1,f2,f3, uno al azar por disparo para que no suene repetitivo

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();//el animator esta en el dragon hijo
        //seguridad x si me olvidode algo
        if (player == null || firePoint == null || enemyProjectile == null || anim == null)
        {
            Debug.LogWarning(name + " faltan referencias");
            enabled = false;
            return;
        }
        agent.speed = speed;
        agent.stoppingDistance = stopDistance;
        agent.updateRotation = false;//la rotacion la manejo yo, asi siempre mira al player para disparar
        nextFire = Time.time + startDelay;
    }

    void Update()
    {
        if (player == null)//si el player murio, que se quede quieto y no tire error
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        //antes iba en linea recta y atravesaba cajas, ahora el agente busca el camino
        if (agent.isOnNavMesh)
            agent.SetDestination(player.position);

        anim.SetFloat("speed", agent.velocity.magnitude > 0.1f ? 1f : 0f, 0.1f, Time.deltaTime);//cambio a blendTree Locomotion
        //mira al player pero sin inclinarse si hay diferencia de altura
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        if (Time.time >= nextFire)
        {
            Shoot();
            nextFire = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject projectile = Pool.Spawn(enemyProjectile, firePoint.position, firePoint.rotation);//CAMBIO A POOOL
        anim.SetTrigger("shoot");
        projectile.GetComponent<ProjectileEnemy>().SetDamage(projectileDamage);
        audioSource.PlayOneShot(fireballSounds[Random.Range(0, fireballSounds.Length)]);//sonido de la bola de fuego
    }
}