using System.Collections.Generic;
using UnityEngine;

public class UnlockedBoardBehaviour : MonoBehaviour
{
    // [SerializeField]
    // private List<string> _allEntities;
    // [SerializeField]
    // private List<string> _unlockedEntities;
    [SerializeField] private Transform _entitiesHandle;

    [SerializeField] private int _entitiesPerRow = 3;

    private int _currentColumn;

    private Dictionary<string,GameObject> _entities = new Dictionary<string,GameObject>();
    private GameManager _gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();

        Vector3 position = Vector3.zero;
        foreach(string prefabName in ChemicalReactionDatabase.GetAllEntities())
        {
            Debug.Log($"UnlockedBoardBehaviour:Start:prefabName:{prefabName}");
            GameObject entity = Instantiate<GameObject>(CatalogBehaviour.GetPrefabByName(prefabName), transform);
            entity.name = prefabName;

            Vector3 scale = entity.transform.localScale * 0.3f;
            scale.z = 0.01f;
            entity.transform.localScale = scale;
            
            entity.transform.localPosition = position;

            Bounds bounds = GetTotalBounds(entity);

            if(_currentColumn == _entitiesPerRow)
            {
                _currentColumn = 0;
                position.x = 0;
                position.y += bounds.size.y + 0.075f;
            }
            else
            {
                position.x += bounds.size.x + 0.1f;
                _currentColumn++;
            }

            StartRemoveRotation(entity);
            StartShowUnlockedState(entity);

            _entities.Add(entity.name, entity);
        }

        EventManager.StartListening(EventType.EntityUnlocked, OnEntityUnlocked);
    }

    private void OnEntityUnlocked(System.Object obj)
    {
        string entityName = (string)obj;
        if(_entities.ContainsKey(entityName))
        {
            StartShowUnlockedState(_entities[entityName]);
        }
    }

    private void StartRemoveRotation(GameObject entity)
    {
        RotationObjects[] rotationObjects = entity.GetComponentsInChildren<RotationObjects>();
        foreach (RotationObjects rotationObject in rotationObjects)
        {
            rotationObject.enabled = false;
        }
    }

    private void StartShowUnlockedState(GameObject entity)
    {
        List<string> unlockedEntities = _gameManager.GetAllUnlockedEntities();
        if(!entity.TryGetComponent<Greyscaleable>(out Greyscaleable greyscaleable))
        {
            greyscaleable = entity.AddComponent<Greyscaleable>();
        }

        if (!unlockedEntities.Contains(entity.name))
        {
            greyscaleable.MakeGrayscale();
        }
        else
        {
            greyscaleable.MakeColorful();
        }
    }

    public static Bounds GetTotalBounds(GameObject gameObject)
    {
        Bounds totalBounds;

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            totalBounds = new Bounds(gameObject.transform.position, Vector3.zero);
        }
        else
        {
            totalBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                totalBounds.Encapsulate(renderers[i].bounds);
            }
        }

        // RectTransform[] rectTransforms = gameObject.GetComponentsInChildren<RectTransform>();
        // foreach (RectTransform rectTransform in rectTransforms)
        // {
        //     Vector3[] corners = new Vector3[4];
        //     rectTransform.GetWorldCorners(corners);

        //     Bounds bounds = new(corners[0], Vector3.zero);
        //     for (int i = 1; i < corners.Length; i++)
        //     {
        //         bounds.Encapsulate(corners[i]);
        //     }

        //     totalBounds.Encapsulate(bounds);
        // }
        // Vector3[] corners = new Vector3[4];
        // rectTransform.GetWorldCorners(corners);

        // Bounds bounds = new Bounds(corners[0], Vector3.zero);
        // for (int i = 1; i < corners.Length; i++)
        // {
        //     bounds.Encapsulate(corners[i]);
        // }

        return totalBounds;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
