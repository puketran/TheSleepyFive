using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private float cycleDuration = 10f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float startIntensity = 1f;

    private bool bIncrease = true;
    private void Start()
    {
        RenderSettings.ambientIntensity = startIntensity;
    }

    private void Update()
    {
        if (bIncrease)
        {
            startIntensity += Time.deltaTime / cycleDuration;
        }
        else
        {
            startIntensity -= Time.deltaTime / cycleDuration;
        }
        if (startIntensity <= minIntensity)
        {
            bIncrease = true;
        }
        else if (startIntensity >= maxIntensity)
        {
            bIncrease = false;
        }

        RenderSettings.ambientIntensity = startIntensity;

        Debug.Log(startIntensity);
    }
}