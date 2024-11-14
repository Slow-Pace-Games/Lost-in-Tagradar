using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;

public class RessourcesInfos : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Image background;

    private float lifeTimer;
    private float initLifeTimer;
    private float alphaBackground;

    // Start is called before the first frame update
    private void Start()
    {
        DisableMessage();

        initLifeTimer = 3f;
        lifeTimer = initLifeTimer;
        alphaBackground =  0.4f;
    }

    public IEnumerator UpdateRessourceInfo()
    {
        lifeTimer = initLifeTimer;
        message.alpha = 1f;
        background.color = new Color(0.22f, 0.22f, 0.22f,alphaBackground);
        while (lifeTimer > 0)
        {
            lifeTimer -= TimeScale.deltaTime;
         yield return null;
        }
        lifeTimer = 1f;
        while (lifeTimer > 0) 
        {
            lifeTimer -= TimeScale.deltaTime;
            message.alpha = Mathf.Lerp(0f, 1f, lifeTimer);
            background.color = new Color(0.22f, 0.22f, 0.22f, Mathf.Lerp(0f, alphaBackground, lifeTimer));
            yield return null;
        }
        DisableMessage();
        yield return null;
    }
    public void EnableMessage()
    {
        message.enabled = true;
        background.enabled = true;
        StopAllCoroutines();
        StartCoroutine(UpdateRessourceInfo());
    }
    private void DisableMessage()
    {
        message.enabled = false;
        background.enabled = false;
    }
    public void SetText(string newText)
    {
        message.text = newText;
    }
}
