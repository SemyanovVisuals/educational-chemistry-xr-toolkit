using UnityEngine;

public class Swipeable : MonoBehaviour
{
    private bool _swipeOutActive = false;
    private Vector3 _startPosition = Vector3.zero;
    private GameObject _target;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    public void SwipeOut(GameObject target)
    {
        _swipeOutActive = true;
        _target = target;
    }

    private void Update()
    {
        if(!_swipeOutActive) return;
        
        float distance = Vector3.Distance(_target.transform.localPosition, transform.localPosition);
        if(distance < 0.01f)
        {
            transform.localPosition = _startPosition;
            gameObject.SetActive(false);
            _swipeOutActive = false;
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _target.transform.localPosition, Time.deltaTime*0.5f);
        }
    }
}
