using System.Collections;
using UnityEngine;

//al empezar el nivel la camara arranca lejos y se acerca hasta su posicion fija, de paso se ve el LOD cambiar
public class CameraIntro : MonoBehaviour
{
    [SerializeField] private float startDistance = 30f;//cuanto mas atras de la posicion fija arranca
    [SerializeField] private float duration = 4f;//tiene que ser menor o igual a introDuration del Level1Intro

    void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        Vector3 endPos = transform.position;//la posicion que dejaste fija en la escena
        Vector3 startPos = endPos - transform.forward * startDistance;//hacia atras, asi mira siempre al mismo lugar
        transform.position = startPos;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;//si se pausa, se frena tambien
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        transform.position = endPos;//por si quedo un poco antes
    }
}