using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public UnityEvent OnPickUp;

    [SerializeField] private bool _isFinal = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isFinal && other.name == "Player")
        {
            OnPickUp.Invoke();
            Destroy(gameObject);
        }
        else if (other.name == "Player")
            LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}