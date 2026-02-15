using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // Destruir la bola al caer en la zona de muerte
            Destroy(other.gameObject);

            // Notificar al GameManager para restar vida y respawnear
            if (GameManager.Instancia != null)
            {
                GameManager.Instancia.PerderVida();
            }
        }
    }
}