using UnityEngine;
using System;
using System.Collections.Generic;

namespace Alchemy.Model
{

    #region Перечисления
    public enum PotionType
    {
        Fire_I,
        Dream_I
    }
    public enum IngredientType
    {
        Root,
        Wood,
        Mushroom
    }
    public enum LevelType
    {
        Forest,
        Mine,
        Lake
    }
    public enum DifficultyType
    {
        I,
        II,
        III
    }
    public enum ResourceType
    {
        // Лес
        Wood,
        GreenGrass,
        RedGrass,
        // Озеро
        Star,
        Coral,
        Fish,
        Shell
    }

    #endregion

    #region Модели

    public class ResourceLevelModel
    {
        public ResourceType resourceType;
        public Sprite sprite;
        public float low_boundary; // нижняя границы диапазона, входящего в [0.00; 100.00].
        public float upper_boundary; // верхняя граница.
    }

    public class LevelModel
    {
        public LevelType levelType;
        public DifficultyType difficultyType;
        public List<PotionType> potions;

        // ресурсы.
        public List<ResourceLevelModel> resources;
    }

    public class LevelPreviewViewModel
    {
        public string levelName;
        public LevelType levelType;
        public DifficultyType difficultyType;
        public Sprite levelImage;
        public List<Potion> potionBonuses;
        public List<Difficulty> difficultyLvls;
    }

    public class RecipeViewModel
    {
        public PotionType potionType;
        public string potionName;
        public string potionDescription;
        public Sprite potionSprite;
        public List<RequiredRecipePotion> requiredPotions;
        public List<RequiredRecipeIngredient> requiredIngredients;
    }

    public class InventoryItemViewModel
    {
        public Sprite sprite;
        public int count;
    }

    public class RequiredRecipeIngredient
    {
        public IngredientType type;
        public int requiredCount;
        public string name;
        public string description;
        public Sprite sprite;
    }

    public class RequiredRecipePotion
    {
        public PotionType type;
        public int requiredCount;
        public string name;
        public string description;
        public Sprite sprite;
    }
    #endregion

    #region Сериализуемые модели для определение структур данных.

    [Serializable]
    public class ResourceToLevel
    {
        public LevelType levelType;
        public DifficultyType difficultyType;
        public ResourceType resourceType;
        public float low_boundary; // нижняя границы диапазона, входящего в [0.00; 100.00].
        public float top_boundary; // верхняя граница.
    }

    [Serializable]
    public class IngredientCost
    {
        public ResourceType resourceType;
        public IngredientType ingredientType;
        public int count;
    }

    [Serializable]
    public class Resource
    {
        public ResourceType resourceType;
        public Sprite sprite;
    }

    [Serializable]
    public class Difficulty
    {
        public Sprite icon;
        public DifficultyType type;
    }

    [Serializable]
    public class Level
    {
        public LevelType levelType;
        public DifficultyType difficultyType;
        public string levelName;
        public Sprite levelImage;
        public List<PotionType> potionBonuses;

        public List<ResourceType> availableResources;
        
    }

    [Serializable]
    public class PlayerData
    {
        public Dictionary<IngredientType, int> ingredients = new Dictionary<IngredientType, int>();
        public Dictionary<PotionType, int> potions = new Dictionary<PotionType, int>();
    }

    [Serializable]
    public class RecipeIngredient
    {
        public IngredientType type;
        public int count;
    }

    [Serializable]
    public class RecipePotion
    {
        public PotionType type;
        public int count;
    }

    [Serializable]
    public class Recipe
    {
        public PotionType potionType;
        public int productionCount;
        public List<RecipeIngredient> requiredIngredients;
        public List<RecipePotion> requiredPotions;
    }

    [Serializable]
    public class Ingredient
    {
        public IngredientType type;
        public string name;
        public string description;
        public Sprite sprite;
    }

    [Serializable]
    public class Potion
    {
        public PotionType type;
        public string name;
        public string description;
        public Sprite sprite;
    }
    #endregion

    [Serializable]
    public class ApplicationData
    {
        [SerializeField]
        public List<Ingredient> ingredientList;
        [SerializeField]
        public List<Potion> potionList;
        [SerializeField]
        public List<Recipe> recipeList;
        [SerializeField]
        public List<Level> levelList;
        [SerializeField]
        public List<Difficulty> levelDifficultyTypesList;
        [SerializeField]
        public List<Resource> resources;
        [SerializeField]
        public List<ResourceToLevel> resourcesToLevel;
        [SerializeField]
        public List<IngredientCost> ingredientCosts;
    }
}