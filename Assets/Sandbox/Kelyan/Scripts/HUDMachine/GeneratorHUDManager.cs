using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorHUDManager : MonoBehaviour
{
    private float currentMeltTimer;
    private float maxTimerCurrentRecipe;
    private bool isMachineProducing = false;
    [SerializeField] private Slider sliderMeltTimer;
    [SerializeField] private TextMeshProUGUI meltTimeRecipeText;
    [SerializeField] private ItemHUD itemInput;

    public void InitGeneratorMenu(SOTransition currentRecipe, float currentTimer, float maxTimer, bool isProducing, List<SOItems> itemAccepted)
    {
        MachineHUDManager.Instance.onInputStackChange += UpdateInputStack;
        MachineHUDManager.Instance.onMachineStateChange += UpdateMachineState;
        MachineHUDManager.Instance.onMaxTimerChange += UpdateMaxTimer;
        MachineHUDManager.Instance.onUpdateUIGenerator += UpdateUIMachine;
        isMachineProducing = isProducing;
        maxTimerCurrentRecipe = maxTimer;
        currentMeltTimer = currentTimer;

        if (currentRecipe != null)
        {
            meltTimeRecipeText.text = currentRecipe.Timer.ToString() + " Sec";

            itemInput.Init(currentRecipe.Quantity, currentRecipe.Timer, currentRecipe.Resource, itemAccepted);
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

        itemInput.SetStackValue(inputStackList[0]);
    }

    private void UpdateMachineState(bool isMachineProducing)
    {
        this.isMachineProducing = isMachineProducing;
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

    public void UpdateUIMachine(SOTransition selectedRecipe)
    {
        itemInput.GetComponent<ItemHUD>().UpdateUI(selectedRecipe.Quantity, selectedRecipe.Timer,selectedRecipe.Resource);
    }

    public void CleanGeneratorMenu()
    {
        meltTimeRecipeText.text = " Sec";

        MachineHUDManager.Instance.onInputStackChange -= UpdateInputStack;
        MachineHUDManager.Instance.onMachineStateChange -= UpdateMachineState;
        MachineHUDManager.Instance.onMaxTimerChange -= UpdateMaxTimer;
        MachineHUDManager.Instance.onUpdateUIGenerator -= UpdateUIMachine;
    }
}
