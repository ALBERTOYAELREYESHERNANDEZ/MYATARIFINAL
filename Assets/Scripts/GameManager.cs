using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar/cambiar escenas
using System.Collections;
using UnityEngine.UI; // Necesario para trabajar con componentes de UI como 'Image'

public class GameManager : MonoBehaviour
{
    // 1. Singleton - Permite acceder a esta instancia desde cualquier script
    public static GameManager Instancia { get; private set; }

    [Header("Variables de Juego")]
    public int Puntuacion = 0;
    public int Vidas = 3;
    private int ladrillosRestantes; // Para saber cuándo ganar

    [Header("Prefabs")]
    [Tooltip("Arrastra aquí el Prefab de la bola.")]
    public GameObject ballPrefab;

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
            transform.SetParent(null); // Asegura que el GameManager sea un objeto raíz
            DontDestroyOnLoad(gameObject); // Evita que el GameManager se destruya al cargar otra escena

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

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        // Buena práctica para evitar memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Aquí puedes añadir lógica para diferenciar entre menú y niveles de juego
        CargarNivel();
    }

    // 3. Método para inicializar un nivel
    private void CargarNivel()
    {
        // Encontrar todos los ladrillos al inicio del nivel y reiniciar puntuación si es necesario
        ladrillosRestantes = FindObjectsByType<Brick>(FindObjectsSortMode.None).Length;
        Debug.Log("Ladrillos encontrados: " + ladrillosRestantes);
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

        // Crear una nueva bola desde el prefab
        Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
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
        ladrillosRestantes--;
        
        // Comprobar si ya no quedan ladrillos (Condición de victoria)
        if (ladrillosRestantes <= 0 && FindObjectsByType<Brick>(FindObjectsSortMode.None).Length <= 0)
        {
            // TODO: Lógica de ganar nivel/juego
            Debug.Log("¡Ganaste! Puntuación final: " + Puntuacion);
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Carga el siguiente nivel
        }
    }

    // 6. Método llamado por la 'Zona de Muerte' (DeathZone)
    public void PerderVida()
    {
        Vidas--;
        Puntuacion = 0; // Reiniciamos la puntuación a 0
        // TODO: Actualizar el texto de la UI para que muestre la nueva puntuación
        Debug.Log("Puntuación reiniciada. Vidas restantes: " + Vidas);

        if (Vidas <= 0)
        {
            // Condición de Game Over
            Debug.Log("Game Over. Puntuación: " + Puntuacion);
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
}