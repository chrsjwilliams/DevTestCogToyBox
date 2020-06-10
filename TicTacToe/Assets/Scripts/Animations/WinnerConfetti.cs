using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerConfetti : MonoBehaviour
{

    private bool _fadeOut;
    private float _duration = 1.25f;
    private float _fadeT;
    private ParticleSystem _ps;

    public void Init(Player player)
    {
        transform.position = new Vector3(1, 8, -4);
        _ps = GetComponent<ParticleSystem>();
        _ps.startColor = player.playerColor[0];
        _ps.textureSheetAnimation.SetSprite(0, player.PlayerIcon);

        _fadeOut = false;
        _fadeT = 0;
        Services.EventManager.Register<RefreshGameBaord>(OnRefreshGameBoard);
    }

    public void OnRefreshGameBoard(RefreshGameBaord e)
    {
        _fadeOut = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_fadeOut)
        {
            _fadeT += Time.deltaTime;
            _ps.startColor = Color.Lerp(_ps.startColor, new Color(1, 1, 1, 0), _fadeT * 0.75f);
            if(_fadeT > 2f)
            {
                Destroy(gameObject);

            }
        }
    }
}
