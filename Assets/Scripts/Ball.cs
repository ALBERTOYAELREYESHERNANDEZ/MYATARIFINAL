using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Fuerza del impulso inicial hacia arriba.")]
    public float fuerzaImpulso = 300f;

    [Tooltip("Velocidad vertical fija al rebotar en el paddle.")]
    public float velocidadRebote = 10f;

    private Rigidbody2D rb;
    private GameObject paddle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Activar la gravedad para que la pelota caiga
            rb.gravityScale = 1f;

            // Aplicar impulso hacia arriba
            rb.AddForce(Vector2.up * fuerzaImpulso);
        }
    }

    public void SetPaddle(GameObject _paddle)
    {
        paddle = _paddle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos si es el paddle por referencia O buscando el componente (más seguro tras reinicios)
        bool esPaddle = (paddle != null && collision.gameObject == paddle) || 
                        collision.gameObject.GetComponent<PaddleMovement>() != null;

        if (esPaddle)
        {
            // Generamos una dirección aleatoria en X entre -1 (izquierda) y 1 (derecha)
            float xAleatorio = Random.Range(-1f, 1f);
            
            // Creamos un vector de dirección normalizado, siempre hacia arriba (y=1)
            Vector2 direccion = new Vector2(xAleatorio, 1f).normalized;

            // Aplicamos la velocidad en esa dirección aleatoria
            rb.linearVelocity = direccion * velocidadRebote;
        }
    }
}