using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StructureManager : SingletonDontDestroyMono<StructureManager>
{
    public StructurePrefabWeighted[] housesPrefabs, specialPrefabs;

    private float[] _houseWeights, _specialWeigths;


    private void Start()
    {
        _houseWeights = housesPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        _specialWeigths = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void PlaceHouse(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(_houseWeights);
            PlacementManager.Instance.PlaceObjectOnTheMap(position,housesPrefabs[randomIndex].prefab,CellType.Structure);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(_specialWeigths);
            PlacementManager.Instance.PlaceObjectOnTheMap(position, specialPrefabs[randomIndex].prefab, CellType.Structure);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }
        float randomValue = Random.Range(0, sum);
        float tempSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if(randomValue >= tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0;
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if(PlacementManager.Instance.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("out bound");
            return false;
        }

        if(PlacementManager.Instance.CheckIfPositionIsFree(position) == false)
        {
            Debug.Log("not empty");
            return false;
        }

        if(PlacementManager.Instance.GetNeighbourTypesFor(position,CellType.Road).Count <= 0)
        {
            Debug.Log("not empty");
            return false;
        }

        return true;
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject prefab;
    [Range(0,1)]
    public float weight;
}