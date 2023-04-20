using UnityEditor;
using UnityEngine;

namespace HaiPackage.Attribute
{
    /// <summary>
    /// 特性写入,设置场景名字
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        private int _sceneIndex = -1;

        private GUIContent[] _sceneNames;

        private readonly string[] _scenePathSplit = { "/", ".unity" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorBuildSettings.scenes.Length == 0) return;
            if (_sceneIndex == -1) GetSceneNameArray(property);
            var oldIndex = _sceneIndex;
            _sceneIndex = EditorGUI.Popup(position, label, _sceneIndex, _sceneNames);
            if (oldIndex == _sceneIndex) return;
            property.stringValue = _sceneNames[_sceneIndex].text;
            SetValue(property);
        }

        private void GetSceneNameArray(SerializedProperty property)
        {
            var scenes = EditorBuildSettings.scenes;
            _sceneNames = new GUIContent[scenes.Length];
            for (var i = 0; i < _sceneNames.Length; i++)
            {
                var path = scenes[i].path;
                var splitPath = path.Split(_scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);
                var sceneName = splitPath.Length <= 0 ? "(Deleted Scene)" : splitPath[^1];
                _sceneNames[i] = new GUIContent(sceneName);
            }

            if (_sceneNames.Length == 0)
            {
                _sceneNames = new[] { new GUIContent("Check Your Build Settings") };
            }

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                var key = false;
                for (var i = 0; i < _sceneNames.Length; i++)
                {
                    if (_sceneNames[i].text != property.stringValue) continue;
                    _sceneIndex = i;
                    key = true;
                    break;
                }

                if (key == false)
                {
                    _sceneIndex = 0;
                }
            }
            else
            {
                _sceneIndex = 0;
            }

            SetValue(property);
        }

        private void SetValue(SerializedProperty property)
        {
            property.stringValue = _sceneNames[_sceneIndex].text;
        }
    }
}