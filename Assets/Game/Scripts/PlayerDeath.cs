using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    Transform _checkpoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Cuando inicie el player, detectamos el checkpoint
        _checkpoint = GameObject.FindGameObjectWithTag(Names.TAG_CHECKPOINT).transform;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Names.TAG_DEADZONE))
        {
            // Cuando el player choca con la zona de muerte, su posición se manda a la posición del checkpoint
            transform.localPosition = _checkpoint.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
