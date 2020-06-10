using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private KeyCode RESTART_GAME = KeyCode.Backspace;

    private const string RELOAD_GAME = "_Main";

    public const int LEFT_CLICK = 0;
    public const int RIGHT_CLICK = 1;

    [SerializeField] private int _totalPlayers;
    public int TotalPlayers
    {
        get { return _totalPlayers; }

    }

    [SerializeField] private int _numPlayers;
    public int NumPlayers
    {
        get { return _numPlayers; }
        private set
        {
            if (_numPlayers <= 0)
            {
                _numPlayers = 1;
            }
            else
            {
                _numPlayers = value;
            }
        }
    }

    [SerializeField] private Color[] _player1Color;
    public Color[] Player1Color{
        get { return _player1Color; }
    }

    [SerializeField] private Color[] _player2Color;
    public Color[] Player2Color{
        get { return _player2Color; }
    }


    public float duration { get; private set; }


    [SerializeField] private Camera _mainCamera;
    public Camera MainCamera
    {
        get { return _mainCamera; }
    }

    [SerializeField] private Sprite[] _availableIcons;
    public Sprite[] AvailableIcons
    {
        get { return _availableIcons; }
    }

    public Sprite[] selectedIcons;

    [SerializeField] private Sprite[] _audioIcons;
    private enum AudioIcon { PLAY = 0, MUTE}

    public void Init()
    {
        NumPlayers = 1;
        _mainCamera = Camera.main;
        Services.EventManager.Register<KeyPressedEvent>(OnKeyPressed); 
    }

    private void OnKeyPressed(KeyPressedEvent e)
    {
        if (e.key == RESTART_GAME) ReloadGame();
    }

    public void ShowInstructions()
    {
        Services.Scenes.PushScene<InstructionSceneScript>();
    }

    public void PopScene()
    {
        Services.Scenes.PopScene();
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(RELOAD_GAME);
    }

    public void RefreshButton()
    {
        Debug.Assert(Services.Scenes.CurrentScene is GameSceneScript);

        RefreshGame();
    }

    public void RefreshGame()
    {
        Services.Scenes.PopScene();
        Services.Scenes.PushScene<GameSceneScript>();    
    }

	// Use this for initialization
	public void Init (int players)
    {
        NumPlayers = players;
        _mainCamera = Camera.main;
	}
	

    public void UpdateMutIcon(Image audioStatusIcon)
    {
        if(Services.AudioManager.muted)
        {
            audioStatusIcon.sprite = _audioIcons[(int)AudioIcon.MUTE];
        }
        else
        {
            audioStatusIcon.sprite = _audioIcons[(int)AudioIcon.PLAY];
        }
    }
    public void ToggleMute(Image audioStatusIcon){
        Services.AudioManager.ToggleMute();
        UpdateMutIcon(audioStatusIcon);

    }

    public void ChangeCameraTo(Camera camera)
    {
        _mainCamera = camera;
    }

    public void SetDuration(float t) { duration = t; }

    // Update is called once per frame
    void Update()
    {
        Services.InputManager.Update();
    }

    public static Color float_array_to_Color(float[] arr)
    {
        Debug.Assert(arr.Length == 3);

        return new Color(arr[0], arr[1], arr[2]);
    }

    public static float[] color_to_float_array(Color color)
    {
        return new float[] { color.r, color.g, color.b };
    }

    private Color vector3_to_color(Vector3 vec)
    {
        return new Color(vec.x, vec.y, vec.z);
    }

    private Vector3 color_to_vector3(Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }
}
