namespace ArcMo.GameKit
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using Random = UnityEngine.Random;

    public static class ExtensionFloat
    {
        /// <summary>平方</summary>
        public static float Sqr(this float f) => f * f;
        /// <summary>平方根</summary>
        public static float SqrRt(this float f) => Mathf.Sqrt(f);
        /// <summary>是否近似于f2</summary>
        public static bool Approx(this float f, float f2) => Mathf.Approximately(f, f2);
        /// <summary>是否近似于0</summary>
        public static bool ApproxZero(this float f) => Mathf.Approximately(0, f);

        /// <summary>以度为单位归一化角度</summary>
		public static float NormalizeAngle(this float angleInDegrees)
        {
            angleInDegrees = angleInDegrees % 360f;
            if (angleInDegrees < 0)
            {
                angleInDegrees += 360f;
            }
            return angleInDegrees;
        }

        /// <summary>向下舍入浮点数</summary>
        public static float RoundDown(this float number, int decimalPlaces)
        {
            return Mathf.Floor(number * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
        }
    }

    public static class ExtensionGameObject
    {
        /// <summary>获取组件<T>,如果不存在就增加</summary>
        public static T GetComponentOrAdd<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() ?? obj.AddComponent<T>();
        }

        /// <summary>移除组件<T>,如果存在</summary>
        public static void RemoveComponent<T>(this GameObject obj) where T : Component
        {
            var t = obj.GetComponent<T>();

            if (t != null) UnityEngine.Object.Destroy(t);
        }

        /// <summary>移除组件集<T>,如果存在</summary>
        public static void RemoveComponents<T>(this GameObject obj) where T : Component
        {
            var t = obj.GetComponents<T>();

            for (var i = 0; i < t.Length; i++)
            {
                UnityEngine.Object.Destroy(t[i]);
            }
        }

        /// <summary>启用组件<T>,如果存在</summary>
        /// <returns>这个组件是否存在</returns>
        public static bool EnableComponent<T>(this GameObject obj, bool enable = true) where T : MonoBehaviour
        {
            var t = obj.GetComponent<T>();

            if (t == null) return false;

            t.enabled = enable;

            return true;
        }

        /// <summary>设置游戏物体及其所有子对象的层级</summary>
        public static void SetLayerRecursive(this GameObject o, int layer)
        {
            SetLayerInternal(o.transform, layer);
        }
        /// <summary>循环遍历并设置层级</summary>
        private static void SetLayerInternal(Transform t, int layer)
        {
            t.gameObject.layer = layer;

            foreach (Transform o in t)
            {
                SetLayerInternal(o, layer);
            }
        }

        private static List<Component> _ComponentCache = new();

        public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
        {
            @this.GetComponents(componentType, _ComponentCache);//results会自动清空,并填入所得的组件。
            var component = _ComponentCache.Count > 0 ? _ComponentCache[0] : null;
            _ComponentCache.Clear();
            return component;
        }

        /// <summary>获取一个组件，而没有无用地分配内存</summary>
        public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
        {
            @this.GetComponents(typeof(T), _ComponentCache);
            var component = _ComponentCache.Count > 0 ? _ComponentCache[0] : null;
            _ComponentCache.Clear();
            return component as T;
        }

        /// <summary>获取对象上的组件,或其子对象上的组件,或父对象上的组件,如果没有找到则将其添加到对象中</summary>
        public static T GetComponentAroundOrAdd<T>(this GameObject @this) where T : Component
        {
            var component = @this.GetComponentInChildren<T>(true);
            component ??= @this.GetComponentInParent<T>();
            component ??= @this.AddComponent<T>();

            return component;
        }

        public static bool HasComponent<T>(this GameObject @this) where T : Component
        {
            return @this.GetComponentNoAlloc<T>() != null;
        }

    }

    public static class ExtensionAnimator
    {
        /// <summary>根据类型和名称确定animator是否包含特定参数</summary>
        public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var parameters = self.parameters;//获取animator的所有参数
            foreach (var currParam in parameters)
            {
                if (currParam.type == type
                && currParam.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>将动画参数名称添加到参数列表(如果该参数存在)</summary>
        public static void AddAnimatorParam(Animator animator, string parameterName, out int parameter, AnimatorControllerParameterType type, HashSet<int> parameterList)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameter = -1;
                return;
            }

            parameter = Animator.StringToHash(parameterName);

            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameter);
            }
        }

        /// <summary>将动画参数名称添加到参数列表(如果该参数存在)</summary>
        public static void AddAnimatorParam(Animator animator, string parameterName, AnimatorControllerParameterType type, HashSet<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }




        // <summary>Try Set the animator bool.</summary>
        public static bool TrySetBool(Animator animator, int parameter, bool value, HashSet<int> parameterList)
        {
            if (!parameterList.Contains(parameter))
            {
                return false;
            }
            animator.SetBool(parameter, value);
            return true;
        }

        /// <summary>Try Set the animator int.</summary>
        public static bool TrySetInt(Animator animator, int parameter, int value, HashSet<int> parameterList)
        {
            if (!parameterList.Contains(parameter))
            {
                return false;
            }
            animator.SetInteger(parameter, value);
            return true;
        }

        /// <summary>Try Set the animator trigger</summary>
        public static bool TrySetTrigger(Animator animator, int parameter, HashSet<int> parameterList)
        {
            if (!parameterList.Contains(parameter))
            {
                return false;
            }
            animator.SetTrigger(parameter);
            return true;
        }

        /// <summary>Try Set the animator Float</summary>
        public static bool UpdateAnimatorFloat(Animator animator, int parameter, float value, HashSet<int> parameterList)
        {
            if (!parameterList.Contains(parameter))
            {
                return false;
            }
            animator.SetFloat(parameter, value);
            return true;
        }

        /// <summary>
        /// Updates the animator integer.
        /// </summary>
        public static bool UpdateAnimatorInteger(Animator animator, int parameter, int value, HashSet<int> parameterList, bool performSanityCheck = true)
        {
            if (performSanityCheck && !parameterList.Contains(parameter))
            {
                return false;
            }
            animator.SetInteger(parameter, value);
            return true;
        }



        // STRING PARAMETER METHODS -------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region StringParameterMethods

        // <summary>
        /// Updates the animator bool.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }

        /// <summary>
        /// Sets an animator's trigger of the string parameter name specified
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterList"></param>
        public static void UpdateAnimatorTrigger(Animator animator, string parameterName, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Triggers an animator trigger.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTrigger(Animator animator, string parameterName, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator integer.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetInteger(parameterName, value);
            }
        }

        // <summary>
        /// Updates the animator bool after checking the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBoolIfExists(Animator animator, string parameterName, bool value, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(parameterName, value);
            }
        }

        /// <summary>
        /// Updates an animator trigger if it exists
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="parameterName"></param>
        public static void UpdateAnimatorTriggerIfExists(Animator animator, string parameterName, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Triggers an animator trigger after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTriggerIfExists(Animator animator, string parameterName, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloatIfExists(Animator animator, string parameterName, float value, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator integer after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorIntegerIfExists(Animator animator, string parameterName, int value, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(parameterName, value);
            }
        }

        #endregion


    }

    public static class ExtensionIList
    {
        public static T RandomValue<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("List needs at least one entry to call Random()");
            if (list.Count == 1) return list[0];

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T RandomOrDefault<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }

            return list.RandomValue();
        }

        public static T PopLast<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException();
            }

            var t = list[list.Count - 1];

            list.RemoveAt(list.Count - 1);

            return t;
        }

        /// <summary>交换列表中的两个项目</summary>
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            (list[i], list[j]) = (list[j], list[i]);
        }

        /// <summary>打乱列表</summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.Swap(i, Random.Range(i, list.Count));
            }
        }

    }

    public static class ExtensionArray
    {
        /// <summary>返回数组中的随机值</summary>
        public static T RandomValue<T>(this T[] array)
        {
            int newIndex = UnityEngine.Random.Range(0, array.Length);
            return array[newIndex];
        }

        /// <summary>打乱数组</summary>
        public static T[] Shuffle<T>(this T[] array)
        {
            // Fisher Yates shuffle algorithm, see https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            for (int t = 0; t < array.Length; t++)
            {
                int randomIndex = Random.Range(t, array.Length);
                (array[t], array[randomIndex]) = (array[randomIndex], array[t]);
            }
            return array;
        }
    }

    public static class ExtensionBounds
    {
        /// <summary>返回作为参数设置的边界内的随机点</summary>
        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3
            (
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        /// <summary>获取对象的碰撞器边界(来自Collider2D)</summary>
        public static Bounds GetColliderBounds(GameObject theObject)
        {
            Bounds returnBounds;

            // if the object has a collider at root level, we base our calculations on that
            if (theObject.GetComponent<Collider>() != null)
            {
                returnBounds = theObject.GetComponent<Collider>().bounds;
                return returnBounds;
            }

            // if the object has a collider2D at root level, we base our calculations on that
            if (theObject.GetComponent<Collider2D>() != null)
            {
                returnBounds = theObject.GetComponent<Collider2D>().bounds;
                return returnBounds;
            }

            // if the object contains at least one Collider we'll add all its children's Colliders bounds
            if (theObject.GetComponentInChildren<Collider>() != null)
            {
                Bounds totalBounds = theObject.GetComponentInChildren<Collider>().bounds;
                Collider[] colliders = theObject.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    totalBounds.Encapsulate(col.bounds);
                }
                returnBounds = totalBounds;
                return returnBounds;
            }

            // if the object contains at least one Collider2D we'll add all its children's Collider2Ds bounds
            if (theObject.GetComponentInChildren<Collider2D>() != null)
            {
                Bounds totalBounds = theObject.GetComponentInChildren<Collider2D>().bounds;
                Collider2D[] colliders = theObject.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in colliders)
                {
                    totalBounds.Encapsulate(col.bounds);
                }
                returnBounds = totalBounds;
                return returnBounds;
            }

            returnBounds = new Bounds(Vector3.zero, Vector3.zero);
            return returnBounds;
        }

        /// <summary>获取渲染器的界限</summary>
        public static Bounds GetRendererBounds(GameObject theObject)
        {
            Bounds returnBounds;

            // if the object has a renderer at root level, we base our calculations on that
            if (theObject.GetComponent<Renderer>() != null)
            {
                returnBounds = theObject.GetComponent<Renderer>().bounds;
                return returnBounds;
            }

            // if the object contains at least one renderer we'll add all its children's renderer bounds
            if (theObject.GetComponentInChildren<Renderer>() != null)
            {
                Bounds totalBounds = theObject.GetComponentInChildren<Renderer>().bounds;
                Renderer[] renderers = theObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
                returnBounds = totalBounds;
                return returnBounds;
            }

            returnBounds = new Bounds(Vector3.zero, Vector3.zero);
            return returnBounds;
        }
    }

    public static class ExtensionCamera
    {
        /// <summary>
        /// 以世界空间单位返回相机的宽度，对于透视相机在指定深度，对于正交相机在任何位置
        /// </summary>
        public static float CameraWorldSpaceWidth(this Camera camera, float depth = 0f)
        {
            if (camera.orthographic)
            {
                return camera.aspect * camera.orthographicSize * 2f;
            }
            else
            {
                float fieldOfView = camera.fieldOfView * Mathf.Deg2Rad;
                return camera.aspect * depth * Mathf.Tan(fieldOfView);
            }
        }

        /// <summary>
        /// 以世界空间单位返回相机的高度，对于透视相机在指定深度，对于正交相机在任何位置
        /// </summary>
        public static float CameraWorldSpaceHeight(this Camera camera, float depth = 0f)
        {
            if (camera.orthographic)
            {
                return camera.orthographicSize * 2f;
            }
            else
            {
                float fieldOfView = camera.fieldOfView * Mathf.Deg2Rad;
                return depth * Mathf.Tan(fieldOfView);
            }
        }
    }

    public static class ExtensionColor
    {
        /// <summary>将颜色的所有部分相加并返回一个浮点数</summary>
        public static float Sum(this Color color)
        {
            return color.r + color.g + color.b + color.a;
        }

        /// <summary>返回r、g和b之间的平均值</summary>
        public static float MeanRGB(this Color color)
        {
            return (color.r + color.g + color.b) / 3f;
        }

        /// <summary>计算颜色的亮度值</summary>
        public static float Luminance(this Color color)
        {
            return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
        }
    }

    public static class ExtensionDictionary
    {
        /// <summary>查找与参数中设置的值匹配的键(如果有)</summary>
        public static T KeyByValue<T, W>(this Dictionary<T, W> dictionary, T value)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in dictionary)
            {
                if (pair.Value.Equals(value))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }
    }

    public static class ExtensionLayermask
    {
        /// <summary>如果图层在layermask内,则返回true</summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) > 0;
        }

        /// <summary>如果游戏对象在layermask中，则返回true</summary>
        public static bool Contains(this LayerMask mask, GameObject gameobject)
        {
            return ((mask.value & (1 << gameobject.layer)) > 0);
        }
    }

    public static class ExtensionRect
    {
        /// <summary>如果此矩形与另一个指定的矩形相交，则返回true</summary>
        public static bool Intersects(this Rect thisRectangle, Rect otherRectangle)
        {
            return !((thisRectangle.x > otherRectangle.xMax)
                  || (thisRectangle.xMax < otherRectangle.x)
                  || (thisRectangle.y > otherRectangle.yMax)
                  || (thisRectangle.yMax < otherRectangle.y));
        }
    }

    public static class ExtensionRectTransform
    {
        /// <summary>将RectTransform的左偏移量设置为指定值</summary>
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        /// <summary>将RectTransform的右偏移量设置为指定值</summary>
        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        /// <summary>将RectTransform的上偏移量设置为指定值</summary>
        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        /// <summary>将RectTransform的下偏移量设置为指定值</summary>
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }

    public static class ExtensionRenderer
    {
        /// <summary>如果渲染器在摄像机中可见,则返回true</summary>
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }
    }

    public static class ExtensionScrollRect
    {
        /// <summary>将滚动条滚动到顶部</summary>
        public static void ToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        /// <summary>将滚动条滚动到底部</summary>
        public static void ToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }

    public static class ExtensionSerializedProperty
    {
#if UNITY_EDITOR
        /// <summary>返回目标序列化属性的对象值</summary>
        public static object GetObjectValue(this SerializedProperty property)
        {
            if (property == null) return null;


            string propertyPath = property.propertyPath.Replace(".Array.data[", "[");
            object targetObject = property.serializedObject.targetObject;
            var elements = propertyPath.Split('.');
            foreach (var element in elements)
            {
                if (!element.Contains("["))
                {
                    targetObject = GetPropertyValue(targetObject, element);
                }
                else
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int elementIndex = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    targetObject = GetPropertyValue(targetObject, elementName, elementIndex);
                }
            }
            return targetObject;
        }

        private static object GetPropertyValue(object source, string propertyName)
        {
            if (source == null) return null;


            Type propertyType = source.GetType();

            while (propertyType != null)
            {
                FieldInfo fieldInfo = propertyType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(source);
                }
                PropertyInfo propertyInfo = propertyType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(source, null);
                }
                propertyType = propertyType.BaseType;
            }
            return null;
        }

        private static object GetPropertyValue(object source, string propertyName, int index)
        {
            var enumerable = GetPropertyValue(source, propertyName) as System.Collections.IEnumerable;
            if (enumerable == null)
            {
                return null;
            }
            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }
            return enumerator.Current;
        }
