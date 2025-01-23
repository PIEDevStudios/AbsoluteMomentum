using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;
    [SerializeField] private Material material;

    private void Awake()
    {
        material = Instantiate(material);

        material.SetFloat("_FlashAmount", 0f);

        SpriteRenderer[] _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer _spriteRenderer in _spriteRenderers)
        {
            _spriteRenderer.material = material;
        }
    }

    public void CallDamageFlash()
    {
       StopAllCoroutines();
       StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        //set flash color
        SetFlashColor();
        
        float _currentFlashAmount = 1f;
        float _elapsedTime = 0f;
        SetFlashAmount(_currentFlashAmount);
        while (_elapsedTime < flashTime)
        {
            //iterate elapsedTime
            _elapsedTime += Time.deltaTime;

            //lerp flash amount
            _currentFlashAmount = Mathf.Lerp(1f, 0f, _elapsedTime / flashTime);
            SetFlashAmount(_currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlashColor()
    {
        material.SetColor("_FlashColor", flashColor);
    }

    private void SetFlashAmount(float _amount)
    {
        material.SetFloat("_FlashAmount", _amount);
    }

}
