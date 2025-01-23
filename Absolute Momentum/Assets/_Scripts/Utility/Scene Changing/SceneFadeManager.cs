using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManager : Singleton<SceneFadeManager>
{
    [SerializeField] private Image fadeOutImage;
    [Range(0.1f, 10f), SerializeField] private float fadeOutSpeed = 5f;
    [Range(0.1f, 10f), SerializeField] private float fadeInSpeed = 5f;

    [SerializeField] private Color fadeOutStartColor;


    public bool IsFadingOut { get; private set; }
    public bool IsFadingIn { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        fadeOutStartColor.a = 0f;
    }

    private void Update()
    {
        if(IsFadingOut)
        {
            if(fadeOutImage.color.a < 1f)
            {
                fadeOutStartColor.a += Time.deltaTime * fadeOutSpeed;
                fadeOutImage.color = fadeOutStartColor;
            }
            else
            {
                IsFadingOut = false;
            }
        }

        if (IsFadingIn)
        {
            if (fadeOutImage.color.a > 0f)
            {
                fadeOutStartColor.a -= Time.deltaTime * fadeInSpeed;
                fadeOutImage.color = fadeOutStartColor;
            }
            else
            {
                IsFadingIn = false;
                //InputManager.Instance.EnablePlayerInput(true);
            }
        }


    }

    public void StartFadeOut()
    {
        fadeOutImage.color = fadeOutStartColor;
        IsFadingOut = true;
    }

    public void StartFadeIn()
    {
        if (fadeOutImage.color.a >= 1f)
        {
            fadeOutImage.color = fadeOutStartColor;
            IsFadingIn = true;
        }
    }

}
