using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar/cambiar escenas
using System.Collections;
using UnityEngine.UI; // Necesario para trabajar con componentes de UI como 'Image'
using TMPro; // Necesario para trabajar con TextMeshPro

public class GameManager : MonoBehaviour
{
    // 1. Singleton - Permite acceder a esta instancia desde cualquier script
    public static GameManager Instancia { get; private set; }

    [Header("Variables de Juego")]
    public int Puntuacion = 0;
    public int Vidas = 3;
    private int ladrillosRestantes; // Para saber cuándo ganar

    [Header("UI de Juego")]
    [Tooltip("Arrastra aquí el texto para mostrar la puntuación actual.")]
    public TextMeshProUGUI textoPuntuacion;
    [Tooltip("Arrastra aquí el texto para mostrar las vidas restantes.")]
    public TextMeshProUGUI textoVidas;
    [Tooltip("Arrastra aquí el objeto vacío que contiene la pantalla de Game Over.")]
    public GameObject pantallaGameOver;
    [Tooltip("Arrastra aquí el objeto vacío que contiene la pantalla de Ganador.")]
    public GameObject pantallaGanador;

    [Header("Prefabs")]
    [Tooltip("Arrastra aquí el Prefab de la bola.")]
    public GameObject ballPrefab;
    [Tooltip("Arrastra aquí el objeto Paddle de la escena.")]
    public GameObject paddle;
    [Tooltip("Arrastra aquí un objeto vacío (NO el Canvas) para organizar las bolas.")]
    public Transform contenedorBolas;

    [Header("Audio")]
    [Tooltip("Arrastra aquí el clip de audio para la música de fondo.")]
    public AudioClip musicaDeFondo;
    [Tooltip("Sonido que se reproduce al romper un ladrillo.")]
    public AudioClip sonidoLadrilloRoto;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    // ----------------------------------------------------
    
    // 2. Awake() se llama al cargar el script, antes de Start()
    private void Awake()
    {
        // Implementación del Singleton
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject); // Destruye si ya existe otra instancia
        }
        else
        {
            Instancia = this; // Se establece como la única instancia

            // Configurar fuentes de audio
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            if (musicaDeFondo != null)
            {
                musicSource.clip = musicaDeFondo;
                musicSource.loop = true; // Para que la música se repita
                musicSource.Play();
            }

            // Asegurarse de que el ScoreManager exista
            if (ScoreManager.Instancia == null) { /* No hacer nada, pero la referencia fuerza su creación si está bien configurado */ }
        }
    }

    private void Start()
    {
        CargarNivel();
    }

    // 3. Método para inicializar un nivel
    private void CargarNivel()
    {


        // Encontrar todos los ladrillos al inicio del nivel y reiniciar puntuación si es necesario
        ladrillosRestantes = FindObjectsByType<Brick>(FindObjectsSortMode.None).Length;
        Debug.Log("Ladrillos encontrados: " + ladrillosRestantes);


        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(false);
        }

        if (pantallaGanador != null)
        {
            pantallaGanador.SetActive(false);
        }

        ActualizarUI(); // Actualizamos la UI al cargar el nivel
        // Reiniciar la bola para el nuevo nivel
        StartCoroutine(ReiniciarBolaConRetraso(1f));
    }

    // 4. Método para instanciar la bola y prepararla para el lanzamiento
    public void ReiniciarBola()
    {
        StartCoroutine(ReiniciarBolaConRetraso(0.1f));
    }

    private IEnumerator ReiniciarBolaConRetraso(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Destruir cualquier bola existente antes de crear una nueva
        GameObject bolaExistente = GameObject.FindWithTag("Ball"); 
        if (bolaExistente != null)
        {
            Destroy(bolaExistente);
        }
        
        if (ballPrefab == null)
        {
            Debug.LogError("El prefab de la bola no está asignado en el GameManager.");
            yield break;
        }

        // Buscar el paddle si no está asignado para alinear la bola en el eje Z
        if (paddle == null)
        {
            PaddleMovement pm = FindObjectOfType<PaddleMovement>();
            if (pm != null) paddle = pm.gameObject;
        }

        // Usar la posición Z del paddle para que estén en el mismo plano
        Vector3 spawnPos = (paddle != null) ? new Vector3(0, 0, paddle.transform.position.z) : Vector3.zero;

        // Crear una nueva bola desde el prefab
        GameObject nuevaBola = Instantiate(ballPrefab, spawnPos, Quaternion.identity, contenedorBolas);

        // Pasar la referencia del paddle a la bola para el rebote
        Ball ballScript = nuevaBola.GetComponent<Ball>();
        if (ballScript != null)
        {
            ballScript.SetPaddle(paddle);
        }
    }

    // 5. Método llamado por los ladrillos al ser destruidos
    public void LadrilloDestruido(int puntosLadrillo)
    {
        // Reproducir sonido de ladrillo roto si está asignado
        if (sonidoLadrilloRoto != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(sonidoLadrilloRoto);
        }

        Puntuacion += puntosLadrillo;
        ActualizarUI(); // Actualizamos la UI para que se vea la nueva puntuación
        ladrillosRestantes--;


        // Comprobar si ya no quedan ladrillos (Condición de victoria)
        if (Puntuacion >= 9)         {
            Debug.Log("¡Ganaste! Puntuación final: " + Puntuacion);

            // Detener la bola
            GameObject bolaExistente = GameObject.FindWithTag("Ball");
            if (bolaExistente != null)
            {
                Destroy(bolaExistente);
            }

            // Activar la pantalla de ganador
            if (pantallaGanador != null)
            {
                pantallaGanador.SetActive(true);
            }

            // Registrar la puntuación final en el ScoreManager
            if (ScoreManager.Instancia != null)
            {
                ScoreManager.Instancia.RegistrarNuevaPuntuacion(Puntuacion);
            }
        }
    }

    // 6. Método llamado por la 'Zona de Muerte' (DeathZone)
    public void PerderVida()
    {
        Vidas--;
        ActualizarUI(); // Actualizamos la UI para mostrar los cambios

        if (Vidas <= 0)
        {
            // Condición de Game Over
            Debug.Log("Game Over. Puntuación: " + Puntuacion);
            
            if (pantallaGameOver != null)
            {
                pantallaGameOver.SetActive(true);
            }

            // Registra la puntuación final en el ScoreManager
            if (ScoreManager.Instancia != null)
            {
                ScoreManager.Instancia.RegistrarNuevaPuntuacion(Puntuacion);
            }
            // TODO: Cargar la escena de Game Over o reiniciar el juego.
        }
        else
        {
            // Si quedan vidas, se reinicia la bola
            ReiniciarBola(); 
        }
    }

    // Método para actualizar toda la UI de juego (puntos y vidas)
    private void ActualizarUI()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = "Puntuación: " + Puntuacion;
        }
        if (textoVidas != null)
        {
            textoVidas.text = "Vidas: " + Vidas;
        }
    }

    // Método para reiniciar el juego (conectar al botón Retry)
    public void ReiniciarJuego()
    {
        Puntuacion = 0;
        Vidas = 3;
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}