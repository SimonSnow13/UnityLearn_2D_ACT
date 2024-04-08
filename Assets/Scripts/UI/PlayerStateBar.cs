using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image staminaImage;
    private bool isRecovering;
    private Character currentCharacter;

    public float delaySpeed;

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= delaySpeed * Time.deltaTime;
        }

        if (isRecovering)
        {
            float percent = currentCharacter.currentStamina / currentCharacter.maxStamina;
            staminaImage.fillAmount = percent;
            if (percent >= 1)
            {
                isRecovering = false;
                return;
            }
        }
    }

    public  void OnHealthChange(float percent)
    {
        healthImage.fillAmount = percent;
    }

    public void OnStaminaChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}
