using UnityEngine;

public class PlayerController : Racer
{
    protected override void Act()
    {
        // Input is already handled inside Racer.cs when isPlayer = true
        // Add extra player-only logic here later
    }

    public override void TakeHit()
    {
        base.TakeHit();
    }
}