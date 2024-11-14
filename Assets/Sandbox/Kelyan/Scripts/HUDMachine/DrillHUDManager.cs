using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrillHUDManager : MonoBehaviour
{
    private float currentMeltTimer;
    private float maxTimerCurrentRecipe;
    private bool isMachineProducing = false;
    [SerializeField] private Slider sliderMeltTimer;
    [SerializeField] private TextMeshProUGUI meltTimeRecipeText;
    [SerializeField] private TextMeshProUGUI ElecRecipeText;
    [SerializeField] private ItemHUD itemOutput;

    public void InitDrillMenu(SOTransition currentRecipe, float currentTimer, float maxTimer, bool isProducing)
    {
        MachineHUDManager.Instance.onOutputStackChange += UpdateOutputStack;
        MachineHUDManager.Instance.onMachineStateChange += UpdateMachineState;
        MachineHUDManager.Instance.onMaxTimerChange += UpdateMaxTimer;
        isMachineProducing = isProducing;
        maxTimerCurrentRecipe = maxTimer;
        currentMeltTimer = currentTimer;
        if (currentRecipe != null)
        {
            meltTimeRecipeText.text = currentRecipe.Timer.ToString() + " Sec";
            ElecRecipeText.text = currentRecipe.ElectricityCost.ToString() + " Watts";
            itemOutput.Init(currentRecipe.Quantity, currentRecipe.Timer, currentRecipe.Resource);
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

    private void UpdateOutputStack(List<int> outputStackList)
    {

        itemOutput.SetStackValue(outputStackList[0]);

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

    public void CleanDrillMenu()
    {
        meltTimeRecipeText.text = " Sec";
        ElecRecipeText.text = " Watts";
        MachineHUDManager.Instance.onOutputStackChange -= UpdateOutputStack;
        MachineHUDManager.Instance.onMachineStateChange -= UpdateMachineState;
        MachineHUDManager.Instance.onMaxTimerChange -= UpdateMaxTimer;
    }
}
