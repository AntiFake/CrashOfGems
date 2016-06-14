using UnityEngine;
using System.Collections;
using System;

namespace Match3.Components
{
    public enum BonusType { None, Bomb, Light, Multiplication }

    [Serializable]
    public class BonusComponent
    {
        [SerializeField]
        public BonusType type;

        [SerializeField]
        public int value;
    }
}
