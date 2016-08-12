using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Model;

namespace Alchemy
{
    /// <summary>
    /// Класс используется для управления всеми конфигурационными параметрами приложения (прогресс игрока, его предметы и т.д.)
    /// </summary>
    public partial class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public ApplicationData applicationData;
        private PlayerData playerData;

        private void Awake()
        {
            if (Instance == null)
            {
                // Не уничтожать объект при переходе между сценами.
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            //else if (gameManager != this)
            //{
            //    Destroy(gameObject);
            //}

            // Удалить. Для теста.
            playerData = new PlayerData();
            playerData.ingredients = new Dictionary<IngredientType, int>();

            playerData.ingredients.Add(IngredientType.Mushroom, 100);
            playerData.ingredients.Add(IngredientType.Root, 100);
            playerData.ingredients.Add(IngredientType.Wood, 100);

            playerData.potions = new Dictionary<PotionType, int>();
            playerData.potions.Add(PotionType.Fire_I, 5);

            // Загружаем данные пользователя.
            LoadUserData();
        }

        #region ApplicationData
        /// <summary>
        /// Cписок зелий.
        /// </summary>
        public List<Potion> PotionList
        {
            get
            {
                return applicationData.potionList;
            }
        }

        /// <summary>
        /// Cписок рецептов.
        /// </summary>
        public List<Recipe> RecipeList
        {
            get
            {
                return applicationData.recipeList;
            }
        }

        /// <summary>
        /// Cписок игредиентов.
        /// </summary>
        public List<Ingredient> IngredientList
        {
            get
            {
                return applicationData.ingredientList;
            }
        }

        /// <summary>
        /// Получение модельки для отображения данных по рецепту.
        /// </summary>
        /// <param name="potionType"></param>
        /// <returns></returns>
        public RecipeViewModel GetRecipeViewModel(PotionType potionType)
        {
            var recipe = applicationData.recipeList.First(i => i.potionType == potionType);
            var potion = applicationData.potionList.First(i => i.type == potionType);
            return new RecipeViewModel()
            {
                potionType = potionType,
                potionName = potion.name,
                potionDescription = potion.description,
                potionSprite = potion.sprite,
                requiredIngredients = GetRequiredRecipeIngredients(recipe),
                requiredPotions = GetRequiredRecipePotions(recipe)
            };
        }

        /// <summary>
        /// Список всех необходимых для приготовления ингредиентов со спрайтами и т.д.
        /// </summary>
        private List<RequiredRecipeIngredient> GetRequiredRecipeIngredients(Recipe recipe)
        {
            return (
                from i in applicationData.ingredientList
                join ri in recipe.requiredIngredients on i.type equals ri.type
                select new RequiredRecipeIngredient()
                {
                    type = i.type,
                    requiredCount = ri.count,
                    name = i.name,
                    description = i.description,
                    sprite = i.sprite
                }).ToList();
        }

        /// <summary>
        /// Список всех необходимых для приготовления зелий.
        /// </summary>
        private List<RequiredRecipePotion> GetRequiredRecipePotions(Recipe recipe)
        {
            return (
                from p in applicationData.potionList
                join rp in recipe.requiredPotions on p.type equals rp.type
                select new RequiredRecipePotion()
                {
                    type = p.type,
                    requiredCount = rp.count,
                    description = p.description,
                    name = p.name,
                    sprite = p.sprite
                }
            ).ToList();
        }
        #endregion

        #region PlayerData

        #region public...
        /// <summary>
        /// Получение ингредиентов и зелий игрока.
        /// </summary>
        /// <returns></returns>
        public List<InventoryItemViewModel> GetPlayerInventory()
        {
            return (
                from i in playerData.ingredients
                join iad in applicationData.ingredientList on i.Key equals iad.type
                select new InventoryItemViewModel()
                {
                    sprite = iad.sprite,
                    count = i.Value
                }
                
            ).Union(
                from p in playerData.potions
                join pad in applicationData.potionList on p.Key equals pad.type
                select new InventoryItemViewModel()
                {
                    sprite = pad.sprite,
                    count = p.Value
                }
            )
            .ToList();
        }

        /// <summary>
        /// Проверяет возможность приготовления зелья.
        /// </summary>
        public bool CheckCooking(PotionType potionType)
        {
            var recipe = applicationData.recipeList.First(i => i.potionType == potionType);

            // Проверка ингредиентов.
            foreach (var ingredient in recipe.requiredIngredients)
            {
                if (!playerData.ingredients.ContainsKey(ingredient.type))
                    return false;

                if (playerData.ingredients[ingredient.type] < ingredient.count)
                    return false;
            }

            // Проверка зелий.
            foreach (var potion in recipe.requiredPotions)
            {
                if (!playerData.potions.ContainsKey(potion.type))
                    return false;

                if (playerData.potions[potion.type] < potion.count)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Приготовление.
        /// </summary>
        public void Cook(PotionType potionType)
        {
            var recipe = applicationData.recipeList.First(i => i.potionType == potionType);

            // Использование нужных ингредиентов.
            recipe.requiredIngredients.ForEach(i =>
            {
                UseIngredient(i.type, i.count);
            });

            // Использование зелий.
            recipe.requiredPotions.ForEach(p =>
            {
                UsePotion(p.type, p.count);
            });

            // Создание указанного в рецепте количества зелий.
            AddPotion(potionType, recipe.productionCount);
        }
        #endregion

        #region private...
        private void UseIngredient(IngredientType type, int count)
        {
            playerData.ingredients[type] -= count;
            SaveUserData();
        }

        private void UsePotion(PotionType type, int count)
        {
            playerData.potions[type] -= count;
            SaveUserData();
        }

        private void AddIngredient(IngredientType type, int count)
        {
            if (!playerData.ingredients.ContainsKey(type))
                playerData.ingredients.Add(type, count);

            playerData.ingredients[type] += count;
            SaveUserData();
        }

        private void AddPotion(PotionType type, int count)
        {
            if (!playerData.potions.ContainsKey(type))
                playerData.potions.Add(type, count);

            playerData.potions[type] += count;
            SaveUserData();
        }

        /// <summary>
        /// Сохранение данных пользователя.
        /// </summary>
        private void SaveUserData()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/alApplicationInfo.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, playerData);
            file.Close();
        }

        /// <summary>
        /// Загрузить пользовательские данные из файла.
        /// </summary>
        private void LoadUserData()
        {
            if (File.Exists(Application.persistentDataPath + "/alApplicationInfo.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/alApplicationInfo.dat", FileMode.Open);
                playerData = (PlayerData)bf.Deserialize(file);
                file.Close();
            }
        }
        #endregion

        #endregion

        #region LevelInfo

        /// <summary>
        /// Данные модели используются для построения уровня.
        /// </summary>
        public LevelModel LevelModel { get; set; }

        /// <summary>
        /// Возвращение модели для отображения preview-окна уровня.
        /// </summary>
        public LevelPreviewViewModel GetLevelPreviewViewModel(LevelType lvlType, DifficultyType dfcType)
        {
            var lvl = applicationData.levelList.First(i => i.levelType == lvlType && i.difficultyType == dfcType);
            var potionList = (
                from pb in lvl.potionBonuses
                join p in applicationData.potionList on pb equals p.type
                select p
            ).ToList();

            return new LevelPreviewViewModel()
            {
                levelType = lvlType,
                difficultyType = dfcType,
                levelImage = lvl.levelImage,
                levelName = lvl.levelName,
                potionBonuses = potionList,
                difficultyLvls = applicationData.levelDifficultyTypesList
            };
        }

        /// <summary>
        /// Задает концигурацию нового уровня.
        /// </summary>
        public void SaveNewLevelSettings(LevelType lvlType, DifficultyType dflcType, List<PotionType> potionBonuses)
        {
            LevelModel.potions = potionBonuses;
            LevelModel.difficultyType = dflcType;
            LevelModel.levelType = lvlType;
        }

        #endregion

        public void OnGUI()
        {
            
        }
    }
}