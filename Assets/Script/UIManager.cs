using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private CanvasGroup _circleCg;
    [SerializeField] private CanvasGroup _toastCg;
    [SerializeField] private TMP_Text _toastTxt;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPermantCircle()
    {
        _circleCg.ShowCg();
    }
    
    public void ShowCircle()
    {
        _circleCg.ShowCg();
        StartCoroutine(Delay());
        return;
        
        IEnumerator Delay()
        {
            yield return SonCache.WaitSeconds;
            _circleCg.HideCg();
        }
    }
    
    public void HideCircle()
    {
        _circleCg.HideCg();
    }
    
    public void ShowToast(string message)
    {
        _toastCg.ShowCg();
        StartCoroutine(Delay());
        _toastTxt.text = message;
        return;
        
        IEnumerator Delay()
        {
            yield return SonCache.WaitSeconds;
            _toastCg.HideCg();
        }
    }
}
