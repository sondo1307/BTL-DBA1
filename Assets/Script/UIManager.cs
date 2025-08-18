using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class SonCache
{
    public static WaitForSeconds WaitSeconds = new WaitForSeconds(1);
}

public static class UIHelper
{
    public static void ShowCg(this CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
        cg.gameObject.SetActive(true);
    }
    
    public static void HideCg(this CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        cg.gameObject.SetActive(false);
    }
}

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
