using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Composite")]
public class CompositeBehavior : FlockBehavior
{
    public FlockBehavior[] behaviors;
    public float[] behaviorWeights;

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if(behaviors.Length != behaviorWeights.Length) return Vector2.zero;

        Vector2 move = Vector2.zero;

        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector2 partialMove = behaviors[i].CalculateMove(agent, context, flock) * behaviorWeights[i];

            if(partialMove != Vector2.zero)
            {
                if(partialMove.sqrMagnitude > behaviorWeights[i] * behaviorWeights[i])
                {
                    partialMove.Normalize();
                    partialMove *= behaviorWeights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
