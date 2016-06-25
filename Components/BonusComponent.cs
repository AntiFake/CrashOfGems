using UnityEngine;
using CrashOfGems.Enums;

namespace CrashOfGems.Components
{
    /// <summary>
    /// Компонент-бонуса определенного типа.
    /// Компонент назначается блоку на поле.
    /// </summary>
    public class BonusComponent : MonoBehaviour
    {
        public BonusType type;
        public int value;
    }
}
