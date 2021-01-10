using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]private Animator anim;

    public void ChangeScene(string sceneName)
    {
        GameManager.Instance.OnSaveEvent();
        UIManager.Instance.loadingScreen.GetComponent<Canvas>().sortingOrder = 2;
        StartCoroutine(NextScene(sceneName));
    }

    IEnumerator NextScene(string sceneName)
    {
        anim.SetTrigger("exit");
        Debug.Log(sceneName + "played");
        Time.timeScale = 1;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }
}
