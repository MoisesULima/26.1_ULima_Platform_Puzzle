using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMusic(MusicTrack.Level);
    }

    void Destroy()
    {
        AudioManager.Instance.StopMusic();
    }
}
