using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maything.UI.DataGridUI
{

    [CreateAssetMenu(menuName = "Data Grid UI/Create New Theme")]
    public class DataGridTheme : ScriptableObject
    {
        [Header("ScrollBar")]
        public float scrollBarSize = 10;
        public Color scrollBarBackgroundColor = Color.white;
        public Color scrollBarNormalColor = Color.white;
        public Color scrollBarHighlightedColor = new Color(0.96f, 0.96f, 0.96f);
        public Color scrollBarPressedColor = new Color(0.78f, 0.78f, 0.78f);
        public Color scrollBarSelectedColor = new Color(0.96f, 0.96f, 0.96f);
        public Color scrollBarDisabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);

        [Header("Border")]
        public bool isBorder = true;
        public Color borderColor = Color.gray;

        [Header("Column")]
        public DataGridColumnTheme columnTheme;
        public DataGridColumnItemTheme columnItemTheme;

        [Header("Row")]
        public DataGridRowTheme rowTheme;

        [Header("Row Controls")]
        public DataGridCheckBoxTheme checkBoxTheme;
        public DataGridPercentageTheme percentageTheme;

        [Header("Page Bar")]
        public PageBarTheme pageBarTheme;
    }

    [Serializable]
    public class DataGridColumnTheme
    {
        public float columnHeight;
        public GameObject columnTemplate;
        public GameObject columnSplitMoverTemplate;
    }

    [Serializable]
    public class DataGridColumnItemTheme
    {
        public GameObject columnItemTemplate;

        public int textFontSize = 14;
        public float textSpace = 15;
        public TextAnchor textAlignment = TextAnchor.MiddleLeft;


        public Color backgroundColor = Color.white;
        public Color textColor = Color.black;

        public Color selectedColor = Color.white;
        public Color selectedTextColor = Color.black;

        public bool isBorder = true;
        public Color borderColor = Color.gray;


    }

    [Serializable]
    public class DataGridCheckBoxTheme
    {
        public Sprite normalIcon;
        public Sprite checkIcon;
        public float iconSize = 18;
    }

    [Serializable]
    public class DataGridPercentageTheme
    {
        public float barHeight = 25;
        public Color textColor = Color.black;
        public Color barColor = new Color(0.05f, 0.43f, 0.74f,1f);
        public Color barBackgroundColor = new Color(0.83f, 0.83f, 0.83f, 1f);
        public List<DataGridPercentageRangeTheme> rangeColors;
    }

    [Serializable]
    public class DataGridPercentageRangeTheme
    {
        public float startRange = 0;
        public float endRange = 0;
        public Color barColor = new Color(0.05f, 0.43f, 0.74f,1f);
        public Color textColor = Color.black;
    }


    [Serializable]
    public class DataGridRowTheme
    {
        public float rowHeight = 35;

        public int textFontSize = 14;
        public float controlSpace = 15;

        public float controlHeight = 30;
        public float dropdownHeight = 20;

        [Header("Normal Color")]
        public Color backgroundColor = Color.white;
        public Color textColor = Color.black;

        public Color alternatingBackgroundColor = Color.white;
        public Color alternatingTextColor = Color.black;

        [Header("Enter Color")]
        public Color enterColor = Color.white;
        public Color enterTextColor = Color.black;

        [Header("Select Color")]
        public Color selectedColor = Color.white;
        public Color selectedTextColor = Color.black;

        [Header("Border")]
        public bool isColumnBorder = true;
        public bool isRowBorder = true;
        public Color borderColor = Color.gray;

    }

    [Serializable]
    public class PageBarTheme
    {
        public int textFontSize = 14;
        public Color textColor = Color.black;
        public float textWidth = 50;

        public bool isBorder = true;
        public Color borderColor = Color.gray;

        public Color backgroundColor = Color.white;

        public Color arrowBackground = Color.gray;
        public Color arrowIconColor = Color.black;

    }
}