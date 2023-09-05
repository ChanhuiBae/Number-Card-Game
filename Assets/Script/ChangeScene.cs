using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    public void ChangeStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
        
    public void ChangeGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("StartScene");
        SceneManager.LoadScene("GameScene");
    }

    public void ChangeExplanation1()
    {
        SceneManager.LoadScene("Explanation1");
    }

    public void ChangeExplanation2()
    {
        SceneManager.LoadScene("Explanation2");
    }

    public void ChangeExplanation3()
    {
        SceneManager.LoadScene("Explanation3");
    }

    public void ChangeExplanation4()
    {
        SceneManager.LoadScene("Explanation4");
    }

    public void ChangeExplanation5()
    {
        SceneManager.LoadScene("Explanation5");
    }

    public void ChangeCreatorScene()
    {
        SceneManager.LoadScene("CreatorScene");
    }
}
