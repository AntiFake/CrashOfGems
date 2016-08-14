using UnityEngine;
using System.Collections.Generic;
using Alchemy.Model;

namespace Alchemy.UI
{
    public class UILevelChoiceComponent : MonoBehaviour
    {
        public GameObject levelChoiceItemInstance;

        public void Visualize(List<Difficulty> difficultyLvls, LevelType lvlType)
        {
            foreach (var lvlDflc in difficultyLvls)
            {
                var level = Instantiate(levelChoiceItemInstance);
                level.GetComponent<UILevelChoiceItemComponent>().Visualize(lvlDflc.icon, lvlDflc.type, lvlType);
                level.transform.SetParent(gameObject.transform, false);
                level.SetActive(true);
            }
        }

        public void Devisualize()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var childItem = gameObject.transform.GetChild(i);

                if (!childItem.gameObject.activeSelf)
                    continue;

                Destroy(childItem.gameObject);
            }
        }

        /// <summary>
        /// Обеспечивает получение информации о выбранном уровне и сложности.
        /// </summary>
        public void GetSelectedDifficultyAndLevelTypes(out LevelType levelType, out DifficultyType difficultyType)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var childItem = gameObject.transform.GetChild(i);

                if (!childItem.gameObject.activeSelf)
                    continue;

                var comp = childItem.GetComponent<UILevelChoiceItemComponent>();

                if (comp.isSelected)
                {
                    levelType = comp.LevelType;
                    difficultyType = comp.DifficultyType;
                    return;
                }
            }

            // default out.
            levelType = LevelType.Forest;
            difficultyType = DifficultyType.I;
        }
    }
}
