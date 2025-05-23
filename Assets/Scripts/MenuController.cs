using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void FirstTutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

   // public void Practice1()
   // {
      //  SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    //}

}
