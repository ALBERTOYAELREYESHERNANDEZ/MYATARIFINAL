using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Ahora requerimos el Rigidbody 2D
public class Ball : MonoBehaviour
{
    private Rigidbody2D rb; // Cambiamos a Rigidbody2D
    public float velocidadMaxima = 20f;
    public float velocidadMinima = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtenemos el Rigidbody2D
        
        // --- FORZAR CONFIGURACIÓN DEL RIGIDBODY DESDE CÓDIGO ---
        // Esto anula cualquier configuración incorrecta en el Inspector.
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // Sin gravedad.
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Solo congelamos la rotación.
        
        LanzarBola();
    }

    private void LanzarBola()
    {
        float direccionX = Random.value < 0.5f ? -1f : 1f; // Dirección horizontal aleatoria

        // Usamos Vector2 para la dirección en 2D.
        Vector2 direccion = new Vector2(direccionX * 0.5f, 1).normalized;

        rb.linearVelocity = direccion * velocidadMinima;

        rb.WakeUp(); // Aseguramos que el Rigidbody esté activo
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0)
        {
            // Mantenemos la velocidad dentro de los límites
            rb.linearVelocity = rb.linearVelocity.normalized * 
                Mathf.Clamp(rb.linearVelocity.magnitude, velocidadMinima, velocidadMaxima);
        }
    }

    // Cambiamos a OnCollisionEnter2D para colisiones 2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ContactPoint2D contacto = collision.GetContact(0);

            float desplazamientoX =
                (contacto.point.x - collision.transform.position.x)
                / collision.collider.bounds.size.x;

            // Nueva dirección de rebote en 2D
            Vector2 nuevaDireccion = new Vector2(desplazamientoX, 1f).normalized;

            rb.linearVelocity = nuevaDireccion * rb.linearVelocity.magnitude;
        }
    }
}