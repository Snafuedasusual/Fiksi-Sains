using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerSection3: BaseHandler
{
    [SerializeField] Transform plrStart;
    [SerializeField] Transform[] levers;
    [SerializeField] LayerMask lyrMask;
    [SerializeField] List<Transform> activeLevers;
    [SerializeField] int amountOfLevers;
    [SerializeField] int activatedLevers;
    [SerializeField] Door_Section doorSection;

    public override void Restart()
    {
        player.transform.position = plrStart.position;
        SetLevers();
    }

    float minDistance = 18;

    private void SetLevers()
    {
        for (int i = 0; i < amountOfLevers; i++)
        {
            var selectedLever = levers[Random.Range(0, levers.Length - 1)].transform;
            if(activeLevers.Count > 0)
            {
                for(int j = 0; j < activeLevers.Count; j++)
                {
                    var dist = Vector3.Distance(activeLevers[j].position, selectedLever.position);
                    if(dist < minDistance || activeLevers[j].transform == selectedLever.transform)
                    {
                        break;
                    }
                    else
                    {
                        if(j == activeLevers.Count - 1)
                        {
                            selectedLever.gameObject.SetActive(true);
                            activeLevers.Add(selectedLever);
                        }
                    }
                }
            }
            else if(activeLevers.Count == 0)
            {
                selectedLever.gameObject.SetActive(true);
                activeLevers.Add(selectedLever);
            }
        }
        if(activeLevers.Count > amountOfLevers)
        {
            activeLevers.RemoveAt(activeLevers.Count - 1);
        }
        LeverChecker();
        //DeactivateNonActiveLevers();
    }

    private void LeverChecker()
    {
        if(activeLevers.Count < amountOfLevers)
        {
            for(int i = 0; i < levers.Length; i++)
            {

                for(int j = 0; j < activeLevers.Count; j++)
                {
                    float distance = Vector3.Distance(activeLevers[j].position, levers[i].position);
                    if (distance < minDistance || activeLevers[j].transform == levers[i].transform)
                    {
                        break;
                    }
                    else
                    {
                        if(j == activeLevers.Count - 1)
                        {
                            activeLevers.Add(levers[i].transform);
                            levers[i].gameObject.SetActive(true);
                        }
                    }
                }
                if(activeLevers.Count == amountOfLevers)
                {
                    break;
                }
                else
                {

                }
            }
        }
    }

    public void InteractedLever()
    {
        activatedLevers++;
        Debug.Log("Yo!");
        if(activatedLevers == activeLevers.Count)
        {
            doorSection.canFinish = true;
        }
    }
}
