using UnityEngine;
using System.Collections;

public class DockingEffect : MonoBehaviour
{

    private static DockingEffect instance_;
    public static DockingEffect Instance
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
