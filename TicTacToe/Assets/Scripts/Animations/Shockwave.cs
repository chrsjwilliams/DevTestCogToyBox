using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    private float _duration = 1.25f;
    private float _t;
    private float _fadeT;
    private ParticleSystem _ps;

    public void Init(Player player, Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, pos.z);
        _ps = GetComponent<ParticleSystem>();
        _ps.startColor = player.playerColor[0];

        _t = 0;
        _fadeT = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _t += Time.deltaTime;
        if(_t > _duration)
        {
            _fadeT += Time.deltaTime;
            _ps.startColor = Color.Lerp(_ps.startColor, new Color(1, 1, 1, 0), _fadeT);
            if(_fadeT > 1f)
            {
                Destroy(gameObject);

            }
        }
    }
}
