using UnityEngine;
using UnityEngine.SceneManagement; // 👈 1. Necesitas esta librería para trabajar con escenas

public class SceneLoaderManager : MonoBehaviour
{
    // 2. Variable pública para asignar el índice de la escena desde el Inspector
    [Tooltip("El índice de la escena a cargar (ver en File > Build Settings).")]
    public int indiceEscenaACargar;

    // 3. Método público para que el botón pueda llamarlo
    public void CargarEscenaPorIndice()
    {
        // 4. Verifica si el índice de la escena es válido
        // SceneManager.sceneCountInBuildSettings devuelve el número de escenas en Build Settings
        if (indiceEscenaACargar >= 0 && indiceEscenaACargar < SceneManager.sceneCountInBuildSettings)
        {
            // 5. Llama al gestor de escenas para cargar la escena por su índice
            SceneManager.LoadScene(indiceEscenaACargar);
        }
        else
        {
            // Muestra un error si la escena no se encuentra
            Debug.LogError("El índice de escena '" + indiceEscenaACargar + "' no es válido o no está en la configuración de Build Settings.");
        }
    }
    
    // 6. Método para salir del juego (para el botón "Salir")
    public void SalirDelJuego()
    {
        // Esto solo funciona en una aplicación compilada (build)
        Application.Quit();

        // Para el editor, se usa esta línea (comentar en la versión final)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}