#endif
    }

    public static class ExtensionTransform
    {
        public static void DestroyAllChildren(this Transform transform)
        {
            for (var t = transform.childCount - 1; t >= 0; t--)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(t).gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(t).gameObject);
                }
            }
        }

        /// <summary>广度优先(BFS)查找子物体</summary>
        public static Transform FindDeepChildBFS(this Transform parent, string transformName)
        {
            var queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                if (child.name == transformName)
                {
                    return child;
                }
                foreach (Transform t in child)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }

        /// <summary>深度优先(DFS)查找子物体</summary>
        public static Transform FindDeepChildDFS(this Transform parent, string transformName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == transformName)
                {
                    return child;
                }

                Transform result = child.FindDeepChildDFS(transformName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>将变换层及其所有子层更改为新层</summary>
        public static void ChangeLayersRecursively(this Transform transform, string layerName)
        {
            transform.gameObject.layer = LayerMask.NameToLayer(layerName);
            foreach (Transform child in transform)
            {
                child.ChangeLayersRecursively(layerName);
            }
        }

        /// <summary>将变换层及其所有子层更改为新层</summary>
        public static void ChangeLayersRecursively(this Transform transform, int layerIndex)
        {
            transform.gameObject.layer = layerIndex;
            foreach (Transform child in transform)
            {
                child.ChangeLayersRecursively(layerIndex);
            }
        }

        public static IEnumerable<Transform> GetChildren(this Transform t)
        {
            var i = 0;

            while (i < t.childCount)
            {
                yield return t.GetChild(i);
                ++i;
            }
        }

        /// <summary>将Transform的所有本地值重置为identity</summary>
        public static void ResetLocal(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        /// <summary>创建此Transform的空子对象,并且更换层级</summary>
        public static GameObject CreateChild(this Transform t, string name)
        {
            var go = new GameObject(name);
            go.transform.parent = t;
            go.transform.ResetLocal();
            go.gameObject.layer = t.gameObject.layer;

            return go;
        }

        /// <summary>设置此转换的父级,但保留localScale、localPosition和localRotation值</summary>
        public static void SetParentMaintainLocals(this Transform t, Transform parent)
        {
            t.SetParent(parent, false);
        }

        /// <summary>从其他Transform中复制局部位置、旋转和缩放</summary>
        public static void SetLocals(this Transform t, Transform from)
        {
            t.localPosition = from.localPosition;
            t.localRotation = from.localRotation;
            t.localScale = from.localScale;
        }

        /// <summary>将位置/旋转设置为from。比例不变</summary>
        public static void SetForm(this Transform t, Transform from)
        {
            t.position = from.position;
            t.rotation = from.rotation;
        }

        /// <summary>删除所有子游戏物体</summary>
        public static void DestroyChildren(this Transform t)
        {
            foreach (var child in t)
            {
                UnityEngine.Object.Destroy(((Transform)child).gameObject);
            }
        }


    }

    public static class ExtensionsVector2
    {
        /// <summary>将vector2旋转一个角度(以度为单位的浮点数)</summary>
        public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
        {
            float sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }


        public static Vector2 SetX(this Vector2 vector, float newValue)
        {
            vector.x = newValue;
            return vector;
        }
        public static Vector2 SetY(this Vector2 vector, float newValue)
        {
            vector.y = newValue;
            return vector;
        }
    }

    public static class ExtensionVector3
    {
        public static Vector3 SetX(this Vector3 vector, float newValue)
        {
            vector.x = newValue;
            return vector;
        }
        public static Vector3 SetY(this Vector3 vector, float newValue)
        {
            vector.y = newValue;
            return vector;
        }
        public static Vector3 SetZ(this Vector3 vector, float newValue)
        {
            vector.z = newValue;
            return vector;
        }

        /// <summary>反转(倒数)</summary>
        public static Vector3 Invert(this Vector3 newValue)
        {
            return new Vector3
            (
                1.0f / newValue.x,
                1.0f / newValue.y,
                1.0f / newValue.z
            );
        }

        /// <summary>投影到另一个向量</summary>
        public static Vector3 Project(this Vector3 vector, Vector3 projectedVector)
        {
            float _dot = Vector3.Dot(vector, projectedVector);
            return _dot * projectedVector;
        }

        /// <summary>垂直投影到另一个向量</summary>
        public static Vector3 Reject(this Vector3 vector, Vector3 rejectedVector)
        {
            return vector - vector.Project(rejectedVector);
        }

        /// <summary>四舍五入所有的分量</summary>
        public static Vector3 Round(this Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);
            return vector;
        }
    }

    public static class ExtensionType
    {
        /// <summary> 该种类是否属于另一个类型 </summary>
        public static bool IsAssignableFrom(this Type @this, Type t)
        {
            return @this.GetTypeInfo().IsAssignableFrom(t.GetTypeInfo());
        }
        /// <summary> 该实例是否属于另一个类型 </summary>
        public static bool IsInstanceOfType(this Type @this, object obj)
        {
            return @this.IsAssignableFrom(obj.GetType());
            // 如果满足下列任一条件，则为 true：
            // Type 和当前实例表示相同类型。
            // Type 是从当前实例直接或间接派生的。 如果继承于当前实例，则 c 是从当前实例直接派生的；如果继承于从当前实例继承的接连一个或多个类，则 c 是从当前实例间接派生的。
            // 当前实例是实现 的 Type 接口。
            // Type 是一个泛型类型参数，并且当前实例表示 Type 的约束之一。
            // Type 表示值类型，当前实例表示 Nullable<c> Visual Basic) Nullable(Of Type) 中的
            // 如果不满足上述任何一个条件或者 Type 为 false，则为 null。
        }

    }

}
