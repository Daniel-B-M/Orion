using UnityEngine;
using System.Collections.Generic;

public class MixManager : MonoBehaviour
{

    public enum MixResult
    {
        Success,
        CriticalFail,
        CommonFail
    }

    [System.Serializable]
    public class Recipe
    {
        public int[] ingredientsIds;
        public string resultName;
        public Sprite icon;
    }
    [SerializeField] private List<Recipe> recipes;

    public class MixOutcome
    {
        public MixResult result;
        public string resultName;
        public Recipe matchedRecipe;
    }

    public MixOutcome GetMixResult(int[] mixedIds)
    {

        System.Array.Sort(mixedIds);
        foreach (Recipe recipe in recipes)
        {
            if (mixedIds [0] == recipe.ingredientsIds[0] && 
                mixedIds [1] == recipe.ingredientsIds[1] && 
                mixedIds [2] == recipe.ingredientsIds[2])
            {
                MixOutcome outcome = new MixOutcome();
                outcome.result = MixResult.Success;
                outcome.resultName = recipe.resultName;
                outcome.matchedRecipe = recipe;
                return outcome;
            } 
        }

        bool containsId4 = false;
        foreach (int id in mixedIds)
        {
            if (id == 4)
            {
                containsId4 = true;
            }
        }

        if (containsId4)
        {
            MixOutcome outcome = new MixOutcome();
            outcome.result = MixResult.CriticalFail;
            outcome.resultName = "Critical Fail";
            return outcome;
        }
        else
        {
            MixOutcome outcome = new MixOutcome();
            outcome.result = MixResult.CommonFail;
            outcome.resultName = "Common Fail";
            return outcome;
        }
    }
    public Recipe GetRandomRecipe()
    {
        int randomIndex = Random.Range(0, recipes.Count);
        return recipes[randomIndex];
    }
}
