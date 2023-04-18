using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private List<GameObject> particleList;

    private void Awake()
    {
        for (int i = 0; i < particleList.Count; i++)
        {
            if (StaticRuntimeSets.Items.ContainsKey(particleList[i].name))
                continue;
            StaticRuntimeSets.Add(particleList[i].name, particleList[i]);
            particleList[0].SetActive(false);
        }
    }
}
