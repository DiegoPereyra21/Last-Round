using System.Collections.Generic;
using UnityEngine;

//pool simple: recicla objetos en vez de Instantiate/Destroy (Unidad 2, 4.2)
public class Pool : MonoBehaviour
{
    //un pool por prefab, se crean solos la primera vez que se pide uno
    static readonly Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();

    [SerializeField] private int initialAmount = 10;//se crean de antemano para evitar picos al disparar
    private GameObject prefab;
    private readonly List<GameObject> objects = new List<GameObject>();

    //reemplaza a Instantiate(prefab, pos, rot)
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        //"pool == null" tambien cubre el caso de un pool destruido por cambiar de escena
        if (!pools.TryGetValue(prefab, out Pool pool) || pool == null)
        {
            pool = new GameObject("Pool_" + prefab.name).AddComponent<Pool>();
            pool.prefab = prefab;
            pool.Prewarm();
            pools[prefab] = pool;
        }
        return pool.Get(position, rotation);
    }

    void Prewarm()
    {
        for (int i = 0; i < initialAmount; i++)
        {
            GameObject go = Instantiate(prefab, transform);
            go.SetActive(false);
            objects.Add(go);
        }
    }

    GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject go = null;
        //si hay objetos libres recicla el primero, si no crea uno nuevo
        while (objects.Count > 0 && go == null)
        {
            go = objects[0];
            objects.RemoveAt(0);
        }
        if (go == null) go = Instantiate(prefab, transform);

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        go.SendMessage("OutPool", this, SendMessageOptions.DontRequireReceiver);//le avisa de que pool salio
        return go;
    }

    public void Return(GameObject go)
    {
        if (!go.activeSelf) return;//por si vuelve dos veces
        go.SetActive(false);
        objects.Add(go);
    }
}