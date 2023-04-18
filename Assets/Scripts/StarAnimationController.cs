using System.Collections;
using UnityEngine;

public class StarAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _timeOffset;

    private void Start() => StartCoroutine(TimeOffset(_timeOffset));

    IEnumerator TimeOffset(float time)
    {
        yield return new WaitForSeconds(time);
        _animator.enabled = true;
    }
}