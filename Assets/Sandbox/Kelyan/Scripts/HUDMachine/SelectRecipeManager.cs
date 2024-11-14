using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRecipeManager : MonoBehaviour
{

    [Header("Prefab")]
    [SerializeField] private GameObject prefabButton;
    [SerializeField] private GameObject prefabOverPanel;

    [Header("Container")]
    [SerializeField] private Transform containerButtonRecipe;
    [SerializeField] private Transform containerOverRecipe;

    [Header("Index Recipe")]
    private int currentIndex = -1;

    private void Start()
    {
        MachineHUDManager.Instance.onHover += Hover;
    }

    public void InitRecipesMenu(List<SORecipe> recipes)
    {
        CreateRecipesButtons(recipes);//crée les boutons pour le hover
        CreateOverRecipe(recipes);//crée les panel sur la droite dans le même ordre que les boutons
    }

    private void CreateRecipesButtons(List<SORecipe> recipes)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject newButton = Instantiate(prefabButton, containerButtonRecipe);//instatiation du boutton
            newButton.GetComponent<RecipeButton>().Init(recipes[i]);//get le code pour init le bouton avec la recipe

            SORecipe currentRecipe = recipes[i];//on met la recipe dans un variable temp pour éviter la ref qui change du au i
            newButton.GetComponent<Button>().onClick.AddListener(() => MachineHUDManager.Instance.onClickRecipe?.Invoke(currentRecipe));//si le bouton est cliqué invoke les funcs
                                                                                                                                       //(change vers prod menu et donne le bon SORecipe au prod menu pour qu'il se construise)
        }
    }

    private void CreateOverRecipe(List<SORecipe> recipes)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject newButton = Instantiate(prefabOverPanel, containerOverRecipe);//instantiation du prefab du panel
            newButton.GetComponent<OverRecipeButton>().Init(recipes[i]);//get le code pour init le panel avec la recipe
            newButton.SetActive(false);
        }
    }

    public void CleanSelectRecipeMenu()
    {
        for (int i = 0; i < containerButtonRecipe.childCount; i++)
        {
            Destroy(containerButtonRecipe.GetChild(i).gameObject);
            Destroy(containerOverRecipe.GetChild(i).gameObject);
            currentIndex = -1;
        }
    }

    private void Hover(int index)
    {
        if (index >= 0)//l'index est dans les boutons créés
        {
            containerOverRecipe.GetChild(index).gameObject.SetActive(true);//on prend le bouton en hover et on active le panel

            if (currentIndex != -1)//on verifie que la souris était ou pas sur un bouton avant
            {
                containerOverRecipe.GetChild(currentIndex).gameObject.SetActive(false);//on desactive le bouton précédent
            }
            currentIndex = index;//changement du current index bouton selectioné
        }
        else// index = -1 donc on n'est sur aucun bouton
        {
            if (currentIndex != -1)//verifie que l'on était déjà plus sur un bouton
            {
                containerOverRecipe.GetChild(currentIndex).gameObject.SetActive(false);//on accèdes au bouton qui perd le focus
                currentIndex = -1;//et on réassigne le current index a -1 pour le vide
            }
        }
    }
}