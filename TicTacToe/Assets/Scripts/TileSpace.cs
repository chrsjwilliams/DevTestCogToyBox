using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TileSpace : MonoBehaviour
{
    public enum TileSpaceState { EMPTY = 0, PLAYER1, PLAYER2 }

    public Vector2 coord;
    private int _touchID;

    public int tileNum { get; private set; }
    public Player occupyingPlayer;
    public TileSpaceState state { get; private set; }

    public SpriteRenderer occupyingIcon;

    public SpriteRenderer renderer;

    private TaskManager _tm = new TaskManager();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int xCoord, int yCoord, int tn)
    {
        coord = new Vector2(xCoord, yCoord);
        tileNum = tn;
        renderer = GetComponent<SpriteRenderer>();
        
        _touchID = -1;

        state = TileSpaceState.EMPTY;

        
        Services.EventManager.Register<GameEndEvent>(OnGameEnd);
    }

    public void EnableInput(bool enable)
    {
        if(enable)
        {
            Services.EventManager.Register<TouchDown>(OnTouchDown);
            Services.EventManager.Register<MouseDown>(OnMouseDown);
        }
        else
        {
            Services.EventManager.Unregister<TouchDown>(OnTouchDown);
            Services.EventManager.Unregister<MouseDown>(OnMouseDown);
        }
    }

    private void OnDestroy() 
    {
        Services.EventManager.Unregister<TouchDown>(OnTouchDown);
        Services.EventManager.Unregister<MouseDown>(OnMouseDown);
        Services.EventManager.Unregister<TouchUp>(OnTouchUp);
        Services.EventManager.Unregister<MouseUp>(OnMouseUp);
        Services.EventManager.Unregister<GameEndEvent>(OnGameEnd);

    }

    public void OnGameEnd(GameEndEvent e)
    {
        Services.EventManager.Unregister<TouchDown>(OnTouchDown);
        Services.EventManager.Unregister<MouseDown>(OnMouseDown);

    }

    private bool PointContainedInTile(Vector3 point)
    {
        Vector3 extents = renderer.bounds.extents;
        Vector3 centerPoint = renderer.transform.position;

        return  point.x >= centerPoint.x - extents.x && point.x <= centerPoint.x + extents.x &&
                point.y >= centerPoint.y - extents.y && point.y <= centerPoint.y + extents.y;
    }

    public void OnTouchDown(TouchDown e)
    {
        Vector3 touchWorldPos = 
                    Services.GameManager.MainCamera.ScreenToWorldPoint(
                                                        new Vector3( 
                                                                e.touch.position.x,
                                                                e.touch.position.y,
                                                                -Services.GameManager.MainCamera.transform.position.z));
        if(PointContainedInTile(touchWorldPos) && _touchID == -1)
        {
            OnInputDown();
        }
        
    }

    public void OnMouseDown(MouseDown e)
    {
        Vector3 mouseWorldPos = 
                    Services.GameManager.MainCamera.ScreenToWorldPoint(
                                                        new Vector3( 
                                                                e.mousePos.x,
                                                                e.mousePos.y,
                                                                -Services.GameManager.MainCamera.transform.position.z  ));
        if(PointContainedInTile(mouseWorldPos))
        {
            OnInputDown();
        }
    }

    private void OnInputDown()
    {

        // Check if occupying piece is null and provide user feedback

        if(occupyingPlayer == null)
        {
            Shockwave shockwave = Instantiate(Services.Prefabs.Shockwave, transform.position, Quaternion.identity);
            shockwave.Init(Services.GameScene.currentPlayer, transform.position);
            SetOccupyingPlayer(Services.GameScene.currentPlayer);
            Services.EventManager.Fire(new PlayMadeEvent(occupyingPlayer, this));
            float pitch = occupyingPlayer.playerNum == 0 ? 0.8f : 2f;
            Services.AudioManager.CreateTrackAndPlay(Clips.PLACE, pitch);

        }

        Services.EventManager.Register<TouchUp>(OnTouchUp);
        Services.EventManager.Register<MouseUp>(OnMouseUp);

        Services.EventManager.Unregister<TouchDown>(OnTouchDown);
        Services.EventManager.Unregister<MouseDown>(OnMouseDown);
    }

    public void OnTouchUp(TouchUp e)
    {
        if(e.touch.fingerId == _touchID)
        {
            OnInputUp();
            _touchID = -1;
        }   
    }

    public void OnMouseUp(MouseUp e)
    {
        OnInputUp();
    }

    private void OnInputUp()
    {
        
        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDown);

        Services.EventManager.Unregister<TouchUp>(OnTouchUp);
        Services.EventManager.Unregister<MouseUp>(OnMouseUp);
    }

    public void SetOccupyingPlayer(Player p){
        state = (TileSpaceState)p.playerNum;
        occupyingPlayer = p;
        occupyingIcon.color = p.playerColor[0];
        occupyingIcon.sprite = occupyingPlayer.PlayerIcon;
        occupyingIcon.transform.localScale = Vector3.zero;
        Task addIcon = new PiecePlacementAnimation(occupyingIcon);

        _tm.Do(addIcon);
    }

    // TODO: Animation on being selected


    // Update is called once per frame
    void Update()
    {
        _tm.Update();
    }
}
