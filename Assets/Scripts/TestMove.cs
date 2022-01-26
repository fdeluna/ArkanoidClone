using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public LayerMask layer;
    List<Collider2D> _colliders = new List<Collider2D>();

    // Update is called once per frame
    void Update()
    {        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1, 1 << LayerMask.NameToLayer("Brick"));


        _colliders.ForEach(x => x.GetComponent<SpriteRenderer>().color = Color.white);

        for (int i = 0; i < colliders.Length; i++)
        {
            SpriteRenderer sprite = colliders[i].GetComponent<SpriteRenderer>();
            sprite.color = Color.red;               
            if (!_colliders.Contains(colliders[i]))
            {                
                _colliders.Add(colliders[i]);
            }
        }
    }
}
