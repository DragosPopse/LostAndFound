using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTextAnimation : MonoBehaviour
{
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.simulated = false;
    }

   
    
}
