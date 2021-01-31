using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ReloadLevel());
    }

    private IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
