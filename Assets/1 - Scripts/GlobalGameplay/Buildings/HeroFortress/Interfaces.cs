using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public interface IUpgradable
{
    public void TryToBuild();

    public BuildingsRequirements GetRequirements();
}
