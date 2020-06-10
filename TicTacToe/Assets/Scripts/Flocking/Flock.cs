using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public enum FlockIcon {X = 0, CIRCLE}
    [SerializeField] private FlockIcon _icon;
    public FlockIcon Icon{
        get { return _icon; }
    }
    public FlockAgent agentPrefab;
    private List<FlockAgent> _agents = new List<FlockAgent>();

    public FlockBehavior behavior;

    [Range(10, 300)]
    public int startingCount = 100;
    private const float AGENT_DENSITY = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10;
    [Range(1f, 100f)]
    public float maxSpeed = 5;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.3f;

    private float _sqrMaxSpeed;
    private float _sqrNeighborRadius;
    private float _sqrAvoidanceRadius;
    public float SqaureAvoidanceRadius{get { return _sqrAvoidanceRadius; } }

    private Color _transparent = new Color(1, 1, 1, 0);
    private TaskManager _tm = new TaskManager();

    // Start is called before the first frame update
    void Start()
    {
        _sqrMaxSpeed = maxSpeed * maxSpeed;
        _sqrNeighborRadius = neighborRadius * neighborRadius;
        _sqrAvoidanceRadius = _sqrNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(agentPrefab,
                                                Random.insideUnitCircle * startingCount * AGENT_DENSITY,
                                                Quaternion.Euler(Vector3.forward * Random.Range(0, 360)),
                                                transform);
            newAgent.Init(this);
            if(_icon == FlockIcon.CIRCLE)
            {
                newAgent.sr.sprite = Services.GameManager.AvailableIcons[1];
            }
            newAgent.sr.color = _transparent;

            newAgent.name = "Agent " + i;
            _agents.Add(newAgent);
        }
    }

    private List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach(Collider2D col in contextColliders)
        {
            if(col != agent.AgentCollider)
            {
                context.Add(col.transform);
            }
        }
        return context;
    }

    public void FadeInFlockAgents()
    {
        TaskTree fadeAgents = new TaskTree(new EmptyTask());

        Color targetColor;
        if(_icon == FlockIcon.X)
        {
            targetColor = Services.GameManager.Player1Color[1];
        }
        else
        {
            targetColor = Services.GameManager.Player2Color[1];
        }

        foreach(FlockAgent agent in _agents)
        {
            Task fadeOut = new LERPColor(agent.sr, _transparent,targetColor, 0.3f);
            fadeAgents.AddChild(fadeOut);
        }
        _tm.Do(fadeAgents);
    }

    public void FadeOutFlockAgents()
    {
        TaskTree fadeAgents = new TaskTree(new EmptyTask());

        foreach(FlockAgent agent in _agents)
        {
            Task fadeOut = new LERPColor(agent.sr, agent.sr.color, _transparent, 0.3f);
            fadeAgents.AddChild(fadeOut);
        }
        _tm.Do(fadeAgents);
    }


    // Update is called once per frame
    void Update()
    {

        _tm.Update();
        foreach(FlockAgent agent in _agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if(move.sqrMagnitude > _sqrMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }
}
