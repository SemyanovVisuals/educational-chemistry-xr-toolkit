using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class CatalogBehaviour : MonoBehaviour
{
    private static Dictionary<string,GameObject> _cachedPrefabs = new Dictionary<string, GameObject>();

    [SerializeField] private AudioClip _rightPalmUpSound;
    [SerializeField] private AudioClip _swipeForwardSound;
    [SerializeField] private TextMeshProUGUI _entityNameText;
    [SerializeField] private Vector3 _summonOffset;
    [SerializeField] private GameObject _swipeForwardTarget;

    private AudioSource _audioSource;

    private List<GameObject> _entities;
    private List<string> _unlockedEntities;
    private int _currentEntityIndex = 0;
    private float _timeLastPalmUpActivation = 0f;
    private float _timeBeforePalmUpReactivation = 2f;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        _unlockedEntities = new List<string>(gameManager.GetUnlockedEntities());
        // _unlockedEntities = gameManager.GetUnlockedEntities();

        LoadPrefabs();

        EventManager.StartListening(EventType.EntityUnlocked, EntityUnlocked);
        EventManager.StartListening(EventType.SwipeForwardActivated, SwipeForwardActivated);
        EventManager.StartListening(EventType.RightPalmUpActivated, RightPalmUpActivated);
    }

    private void LoadPrefabs()
    {
        Debug.Log("CatalogBehaviour:LoadingPrefabs");
        _entities = new List<GameObject>();
        foreach (string entityName in _unlockedEntities)
        {
            Debug.Log($"CatalogBehaviour:LoadingPrefab:{entityName}");
            GameObject entity = Instantiate(GetPrefabByName(entityName), transform);
            entity.name = entityName;
            entity.transform.localPosition = Vector3.zero;
            entity.transform.localScale = entity.transform.localScale * 0.5f;
            entity.AddComponent<Hologram>();
            entity.AddComponent<Swipeable>();
            entity.SetActive(false);
            _entities.Add(entity);
        }
        _entities[_currentEntityIndex].SetActive(true);
        _entityNameText.text = _entities[_currentEntityIndex].name;
        Debug.Log("CatalogBehaviour:LoadedPrefabs");
    }

    public static GameObject GetPrefabByName(string prefabName)
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

    private void EntityUnlocked(System.Object obj)
    {
        string entityName = (string) obj;
        Debug.Log($"CatalogBehaviour:EntityUnlocked:{entityName}");
        if(_unlockedEntities.Contains(entityName)) return;

        Debug.Log($"CatalogBehaviour:EntityUnlocked:{entityName}");
        _unlockedEntities.Add(entityName);
        GameObject entity = Instantiate(GetPrefabByName(entityName), transform);
        entity.name = entityName;
        entity.transform.localPosition = Vector3.zero;
        entity.transform.localScale = entity.transform.localScale * 0.5f;
        entity.AddComponent<Hologram>();
        entity.AddComponent<Swipeable>();
        entity.SetActive(false);
        _entities.Add(entity);
    }

    private void SwipeForwardActivated(System.Object obj)
    {
        if(!gameObject.activeSelf) return;

        Debug.Log("SwipeForwardActivated");
        _audioSource.PlayOneShot(_swipeForwardSound);
        _entities[_currentEntityIndex].GetComponent<Swipeable>().SwipeOut(_swipeForwardTarget);
        _currentEntityIndex = (_currentEntityIndex + 1) % _entities.Count;
        _entities[_currentEntityIndex].SetActive(true);
        _entityNameText.text = _entities[_currentEntityIndex].name;
    }

    private void RightPalmUpActivated(System.Object obj)
    {
        if(!gameObject.activeSelf) return;

        if(Time.time - _timeLastPalmUpActivation < _timeBeforePalmUpReactivation) return;
        _timeLastPalmUpActivation = Time.time;

        Debug.Log("RightPalmUpActivated");
        _audioSource.PlayOneShot(_rightPalmUpSound);
        string currentlySelectedEntity = _entities[_currentEntityIndex].name;
        GameObject entity = Instantiate(GetPrefabByName(currentlySelectedEntity));
        entity.name = currentlySelectedEntity;
        entity.transform.position = _entities[_currentEntityIndex].transform.position;

        if(entity.TryGetComponent<InteractableUnityEventWrapper>(out InteractableUnityEventWrapper interactableUnityEventWrapper))
        {
            interactableUnityEventWrapper.WhenHover.RemoveAllListeners();
            interactableUnityEventWrapper.WhenUnhover.RemoveAllListeners();
        }

        GameObject target = (GameObject) obj;
        entity.AddComponent<Summonable>().ToTarget(target.transform.position+_summonOffset);

        if(!entity.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb = entity.AddComponent<Rigidbody>();
        }
        entity.AddComponent<VelocityBrake>();
        rb.isKinematic = false;
    }
}
