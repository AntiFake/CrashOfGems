using UnityEngine;
using System.Collections;
using System;

namespace Match3.Components
{
    public enum BonusType { None, Bomb, Light, Multiplication }

    public class BonusComponent : MonoBehaviour
    {
        public BonusType type;
        public int value;
    }
}
