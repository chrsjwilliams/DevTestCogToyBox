using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PiecePlacementAnimation : Task
{
    private float _timeElapsed;
    private const float _animDuration = 2f;
    private float _totalDuration;
    private SpriteRenderer _piece;


    public PiecePlacementAnimation(SpriteRenderer piece)
    {
        _piece = piece;
    }


    protected override void Init()
    {
        _timeElapsed = 0;

        _totalDuration = _animDuration;

    }

    internal override void Update()
    {
        _timeElapsed += Time.deltaTime;

        _piece.transform.localScale = Vector3.LerpUnclamped(_piece.transform.localScale, Vector3.one,
                                                            EasingEquations.Easing.ElasticEaseOut(_timeElapsed / 20));


        if (_timeElapsed >= _totalDuration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        _piece.transform.localScale = Vector3.one;
    }
}
