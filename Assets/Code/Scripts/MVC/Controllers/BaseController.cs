using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class BaseController : MonoBehaviour
{
    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Dectivate()
    {
        gameObject.SetActive(false);
    }
}

public class BaseController<T> : BaseController where T : BaseView
{
    [Header("BASE CONTROLLER")]
    [DisplayInspector]
    [SerializeField] protected T view;

    public override void Activate()
    {
        base.Activate();
        view.Activate();
    }

    public override void Dectivate()
    {
        base.Dectivate();
        view.Dectivate();
    }
}
