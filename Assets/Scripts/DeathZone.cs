using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // 1. OnTriggerEnter2D se llama cuando un collider (marcado como Is Trigger) es atravesado
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 2. Comprobar si el objeto que lo atraviesa es la Bola
        if (other.CompareTag("Ball"))
        {
            // 3. Informar al GameManager que se perdió una vida
            GameManager.Instancia.PerderVida();
        }
    }
}