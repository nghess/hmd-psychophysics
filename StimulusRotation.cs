using UnityEngine;
using UnityEngine.UI; // Include the UI namespace to work with UI elements.

public class RotateWithSlider : MonoBehaviour
{
    public GameObject objectToRotate; // Assign the GameObject you want to rotate.
    public Slider rotationSlider; // Assign the slider.

    void Start()
    {
        // Optional: Initialize the slider's value or limits based on the object's current rotation.
        rotationSlider.minValue = 0; // Minimum value of the slider.
        rotationSlider.maxValue = 360; // Maximum value of the slider.
        rotationSlider.value = objectToRotate.transform.eulerAngles.x; // Initial slider value based on object's current rotation.

        // Add a listener to the slider to call the OnSliderValueChanged method whenever the slider's value changes.
        rotationSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        // Update the GameObject's rotation based on the slider's value.
        objectToRotate.transform.rotation = Quaternion.Euler(value, 90, -90); // Rotate the object around its Y axis.
    }
}
