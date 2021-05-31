using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class quickmatch : MonoBehaviour
{
    public void StartMatch()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
