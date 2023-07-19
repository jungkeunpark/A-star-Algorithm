using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : GSingleton<PathFinder>
{
    #region ���� Ž���� ���� ����
    public GameObject sourceObj = default;
    public GameObject destinationObj = default;
    public MapBoard mapboard = default;
    #endregion      //���� Ž���� ���� ����

    #region A star �˰������� �ִܰŸ��� ã�� ���� ����
    private List<AstarNode> aStarResultPath = default;
    private List<AstarNode> aStarOpenPath = default;
    private List<AstarNode> aStarClosePath = default;

    #endregion  //A star �˰������� �ִܰŸ��� ã�� ���� ����

    //! ������� ������ ������ ���� ã�� �Լ�
    public void FindPath_Astar()
    {
        StartCoroutine(DelayFindPath_Astar(0.3f));
    }   //FindPath_Astar

    //! Ž�� �˰��� �����̸� �Ǵ�.
    private IEnumerator DelayFindPath_Astar(float delay_)
    {
        //A star �˰����� ����ϱ� ���ؼ� �н� ����Ʈ�� �ʱ�ȭ�Ѵ�.
        aStarOpenPath = new List<AstarNode>();
        aStarClosePath = new List<AstarNode>();
        aStarResultPath = new List<AstarNode>();

        TerrainController targetTerrain = default;

        // ������� �ε����� ���ؼ�, ����� ��带 ã�ƿ´�.
        string[] sourceObjectNameParts = sourceObj.name.Split('_');
        int sourceIdx1D = -1;
        int.TryParse(sourceObjectNameParts[sourceObjectNameParts.Length - 1], out sourceIdx1D);
        targetTerrain = mapboard.GetTerrain(sourceIdx1D);
        //ã�ƿ� ����� ��带 Open ����Ʈ�� �߰��Ѵ�.
        AstarNode targetNode = new AstarNode(targetTerrain, destinationObj);
        Add_AstarOpenList(targetNode);

        int loopIdx = 0;
        bool isFoundDestination = false;
        bool isNoWayToGo = false;



        while (isFoundDestination==false && isNoWayToGo == false)
        {
            //{ Open ����Ʈ�� ��ȸ�ؼ� ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.
            AstarNode minCostNode = default;
            foreach(var TerrainNode in aStarOpenPath)
            {
                if(minCostNode == default)
                {
                    minCostNode = TerrainNode;
                }       //if: ���� ���� �ڽ�Ʈ�� ��尡 ��� �ִ°��
                else
                {
                    //terrainNode�� �� ���� �ڽ�Ʈ�� ������ ���
                    //minCostNode �� ������Ʈ ȯ��.
                    if(TerrainNode.AstarF <  minCostNode.AstarF)
                    {
                        minCostNode = TerrainNode;
                    }       
                    else { continue; } //else : ���� ���� �ڽ�Ʈ�� ��尡 ĳ�� �Ǿ��ִ� ���
                }

            }       //Loop : ���� �ڽ�Ʈ�� ���� ��带 ã�� ����
            //} Open ����Ʈ�� ��ȸ�ؼ� ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.
            minCostNode.ShowCost_Astar();
            minCostNode.Terrain.SetTileActiveColor(RDefine.TileStatusColor.SEARCH);

            // ����ȯ ��尡 �������� �����ߴ��� Ȯ���Ѵ�.
            bool isArriveDest = mapboard.GetDistance2D(minCostNode.Terrain.gameObject, destinationObj).Equals(Vector2Int.zero);
            if(isArriveDest)
            {
                // { �������� �����ߴٸ� aStarResultPath ����Ʈ�� �����Ѵ�.
                AstarNode resultNode = minCostNode;
                bool isSet_aStarResultPathOk = false;
                while(isSet_aStarResultPathOk== false)
                {
                    aStarResultPath.Add(resultNode);
                    if (resultNode.AstarPrevNode == default || resultNode.AstarPrevNode == null)
                    {
                        isSet_aStarResultPathOk = true;
                        break;
                    }
                    else {/*Do nothing*/ }

                    resultNode = resultNode.AstarPrevNode;
                }       //loop : ���� ��带 ã�� ��Ȱ ������ ��ȸ�ϴ� ����
                // } �������� �����ߴٸ� aStarResultPath ����Ʈ�� �����Ѵ�.

                // Open List�� Close List�� �����Ѵ�.
                aStarOpenPath.Clear();
                aStarClosePath.Clear();
                isFoundDestination = true;
                break;


            }       //if : ������ ��尡 �������� ������ ���
            else
            {
                // { �������� �ʾҴٸ� ���� Ÿ���� �������� �׹��� ��带 ã�ƿ´�.

                List<int> nextSearchIdx1Ds = mapboard.GetTileIdx2D_Around4ways(minCostNode.Terrain.TileIdx2D);

                //ã�ƿ� ����߿��� �̵� ������ ���� Open List�� �߰��Ѵ�.
                AstarNode nextNode = default;
                foreach(var nextIdx1D in nextSearchIdx1Ds)
                {
                    nextNode = new AstarNode(mapboard.GetTerrain(nextIdx1D), destinationObj);
                    if (nextNode.Terrain.IsPassable == false) { continue; }

                    Add_AstarOpenList(nextNode, minCostNode);
                }       //loop: �̵� ������ ��带 OpenList�� �߰��ϴ� ����
                // } �������� �ʾҴٸ� ���� Ÿ���� �������� �׹��� ��带 ã�ƿ´�.

                // Ž���� ���� ���� Close List�� �߰��ϰ�, OpenList���� �����Ѵ�.
                // �� ��, Open List�� ����ִٸ�, ���̻� Ž���Ҽ� �ִ� ���� ���̻� �������� �ʴ� ���̴�.
                aStarClosePath.Add(minCostNode);
                aStarOpenPath.Remove(minCostNode);
                if (aStarOpenPath.IsValid() == false)
                {
                    GFunc.LogWarning("[warning] There are no more tiles to explore.");
                    isNoWayToGo = true;
                }       //if: �������� �������� ���ߴµ�, ���̻� Ž���� �� �ִ� ���� ���� ���

                foreach(var tempNode in aStarOpenPath)
                {
                    GFunc.Log($"Idx: {tempNode.Terrain.TileIdx1D}," + $"Cost: {tempNode.AstarF}");
                }



            }       //else: ������ ��尡 �������� �������� ���� ���

            loopIdx++;
            yield return new WaitForSeconds(delay_);

        }       //loop: Astar �˰������� ���� ã�� ���� ����


    }       //DelayFindPath_Astar()


    //! ����� ������ ��带 Open ����Ʈ�� �߰��Ѵ�.
    private void Add_AstarOpenList(AstarNode targetTerrain_, AstarNode prevNode = default)
    {
        //Open ����Ʈ�� �߰��ϱ� ���� �˰��� ����� �����Ѵ�.
        Update_AstarCostToTerrain(targetTerrain_, prevNode);
        AstarNode closeNode = aStarClosePath.FindNode(targetTerrain_);
        if (closeNode != default && closeNode != null)
        {
            //�̹� Ž���� ���� ��ǥ�� ��尡 �����ϴ� ��쿡��
            //Open ����Ʈ�� �߰����� �ʴ´�.
            /* Do nothing*/
        }       //!if: Close List�� �̹� Ž���� ���� ��ǥ�� ��尡 �����ϴ� ���
        else
        {
            AstarNode openedNode = aStarOpenPath.FindNode(targetTerrain_);
            if (openedNode != default && openedNode != null)
            {
                // Ÿ�� ����� �ڽ�Ʈ�� �� ���� ��쿡�� Open List ���� ��带 ��ü�Ѵ�.
                // Ÿ�� ����� �ڽ�Ʈ�� �� ū ��쿡�� Open List�� �߰����� �ʴ´�.
                if (targetTerrain_.AstarF < openedNode.AstarF)
                {
                    aStarOpenPath.Remove(openedNode);
                    aStarOpenPath.Add(targetTerrain_);
                }
                else
                { /*Do nothing*/}
            }       //if: Open����Ʈ�� ���� �߰��� ���� ���� ��ǥ�� ��尡 �����ϴ� ���
            else
            {
                aStarOpenPath.Add(targetTerrain_);
            }       //else : Open List�� ���� �߰��� ���� ���� ��ǥ�� ��尡 ���°��

        }       //else : ���� Ž���� ������ ���� ����� ���
    }//Add_AstarOpenList()



    //! Target ���� ������ Destination ���� ������ Distance �� Heuristic �� �����ϴ� �Լ�
    private void Update_AstarCostToTerrain(AstarNode targetNode, AstarNode prevNode)
    {
        //{Target �������� Destination ������ 2D Ÿ�� �Ÿ��� ����ϴ� ����
        Vector2Int distance2D = mapboard.GetDistance2D(targetNode.Terrain.gameObject, destinationObj);
        int totalDistance2D = distance2D.x + distance2D.y;
        //Heuristic �� �����Ÿ��� �����Ѵ�.
        Vector2 localDistance = destinationObj.transform.localPosition - targetNode.Terrain.transform.localPosition;
        float heuristic = Mathf.Abs(localDistance.magnitude);
        //}Target �������� Destination ������ 2D Ÿ�� �Ÿ��� ����ϴ� ����

        //{ ���� ��尡 �����ϴ� ���, ���� ����� �ڽ�Ʈ�� �߰��ؼ� �����Ѵ�.
        if (prevNode == default || prevNode == null) { /*Do nothing */}
        else
        {
            totalDistance2D = Mathf.RoundToInt(prevNode.AstarG + 1.0f);
        }
        targetNode.UpdateCost_Astar(totalDistance2D, heuristic, prevNode);
        //} ���� ��尡 �����ϴ� ���, ���� ����� �ڽ�Ʈ�� �߰��ؼ� �����Ѵ�.

    }       //Update_AstarCostToTerrain
}   //class PathFinder
