using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverRecipeButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameRecipe;
    [SerializeField] private Image iconOutput;
    [SerializeField] private TextMeshProUGUI valueStackOutput;
    [SerializeField] private Transform containerInput;

    [Header("Info")]
    [SerializeField] private TextMeshProUGUI meltTime;
    [SerializeField] private TextMeshProUGUI electricityCost;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Prefab")]
    [SerializeField] private GameObject input;

    public void Init(SORecipe recipe)//jutse un init
    {
        //ouptut
        nameRecipe.text = recipe.NameRecipe;
        iconOutput.sprite = recipe.ItemOutput.Sprite;
        valueStackOutput.text = recipe.ValueStackOutput.ToString();

        //input
        for (int i = 0; i < recipe.ItemsInput.Count; i++)
        {
            GameObject newInput = Instantiate(input, containerInput);
            newInput.GetComponent<Image>().sprite = recipe.ItemsInput[i].Item.Sprite;
            newInput.GetComponentInChildren<TextMeshProUGUI>().text = recipe.ItemsInput[i].ValueStack.ToString();
        }

        //info
        meltTime.text = recipe.MeltTime.ToString() + " sec";
        electricityCost.text = recipe.ElectricityCost.ToString();
        description.text = recipe.Description;
    }
}