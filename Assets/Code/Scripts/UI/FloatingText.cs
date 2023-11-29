using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour, IPoolable<FloatingText>
{
    public Animator animator;
    public Image iconDisplay;

    private Vector3 objectPosition;
    public Vector3 ObjectPosition
    {
        get
        {
            return objectPosition;
        }
        set
        {
            objectPosition = value;
            transform.position = Camera.main.WorldToScreenPoint(objectPosition);
        }
    }

    [SerializeField] private TMP_Text text;

    public ObjectPool<FloatingText> Pool { get; set; }

    public void Init(Vector3 objectPosition, bool repeated = false)
    {
        if (repeated)
        {
            animator.speed = 0;
        } 
        else
        {
            animator.speed = 1;
            AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
            StartCoroutine(RelaseOnAnimationFinish(clipInfo.clip.length));
        }

        ObjectPosition = objectPosition;
    }

    IEnumerator RelaseOnAnimationFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        try
        {
            Pool.Release(this);
        } catch
        {
            Destroy(gameObject);
        }
    }

    public void SetIcon(Sprite icon, Vector3 scale)
    {
        if(icon != null)
        {
            iconDisplay.sprite = icon;
            iconDisplay.transform.localScale = scale;
            iconDisplay.gameObject.SetActive(true);
        } else
        {
            iconDisplay.gameObject.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale,scale,1);
    }

    public void Deinit()
    {
        Pool.Release(this);
    }
}
