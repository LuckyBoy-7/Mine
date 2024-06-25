using System.Collections.Generic;
using UnityEngine;

namespace Lucky.UI.TMP_Text
{
    [CreateAssetMenu(menuName = "Dialogue_SO")]
    public class Dialogue_SO : ScriptableObject
    {
        [TextArea(3, 10)] public List<string> contents;
    }
}