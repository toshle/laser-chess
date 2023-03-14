using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject _tileContainer;
    [SerializeField] private GameObject _unitsContainer;

    public Dictionary<Vector2, Tile> _tileDict;
    private List<Unit> _units;

    private Tile _selectedTile;

    public Tile SelectedTile
    {
        get { return _selectedTile; }
        set { _selectedTile = value; }
    }

    public List<Unit> Units
    {
        get { return _units; }
    }


    private void Awake()
    {
        _tileDict = new Dictionary<Vector2, Tile>();
        _units = new List<Unit>();
        Tile.TileSelected += OnSelectTile;

        GameManager.BoardInstance = this;
        GenerateBoard(_width, _height);
        transform.position = new Vector3(-(float)_width / 2 + 0.5f, 0, -(float)_height / 2 + 0.5f);
    }

    private void OnDisable()
    {
        Tile.TileSelected -= OnSelectTile;
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    private void OnSelectTile(Tile tile)
    {
        ClearHighlights();
    }

    public void ClearHighlights(List<Tile> tiles = null)
    {
        var highLightedTiles = tiles != null ? tiles : _tileDict.Values.ToList();
        foreach (var tile in highLightedTiles)
        {
            tile.SetHighlight(TileState.Default);
        }
    }

    public void HighlightMoves(List<Tile> tiles)
    {
        ClearHighlights();
        foreach (var tile in tiles)
        {
            tile.SetHighlight(TileState.Move);
        }
    }

    public void HighlightTargets(List<Tile> tiles)
    {
        //ClearHighlights();
        foreach (var tile in tiles)
        {
            tile.SetHighlight(TileState.Target);
        }
    }

    public void GenerateBoard(int width, int height)
    {
        _width = width;
        _height = height;
        for (var x = 0; x < _width; x++)
        {
            for (var z = 0; z < _height; z++)
            {
                var spawnedTile = Instantiate(_tilePrefab, _tileContainer.transform);
                spawnedTile.transform.position = new Vector3(x, 0, z);
                spawnedTile.name = $"Tile {x}, {z}";
                spawnedTile.Init(x, z, this);
                _tileDict[new Vector2(x, z)] = spawnedTile;
            }
        }
    }

    public void SpawnUnits(List<UnitSpawnPoint> units)
    {
        foreach (var unitSpawnPoint in units)
        {
            var spawnedUnit = Instantiate(unitSpawnPoint.Unit.UnitPrefab, _unitsContainer.transform);
            var tile = _tileDict[new Vector2(unitSpawnPoint.x, unitSpawnPoint.y)];
            spawnedUnit.Init();
            spawnedUnit.Movement.MoveTo(tile);
            if(unitSpawnPoint.y >= _height - 4)
            {
                spawnedUnit.transform.LookAt(transform.position - new Vector3(0, 0, 1) + new Vector3(0, 0.5f, 0), Vector3.up);
            }
            if (unitSpawnPoint.y <= 3)
            {
                spawnedUnit.transform.LookAt(transform.position + new Vector3(0, 0.5f, 1), Vector3.up);
            }
            _units.Add(spawnedUnit);
        }
    }

    public List<Tile> GetTilesInDirection(Tile startTile, Vector2 direction, int range = 0)
    {
        var x = startTile.Position.x + direction.x;
        var y = startTile.Position.y + direction.y;
        List<Tile> tilesInDirection = new();

        while (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            if (range > 0)
            {
                if (Math.Abs(x - startTile.Position.x) <= range && Math.Abs(y - startTile.Position.y) <= range)
                {
                    tilesInDirection.Add(GetTileAtPosition(new Vector2(x, y)));
                }
            }
            else
            {
                tilesInDirection.Add(GetTileAtPosition(new Vector2(x, y)));
            }
            x += direction.x;
            y += direction.y;

        }

        return tilesInDirection;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tileDict.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 1);
        for (var x = 0; x < _width; x++)
        {
            for (var z = 0; z < _height; z++)
            {
                Gizmos.DrawCube(new Vector3(x, 0, z), new Vector3(0.95f, 0.001f, 0.95f));

            }
        }
    }
}
