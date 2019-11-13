using UnityEngine;

public class LevelClearEffect : MonoBehaviour
{

    private static LevelClearEffect instance_;
    public static LevelClearEffect Instance
    {
        get { return instance_; }
    }

    private AudioSource source_;

    void Start()
    {
        instance_ = this;
        source_ = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
    }

    public void Play()
    {
        source_.Play();
    }
}
