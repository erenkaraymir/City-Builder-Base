using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : SingletonDontDestroyMono<PlacementManager>
{
    [SerializeField] private int _widht,_height;
    private Grid _placementGrid;

    private Dictionary<Vector3Int, StructureModel> _temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>();

    private Dictionary<Vector3Int, StructureModel> _structureDictionary = new Dictionary<Vector3Int, StructureModel>();


    private void Start()
    {
        _placementGrid = new Grid(_widht, _height);
    }

    public bool CheckIfPositionInBound(Vector3Int position)
    {
        if(position.x >= 0 && position.x < _widht && position.z >= 0 && position.z < _height)
        {
            return true;
        }
        return false;
    }

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        _placementGrid[position.x, position.z] = type;
        StructureModel structure = CreateNewStructureModel(position, structurePrefab, type);
        _structureDictionary.Add(position, structure);
        DestroyNatureAt(position);
    }

    private void DestroyNatureAt(Vector3Int position)
    {
        RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), new Vector3(.5f, .5f, .5f), transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature"));

        foreach (var item in hits)
        {
            Destroy(item.collider.gameObject);
        }
    }

    public bool CheckIfPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int position,CellType type)
    {
        return _placementGrid[position.x, position.z] == type;
    }

    public void PlaceTemporaryStructure(Vector3Int position,GameObject structurePrefab,CellType cellType)
    {
        _placementGrid[position.x, position.z] = cellType;
        StructureModel structure = CreateNewStructureModel(position, structurePrefab, cellType);
        _temporaryRoadObjects.Add(position, structure);
    }

 
    private StructureModel CreateNewStructureModel(Vector3Int position, GameObject structurePrefab, CellType cellType)
    {
        GameObject structure = new GameObject(cellType.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);

        return structureModel;
    }

    public void ModifyStructureModel(Vector3Int position,GameObject newModel,Quaternion rotation)
    {
        if (_temporaryRoadObjects.ContainsKey(position))
        {
            _temporaryRoadObjects[position].SwapModel(newModel, rotation);
        }
        else if (_structureDictionary.ContainsKey(position))
        {
            _structureDictionary[position].SwapModel(newModel, rotation);
        }

    }

    public CellType[] GetNeighbourTypesFor(Vector3Int temporaryPosition)
    {
        return _placementGrid.GetAllAdjacentCellTypes(temporaryPosition.x, temporaryPosition.z);
    }

    public List<Vector3Int> GetNeighbourTypesFor(Vector3Int position,CellType type)
    {
        var neighbourVertices = _placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return neighbours;
    }

    internal void AddTemporaryStructuresToStrucutreDictionary()
    {
        foreach (var structure in _temporaryRoadObjects)
        {
            _structureDictionary.Add(structure.Key,structure.Value);
            DestroyNatureAt(structure.Key);
        }

        _temporaryRoadObjects.Clear();
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition,Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(_placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x, endPosition.z));
        List<Vector3Int> path = new List<Vector3Int>();
        foreach (Point point in resultPath)
        {
            path.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return path;
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in _temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            _placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        _temporaryRoadObjects.Clear();
    }

}
