using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D()
    {
        Debug.Log("Quitting"); //simple check here since application doesn't quit in editor
        Application.Quit();
    }
}