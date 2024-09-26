using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GhostAnimPreview : MonoBehaviour
{
    public Animator[] ghostDirs;
    public Animator ghostStateDemo;
    public TextMeshPro currentState;
    private float timer;
    private float checkpoint;
    // Start is called before the first frame update
    void Start()
    {
        checkpoint = 0.0f;

        for (int i = 0; i < ghostDirs.Length; i++) {
            ghostDirs[i].SetInteger("Direction", i);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer -  checkpoint >= 3.0f) {
            checkpoint = timer;
            ghostStateDemo.SetBool("NextState", true);
        } else {
            ghostStateDemo.SetBool("NextState", false);
        }

        if (ghostStateDemo.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Scared")) {
            currentState.SetText("Scared State");
        } else if (ghostStateDemo.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Recovering")) {
            currentState.SetText("Recovering State");
        } else if (ghostStateDemo.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Dead")) {
            currentState.SetText("Dead State");
        } else {currentState.SetText("Normal State");}

        timer += Time.deltaTime;
    }
}
