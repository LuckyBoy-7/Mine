using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Celeste.Celeste
{
    public class Leader : MonoBehaviour
    {
        public const int MaxPastPoints = 350;
        public List<Follower> Followers;
        public List<Vector2> PastPoints;
        public Vector2 Position;
        // private static List<Strawberry> storedBerries;
        private static List<Vector2> storedOffsets;

        private void Awake()
        {
            Followers = new List<Follower>();
            PastPoints = new List<Vector2>();
        }

        // 注册follower
        public void GainFollower(Follower follower)
        {
            Followers.Add(follower);
            follower.OnGainLeaderUtil(this);
        }

        // 注销follower
        public void LoseFollower(Follower follower)
        {
            Followers.Remove(follower);
            follower.OnLoseLeaderUtil();
        }

        // 注销全部follower
        public void LoseFollowers()
        {
            foreach (Follower follower in Followers)
            {
                follower.OnLoseLeaderUtil();
            }

            Followers.Clear();
        }

        /// <summary>
        /// 记录历史位置，lerp follower的位置
        /// </summary>
        public void Update()
        {
            Vector2 vector = transform.position;
            // 维护一个leader位置的序列（按时间排序），当与上次位置相隔一定距离才加入（不然不动就会一直加）
            if (Timer.OnInterval(0.02f) && (PastPoints.Count == 0 || (vector - PastPoints[0]).magnitude >= 3f))
            {
                PastPoints.Insert(0, vector);
                if (PastPoints.Count > MaxPastPoints)
                    PastPoints.RemoveAt(PastPoints.Count - 1);
            }

            int num = 5;
            foreach (Follower follower in Followers)
            {
                if (num >= PastPoints.Count)
                    break;

                Vector2 vector2 = PastPoints[num];
                if (follower.DelayTimer <= 0f && follower.MoveTowardsLeader)
                {
                    // 大概就是随着past position做了个lerp的感觉（我说怎么好像草莓是按既定路线移动但又好像有点偏的感觉呢）
                    follower.transform.position += ((Vector3)vector2 - follower.transform.position) * (1f - (float)Math.Pow(0.01f, Time.deltaTime));
                }

                num += 5;
            }
        }

        // player死亡或其他transition的时候调用
        // public void TransferFollowers()
        // {
        //     for (int i = 0; i < Followers.Count; i++)
        //     {
        //         Follower follower = Followers[i];
        //         if (!follower.Entity.TagCheck(Tags.Persistent))
        //         {
        //             LoseFollower(follower);
        //             i--;
        //         }
        //     }
        // }

        // 在一些跨场景又带剧情的地方使用，比如5a变新浪那里，如果点完两个火把而跳过剧情，那么就要store，不然草莓就会从很远处飞过来
        // public static void StoreStrawberries(Leader leader)
        // {
        //     storedBerries = new List<Strawberry>();
        //     storedOffsets = new List<Vector2>();
        //     foreach (Follower follower in leader.Followers)
        //     {
        //         Strawberry s = follower.GetComponent<Strawberry>();
        //         if (s)
        //         {
        //             storedBerries.Add(s);
        //             storedOffsets.Add(s.transform.position - leader.transform.position);
        //         }
        //     }
        //
        //     foreach (Strawberry strawberry in storedBerries)
        //     {
        //         leader.Followers.Remove(strawberry.Follower);
        //         strawberry.Follower.Leader = null;
        //     }
        // }
        //
        // public static void RestoreStrawberries(Leader leader)
        // {
        //     leader.PastPoints.Clear();
        //     for (int i = 0; i < storedBerries.Count; i++)
        //     {
        //         Strawberry strawberry = storedBerries[i];
        //         leader.GainFollower(strawberry.Follower);
        //         strawberry.transform.position = leader.Position + storedOffsets[i];
        //     }
        // }
    }
}