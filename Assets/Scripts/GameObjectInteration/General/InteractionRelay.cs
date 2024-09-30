using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRelay : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("The GameObject that can handle the actions")]
    public GameObject actionHandlerObject = null;

    private IActionInterface actionInterface = null;

    public void Awake()
    {
        if (actionHandlerObject is null)
        {
            throw new ArgumentException(this.name + ": " + nameof(actionHandlerObject));
        }

        if (actionHandlerObject.TryGetComponent<IActionInterface>(out actionInterface))
        {
            GameLog.Message(LogType.Log, this, "Found Action Interface for {0}", this.name);
        }
        else
        {
            GameLog.Message(LogType.Error, this, "Didn't find Action Interface for {0}", this.name);
        }
    }
    public bool AdvertiseInteraction()
    {
        if (actionInterface is null)
        {
            GameLog.ErrorMessage(this, "AdvertiseAction: For '{0}', actionInterface was NULL", this.name);
            return (false);
        }

        return( actionInterface.AdvertiseInteraction() );
    }
    public bool PerformInteraction()
    {
        if (actionInterface is null)
        {
            GameLog.ErrorMessage(this, "AdvertiseAction: For '{0}', actionInterface was NULL", this.name);
            return (false);
        }

        return ( actionInterface.PerformInteraction() );
    }

    public InteractionStatus ContinueInteraction()
    {
        if (actionInterface is null)
        {
            GameLog.ErrorMessage(this, "AdvertiseAction: For '{0}', actionInterface was NULL", this.name);
            return InteractionStatus.Completed;
        }
        
        return( actionInterface.ContinueInteraction() );
    }
}
