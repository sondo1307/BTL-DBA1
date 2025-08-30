using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainCrud : MonoBehaviour
{
    public static MainCrud Instance;
    
    [FormerlySerializedAs("CauThu")] public MainCrudPlayer player;
    [FormerlySerializedAs("DoiBong")] public MainCrudTeam team;
    [FormerlySerializedAs("TrongTai")] public MainCrudReferee referee;
    [FormerlySerializedAs("Stadium")] public MainCrudStadium stadium;
    [ReadOnly] public MainCrudObjectBase CurrentMainCrud;

    [FormerlySerializedAs("_cauThuToggle")] [SerializeField]
    private Toggle _playerToggle;

    [FormerlySerializedAs("_doiBongToggle")] [SerializeField]
    private Toggle _teamToggle;

    [FormerlySerializedAs("_trongTaiToggle")] [SerializeField]
    private Toggle _refToggle;

    [SerializeField] private Toggle _stadiumToggle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _playerToggle.onValueChanged.AddListener((isOn) =>
        {
            SetCurrent(false);
            CurrentMainCrud = player;
            CurrentMainCrud.Setup(isOn);
            SetCurrent(true);
        });
        _teamToggle.onValueChanged.AddListener((isOn) =>
        {
            SetCurrent(false);
            CurrentMainCrud = team;
            CurrentMainCrud.Setup(isOn);
            SetCurrent(true);
        });
        _refToggle.onValueChanged.AddListener((isOn) =>
        {
            SetCurrent(false);
            CurrentMainCrud = referee;
            CurrentMainCrud.Setup(isOn);
            SetCurrent(true);
        });
        _stadiumToggle.onValueChanged.AddListener((isOn) =>
        {
            SetCurrent(false);
            CurrentMainCrud = stadium;
            CurrentMainCrud.Setup(isOn);
            SetCurrent(true);
        });
    }

    private void SetCurrent(bool setState)
    {
        if (CurrentMainCrud)
        {
            CurrentMainCrud.gameObject.SetActive(setState);
        }
    }

    public void OnAddBtnClick()
    {
        CurrentMainCrud.ShowAddDataGob();
    }

    public void OnUpdateBtnClick()
    {
        CurrentMainCrud.UpdateOneRow(CurrentMainCrud.DataGridUI);
    }

    public void OnDeleteBtnClick()
    {
        CurrentMainCrud.DeleteSelectedRow(CurrentMainCrud.DataGridUI);
    }

    [ContextMenu("Refresh Data")]
    public void RefreshData()
    {
        CurrentMainCrud.Setup(true);
    }
}