using System;
using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class TranDauDetailClass : MonoBehaviour
{
    public GameObject TranDauDetail;
    [SerializeField] private EachTranDauDgObjectBase[] _dgs;
    public EachTranDauDgObjectBase[] Dgs => _dgs;
    [SerializeField] private Toggle[] _toggles;
    [SerializeField][ReadOnly] private int _matchID;
    [SerializeField] private Button _closeBtn;
    
    
    private void Start()
    {
        _closeBtn.onClick.AddListener(Close);
        
        for (var i = 0; i < _toggles.Length; i++)
        {
            var i1 = i;
            var toggle = _toggles[i1];
            toggle.onValueChanged.AddListener((isOn) =>
            {
                _dgs[i1].Open(_matchID);
                _dgs[i1].gameObject.SetActive(isOn);
            });
        }
    }

    public void Open(int matchID)
    {
        _matchID = matchID;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Main_SeasonDetail.Instance.LoadGiaiDau();
    }
}