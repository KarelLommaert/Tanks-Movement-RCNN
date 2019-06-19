using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get { return _instance; } }
    private static SceneLoader _instance = null;

    public GameObject SavingDataTextPrefab;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject.FindObjectOfType<PictorialRepresentationSaver>().SaveAllStoredSequences(false);
            }
        }
    }

    //private IEnumerator OpenLoadingScreenAndSave()
    //{
    //    Instantiate(SavingDataTextPrefab);
    //    yield return null;
    //    GameObject.FindObjectOfType<PictorialRepresentationSaver>().SaveAllStoredSequences();
    //    SceneManager.LoadScene(0);
    //}
}