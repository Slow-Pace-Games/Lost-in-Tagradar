using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AnimationController
{
    public static IEnumerator PlayAnimation(List<Sprite> _listSprite, Image _image, bool _isLooping, float _delayBetweenFrame)
    {
        _image.gameObject.SetActive(true);
        float timer = 0f;
        int index = 0;
        _image.sprite = _listSprite[index];
        if (_isLooping)
        {
            while (true)
            {
                timer += TimeScale.deltaTime;
                if (timer > _delayBetweenFrame)
                {
                    index++;
                    if (index > _listSprite.Count - 1)
                    {
                        index = 0;
                    }
                    _image.sprite = _listSprite[index];
                    timer = 0f;
                }
                yield return null;
            }
        }
        else
        {
            while (index < _listSprite.Count - 1)
            {
                timer += TimeScale.deltaTime;
                if (timer > _delayBetweenFrame)
                {
                    index++;
                    if (index > _listSprite.Count - 1)
                    {
                        index = 0;
                    }
                    _image.sprite = _listSprite[index];
                    timer = 0f;
                }

                yield return null;
            }
        }
        _image.gameObject.SetActive(false);
        yield return null;

    }
    public static IEnumerator PlayAnimation(Sprite[] _listSprite, Image _image, bool _isLooping, float _delayBetweenFrame, LoidUI.AnimFinishEvent animFinishEvent)
    {
        _image.gameObject.SetActive(true);
        float timer = 0f;
        int index = 0;
        _image.sprite = _listSprite[index];
        if (_isLooping)
        {
            while (true)
            {
                timer += TimeScale.deltaTime;
                if (timer > _delayBetweenFrame)
                {
                    index++;
                    if (index > _listSprite.Length - 1)
                    {
                        index = 0;
                    }
                    _image.sprite = _listSprite[index];
                    timer = 0f;
                }
                yield return null;
            }
        }
        else
        {
            while (index < _listSprite.Length - 1)
            {
                timer += TimeScale.deltaTime;
                if (timer > _delayBetweenFrame)
                {
                    index++;
                    if (index > _listSprite.Length - 1)
                    {
                        index = 0;
                    }
                    _image.sprite = _listSprite[index];
                    timer = 0f;
                }

                yield return null;
            }
        }
        _image.gameObject.SetActive(false);
        yield return null;
        animFinishEvent.Invoke();
    }
    public static IEnumerator PlayAnimation(Sprite[] _listSprite, Image _image, bool _isLooping, float _delayBetweenFrame)
    {
        _image.gameObject.SetActive(true);
        float timer = 0f;
        int index = 0;
        _image.sprite = _listSprite[index];
        if (_isLooping)
        {
            while (true)
            {
                timer += TimeScale.deltaTime;
                if (timer > _delayBetweenFrame)
                {
                    index++;
                    if (index > _listSprite.Length - 1)
                    {
                        index = 0;
                    }
                    _image.sprite = _listSprite[index];
                    timer = 0f;
                }
                yield return null;
            }
        }
        else
        {
            while (index < _listSprite.Length - 1)
            {
                timer += TimeScale.deltaTime;
                if (timer > _delayBetweenFrame)
                {
                    index++;
                    if (index > _listSprite.Length - 1)
                    {
                        index = 0;
                    }
                    _image.sprite = _listSprite[index];
                    timer = 0f;
                }

                yield return null;
            }
        }
        _image.gameObject.SetActive(false);
        yield return null;
    }
    public static void PlayAnimationAssignedValue(List<Sprite> listSprite, Image image, float currentSliderValue, float startValue = 0f, float finishedValue = 1.0f, float delayBetweenFrame = 0.05f)
    {
        float size = finishedValue - startValue;
        if (size > 0f)
        {
            float sliderRatio = size - (finishedValue - currentSliderValue);
            int index = Mathf.RoundToInt(sliderRatio * (listSprite.Count - 1) / size);
            image.sprite = listSprite[index];
        }
    }
}
