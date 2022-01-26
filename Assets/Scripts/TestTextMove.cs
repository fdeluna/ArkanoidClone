using CharTween;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TestTextMove : MonoBehaviour
{
    public TextMeshProUGUI text;
    public bool debug = true;
    public CharTweener _tweener;

    public float X = 0;
    public float Y = 0;

    public float time = 2f;
    public float amplitude = 20f;
    
    private void Awake()
    {
        _tweener = text.GetCharTweener();

    }

    //// Update is called once per frame
    void Update()
    {
       if(debug)
        {
            text.SinWaveFadeMove(amplitude, time);
            debug = false;
        }
    }

    private void MoveText()
    {
        debug = false;
        var sequence = DOTween.Sequence();
        for (var i = 0; i <= _tweener.CharacterCount; ++i)
        {
            //var timeOffset = Mathf.Lerp(0, 1, Random.Range(0, _tweener.CharacterCount) / (float)(_tweener.CharacterCount));
            var charSequence = DOTween.Sequence();
            charSequence.Join(_tweener.DOMove(i,Vector3.zero, 0));
            //int randomTurns = Random.Range(-5, 5);
            //charSequence.Join(_tweener.DOFade(i, 0, time * 0.5f).SetEase(Ease.Linear).From().SetDelay(timeOffset))
            //.Join(_tweener.DORotate(i, new Vector3(0, 360f * randomTurns, 0f), time, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuart));
        }
    }
    //Vector2 spawnPosition = Vector3.zero;

}
