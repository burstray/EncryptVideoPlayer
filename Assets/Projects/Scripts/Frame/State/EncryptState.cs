using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using MTFrame.MTEvent;

public class EncryptState : BaseState
{
    public override string[] ListenerMessageID
    {
        get
        {
            return new string[]
            {

            };
        }
        set { }
    }

    public override void OnListenerMessage(EventParamete parameteData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        CurrentTask.ChangeTask(new EncryptTask(this));
    }
}
