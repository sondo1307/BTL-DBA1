using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class VongDauDetailClass : MonoBehaviour
{
    [SerializeField] private EachVongDauDgBase[] _dgs;
    public EachVongDauDgBase[] Dgs => _dgs;
    [SerializeField] private Toggle[] _toggles;
    [SerializeField] private Button _closeBtn;

    [Header("Update")] [SerializeField] [ReadOnly]
    private int _currentOpenTournamentRound;


    private void Start()
    {
        _closeBtn.onClick.AddListener(Close);

        for (var i = 0; i < _toggles.Length; i++)
        {
            var i1 = i;
            var toggle = _toggles[i1];
            toggle.onValueChanged.AddListener((isOn) =>
            {
                _dgs[i1].Open(_currentOpenTournamentRound);
                _dgs[i1].gameObject.SetActive(isOn);
            });
        }
    }

    public void Open(int tournamentRound)
    {
        _currentOpenTournamentRound = tournamentRound;
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}