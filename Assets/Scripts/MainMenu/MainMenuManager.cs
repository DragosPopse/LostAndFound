using System.Collections;
using UnityEngine;

public sealed class MainMenuManager : utility.Singleton<MainMenuManager>
{
    [SerializeField] 
    private int _panelPartialAlpha;

    [SerializeField]
    private float[] _durations;

    [SerializeField]
    private AnimationCurve[] _curves;

    private MainMenuFence _fence;
    private TitleText _titleText;
    private BlinkPanel _panel;




    private void Start()
    {
        GameManager.Instance.GameState = GameManager.State.Menu;

        _fence = MainMenuFence.Instance;
        _titleText = TitleText.Instance;
        _panel = BlinkPanel.Instance;
        
        StartCoroutine(Entry());
    }


    private IEnumerator Entry()
    {
        yield return Entry_EmptyRoom();
        yield return Entry_PeopleFar();
        yield return Entry_PeopleFull();
    }


    private IEnumerator Entry_EmptyRoom()
    {
        yield break;
    }


    private IEnumerator Entry_PeopleFar()
    {
        yield break;
    }


    private IEnumerator Entry_PeopleFull()
    {
        yield break;
    }


    public void ButtonDown_OpenCounter()
    {
        _fence.PlayLiftAnimation();
        _titleText.PlayAnimation();
        GameManager.Instance.GameState = GameManager.State.Game;
    }
}
