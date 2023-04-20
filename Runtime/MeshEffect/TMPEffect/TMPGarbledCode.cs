using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HaiPackage.MeshEffect.TMPEffect
{
    /// <summary>
    /// 乱码复原
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPGarbledCode : MonoBehaviour
    {
        [Range(0, 1)] public float progress;

        private TextMeshProUGUI _text;
        private TextMeshProUGUI TextMeshPro
        {
            get
            {
                if (!_text)
                {
                    _text = GetComponent<TextMeshProUGUI>();
                }

                return _text;
            }
        }

        private readonly List<char> _codes = new()
        {
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '-', '=', '_', '`', '~', '<', '>', ',', ';', '?', '\'', ':', '"', '{', '}',
        };

        public void ResetText(string str)
        {
            var index = Mathf.FloorToInt(progress / (1f / (str.Length)));
            var startString = str[..index];
            var endStringCount = str[index..].Length;
            var endString = string.Empty;
            for (var i = 0; i < endStringCount * 1.5f; i++)
            {
                endString += _codes[Random.Range(0, _codes.Count)];
            }

            TextMeshPro.text = startString + endString;
        }
    }
}