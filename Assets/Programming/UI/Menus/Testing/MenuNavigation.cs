using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    private void Update() {
        _Input();
    }

    private void _Input () {
        if (Input.GetButtonDown("A")) {
            Play();
        }
        if (Input.GetButtonDown("B")) {
            Quit();
        }
    }

    public void Play () {
        SceneManager.LoadScene(1);
    }
    public void Quit () {
        Application.Quit();
    }
}
