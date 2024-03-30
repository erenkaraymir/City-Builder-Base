using SVS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : SingletonDontDestroyMono<RoadManager>
{
    public List<Vector3Int> TemporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> RoadPositionsToCheck = new List<Vector3Int>();

    private Vector3Int _startPosition;
    private bool _placementMode = false;

    public void PlaceRoad(Vector3Int position)
    {
        if (PlacementManager.Instance.CheckIfPositionInBound(position) == false)
        {
            return;
        }
        if (PlacementManager.Instance.CheckIfPositionIsFree(position) == false)
        {
            return;
        }
        if (!_placementMode)
        {
            TemporaryPlacementPositions.Clear();

            foreach (var positionToFix in RoadPositionsToCheck)
            {
                RoadFixer.Instance.FixRoadAtPosition(PlacementManager.Instance, positionToFix);
            }

            RoadPositionsToCheck.Clear();

            _placementMode = true;
            _startPosition = position;

            TemporaryPlacementPositions.Add(position);
            PlacementManager.Instance.PlaceTemporaryStructure(position, RoadFixer.Instance.DeadEnd, CellType.Road);
        }
        else
        {
            PlacementManager.Instance.RemoveAllTemporaryStructures();
            TemporaryPlacementPositions.Clear();
            RoadPositionsToCheck.Clear();

            TemporaryPlacementPositions = PlacementManager.Instance.GetPathBetween(_startPosition, position);
            foreach (var temporaryPosition in TemporaryPlacementPositions)
            {
                if (PlacementManager.Instance.CheckIfPositionIsFree(temporaryPosition) == false)
                {
                    continue;
                }
                PlacementManager.Instance.PlaceTemporaryStructure(temporaryPosition, RoadFixer.Instance.DeadEnd, CellType.Road);
            }
        }
        FixRoadPrefabs();

    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in TemporaryPlacementPositions)
        {
            RoadFixer.Instance.FixRoadAtPosition(PlacementManager.Instance, temporaryPosition);
            var neighbours = PlacementManager.Instance.GetNeighbourTypesFor(temporaryPosition, CellType.Road);
            foreach (var roadPosition in neighbours)
            {
                if(RoadPositionsToCheck.Contains(roadPosition) == false)
                {
                    RoadPositionsToCheck.Add(roadPosition);
                }
            }
        }

        foreach (var positionToFix in RoadPositionsToCheck)
        {
            RoadFixer.Instance.FixRoadAtPosition(PlacementManager.Instance, positionToFix);
        }
    }

    public void FinishPlacingRoad()
    {
        _placementMode = false;
        PlacementManager.Instance.AddTemporaryStructuresToStrucutreDictionary();
        if(TemporaryPlacementPositions.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        TemporaryPlacementPositions.Clear();
        _startPosition = Vector3Int.zero;
    }
}
