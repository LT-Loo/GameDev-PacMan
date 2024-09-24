using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip ghostNormalBGM;
    public AudioClip ghostScaredBGM;
    public AudioClip ghostDeathBGM;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && audioSource.clip.name == intro.name) {
            audioSource.clip = ghostNormalBGM;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
