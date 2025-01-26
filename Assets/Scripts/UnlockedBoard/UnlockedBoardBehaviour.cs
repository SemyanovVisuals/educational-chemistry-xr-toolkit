using System.Collections.Generic;
using UnityEngine;

public class UnlockedBoardBehaviour : MonoBehaviour
{
    [SerializeField] private List<string> _allEntities;
    [SerializeField] private List<string> _unlockedEntities;
    [SerializeField] private Transform _entitiesHandle;

    [SerializeField] private int _entitiesPerRow = 3;

    private int _currentColumn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 position = Vector3.zero;
        foreach(string prefabName in _allEntities)
        {
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
        Greyscaleable greyscaleable = entity.AddComponent<Greyscaleable>();
        if (!_unlockedEntities.Contains(entity.name))
        {
            greyscaleable.MakeGrayscale();
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
