using UnityEngine;

public class Move
{
    public MoveBaseInformation BaseInformation { get; set; }
    public int PP { get; set; }

    public Move(MoveBaseInformation baseInformation)
    {
        BaseInformation = baseInformation;
        PP = baseInformation.PP;
    }


}
