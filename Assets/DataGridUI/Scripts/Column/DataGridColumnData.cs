using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maything.UI.DataGridUI
{


    [Serializable]
    public class DataGridColumnData : ICloneable
    {
        public enum enumColumnType
        {
            Text,
            CheckBox,
            GridMultipleCheckBox,
            Photo,
            Dropdown,
            InputField,
            Button,
            MultilineInputField,
            Percentage,
            Int,
        }

        public string name;
        public float width;
        public TextAnchor columnTextAlignment = TextAnchor.MiddleLeft;
        public TextAnchor rowTextAlignment = TextAnchor.MiddleLeft;
        public enumColumnType columnType = enumColumnType.Text;
        public string[] dropdownData;

        [HideInInspector]
        public GameObject objectUI;

        [HideInInspector]
        public GameObject moverUI;

        public override string ToString()
        {
            return name + "[" + columnType.ToString() + "]";

        }

        public object Clone()
        {
            return this.MemberwiseClone(); // shallow copy
        }
    }
}