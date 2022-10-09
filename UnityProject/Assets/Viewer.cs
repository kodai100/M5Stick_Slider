using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Viewer : MonoBehaviour
{

    [SerializeField] private Image image;

    [SerializeField] private SliderDataReceiver sliderDataReceiver;
    
    

    // Update is called once per frame
    private void Update()
    {
        image.rectTransform.localScale = new Vector3(1, sliderDataReceiver.CurrentValue, 1);
    }
}
