using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PacManager : MonoBehaviour
{
    // For background music
    public AudioSource bgmSource;
    
    // For animation
    private Animator anim;
    public bool isDead;

    // For movement
    private Vector3 startPos;
    private Vector3 endPos;
    private int target= 1;
    private float speed = 1.75f;
    private Vector3[] intersect = {
        new Vector3(1, -1, 0),
        new Vector3(6, -1, 0),
        new Vector3(6, -5, 0),
        new Vector3(1, -5, 0)
    };
   
    void Start()
    {
        anim = GetComponent<Animator>();
        startPos = intersect[0];
        endPos = intersect[1];
    }

    void Update()
    {
        anim.SetBool("isDead", isDead);

        if (!isDead && bgmSource.clip.name != "Intro") {PacMovement();}

    }

    // Move PacPumpkin clockwise around top-left inner block
    void PacMovement() {
        Vector3 direction = (endPos - startPos).normalized; // Get moving direction
        
        transform.position += direction * speed * Time.deltaTime; // Run movement

        // When reach destination
        if (Vector3.Distance(transform.position, endPos) <= 0.02f) {
            transform.position = endPos;

            // Set next intersection point as new destination
            startPos = intersect[target++];
            if (target > 3) {target = 0;}
            endPos = intersect[target];

            transform.Rotate(0f, 0f, -90f); // Rotate PacPumpkin
        }
    }
}
