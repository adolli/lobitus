using UnityEngine;

public class RestartGameEffect : MonoBehaviour
{

    private static RestartGameEffect instance_;
    public static RestartGameEffect Instance
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
