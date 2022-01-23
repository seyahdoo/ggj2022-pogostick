using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsExploder : MonoBehaviour
{
    public Collider2D[] visuals;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }
          }

    public void Explode()
    {
        GetComponent<Pogo>().enabled = false;
        foreach (Collider2D t in visuals)
        {
            t.enabled = true;
            t.transform.parent = null;
            t.gameObject.AddComponent<Rigidbody2D>();
        }
    }
}
