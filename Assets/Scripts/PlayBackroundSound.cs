using UnityEngine;

public class PlayBackroundSound : MonoBehaviour
{
    private void Awake() { DontDestroyOnLoad(transform.gameObject); }
}
