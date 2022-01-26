using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBounce : MonoBehaviour
{

    public float delay = 3;
    public GameObject target;
    public Image image;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Bounce", delay);
    }



    public void Bounce()
    {
        transform.DOMove(target.transform.position, 0.5f).SetEase(Ease.OutBounce);
    }
}
