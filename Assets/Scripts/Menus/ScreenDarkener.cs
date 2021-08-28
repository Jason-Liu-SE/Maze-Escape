using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDarkener : MonoBehaviour {
    private Image backgroundImage;

    void Start() {
        backgroundImage = transform.gameObject.GetComponent<Image>();
        
        // changing the dimensions of the background image
        if (backgroundImage != null) {
            if (backgroundImage.minWidth != Screen.width || backgroundImage.minHeight != Screen.height) 
                backgroundImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}
