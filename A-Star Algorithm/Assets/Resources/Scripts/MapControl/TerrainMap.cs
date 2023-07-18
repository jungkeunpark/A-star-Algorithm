using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMap : TileMapController
{
    private const string TERRAIN_TILEMAP_OBJ_NAME = "TerrainTilemap";

    private Vector2Int mapCellSize = default;
    private Vector2 mapCellGap = default;

    private List<TerrainController> allTerrains = default;

    //! Awake Ÿ�ӿ� �ʱ�ȭ Ȱ ������ �������Ѵ�.
    public override void InitAwake(MapBoard mapController_)
    {
        tileMapObjName = TERRAIN_TILEMAP_OBJ_NAME;
        base.InitAwake(mapController_);

        allTerrains = new List<TerrainController>();

        //{ Ÿ���� x�� ������ ��ü Ÿ���� ���� ���� ����, ���� ����� �����Ѵ�.
        mapCellSize = Vector2Int.zero;
        float tempTileY = allTileobjs[0].transform.localPosition.y;
        for( int i = 0; i< allTileobjs.Count; i++ )
        {
            if ( tempTileY.IsEquals(allTileobjs[i].transform.localPosition.y)==false)
            {//if : ù��° Ÿ���� y��ǥ�� �޶����� ���� �������� ���� ���� �� ũ���̴�.
                mapCellSize.x = i;
                break;
            }
        }
        //��üŸ���� ���� ���� ���� �� ũ��� ���� ���� ���� ���� �� ũ���̴�.
        mapCellSize.y = Mathf.FloorToInt(allTileobjs.Count / mapCellSize.x);

        //} Ÿ���� x�� ������ ��ü Ÿ���� ���� ���� ����, ���� ����� �����Ѵ�.

        //{ x �� ���� �� Ÿ�ϰ�, y �� ���� �� Ÿ�� ������ ���� ���������� Ÿ�� ���� �����Ѵ�.
        mapCellGap = Vector2.zero;
        mapCellGap.x = allTileobjs[1].transform.localPosition.x -allTileobjs[0].transform.localPosition.x;
        mapCellGap.y = allTileobjs[mapCellSize.x].transform.localPosition.y - allTileobjs[0].transform.localPosition.y;
        //} x �� ���� �� Ÿ�ϰ�, y �� ���� �� Ÿ�� ������ ���� ���������� Ÿ�� ���� �����Ѵ�.
    } //InitAwake()

    private void Start()
    {
        //Ÿ�ϸ��� �Ϻθ� ����Ȯ���� �ٸ� Ÿ�Ϸ� ��ü�ϴ� ����
        GameObject changeTilePrefab = ResManager.Instance.terrainPrefabs
            [RDefine.TERRAIN_PREF_OCEAN];

        //Ÿ�ϸ��߿� ��� ������ �ٴٷ� ��ü�� ������ �����Ѵ�.
        const float CHANGE_PERCENTAGE = 15.0f;
        float correctChangePercentage = allTileobjs.Count * (CHANGE_PERCENTAGE / 100.0f);

        //�ٴٷ� ��ü�� Ÿ���� ������ ����Ʈ ���·� �����ؼ� ���´�.
        List<int> changedTileResult = GFunc.CreateList(allTileobjs.Count, 1);
        changedTileResult.Shuffle();

        GameObject tempChangeTile = default;
        for (int i = 0; i < allTileobjs.Count; i++)
        {

            if (correctChangePercentage <= changedTileResult[i]) { continue; }

            //�������� �ν��Ͻ�ȭ �ؼ� ��ü�� Ÿ���� Ʈ�������� ��ü�Ѵ�.
            tempChangeTile = Instantiate(changeTilePrefab, tileMap.transform);
            tempChangeTile.name = changeTilePrefab.name;
            tempChangeTile.SetLocalScale(allTileobjs[i].transform.localScale);
            tempChangeTile.SetLocalPos(allTileobjs[i].transform.localPosition);

            allTileobjs.Swap(ref tempChangeTile, i);
            tempChangeTile.DestroyObj();
        }   //loop: ������ ������ ������ ���� Ÿ�ϸʿ� �ٴٸ� �����ϴ� ����

        //{ ������ �����ϴ� Ÿ���� ������ �����ϰ�, ��Ʈ�ѷ��� ĳ���ϴ� ����
        TerrainController tempTerrain = default;
        TerrainType terrainType = TerrainType.NONE;

        int loopCnt = 0;
        foreach(GameObject tile_ in allTileobjs)
        {
            tempTerrain = tile_.GetComponent<TerrainController>();
            switch(tempTerrain.name)
            {
                case RDefine.TERRAIN_PREF_PLAIN:
                    terrainType = TerrainType.PLAIN_PASS;
                    break;
                case RDefine.TERRAIN_PREF_OCEAN:
                    terrainType = TerrainType.OCEAN_N_PASS;
                    break;
                default:
                    terrainType = TerrainType.NONE;
                    break;
            }   //switch: �������� �ٸ� ������ �Ѵ�.

            // TODO: tempTerrain Setup �Լ� �ʿ���.
            tempTerrain.transform.SetAsFirstSibling();
            allTerrains.Add(tempTerrain);
            loopCnt += 1;
        }       //loop: Ÿ���� �̸��� ������ ������� �����ϴ� ����

        //} ������ �����ϴ� Ÿ���� ������ �����ϰ�, ��Ʈ�ѷ��� ĳ���ϴ� ����
    }




    //!�ʱ�ȭ�� Ÿ���� ������ ������ ���� ����, ���� ũ�⸦ �����ϴ� �Լ�
    public Vector2Int GetCellSize() { return mapCellSize; }

    //! �ʱ�ȭ�� Ÿ���� ������ ������ Ÿ�� ������ ���� �����ϴ� �Լ�.
    public Vector2 GetCellGap() { return mapCellGap; }

    //! �ε����� �ش��ϴ� Ÿ���� �����ϴ� �Լ�
    public TerrainController GetTile(int tileIdx1d)
    {
        if(allTerrains.IsValid(tileIdx1d))
        {
            return allTerrains[tileIdx1d];
        }
        return default;
    }   //GetTile()
}
