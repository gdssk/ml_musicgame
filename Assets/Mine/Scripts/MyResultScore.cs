using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mine
{
    public class MyResultScore : MonoBehaviour
    {
        public TextMeshPro NUM;


        public void Show(int num)
        {
            NUM.text = num.ToString();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
