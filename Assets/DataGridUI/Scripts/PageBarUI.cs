using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class PageBarUI : MonoBehaviour
    {
        public DataGridTheme theme;
        public DataGridUI dataGridUI;

        [Header("Controls")]
        public PageBarButtonUI firstPageUI;
        public PageBarButtonUI prevPageUI;
        public PageBarButtonUI nextPageUI;
        public PageBarButtonUI lastPageUI;

        public RectTransform border;

        public Text textCurrent;
        public Text textTotal;

        public InputField inputPage;

        bool isFirst = true;

        // Start is called before the first frame update
        void Start()
        {
            UpdateTheme();
            UpdateTextInfo();
        }

        // Update is called once per frame
        void Update()
        {
            if (dataGridUI == null) return;
            if (isFirst == false) return;


            if (dataGridUI.GetCurrentLastIndex() > 0)
            {
                UpdateTextInfo();
                isFirst = false;
            }
        }

        void UpdateTheme()
        {
            ChangePageBarButtonTheme(firstPageUI);
            ChangePageBarButtonTheme(prevPageUI);
            ChangePageBarButtonTheme(nextPageUI);
            ChangePageBarButtonTheme(lastPageUI);

            textCurrent.color = theme.pageBarTheme.textColor;
            textCurrent.fontSize = theme.pageBarTheme.textFontSize;
            textTotal.color = theme.pageBarTheme.textColor;
            textTotal.fontSize= theme.pageBarTheme.textFontSize;
            inputPage.textComponent.fontSize = theme.pageBarTheme.textFontSize;

            RectTransform inputTransform = inputPage.GetComponent<RectTransform>();
            inputTransform.offsetMin = new Vector2(theme.pageBarTheme.textWidth/-2f, 7);
            inputTransform.offsetMax=new Vector2(theme.pageBarTheme.textWidth/2f, -7);

            Image img = GetComponent<Image>();
            img.color = theme.pageBarTheme.backgroundColor;

            if (theme.pageBarTheme.isBorder == false)
            {
                border.gameObject.SetActive(false);
            }
            else
            {
                border.gameObject.SetActive(true);
                for (int i = 0; i < border.childCount; i++)
                {
                    Image imgBorder = border.GetChild(i).GetComponent<Image>();
                    if (imgBorder != null)
                    {
                        imgBorder.color = dataGridUI.theme.rowTheme.borderColor;
                    }
                }

            }

        }

        void ChangePageBarButtonTheme(PageBarButtonUI barUI)
        {
            barUI.buttonImage.color = theme.pageBarTheme.arrowBackground;
            barUI.arrowImage.color = theme.pageBarTheme.arrowIconColor;
            barUI.pageBarUI = this;
        }

        public void ChangePage(string name)
        {
            switch (name)
            {
                case "FirstPage":
                    ToFirstPage();
                    break;
                case "PrevPage":
                    ToPrevPage();
                    break;
                case "NextPage":
                    ToNextPage();
                    break;
                case "LastPage":
                    ToLastPage();
                    break;
            }
        }

        public void InputPageChange()
        {
            if (dataGridUI == null) return;
            int page = Convert.ToInt32(inputPage.text) - 1;
            if (page < 0) page = 0;

            dataGridUI.ChangePage(page);
            UpdateTextInfo();
        }

        void ToFirstPage()
        {
            if (dataGridUI == null) return;
            dataGridUI.FirstPage();
            UpdateTextInfo();
        }

        void ToPrevPage()
        {
            if (dataGridUI == null) return;
            dataGridUI.PrevPage();
            UpdateTextInfo();
        }

        void ToNextPage()
        {
            if (dataGridUI == null) return;
            dataGridUI.NextPage();
            UpdateTextInfo();
        }

        void ToLastPage()
        {
            if (dataGridUI == null) return;
            dataGridUI.LastPage();
            UpdateTextInfo();
        }

        void UpdateTextInfo()
        {
            if (dataGridUI == null) return;
            if (dataGridUI.isPaging == false) return;
            textCurrent.text = dataGridUI.GetCurrentStartIndex().ToString() + " - " + dataGridUI.GetCurrentLastIndex().ToString() + " items";
            textTotal.text = (dataGridUI.PageCount + 1).ToString() + " Pages";
            inputPage.text = (dataGridUI.CurrentPage + 1).ToString();
        }
    }
}
