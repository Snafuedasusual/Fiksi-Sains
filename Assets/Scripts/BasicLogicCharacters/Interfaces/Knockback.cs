using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockBack
{
    public IEnumerator KBCoolDown();

    public void KnockBack(Transform sender, float power);
}
