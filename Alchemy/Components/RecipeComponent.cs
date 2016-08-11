using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alchemy
{
    public class RecipeComponent : MonoBehaviour
    {
        //public List<RecipeIngredient> recipeIngredients;
        //public List<RecipePotion> recipePotions;
        //public PotionType1 resultPotionType;
        //public int resultCount;
        
        ///// <summary>
        ///// Проверка: хвататет ли ингредиентов для приготовления.
        ///// </summary>
        //private bool IsEnoughIngredients()
        //{
        //    if (recipeIngredients.Count == 0)
        //        return true;

        //    foreach (var ingredient in recipeIngredients)
        //    {
        //        if (!GameManager.gameManager.IsEnoughOfIngredient(ingredient.type, ingredient.count))
        //            return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// Проверка: хватает ли зелий для приготовления.
        ///// </summary>
        //private bool IsEnoughPotions()
        //{
        //    if (recipePotions.Count == 0)
        //        return true;

        //    foreach (var potion in recipePotions)
        //    {
        //        if (!GameManager.gameManager.IsEnoughOfPotion(potion.type, potion.count))
        //            return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// Метод, "приготовляющий" зелье.
        ///// </summary>
        //public bool Cook()
        //{
        //    if (IsEnoughIngredients() && IsEnoughPotions())
        //    {
        //        if (recipeIngredients.Count != 0)
        //        {
        //            recipeIngredients.ForEach(i =>
        //            {
        //                GameManager.gameManager.UseIngredient(i.type, i.count);
        //            });
        //        }

        //        if (recipePotions.Count != 0)
        //        {
        //            recipePotions.ForEach(i =>
        //            {
        //                GameManager.gameManager.UsePotion(i.type, i.count);
        //            });
        //        }
        //    }
        //    return false;
        //}
    }
}
