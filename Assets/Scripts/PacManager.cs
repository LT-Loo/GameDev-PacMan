using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManager : MonoBehaviour
{
    private Animator anim;
    public bool isDead;

    public Transform Target;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public float StartTime;
    public float Duration;

    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isDead", isDead);
    }
}
