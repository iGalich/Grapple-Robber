using UnityEngine;

// Put on objects like game manager so they don't get destroyed on load of new scene
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}