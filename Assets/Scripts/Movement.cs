using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    //muchos cambios con el movimiento xq no tengo animacion de "rotar" y queda rara la animacion al girar
    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSpeed = 720f;//grados por segundo, mas bajo = giro mas lento

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectile1;
    //aparte de daño y tamaño del projectile 2, debo hacerle otra particularidad
    [SerializeField] private GameObject projectile2;
    [SerializeField] private float cdProjectile1 = 0.6f;//la consigna del parcial pide minimo .4 de minimo
    [SerializeField] private float cdProjectile2 = 1.7f;
    private float nextShootProjectile1 = 0f;
    private float nextShootProjectile2 = 0f;
    private Rigidbody rb;
    private Vector3 dir;
    private Camera mainCam;
    private Animator anim;

    //cuanto tiempo sigue mirando al mouse despues de disparar sino se da vuelta en medio de la animacion
    [SerializeField] private float aimHoldTime = 0.4f;
    private float aimUntil = 0f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] fireballSounds1;//para el disparo 1
    [SerializeField] private AudioClip[] fireballSounds2;//para el disparo2

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;//se busca una sola vez en vez de cada frame
        anim = GetComponentInChildren<Animator>();//el animator esta en el dragon hijo
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return;//en pausa no gira ni dispara

        //input viejo, a futuro cambiar al actual 
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        dir = new Vector3(h, 0f, v).normalized;
        anim.SetFloat("speed", dir != Vector3.zero ? 1f : 0f, 0.1f, Time.deltaTime);//cambio a blendTree Locomotion

        //para q no dispare si apretas algun boton
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        //me complique con el temporizador, use el mismo q en enemy
        if (Input.GetMouseButtonDown(0) && Time.time >= nextShootProjectile1 && !overUI)
        {
            Shoot(projectile1, fireballSounds1);
            nextShootProjectile1 = Time.time + cdProjectile1;
        }

        if (Input.GetMouseButtonDown(1) && Time.time >= nextShootProjectile2 && !overUI)
        {
            Shoot(projectile2, fireballSounds2);
            nextShootProjectile2 = Time.time + cdProjectile2;
        }

        rb.linearVelocity = new Vector3(dir.x * speed, rb.linearVelocity.y, dir.z * speed);

        //recien disparo o estoy quieto: mira al mouse. caminando: mira hacia donde va
        //antes siempre miraba al mouse y caminar de costado se veia raro
        if (dir == Vector3.zero || Time.time < aimUntil)
            TurnTowards(GetAimDir());
        else
            TurnTowards(dir);
    }

    void Shoot(GameObject projectile, AudioClip[] sounds)
    {
        //al disparar gira de golpe, sino la bala sale hacia donde miraba antes
        Vector3 aimDir = GetAimDir();
        if (aimDir != Vector3.zero)
            transform.forward = aimDir;

        Pool.Spawn(projectile, firePoint.position, firePoint.rotation);//CAMBIO A POOL, lo pide el parcial y mejora la cosa

        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
        anim.SetTrigger("shoot");
        aimUntil = Time.time + aimHoldTime;
    }

    //gira de a poco en vez de pegar el volantazo
    void TurnTowards(Vector3 lookDir)
    {
        if (lookDir == Vector3.zero) return;
        Quaternion target = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    //para que apunte directamente al puntero, se sentia muy tosco el disparo antes
    Vector3 GetAimDir()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float dist))
        {
            Vector3 lookDir = ray.GetPoint(dist) - transform.position;
            lookDir.y = 0f;
            return lookDir;
        }
        return Vector3.zero;
    }
}