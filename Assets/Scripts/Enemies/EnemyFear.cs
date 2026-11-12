using UnityEngine;

//ia simple para q tome decisiones, si distanciarse x su vida o tal
public class EnemyFear : MonoBehaviour
{
    enum State { Chase, Attack, Flee }
    [SerializeField] private State currentState = State.Chase;//serializado solo para verlo cambiar en el inspector (no deberia tocarlo en el instpecto nuncar)

    public Transform firePoint;
    public GameObject enemyProjectile;
    public float fireRate = 2f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float projectileDamage = 12f;

    [SerializeField] private float fleeDistance = 4f;//si te acercas a mas de esto huye
    [SerializeField] private float attackDistance = 9f;//si esta mas lejos de esto se acerca, no deberia pasar casi
    [SerializeField] private float lowHealthPercent = 0.3f;//poca vida huye siempre

    [SerializeField] private Transform player;
    [SerializeField] private float startDelay = 1.5f;//necesario para q no dispare apenas spawnea
    private float nextFire;
    private Health health;
    private Animator anim;
    //para el sonido de la bola de fuego
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] fireballSounds;//f1,f2,f3, uno al azar por disparo para que no suene repetitivo

    void Start()
    {
        health = GetComponent<Health>();
        anim = GetComponentInChildren<Animator>();//el animator esta en el dragon hijo
        //seguridad x si me olvidee algo
        if (player == null || firePoint == null || enemyProjectile == null || health == null || anim == null)
        {
            Debug.LogWarning(name + " faltan referencias");
            enabled = false;
            return;
        }
        nextFire = Time.time + startDelay;
    }

    void Update()
    {
        if (player == null) return;//si el player murio y se destruyo, que no tire error buscandolo

        transform.LookAt(player);
        Decide();
        Act();
    }

    //decide aca en q estado estar, si Flee, Chase o Attack
    void Decide()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        State newState;

        if (health.GetHealthPercent() <= lowHealthPercent || dist < fleeDistance)
            newState = State.Flee;
        else if (dist > attackDistance)
            newState = State.Chase;
        else
            newState = State.Attack;

        //solo log cuando cambia de estado, sino llenaria la consola con logs
        if (newState != currentState)
        {
            Debug.Log(name + " CAMBIA DE ESTADO: " + currentState + " -> " + newState);
            currentState = newState;
        }
    }

    //la "accion" por ese estado
    void Act()
    {
        anim.SetFloat("speed", currentState != State.Attack ? 1f : 0f, 0.1f, Time.deltaTime);//cambio a blendree

        switch (currentState)
        {
            case State.Chase://lejos = se acerca sin disparar
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                break;

            case State.Attack://distancia comoda = se sienta y dispara
                TryShoot();
                break;

            case State.Flee://cerca o poca vida = huye en direccion contraria al player
                Vector3 fleeTarget = transform.position + (transform.position - player.position);
                transform.position = Vector3.MoveTowards(transform.position, fleeTarget, speed * Time.deltaTime);
                TryShoot();
                break;
        }
    }

    void TryShoot()
    {
        if (Time.time < nextFire) return;

        GameObject projectile = Pool.Spawn(enemyProjectile, firePoint.position, firePoint.rotation);//CAMbIO A POOL
        anim.SetTrigger("shoot");//anim isparo
        //para la consigan de progresion, q haga mas daño luego de cada lvl
        projectile.GetComponent<ProjectileEnemy>().SetDamage(projectileDamage);
        audioSource.PlayOneShot(fireballSounds[Random.Range(0, fireballSounds.Length)]);//sonido de la bola de fuego
        nextFire = Time.time + fireRate;
    }
}