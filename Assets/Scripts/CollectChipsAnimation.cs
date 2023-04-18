using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectChipsAnimation : MonoBehaviour
{
    [SerializeField] private List<GameObject> _chipsList;
    [SerializeField] private Animator _collectAnimation;
    [SerializeField] private Text _chipsCount;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private GameObject moneyBoxWindow;
    private List<Vector3> _startPositions = new List<Vector3>();

    private void Awake()
    {
        foreach(var chip in _chipsList)
            _startPositions.Add(new Vector3(chip.transform.position.x, chip.transform.position.y, chip.transform.position.z));
    }

    private void OnEnable() => StartAnimation();

    private void StartAnimation()
    {
        for (int i = 0; i < _chipsList.Count; i++)
            _chipsList[i].transform.position = new Vector3(_startPositions[i].x, _startPositions[i].y, _startPositions[i].z);
        _collectAnimation.SetTrigger("Collect");
        StartCoroutine(SpawnChips(0.25f));
    }

    private IEnumerator SpawnChips(float delay)
    {
        foreach(var chip in _chipsList)
        {
            chip.SetActive(true);
            var seq = LeanTween.sequence();
            seq.append(LeanTween.move(chip, _endPoint, 0.8f)
                .setOnComplete(() => 
                { 
                    chip.SetActive(false);
                })
                .setEase(LeanTweenType.easeInCirc));
            yield return new WaitForSeconds(delay);
        }
        moneyBoxWindow.SetActive(false);
    }
}