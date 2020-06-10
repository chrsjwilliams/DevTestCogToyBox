using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{
    public Flock flock{ get; private set; }
    public SpriteRenderer sr;
    private Collider2D _agentCollider;
    public Collider2D AgentCollider{ get { return _agentCollider; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Flock f)
    {
        flock = f;
        _agentCollider = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
