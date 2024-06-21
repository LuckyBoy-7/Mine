using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mine.Managers
{
    /// <summary>
    /// 大概就是封装了level开始和结束的各种委托
    /// level的生成逻辑
    /// level的通过逻辑
    /// </summary>
    public abstract class LevelManagerBase : Singleton<LevelManagerBase>
    {
        public Action OnFirstLevelStart;
        public Action OnLevelStart;
        public Action OnLevelEnd;
        public Action OnLastLevelEnd;

        private int curLevel = -1;

        // 关卡所需数据，虽然有的时候用不到
        public int levelNumber = 20;
        public List<LevelData> allLevelData = new();
        protected LevelData levelData => curLevel != -1 ? allLevelData[curLevel] : null;
        public int Count => allLevelData.Count == 0 ? levelNumber : allLevelData.Count;
        protected bool HasStart => curLevel != -1;

        public virtual void StartLevel(int level = 0)
        {
            curLevel = level - 1;
            NextLevel();
        }

        public void NextLevel()
        {
            curLevel += 1;
            if (curLevel >= Count)
            {
                Debug.LogWarning("there's no level to load");
                return;
            }

            if (curLevel == 0)
                OnFirstLevelStart?.Invoke();
            OnLevelStart?.Invoke();
            LoadLevel(curLevel);
        }

        // 子类需实现加载场景的方法，比如生成对应gameobject
        // 有时候就只是单纯加载一个场景就行
        protected abstract void LoadLevel(int level);

        /// 获取level是否达到条件
        /// 子类可以写达到条件持续一定时间后下一关或直接下一关等逻辑
        protected abstract bool GetPassCondition();

        // 可以由外部调用，也可以直接放update里
        public void CheckPassCondition()
        {
            if (HasStart && GetPassCondition()) // 这里认为达到条件即可进行下一关
            {
                InvokeLevelEndEvents();
                NextLevel();
            }
        }

        protected void InvokeLevelEndEvents()
        {
            OnLevelEnd?.Invoke();
            if (curLevel >= allLevelData.Count)
            {
                OnLastLevelEnd?.Invoke();
                // GameManager.instance.GameWin();
            }
        }
    }
}