using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;

public class EncryptTask : BaseTask
{
    public EncryptTask(BaseState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UIManager.CreatePanel<EncryptPanel>(WindowTypeEnum.ForegroundScreen);
    }

    public override void Exit()
    {
        base.Exit();
        UIManager.ChangePanelState<EncryptPanel>(WindowTypeEnum.ForegroundScreen, UIPanelStateEnum.Hide);
    }
}
