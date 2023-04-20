using System;
using HaiPackage.Data;
using UnityEditor;
using UnityEngine;

namespace HaiPackage.Editor
{
    /// <summary>
    /// 导出工具
    /// </summary>
    public static class GetMyUnityPackage
    {
        [MenuItem("HaiPackage/编译 .Tgz _#%e")]
        public static void BuildUnityProgramPackage()
        {
            var outPath = Application.dataPath[..^"/Assets".Length];
            UnityEditor.PackageManager.Client.Pack(System.IO.Path.GetDirectoryName(Application.dataPath + "/HaiPackage/package.json"), outPath);
            Application.OpenURL("file:///" + outPath);
            Debug.Log("已将 (" + Constant.MyUnityPackageName + ") 导出到 (" + outPath + ") 中。");
        }

        [MenuItem("HaiPackage/编译 .UnityPackage")]
        public static void GetUnityPackNameTime()
        {
            var fileName = Constant.MyUnityPackageName + DateTime.Now.ToString("-yyyy-MM-dd HH-mm") + ".unitypackage";
            AssetDatabase.ExportPackage("Assets/HaiPackage", fileName, ExportPackageOptions.Recurse);
            var outPath = Application.dataPath[..^"/Assets".Length];
            GUIUtility.systemCopyBuffer = outPath; //这个方法可以把信息复制到粘贴板
            Application.OpenURL("file:///" + outPath);
            Debug.Log("已将 (" + Constant.MyUnityPackageName + ") 导出到 (" + outPath + ") 中。");
        }

        /// <summary>
        /// 获取脚本路径，只能在编辑模式下使用
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public static string GetCodePath(string scriptName)
        {
            var paths = AssetDatabase.FindAssets(scriptName);
            if (paths.Length > 1)
            {
                Debug.LogError("有同名文件" + scriptName + "获取路径失败");
                return null;
            }

            //将字符串中得脚本名字和后缀统统去除掉
            var path = AssetDatabase.GUIDToAssetPath(paths[0]).Replace((@"/" + scriptName + ".cs"), "");
            return path;
        }
    }
}