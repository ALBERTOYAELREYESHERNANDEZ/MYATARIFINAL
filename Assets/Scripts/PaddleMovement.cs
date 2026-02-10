using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Requerimos Rigidbody2D
public class PaddleMovement : MonoBehaviour
{
    public float velocidad = 10f; // Velocidad de movimiento
    public float limiteHorizontal = 8f; // Límite para que la paleta no salga de la pantalla

    private Rigidbody2D rb; // Usamos Rigidbody2D

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Forzamos la configuración del Rigidbody2D para que sea Kinematic
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }

    // Usamos FixedUpdate para movimientos basados en física
    private void FixedUpdate()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");

        // Calculamos la nueva posición en 2D
        Vector2 nuevaPosicion = rb.position + new Vector2(inputHorizontal, 0) * velocidad * Time.fixedDeltaTime;

        // Limitamos la posición horizontal
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, -limiteHorizontal, limiteHorizontal);
        
        // Movemos el Rigidbody a la nueva posición
        rb.MovePosition(nuevaPosicion);
    }
}