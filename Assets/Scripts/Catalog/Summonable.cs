using UnityEngine;

public class Summonable : MonoBehaviour
{
    private Vector3 _targetPosition = Vector3.zero;
    private bool _summoning = false;

    public void ToTarget(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _summoning = true;
        Debug.Log($"EntitySummon:ToTarget:{_summoning} {transform.position} {_targetPosition}");
    }

    public bool IsSummoning()
    {
        return _summoning;
    }

    private void Update()
    {
        if(_targetPosition == Vector3.zero || !_summoning) return;

        float distance = Vector3.Distance(_targetPosition, transform.position);
        if(distance < 0.01f)
        {
            transform.position = _targetPosition;
            _summoning = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime*2f);
        }
    }
}
