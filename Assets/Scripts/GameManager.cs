using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using utility;
using Random = System.Random;

public sealed class GameManager : Singleton<GameManager>
{
    public Random Random => 
        _random;

    [SerializeField] private int _lives = 5;
    [SerializeField] private Text _livesText = null;

    [SerializeField] private Text _scoreText = null;

    [SerializeField, Tooltip("Leave empty to generate random seed")] 
    private string _seed = "";

    private Random _random = null;
    private int _currentLives = 0;

    public static int currentScore = 0;

    public State GameState
    {
        get => _state;
        set
        {
            bool game = value == State.Game;
            _livesText.gameObject.SetActive(game);
            _scoreText.gameObject.SetActive(game);

            UpdateHealthText();
            UpdateScoreText();

            _state = value;
        }
    }


    public enum State
    {
        Game,
        Menu
    }


    State _state = State.Menu;

    protected override void Awake()
    {
        base.Awake();
        var chosenSeed = _seed == "" ? 
            System.DateTime.Now.GetHashCode() : _seed.GetHashCode();
        _random = new Random(chosenSeed);

        _currentLives = _lives;
        currentScore = 0;
    }

    private void UpdateHealthText()
    {
        _livesText.text = $"Lives: {_currentLives}";
    }

    private void UpdateScoreText()
    {
        _scoreText.text = $"Score: {currentScore}";
    }

    public void OnDamageTaken()
    {
        _currentLives--;
        UpdateHealthText();

        if (_currentLives == 0) 
            OnGameOver();
    }

    public void OnCustomerHelped()
    {
        currentScore++;
        UpdateScoreText();
    }

    private void OnGameOver()
    {
        // Do Whatever.
        SceneManager.LoadScene(1);
    }
}
