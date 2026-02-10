using UnityEngine;
using TMPro; //  1. Necesitas esta librería para trabajar con TextMeshPro
using System.Collections.Generic; // Para usar Listas
using System.Linq; // Para ordenar la lista de puntuaciones

// 2. Hereda de MonoBehaviour para poder añadirlo a un objeto en la escena
public class ScoreManager : MonoBehaviour
{
    // 3. Singleton para acceder fácilmente a esta instancia desde otros scripts
    public static ScoreManager Instancia { get; private set; }

    [Header("UI de Puntuaciones Altas")]
    [Tooltip("Arrastra aquí el objeto de texto para la puntuación más alta (Score #1).")]
    public TextMeshProUGUI highScore1Text;

    [Tooltip("Arrastra aquí el objeto de texto para la segunda puntuación más alta (Score #2).")]
    public TextMeshProUGUI highScore2Text;

    [Tooltip("Arrastra aquí el objeto de texto para la tercera puntuación más alta (Score #3).")]
    public TextMeshProUGUI highScore3Text;

    // 4. Claves para guardar las puntuaciones en el dispositivo (PlayerPrefs)
    private const string HighScore1Key = "HighScore1";
    private const string HighScore2Key = "HighScore2";
    private const string HighScore3Key = "HighScore3";

    private void Awake()
    {
        // Implementación del Singleton
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); // Opcional: si quieres que persista entre escenas
        }
    }

    private void Start()
    {
        // Al iniciar, carga y muestra las puntuaciones guardadas
        ActualizarVisualizacionPuntuaciones();
    }

    public void RegistrarNuevaPuntuacion(int nuevaPuntuacion)
    {
        // Carga las puntuaciones actuales
        int score1 = PlayerPrefs.GetInt(HighScore1Key, 0);
        int score2 = PlayerPrefs.GetInt(HighScore2Key, 0);
        int score3 = PlayerPrefs.GetInt(HighScore3Key, 0);

        // Crea una lista con las puntuaciones actuales y la nueva
        List<int> puntuaciones = new List<int> { score1, score2, score3, nuevaPuntuacion };

        // Ordena la lista de mayor a menor y toma los 3 primeros
        List<int> top3 = puntuaciones.OrderByDescending(p => p).Take(3).ToList();

        // Guarda las nuevas puntuaciones más altas
        PlayerPrefs.SetInt(HighScore1Key, top3[0]);
        PlayerPrefs.SetInt(HighScore2Key, top3[1]);
        PlayerPrefs.SetInt(HighScore3Key, top3[2]);
        PlayerPrefs.Save(); // Guarda los cambios en el disco

        // Actualiza la UI
        ActualizarVisualizacionPuntuaciones();
    }

   
    private void ActualizarVisualizacionPuntuaciones()
    {
        if (highScore1Text != null)
            highScore1Text.text = $"1. {PlayerPrefs.GetInt(HighScore1Key, 0):D6}";

        if (highScore2Text != null)
            highScore2Text.text = $"2. {PlayerPrefs.GetInt(HighScore2Key, 0):D6}";

        if (highScore3Text != null)
            highScore3Text.text = $"3. {PlayerPrefs.GetInt(HighScore3Key, 0):D6}";
    }
}
    

