using CharTween;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods
{
    public static void ClearChildrens(this Transform t)
    {
        //Array to hold all child obj
        //Find all child obj and store to that array
        var allChildren = t.Cast<Transform>().ToList();

        //Now destroy them
        foreach (var child in allChildren)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }

    public static void SinWaveFadeMove(this TextMeshProUGUI tmp, float amplitude, float time, float startFade = 0, float characterOffset = 1)
    {
        CharTweener tweener = tmp.GetCharTweener();
        var sequence = DOTween.Sequence();

        tmp.maxVisibleCharacters = 0;

        for (var i = 0; i <= tweener.CharacterCount; ++i)
        {
            var timeOffset = Mathf.Lerp(0, characterOffset, i / (float)(tweener.CharacterCount));
            var charSequence = DOTween.Sequence();

            charSequence.Join(tweener.DOFade(i, startFade, time * 0.25f).SetEase(Ease.Linear).From().SetDelay(timeOffset))
            .Join(tweener.DOLocalMoveY(i, amplitude, time * 0.25f).SetEase(Ease.Linear))
            .Append(tweener.DOLocalMoveY(i, -amplitude, time * 0.5f).SetEase(Ease.Linear))
            .Append(tweener.DOLocalMoveY(i, 0, time * 0.25f).SetEase(Ease.Linear));

            sequence.Insert(timeOffset, charSequence);
        }
        tmp.maxVisibleCharacters = tmp.text.Length;
    }

    public static void RandomFadeRotation(this TextMeshProUGUI tmp, int maxTurns, float time, float startFade = 0)
    {
        CharTweener tweener = tmp.GetCharTweener();

        tmp.maxVisibleCharacters = 0;
        for (var i = 0; i <= tweener.CharacterCount; ++i)
        {
            var timeOffset = Mathf.Lerp(0, 1, Random.Range(0, tweener.CharacterCount) / (float)(tweener.CharacterCount));
            var charSequence = DOTween.Sequence();
            int randomTurns = Random.Range(-maxTurns, maxTurns) + 1;
            charSequence.Join(tweener.DOFade(i, startFade, time * 0.5f).SetEase(Ease.Linear).From().SetDelay(timeOffset))
            .Join(tweener.DORotate(i, new Vector3(0, 360f * randomTurns, 0f), time, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuart));
        }
        tmp.maxVisibleCharacters = tmp.text.Length;
    }

    public static void FadeText(this TextMeshProUGUI tmp, float time, float startFade = 0)
    {        
        CharTweener tweener = tmp.GetCharTweener();

        for (var i = 0; i <= tweener.CharacterCount; ++i)
        {
            tweener.DOFade(i, startFade, time).SetEase(Ease.Linear).From();
        }
    }


    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }


    private static readonly System.Random Rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rng.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static IEnumerator IFadeInOut(this Image image, bool fadeIn, float time, Color color)
    {
        Color from = color;
        Color to = color;

        from.a = fadeIn ? 0 : 1;
        to.a = fadeIn ? 1 : 0;

        float progress = 0;
        while (progress < time)
        {
            progress += Time.unscaledDeltaTime;
            image.color = Color.Lerp(from, to, progress / time);
            yield return null;
        }
        image.color = to;
    }

}