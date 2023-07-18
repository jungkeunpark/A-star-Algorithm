using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMap : TileMapController
{
    private const string OBSTACLE_TILEMAP_OBJ_NAME = "ObstacleTilemap";
    private GameObject[] castleObjects = default;       //!< ��ã�� �˰������� �׽�Ʈ �� ������� �������� ĳ���� ������Ʈ �迭

    //! Awake Ÿ�ӿ� �ʱ�ȭ �� ������ �������Ѵ�.
    public override void InitAwake(MapBoard mapController_)
    {
        this.tileMapObjName = OBSTACLE_TILEMAP_OBJ_NAME;
        base.InitAwake(mapController_);

    }   //InitAwake()

    private void Start()
    {
        StartCoroutine(DelayStart(0f));
    }




    private IEnumerator DelayStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoStart();
    }


    private void DoStart()
    {
        //{ ������� �������� �����ؼ� Ÿ���� ��ġ�Ѵ�.
        castleObjects = new GameObject[2];
        TerrainController[] passableTerrains = new TerrainController[2];

        List<TerrainController> searchTerrains = default;
        int searchIdx = 0;
        TerrainController foundTile = default;
        // ������� �������� �������� y���� ��ġ�ؼ� �� ������ �޾ƿ´�.
        searchIdx = 0;
        foundTile = default;
        while (foundTile == null || foundTile == default)
        {
            //������ �Ʒ��� ��ġ�Ѵ�.
            searchTerrains = mapController.GetTerrains_Colum(searchIdx, true);
            foreach (var searchTerrain in searchTerrains)
            {
                if (searchTerrain.IsPassable)
                {
                    foundTile = searchTerrain;
                    break;

                }
                else {/*Do nothing*/  }
            }
            if (foundTile != null || foundTile != default) { break; }
            if (mapController.MapCellSize.x - 1 <= searchIdx) { break; }
            searchIdx++;
        }   //loop : ������� ã�� ����
        passableTerrains[0] = foundTile;

        //�������� �������� �������� y���� ��ġ�ؼ� �� ������ �޾ƿ´�.
        searchIdx = mapController.MapCellSize.x - 1;
        foundTile = default;
        while (foundTile == null || foundTile == default)
        {
            //�Ʒ����� ���� ��ġ�Ѵ�.
            searchTerrains = mapController.GetTerrains_Colum(searchIdx);
            foreach (var searchTerrain in searchTerrains)
            {
                if (searchTerrain.IsPassable)
                {
                    foundTile = searchTerrain;
                    break;
                }
                else { /* Do nothing*/ }
            }
        }       //loop : �������� ã�� ����
        //} ������� �������� �����ؼ� Ÿ���� ��ġ�Ѵ�.
    }       //DoStart


    //! ������ �߰��Ѵ�.
    public void Add_Obstarcle(GameObject obstacle)
    {
        allTileobjs.Add(obstacle);
    }    //Add_Obstarcle
}