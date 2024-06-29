using System;
using UnityEngine;

namespace Lucky.Celeste.Celeste
{
    public class Follower : MonoBehaviour
    {
        public int FollowIndex
        {
            get
            {
                if (Leader == null)
                {
                    return -1;
                }

                return Leader.Followers.IndexOf(this);
            }
        }

        public Leader Leader;
        public float FollowDelay = 0.5f;
        public float DelayTimer;
        public bool MoveTowardsLeader;

        public void Awake()
        {
            FollowDelay = 0.5f;
            MoveTowardsLeader = true;
        }

        public void Update()
        {
            if (DelayTimer > 0f)
            {
                DelayTimer -= Time.deltaTime;
            }
        }

        public void OnLoseLeaderUtil()
        {
            Leader = null;
        }

        public void OnGainLeaderUtil(Leader leader)
        {
            Leader = leader;
            DelayTimer = FollowDelay;
        }
    }
}