using System;
using HaiPackage.MeshEffect.TMPEffect;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HaiPackage.To
{
    public static class ToManager
    {
        #region RectTransform
        public static To ToAnchoredPosition(this RectTransform obj, Vector2 endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.anchoredPosition,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.anchoredPosition =
                        Vector2.LerpUnclamped((Vector2)t.StartValue, (Vector2)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.anchoredPosition = (Vector2)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }

        public static To ToLocalScale(this RectTransform obj, Vector3 endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.localScale,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.localScale =
                        Vector3.LerpUnclamped((Vector3)t.StartValue, (Vector3)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.localScale = (Vector3)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }
        #endregion
        #region Transform
        public static To ToPosition(this Transform obj, Vector3 endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.position,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.position = Vector3.LerpUnclamped((Vector3)t.StartValue, (Vector3)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.position = (Vector3)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }
        public static To ToLocalPosition(this Transform obj, Vector3 endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.localPosition,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.localPosition = Vector3.LerpUnclamped((Vector3)t.StartValue, (Vector3)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.localPosition = (Vector3)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }  
        
        
        public static To ToLocalRotation(this Transform obj, Quaternion endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.localRotation,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.localRotation = Quaternion.LerpUnclamped((Quaternion)t.StartValue, (Quaternion)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.localRotation = (Quaternion)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }

        public static To ToLocalScale(this Transform obj, Vector3 endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.localScale,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.localScale = Vector3.LerpUnclamped((Vector3)t.StartValue, (Vector3)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.localScale = (Vector3)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }
        #endregion
        #region CanvasGroup
        public static To ToAlpha(this CanvasGroup obj, float endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.alpha,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.alpha = Mathf.LerpUnclamped((float)t.StartValue, (float)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.alpha = (float)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }
        #endregion
        #region Color
        public static To ToColor(this SpriteRenderer obj, Color endValue, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var to = new To
            {
                StartValue = obj.color,
                EndValue = endValue,
                overTime = overTime,
                Start = start,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    obj.color = Color.LerpUnclamped((Color)t.StartValue, (Color)t.EndValue, curve?.Evaluate(t.Progress) ?? t.Progress);
                },
                Over = t =>
                {
                    obj.color = (Color)t.EndValue;
                    over?.Invoke(t);
                }
            };
            to.Add();
            return to;
        }
        #endregion
        #region TMP
        /// <summary>
        /// 文字海浪效果
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="configure">配置表</param>
        /// <param name="disposable">是否是一次性的 (动画结束后删除特效组件)</param>
        /// <param name="overTime">运行这个动画所需要的时间</param>
        /// <param name="start">开始执行的事件</param>
        /// <param name="upDate">UpDate中执行的事件</param>
        /// <param name="over">结束事件</param>
        /// <param name="curve">动画曲线</param>
        /// <returns>动画自身</returns>
        public static To ToWave(this TextMeshProUGUI obj, TMPWaveEffect.Configure configure, bool disposable = true, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var wordBounce = obj.GetComponent<TMPWaveEffect>();
            if (wordBounce == null)
            {
                wordBounce = obj.gameObject.AddComponent<TMPWaveEffect>();
            }

            wordBounce.configure = configure;
            var to = new To
            {
                Start = start,
                overTime = overTime,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    wordBounce.progress = curve?.Evaluate(t.Progress) ?? t.Progress;
                    wordBounce.SetVerticesDirty();
                },
                Over = t =>
                {
                    wordBounce.progress = 1;
                    if (disposable)
                    {
                        Object.Destroy(wordBounce);
                    }

                    over?.Invoke(t);
                }
            };
            to.Add(ToControlManager.UpdateType.LateUpdate);
            return to;
        }

        /// <summary>
        /// 逐字显示
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="configure">配置表</param>
        /// <param name="disposable">是否是一次性的 (动画结束后删除特效组件)</param>
        /// <param name="overTime">运行这个动画所需要的时间</param>
        /// <param name="start">开始执行的事件</param>
        /// <param name="upDate">UpDate中执行的事件</param>
        /// <param name="over">结束事件</param>
        /// <param name="curve">动画曲线</param>
        /// <returns>动画自身</returns>
        public static To ToShowGradually(this TextMeshProUGUI obj, TMPShowGraduallyEffect.Configure configure, bool disposable = true, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var wordBounce = obj.GetComponent<TMPShowGraduallyEffect>();
            if (wordBounce == null)
            {
                wordBounce = obj.gameObject.AddComponent<TMPShowGraduallyEffect>();
            }

            wordBounce.configure = configure;
            var to = new To
            {
                Start = start,
                overTime = overTime,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    wordBounce.progress = curve?.Evaluate(t.Progress) ?? t.Progress;
                    wordBounce.SetVerticesDirty();
                },
                Over = t =>
                {
                    wordBounce.progress = 1;
                    if (disposable)
                    {
                        Object.Destroy(wordBounce);
                    }

                    over?.Invoke(t);
                }
            };
            to.Add(ToControlManager.UpdateType.LateUpdate);
            return to;
        }

        /// <summary>
        /// 乱码复原
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="endValue"></param>
        /// <param name="disposable">是否是一次性的 (动画结束后删除特效组件)</param>
        /// <param name="overTime">运行这个动画所需要的时间</param>
        /// <param name="start">开始执行的事件</param>
        /// <param name="upDate">UpDate中执行的事件</param>
        /// <param name="over">结束事件</param>
        /// <param name="curve">动画曲线</param>
        /// <returns>动画自身</returns>
        public static To ToGarbledCode(this TextMeshProUGUI obj, string endValue, bool disposable = true, float overTime = 1, Action<To> start = null, Action<To> upDate = null, Action<To> over = null, AnimationCurve curve = null)
        {
            var wordBounce = obj.GetComponent<TMPGarbledCode>();
            if (wordBounce == null)
            {
                wordBounce = obj.gameObject.AddComponent<TMPGarbledCode>();
            }

            var to = new To
            {
                Start = start,
                overTime = overTime,
                UpDate = t =>
                {
                    upDate?.Invoke(t);
                    wordBounce.progress = curve?.Evaluate(t.Progress) ?? t.Progress;
                    wordBounce.ResetText(endValue);
                },
                Over = t =>
                {
                    wordBounce.progress = 1;
                    if (disposable)
                    {
                        Object.Destroy(wordBounce);
                    }

                    over?.Invoke(t);
                }
            };
            to.Add(ToControlManager.UpdateType.LateUpdate);
            return to;
        }
        #endregion
    }
}