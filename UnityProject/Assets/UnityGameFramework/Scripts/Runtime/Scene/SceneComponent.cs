﻿using GameFramework;
using GameFramework.Resource;
using GameFramework.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 场景组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Scene")]
    public sealed class SceneComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;

        private ISceneManager m_SceneManager = null;
        private EventComponent m_EventComponent = null;
        private readonly SortedDictionary<string, int> m_SceneOrder = new SortedDictionary<string, int>(StringComparer.Ordinal);
        private Camera m_MainCamera = null;
        private Scene m_GameFrameworkScene = default(Scene);

        [SerializeField]
        private bool m_EnableLoadSceneUpdateEvent = true;

        /// <summary>
        /// 获取当前场景主摄像机。
        /// </summary>
        public Camera MainCamera
        {
            get
            {
                return m_MainCamera;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_SceneManager = GameFrameworkEntry.GetModule<ISceneManager>();
            if (m_SceneManager == null)
            {
                Log.Fatal("Scene manager is invalid.");
                return;
            }

            m_SceneManager.LoadSceneSuccess += OnLoadSceneSuccess;
            m_SceneManager.LoadSceneFailure += OnLoadSceneFailure;

            if (m_EnableLoadSceneUpdateEvent)
            {
                m_SceneManager.LoadSceneUpdate += OnLoadSceneUpdate;
            }

            m_SceneManager.UnloadSceneSuccess += OnUnloadSceneSuccess;
            m_SceneManager.UnloadSceneFailure += OnUnloadSceneFailure;

            m_GameFrameworkScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(GameSystem.GameFrameworkSceneId);
            if (!m_GameFrameworkScene.IsValid())
            {
                Log.Fatal("Game Framework scene is invalid.");
                return;
            }
        }

        private void Start()
        {
            BaseComponent baseComponent = GameSystem.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_EventComponent = GameSystem.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            m_SceneManager.SetResourceManager(GameFrameworkEntry.GetModule<IResourceManager>());
        }

        /// <summary>
        /// 获取场景名称。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景名称。</returns>
        public static string GetSceneName(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return null;
            }

            int sceneNamePosition = sceneAssetName.LastIndexOf('/');
            if (sceneNamePosition + 1 >= sceneAssetName.Length)
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return null;
            }

            string sceneName = sceneAssetName.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
            {
                sceneName = sceneName.Substring(0, sceneNamePosition);
            }

            return sceneName;
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            return m_SceneManager.SceneIsLoaded(sceneAssetName);
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <returns>已加载场景的资源名称。</returns>
        public string[] GetLoadedSceneAssetNames()
        {
            return m_SceneManager.GetLoadedSceneAssetNames();
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <param name="results">已加载场景的资源名称。</param>
        public void GetLoadedSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetLoadedSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在加载。</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            return m_SceneManager.SceneIsLoading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <returns>正在加载场景的资源名称。</returns>
        public string[] GetLoadingSceneAssetNames()
        {
            return m_SceneManager.GetLoadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <param name="results">正在加载场景的资源名称。</param>
        public void GetLoadingSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetLoadingSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            return m_SceneManager.SceneIsUnloading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <returns>正在卸载场景的资源名称。</returns>
        public string[] GetUnloadingSceneAssetNames()
        {
            return m_SceneManager.GetUnloadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称。</param>
        public void GetUnloadingSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetUnloadingSceneAssetNames(results);
        }

        /// <summary>
        /// 检查场景资源是否存在。
        /// </summary>
        /// <param name="sceneAssetName">要检查场景资源的名称。</param>
        /// <returns>场景资源是否存在。</returns>
        public bool HasScene(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return false;
            }

            return m_SceneManager.HasScene(sceneAssetName);
        }

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">加载完毕时是否主动挂起</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="gcCollect">加载场景是否回收垃圾。</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks = null, LoadSceneMode sceneMode = LoadSceneMode.Single, 
            bool suspendLoad = false, int priority = 100, bool gcCollect = false, string packageName = "", object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            m_SceneManager.LoadScene(sceneAssetName, loadSceneCallbacks, sceneMode, suspendLoad, priority, gcCollect, packageName, userData);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        public void UnloadScene(string sceneAssetName, string packageName = "")
        {
            UnloadScene(sceneAssetName, null, packageName);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        public void UnloadScene(string sceneAssetName, object userData, string packageName = "")
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            m_SceneManager.UnloadScene(sceneAssetName, userData, packageName);
            m_SceneOrder.Remove(sceneAssetName);
        }

        /// <summary>
        /// 设置场景顺序。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="sceneOrder">要设置的场景顺序。</param>
        public void SetSceneOrder(string sceneAssetName, int sceneOrder)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                m_SceneOrder[sceneAssetName] = sceneOrder;
                return;
            }

            if (SceneIsLoaded(sceneAssetName))
            {
                m_SceneOrder[sceneAssetName] = sceneOrder;
                RefreshSceneOrder();
                return;
            }

            Log.Error("Scene '{0}' is not loaded or loading.", sceneAssetName);
        }

        /// <summary>
        /// 刷新当前场景主摄像机。
        /// </summary>
        public void RefreshMainCamera()
        {
            m_MainCamera = Camera.main;
        }

        private void RefreshSceneOrder()
        {
            if (m_SceneOrder.Count > 0)
            {
                string maxSceneName = null;
                int maxSceneOrder = 0;
                foreach (KeyValuePair<string, int> sceneOrder in m_SceneOrder)
                {
                    if (SceneIsLoading(sceneOrder.Key))
                    {
                        continue;
                    }

                    if (maxSceneName == null)
                    {
                        maxSceneName = sceneOrder.Key;
                        maxSceneOrder = sceneOrder.Value;
                        continue;
                    }

                    if (sceneOrder.Value > maxSceneOrder)
                    {
                        maxSceneName = sceneOrder.Key;
                        maxSceneOrder = sceneOrder.Value;
                    }
                }

                if (maxSceneName == null)
                {
                    SetActiveScene(m_GameFrameworkScene);
                    return;
                }

                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(GetSceneName(maxSceneName));
                if (!scene.IsValid())
                {
                    Log.Error("Active scene '{0}' is invalid.", maxSceneName);
                    return;
                }

                SetActiveScene(scene);
            }
            else
            {
                SetActiveScene(m_GameFrameworkScene);
            }
        }

        private void SetActiveScene(Scene activeScene)
        {
            Scene lastActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (lastActiveScene != activeScene)
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
                m_EventComponent.Fire(this, ActiveSceneChangedEventArgs.Create(lastActiveScene, activeScene));
            }

            RefreshMainCamera();
        }

        private void OnLoadSceneSuccess(object sender, GameFramework.Scene.LoadSceneSuccessEventArgs e)
        {
            if (!m_SceneOrder.ContainsKey(e.SceneAssetName))
            {
                m_SceneOrder.Add(e.SceneAssetName, 0);
            }

            m_EventComponent.Fire(this, LoadSceneSuccessEventArgs.Create(e));
            RefreshSceneOrder();
        }

        private void OnLoadSceneFailure(object sender, GameFramework.Scene.LoadSceneFailureEventArgs e)
        {
            Log.Warning("Load scene failure, scene asset name '{0}', error message '{1}'.", e.SceneAssetName, e.ErrorMessage);
            m_EventComponent.Fire(this, LoadSceneFailureEventArgs.Create(e));
        }

        private void OnLoadSceneUpdate(object sender, GameFramework.Scene.LoadSceneUpdateEventArgs e)
        {
            m_EventComponent.Fire(this, LoadSceneUpdateEventArgs.Create(e));
        }

        private void OnUnloadSceneSuccess(object sender, GameFramework.Scene.UnloadSceneSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, UnloadSceneSuccessEventArgs.Create(e));
            m_SceneOrder.Remove(e.SceneAssetName);
            RefreshSceneOrder();
        }

        private void OnUnloadSceneFailure(object sender, GameFramework.Scene.UnloadSceneFailureEventArgs e)
        {
            Log.Warning("Unload scene failure, scene asset name '{0}'.", e.SceneAssetName);
            m_EventComponent.Fire(this, UnloadSceneFailureEventArgs.Create(e));
        }
    }
}
