using UnityEngine;
using System.Collections.Generic;


// Sistema de mezclas: dado un conjunto de 3 IDs de ingredientes, determina
// qué bebida resultó (o si fue un fallo). No sabe nada de interacción del
// jugador ni de UI: solo recibe IDs y devuelve un resultado (MixOutcome).
// El orden en que se mezclan los ingredientes NO importa, solo la combinación.
public class MixManger : MonoBehaviour
{

    public enum MixResult
    {
        // Categoría del resultado de una mezcla.
        Success,
        CriticalFail,
        CommonFail
    }
    // Una receta conocida: qué 3 IDs la componen y qué nombre tiene el resultado.
    // Se carga y edita a mano desde el Inspector (lista "Recipes").
    [System.Serializable]
    public class Recipe
    {
        public int[] ingredientsIds;
        public string resultName;
    }
    [SerializeField] private List<Recipe> recipes;

    // Objeto que se devuelve como resultado de una mezcla: categoría + nombre.
    // Se crea en tiempo de ejecución, no se edita en el Inspector.
    public class MixOutcome
    {
        public MixResult result;
        public string resultName;
    }
    // Punto de entrada del sistema. Recibe los 3 IDs que el jugador mezcló
    // y devuelve el resultado correspondiente:
    // 1) Si coincide con una receta conocida -> Success.
    // 2) Si no coincide y contiene el ingrediente "malo" (ID 4) -> CriticalFail.
    // 3) Si no coincide y no contiene el ID 4 -> CommonFail.
    public MixOutcome GetMixResult(int[] mixedIds)
    {
        // Se ordena para poder comparar sin importar el orden en que se mezcló.
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
                return outcome;
            } 
        }
         // Ninguna receta coincidió: se revisa si el ingrediente "malo" (ID 4) está presente.
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
}
