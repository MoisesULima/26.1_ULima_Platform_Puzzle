using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Despertando al Player...");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Inicializar el player...");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Actualiza el player, se llama muchas veces...");
    }
}
