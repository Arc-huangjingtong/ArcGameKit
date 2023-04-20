using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HaiPackage
{
    public static class MethodExpansion
    {
        #region TMPro.TMP_InputField
        /// <summary>
        /// 值发生改变
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnValueChangedOnly(this TMP_InputField item, UnityAction<string> action)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onValueChanged.AddListener(action);
        }
        /// <summary>
        /// 结束编辑
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnEndEditOnly(this TMP_InputField item, UnityAction<string> action)
        {
            item.onEndEdit.RemoveAllListeners();
            item.onEndEdit.AddListener(action);
        }
        /// <summary>
        /// 选择
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnSelectOnly(this TMP_InputField item, UnityAction<string> action)
        {
            item.onSelect.RemoveAllListeners();
            item.onSelect.AddListener(action);
        }
        /// <summary>
        /// 取消选择
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnDeselectOnly(this TMP_InputField item, UnityAction<string> action)
        {
            item.onDeselect.RemoveAllListeners();
            item.onDeselect.AddListener(action);
        }
        #endregion
        #region UnityEngine.UI.Button
        /// <summary>
        /// 点击
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnClickOnly(this Button item, UnityAction action)
        {
            item.onClick.RemoveAllListeners();
            item.onClick.AddListener(action);
        }
        #endregion
        #region UnityEngine.UI.InputField
        /// <summary>
        /// 值发生改变
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnValueChangedOnly(this InputField item, UnityAction<string> action)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onValueChanged.AddListener(action);
        }

        /// <summary>
        /// 结束编辑
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnEndEditOnly(this InputField item, UnityAction<string> action)
        {
            item.onEndEdit.RemoveAllListeners();
            item.onEndEdit.AddListener(action);
        }
        #endregion
        #region UnityEngine.UI.Slider
        /// <summary>
        /// 值发生改变
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnValueChangedOnly(this Slider item, UnityAction<float> action)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onValueChanged.AddListener(action);
        }
        #endregion
        #region UnityEngine.UI.Toggle
        /// <summary>
        /// 值发生改变
        /// 移除所有事件后添加事件，保证事件唯一性
        /// </summary>
        public static void OnValueChangedOnly(this Toggle item, UnityAction<bool> action)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onValueChanged.AddListener(action);
        }
        #endregion
        #region UnityEngine.UI.ScrollRect
        public static void OnValueChangedOnly(this ScrollRect item, UnityAction<Vector2> action)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onValueChanged.AddListener(action);
        }
        #endregion
        #region UnityEngine.Transform
        public static void Reset(this UnityEngine.Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        #endregion
        #region UnityEngine.RectTransform
        public static void Reset(this RectTransform transform)
        {
            transform.anchorMin = new Vector2(.5f, .5f);
            transform.anchorMax = new Vector2(.5f, .5f);
            transform.pivot = new Vector2(.5f, .5f);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            transform.anchoredPosition3D = Vector3.zero;
        }
        #endregion
    }
}