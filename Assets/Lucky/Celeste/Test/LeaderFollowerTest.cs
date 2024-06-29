using System;
using System.Collections.Generic;
using Lucky.Celeste.Celeste;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Celeste.Test
{
    public class LeaderFollowerTest : MonoBehaviour
    {
        public Leader leader;
        public List<Follower> followers;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && followers.Count > 0)
            {
                leader.GainFollower(followers.Pop(-1));
            }
        }
    }
}