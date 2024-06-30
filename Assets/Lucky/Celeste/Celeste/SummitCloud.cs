using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Celeste.Celeste
{
    public class SummitCloud : MonoBehaviour
    {
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = Instantiate(Resources.Load<SpriteRenderer>("Components/SpriteRenderer"), transform);
            sr.sprite = Resources.Load<Sprite>("Graphics/Celeste/Gameplay/Decals/summit/cloud_cb");

            SineWave sineWave = gameObject.AddComponent<SineWave>();
            sineWave.Init(Random.Range(0.05f, 0.1f));
            sineWave.Randomize();
            sineWave.OnUpdate = f => { sr.transform.localPosition = sr.transform.localPosition.WithY(f * 8f); };
        }
    }
}