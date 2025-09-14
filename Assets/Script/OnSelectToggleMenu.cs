using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnSelectToggleMenu : MonoBehaviour
{
    [SerializeField] private List<Toggle> _toggles; // Kéo các Toggle vào trong Inspector
    [SerializeField] private List<GameObject> _mainObject; // Kéo các GameObject tương ứng vào trong Inspector

    private void Awake() {
        // Khởi tạo tất cả GameObject là không hoạt động
        foreach (var mo in _mainObject)
        {
            if (mo)
            {
                mo.SetActive(false);
            }
        }
    }

    void Start()
    {
        for (int i = 0; i < _toggles.Count; i++)
        {
            int index = i; // Quan trọng: copy biến để tránh lỗi closure
            _toggles[i].onValueChanged.AddListener((bool isOn) =>
            {
                foreach (var mo in _mainObject)
                {
                    if (mo)
                    {
                        mo.SetActive(false); // Tắt tất cả GameObject
                    }
                }
                _mainObject[index].SetActive(isOn);
            });
        }
        
        string csv = "id,name,age\n1,John,25\n2,Mary,30\n3,Tom,28";
        List<string> scores = new List<string> { "80", "90", "85" };

        string newCsv = CSVDataHelper.AddNewColumnToCsv(csv, "score", scores);

        Debug.Log(newCsv);
    }
}
