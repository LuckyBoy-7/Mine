using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mine.Managers
{
    /// <summary>
    /// 在这里有点像一个测试类，如果在游戏里要写一个类似的
    /// </summary>
    public class LevelManager: LevelManagerBase
    {
        private List<GameObject> gameobjects = new();
        private void Start()
        {
            StartLevel();
        }


        protected override void LoadLevel(int level)
        {
            
        }

        protected override bool GetPassCondition()  // 简单的测试，我会手动销毁gameObjects
        {
            return true;
        }
    }
}