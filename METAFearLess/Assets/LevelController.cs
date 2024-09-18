using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelController : MonoBehaviour
{
    // URL del Gist que contiene el JSON
    private string jsonUrl = "https://gist.githubusercontent.com/Sebasurb01/1217423ad5ae40c9f51005ab1b1ab4f7/raw/level_data.json";

    private void Awake()
    {
        // Asegúrate de que solo haya un LevelController en la escena
        if (FindObjectsOfType<LevelController>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(UpdateLevelPeriodically());
    }

    IEnumerator UpdateLevelPeriodically()
    {
        while (true) // Ciclo infinito
        {
            yield return StartCoroutine(GetLevelFromJson());
            yield return new WaitForSeconds(30); // Espera 30 segundos antes de volver a consultar
        }
    }

    IEnumerator GetLevelFromJson()
    {
        UnityWebRequest www = UnityWebRequest.Get(jsonUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener el nivel: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonResponse);
            LoadLevel(levelData.current_level);
        }
    }

    void LoadLevel(string levelName)
    {
        Debug.Log("Cargando nivel: " + levelName);
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("Nivel no encontrado: " + levelName);
        }
    }
}

// Clase para deserializar el JSON
[System.Serializable]
public class LevelData
{
    public string current_level;
}
