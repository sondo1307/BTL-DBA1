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
    
    [ReadOnly] public MainCrudObjectBase CurrentMainCrud;
    [SerializeField] private MainCrudObjectBase[] _dgs;
    
    [SerializeField] private Toggle[] _toggles;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        for (var i = 0; i < _toggles.Length; i++)
        {
            var i1 = i;
            var toggle = _toggles[i1];
            toggle.onValueChanged.AddListener((isOn) =>
            {
                SetCurrent(false);
                CurrentMainCrud = _dgs[i1];
                CurrentMainCrud.Setup();
                SetCurrent(true);
            });
        }
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
        if (!MySQLManager.Instance.IsTableEmpty(SonConst.PrematchTable) && CurrentMainCrud.TableName == SonConst.TeamTable)
        {
            UIManager.Instance.ShowToast("Không thể thêm dữ liệu khi giải đấu đang diễn ra");
            return;
        }
        CurrentMainCrud.ShowAddDataGob();
    }

    public void OnUpdateBtnClick()
    {
        CurrentMainCrud.UpdateOneRow(CurrentMainCrud.DataGridUI);
    }

    public void OnDeleteBtnClick()
    {
        if (!MySQLManager.Instance.IsTableEmpty(SonConst.PrematchTable) && CurrentMainCrud.TableName == SonConst.TeamTable)
        {
            UIManager.Instance.ShowToast("Không thể xóa dữ liệu khi giải đấu đang diễn ra");
            return;
        }
        CurrentMainCrud.DeleteSelectedRow(CurrentMainCrud.DataGridUI);
    }

    [ContextMenu("Refresh Data")]
    public void RefreshData()
    {
        CurrentMainCrud.Setup();
    }
}