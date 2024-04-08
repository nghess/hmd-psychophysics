using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class ShaderControlUI : MonoBehaviour
{
    public Material targetMaterial; // Assign this in the Inspector

    public Slider frequencySlider;
    public Slider contrastSlider;
    public Slider speedSlider;

    void Start()
    {
        // Initialize sliders with current shader property values
        frequencySlider.value = targetMaterial.GetFloat("_Frequency");
        contrastSlider.value = targetMaterial.GetFloat("_Contrast");
        speedSlider.value = targetMaterial.GetFloat("_Speed");

        // Add listeners for slider value changes
        frequencySlider.onValueChanged.AddListener(delegate { UpdateShaderProperties(); });
        contrastSlider.onValueChanged.AddListener(delegate { UpdateShaderProperties(); });
        speedSlider.onValueChanged.AddListener(delegate { UpdateShaderProperties(); });
    }

    void UpdateShaderProperties()
    {
        targetMaterial.SetFloat("_Frequency", frequencySlider.value);
        targetMaterial.SetFloat("_Contrast", contrastSlider.value);
        targetMaterial.SetFloat("_Speed", speedSlider.value);
    }
}
