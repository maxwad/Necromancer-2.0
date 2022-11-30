using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public interface IDoorable<T>
{
    void Open(T building);
}
