using UnityEngine;
using DG.Tweening;

public class TeamNameBox : MonoBehaviour
{
    public GameObject blackTp;
    public GameObject blackBt;
    public Transform blackTop;
    public Transform blackBottom;

    void Start()
    {
        DG.Tweening.DOTween.Init();
        //move y dotween
        blackTop.transform.DOMoveY(1300f, 2f).SetEase(Ease.OutExpo);
        blackBottom.transform.DOMoveY(-200f, 2f).SetEase(Ease.OutExpo);

        // Update is called once per frame
        void Update()
        {

        }

    }

    public void close()
    {
        blackTop.transform.DOMoveY(700f, 2f).SetEase(Ease.OutExpo);
        blackBottom.transform.DOMoveY(300f, 2f).SetEase(Ease.OutExpo);
    }
}
