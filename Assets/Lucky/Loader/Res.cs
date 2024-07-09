using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Loader
{
    public static class Res
    {
        public static T Load<T>(string path) where T : Object => Resources.Load<T>(path);

        public static List<Sprite> LoadSubtextures(string path, int maxLength = 2)
        {
            List<Sprite> res = new();
            string[] str = path.Split('/');
            string s = str[^1];
            int n = 1 << maxLength;
            for (int i = 0; i < n; i++)
            {
                str[^1] = s + i.ToString().PadLeft(maxLength, '0');
                Sprite sprite = Load<Sprite>(string.Join('/', str));
                if (sprite != null)
                    res.Add(sprite);
                else
                    break;
            }

            return res;
        }
        // public static Building circleBuildingPrefab = Resources.Load<Building>("Prefabs/CircleBuilding");
        // public static Building triangleBuildingPrefab = Resources.Load<Building>("Prefabs/TriangleBuilding");
        // public static Building squareBuildingPrefab = Resources.Load<Building>("Prefabs/SquareBuilding");
        //
        // public static Soldier soldierPrefab = Resources.Load<Soldier>("Prefabs/Soldier");
        //
        // public static Enemy enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");
    }
}