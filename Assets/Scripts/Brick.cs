using UnityEngine;

public class Brick : MonoBehaviour
{
    public int puntos = 10; // Puntos que otorga al ser destruido
    
    // 1. OnCollisionEnter2D se llama al detectar una colisión en 2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 2. Comprobar si la colisión fue con un objeto etiquetado como "Ball"
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 3. Llamar al GameManager para actualizar la puntuación y el contador de ladrillos
            // Usamos el Singleton para acceder al GameManager
            GameManager.Instancia.LadrilloDestruido(puntos); 

            // 4. Destruir este objeto (el ladrillo)
            Destroy(gameObject);
        }
    }
}