using System.Collections.Generic;
using UnityEngine;

public class CatalogBehaviour : MonoBehaviour
{
    private static Dictionary<string,GameObject> _cachedPrefabs = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject _swipeForwardTarget;
    [SerializeField] private List<string> _unlockedEntities;

    private List<GameObject> _entities;
    private int _currentEntityIndex = 0;


    private void Start()
    {
        LoadPrefabs();
        _entities[_currentEntityIndex].SetActive(true);

        EventManager.StartListening(EventType.SwipeForwardActivated, SwipeForwardActivated);
    }

    private void LoadPrefabs()
    {
        _entities = new List<GameObject>();
        foreach (string entityName in _unlockedEntities)
        {
            GameObject entity = Instantiate(GetPrefabByName(entityName), transform);
            entity.transform.localPosition = Vector3.zero;
            entity.transform.localScale = entity.transform.localScale * 0.5f;
            entity.AddComponent<Hologram>();
            entity.AddComponent<Swipeable>();
            entity.SetActive(false);
            _entities.Add(entity);
        }
    }

    private static GameObject GetPrefabByName(string prefabName)
    {
        if (_cachedPrefabs.ContainsKey(prefabName))
        {
            return _cachedPrefabs[prefabName];
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
            _cachedPrefabs.Add(prefabName, prefab);
            return prefab;
        }
    }

    private void SwipeForwardActivated(System.Object obj)
    {
        if(!gameObject.activeSelf) return;

        Debug.Log("SwipeForwardActivated");
        _entities[_currentEntityIndex].GetComponent<Swipeable>().SwipeOut(_swipeForwardTarget);
        _currentEntityIndex = (_currentEntityIndex + 1) % _entities.Count;
        _entities[_currentEntityIndex].SetActive(true);
    }
}
