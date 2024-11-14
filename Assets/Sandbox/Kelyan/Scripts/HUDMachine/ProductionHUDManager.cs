using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionHUDManager : MonoBehaviour
{
    private float currentMeltTimer;
    private float maxTimerCurrentRecipe;
    private bool isMachineProducing = false;
    private List<SOItems> itemsAccepted;

    [Header("Prod Panel Data")]
    [SerializeField] private Slider sliderMeltTimer;
    [SerializeField] private TextMeshProUGUI meltTimeRecipeText;
    [SerializeField] private TextMeshProUGUI ElecRecipeText;
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Image imageOutput;
    [SerializeField] private GameObject prefabItemHUD;
    [SerializeField] private Transform transformContainerInput;
    [SerializeField] private Transform transformContainerOutput;

    [Header("Led Data")]
    [SerializeField] private Image leftLed;
    [SerializeField] private Image rightLed;
    [SerializeField] private Sprite redLedSprite;
    [SerializeField] private Sprite offLedSprite;
    [SerializeField] private Sprite greenLedSprite;

    public void InitProductionMenu(SORecipe currentRecipe, float currentTimer, bool isAlimented)
    {
        MachineHUDManager.Instance.onInputStackChange += UpdateInputStack;
        MachineHUDManager.Instance.onOutputStackChange += UpdateOutputStack;
        MachineHUDManager.Instance.onMachineStateChange += UpdateMachineState;
        MachineHUDManager.Instance.onMaxTimerChange += UpdateMaxTimer;
        MachineHUDManager.Instance.onIsAlimentedChange += UpdateIsAlimentedLed;

        currentMeltTimer = currentTimer;

        if(itemsAccepted == null) 
        {
            itemsAccepted = new List<SOItems>();      
        }

        if (currentRecipe != null)
        {
            recipeNameText.text = currentRecipe.NameRecipe;
            meltTimeRecipeText.text = currentRecipe.MeltTime.ToString() + " Sec";
            ElecRecipeText.text = currentRecipe.ElectricityCost.ToString() + " Watts";
            imageOutput.sprite = currentRecipe.ItemOutput.Sprite;

            //for (int i = 0; i < currentRecipe.ItemsInput.Count; i++)
            //{
            //    itemsAccepted.Add(currentRecipe.ItemsInput[i].Item);
            //}

            for (int i = 0; i < currentRecipe.ItemsInput.Count; i++)
            {
                GameObject newInputItem = Instantiate(prefabItemHUD, transformContainerInput);
                newInputItem.GetComponent<ItemHUD>().Init(currentRecipe.ItemsInput[i].ValueStack, currentRecipe.MeltTime, currentRecipe.ItemsInput[i].Item);
            }

            GameObject newOutputItem = Instantiate(prefabItemHUD, transformContainerOutput);

            itemsAccepted.Clear();
            //itemsAccepted.Add(currentRecipe.ItemOutput);
            newOutputItem.GetComponent<ItemHUD>().Init(currentRecipe.ValueStackOutput, currentRecipe.MeltTime, currentRecipe.ItemOutput);

            if(isAlimented) 
            {
                leftLed.sprite = greenLedSprite;
                rightLed.sprite = offLedSprite;
            }
            else
            {
                leftLed.sprite = offLedSprite;
                rightLed.sprite = redLedSprite;
            }
        }
    }

    private void UpdateTimer()
    {
        currentMeltTimer += Time.deltaTime;
        if (currentMeltTimer >= maxTimerCurrentRecipe)
        {
            currentMeltTimer = 0.0f;
        }
    }

    private void UpdateMaxTimer(float maxTimer)
    {
        maxTimerCurrentRecipe = maxTimer;
    }

    private void UpdateInputStack(List<int> inputStackList)
    {
        for (int i = 0; i < inputStackList.Count; i++)
        {
            transformContainerInput.GetChild(i).GetComponent<ItemHUD>().SetStackValue(inputStackList[i]);
        }
    }

    private void UpdateOutputStack(List<int> outputStackList)
    {
        for (int i = 0; i < outputStackList.Count; i++)
        {
            transformContainerOutput.GetChild(i).GetComponent<ItemHUD>().SetStackValue(outputStackList[i]);
        }
    }

    private void UpdateMachineState(bool isMachineProducing)
    {
        this.isMachineProducing = isMachineProducing;
    }
    private void UpdateIsAlimentedLed(bool isAlimented)
    {
        if (isAlimented)
        {
            leftLed.sprite = greenLedSprite;
            rightLed.sprite = offLedSprite;
        }
        else
        {
            leftLed.sprite = offLedSprite;
            rightLed.sprite = redLedSprite;
        }
    }

    public void UpdateUIMachine(SORecipe selectedRecipe, float currentTimer, bool isAlimented)
    {
        if (transformContainerOutput.childCount <= 0)
        {
            InitProductionMenu(selectedRecipe, currentTimer, isAlimented);
        }
        else
        {
            if (itemsAccepted != null) 
            {
                itemsAccepted.Clear();
            }

            for (int i = 0; i < selectedRecipe.ItemsInput.Count; i++)
            {
                itemsAccepted.Add(selectedRecipe.ItemsInput[i].Item);
            }

            ItemHUD tempItemHUD;

            for (int i = 0; i < transformContainerInput.childCount; i++)
            {
                Transform tempInputTransform = transformContainerInput.GetChild(i);
                tempItemHUD = tempInputTransform.GetComponent<ItemHUD>();
                if (tempItemHUD.GetItemType() != selectedRecipe.ItemsInput[i].Item)
                {
                    Player.Instance.AddItem(tempItemHUD.GetItemType(), tempItemHUD.stack);
                    tempItemHUD.SetStackValue(0);
                    tempItemHUD.Init(selectedRecipe.ItemsInput[i].ValueStack, selectedRecipe.MeltTime, selectedRecipe.ItemsInput[i].Item, itemsAccepted);
                }
            }

            tempItemHUD = transformContainerOutput.GetChild(0).GetComponent<ItemHUD>();
            if(tempItemHUD.GetItemType() != selectedRecipe.ItemOutput)
            {
                Player.Instance.AddItem(tempItemHUD.GetItemType(), tempItemHUD.stack);
                tempItemHUD.SetStackValue(0);
                tempItemHUD.Init(selectedRecipe.ValueStackOutput, selectedRecipe.MeltTime, selectedRecipe.ItemOutput);
            }            

            currentMeltTimer = currentTimer;
            recipeNameText.text = selectedRecipe.NameRecipe;
            meltTimeRecipeText.text = selectedRecipe.MeltTime.ToString() + " Sec";
            ElecRecipeText.text = selectedRecipe.ElectricityCost.ToString() + " Watts";
            imageOutput.sprite = selectedRecipe.ItemOutput.Sprite;
        }
    }

    private void Update()
    {
        if (isMachineProducing)
        {
            UpdateTimer();
        }
        if (maxTimerCurrentRecipe > 0.0f)
        {
            sliderMeltTimer.value = (currentMeltTimer * 100) / maxTimerCurrentRecipe;
        }
    }

    public void CleanProductionMenu()
    {
        recipeNameText.text = "";
        meltTimeRecipeText.text = " Sec";
        imageOutput.sprite = null;
        if (itemsAccepted != null)
        {
            itemsAccepted.Clear();
        }
        MachineHUDManager.Instance.onInputStackChange -= UpdateInputStack;
        MachineHUDManager.Instance.onOutputStackChange -= UpdateOutputStack;
        MachineHUDManager.Instance.onMachineStateChange -= UpdateMachineState;
        MachineHUDManager.Instance.onIsAlimentedChange -= UpdateIsAlimentedLed;

        for (int i = 0; i < transformContainerInput.childCount; i++)
        {
            Destroy(transformContainerInput.GetChild(i).gameObject);
        }

        for (int i = 0; i < transformContainerOutput.childCount; i++)
        {
            Destroy(transformContainerOutput.GetChild(i).gameObject);
        }
    }
}