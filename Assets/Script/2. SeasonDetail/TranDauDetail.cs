using System;
using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TranDauDetailClass : MonoBehaviour
{
    [SerializeField] private EachTranDauDgObjectBase[] _dgs;
    public EachTranDauDgObjectBase[] Dgs => _dgs;
    [SerializeField] private Toggle[] _toggles;
    [SerializeField] private Button _closeBtn;
    
    [Header("Update")]
    [FormerlySerializedAs("_matchID")] [SerializeField][ReadOnly] private int _currentOpenMatchID;
    private int _tournamentRound;
    [SerializeField][ReadOnly] private bool _allowEdit;
    public bool AllowEdit => _allowEdit;
    
    public Action<int> EventUpdateMatchID;
    
    private void Start()
    {
        _closeBtn.onClick.AddListener(Close);
        
        for (var i = 0; i < _toggles.Length; i++)
        {
            var i1 = i;
            var toggle = _toggles[i1];
            toggle.onValueChanged.AddListener((isOn) =>
            {
                _dgs[i1].Open(_currentOpenMatchID);
                _dgs[i1].gameObject.SetActive(isOn);
            });
        }
    }

    public void Open(int matchID, int tournamentRound, bool allowEdit)
    {
        _currentOpenMatchID = matchID;
        gameObject.SetActive(true);
        _allowEdit = allowEdit;
        _tournamentRound = tournamentRound;
    }

    private void Close()
    {
        gameObject.SetActive(false);
        EventUpdateMatchID?.Invoke(_currentOpenMatchID);
    }
}