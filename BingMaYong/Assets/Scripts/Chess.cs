using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Direction
{//这个枚举设置棋子的方向
    South,
    West,
    North,
    East,
}

public enum ChessType
{//这个枚举设置棋子的类型,立射俑,车俑,步兵佣,城堡
    Shoot,
    Car,
    Infantry,
    Castle
}
public class Chess : MonoBehaviour
{
    public Side chessSide;                //棋子的阵营

    protected Vector2Int currentPosition;           //当前位于格子坐标

    public Slider chessSlider;            //血条

    public  GameObject victim;                                        //被攻击者
    public  Vector2Int destination;                                   //最终目的地
    public  Vector2Int nextDestination;                              //下一个要到达的位置

    //继承的子类可能需要初始化以下,城堡只用设置血量和棋子类型
    public ChessType chessType;           //棋子类型

    public  bool isAttack;                                          //是否需要攻击
    public  bool isMoving;                                          //是否正在移动
    public  MapController mapController;                            //地图控制器
    public  RoleActionManager action_manager;                       //运动管理器

    public  List<Vector2Int> attackRange;  //攻击范围,使用相对Vector2Int坐标
    public  float normalAttackHurt;        //在正常情况下应该给予的伤害
    public  float forCarChessHurt;         //特殊情况下会给车的伤害
    public  float forShootChessHurt;       //特殊情况下会给立射俑伤害
    public float forInfantryChessHurt;    //特殊情况下会给步兵俑伤害
    public float blood;                   //棋子的血量


    public Direction direction;
    /*
     * 得到兵马俑的当前位置
     * 返回Vector2Int类型的当前位置
     */
    public Vector2Int GetCurrentPosition()
    {
        return currentPosition;
    }

    /*
     * 设置兵马俑的当前位置
     * 传入需要设置的位置pos
     */
    public void SetCurrentPosition(Vector2Int pos)
    {
       currentPosition = pos;
    }

    /*

     * 占领当前位置
     * 无参数,无返回值
     */
    public void OccupyCurrentPosition()
    {
        MapController.instance.SetOccupied(currentPosition, chessSide);
    }
    /*
     * 占领一个位置
     * 传入需要占领的位置pos
     */
    public void OccupyPosition(Vector2Int pos)
    {
        MapController.instance.SetOccupied(pos, chessSide);

    }

    /*
     * 不占领当前位置
     * 无参数,无返回值
     */
    public void ReleaseCurrentPosition()
    {
        MapController.instance.SetReleased(currentPosition);
    }

    /*
     * 设置此兵马俑的血量
     * 传入float类型的血量blo
     */
    public void SetBlood(float blo)
    {
        if (blo <= 0)
            blo = 0;
        blood = blo;
        chessSlider.value = blo;
    }

    /*
     * 返回当前兵马俑的血量
     * 返回float类型的blood
     */
    public float GetBlood()
    {
        return blood;
    }

    /*
     * 得到应该走到攻击目标的哪个位置
     * 传入被攻击者的当前位置
     * 返回单个Vector2Int坐标
     * 通过遍历能攻击到的所有位置,然后通过每个位置都有一条路径长度通过mapController返回找到最近的道路去攻击
     */
    public Vector2Int GetAttackTargetLocations(Vector2Int victimPos)
    {



        Vector2Int targetLocations = currentPosition;
        int minPath = 9999;
        foreach (Vector2Int vec in attackRange)
        {
            //坐标合法
            if((victimPos - vec).x >= 0 && (victimPos - vec).y >= 0 && (victimPos - vec).x <= 9 && (victimPos - vec).y <= 13)
            {
                Tile targetTile = mapController.GetTileWithPosition(victimPos - vec);
                //能攻击的点没有被占领
                if (targetTile.tileState != TileState.Occupied && targetTile.tileState != TileState.Obstacle)
                {
                    int count = mapController.GetPathListCount(currentPosition, victimPos - vec);
                    Debug.Log("步数" + count + "目的地" + (victimPos - vec));
                    if (count > 0 && count < minPath)
                    {
                        minPath = count;
                        targetLocations = victimPos - vec;
                    }
                }
            }
        }

        return targetLocations;

    }

    /*
     * 传入被攻击者
     * 返回被攻击者的伤害
     * 根据被攻击者类型判断伤害值
     */
    public float GetChessHurt(GameObject victim)
    {
        //判断攻击时候对象是否已经被摧毁
        GameObject tempGo = victim ?? null;
        if (tempGo == null) { return normalAttackHurt; }

        if (victim.gameObject.GetComponent<Chess>().chessType == ChessType.Shoot)
        {
            return forShootChessHurt;
        }
        else if (victim.gameObject.GetComponent<Chess>().chessType == ChessType.Infantry)
        {
            return forInfantryChessHurt;
        }
        else if(victim.gameObject.GetComponent<Chess>().chessType == ChessType.Car)
        {
            Vector2Int victimPos = victim.gameObject.GetComponent<Chess>().currentPosition;
            //步兵在侧面打车直接杀死车
            if (chessType == ChessType.Infantry && (victimPos == currentPosition + new Vector2Int(-1, 0) || victimPos == currentPosition + new Vector2Int(1, 0)))
                return forCarChessHurt;
            return normalAttackHurt;
        }
        else
        {
            //对城堡攻击都是使用普通伤害
            return normalAttackHurt;
        }
    }
    public delegate void OnWalkFinished(Vector2Int currentPosition);
    public event OnWalkFinished OnWalk;
    /*
      * 兵马俑移动
      * 无参数
      * 无返回值
      * 根据设置的移动路径,一次拿出一个位置去移动,当到达终点判断是否处于攻击状态,然后攻击
      */
    public virtual void Move()
    {
        //判断是否到达终点
        if (GetCurrentPosition() == destination)
        {
            Debug.Log("到达终点");
            isMoving = false;
            StopMoveAnimation();

            //之后可以智能判断周边是否需要攻击
            if (isAttack)
            {
                //如果是攻击状态在移动结束后需要攻击
                //得到伤害
                float hurt = GetChessHurt(victim);
                //攻击
                action_manager.Attack(gameObject, victim, hurt);
            }
            else
            {
                //自动攻击
                AutoAttacks();
            }
        }
        else
        {
            //得到下一个位置
            nextDestination = mapController.GetNextStep(GetCurrentPosition(), destination);
            Debug.Log("下一个移动到的位置" + nextDestination);
            //如果该位置是合法的，走向该位置
            if (MapController.instance.CanWalk(nextDestination))
            {
                if (this.OnWalk != null)
                {
                    this.OnWalk(currentPosition);//表示占领当前position
                }

                //移动到该位置
                MoveToPosition(nextDestination);
            }
            else
            {
                Debug.Log("该位置不合法，应该停在当前位置");
                isMoving = false;
                isAttack = false;
                StopMoveAnimation();
            }
        }
    }

    /*
     * 播放移动动作
     * 无参数,返回值
     * 寻找第一个子对象,子对象上有Animator
     */
    public void PlayMoveAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Animator>().SetBool("move", true);
            break;
        }
    }

    /*
     * 停止移动动作,返回idle状态
     * 无参数,返回值
     * 寻找第一个子对象,子对象上有Animator
     */
    public void StopMoveAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Animator>().SetBool("move", false);
            break;
        }
    }

    /*
     * 设置攻击目标,转变为攻击状态
     * 传入攻击目标vic
     * 无返回值
     */
    public void SetAttack(GameObject vic)
    {
        victim = vic;
        isAttack = true;
    }
    /*
     * 减少自身的血量
     * 传入攻击者的伤害hurt
     * 无返回值
     * 调用了Chess的方法
     */
    public void ReduceBoold(float hurt)
    {
        Debug.Log("扣血" + hurt);
        float blo = GetBlood();
        SetBlood(blo - hurt);
    }

    /*
     * 播放攻击动画
     * 无参数,无返回值
     * 获取子对象的动画组件设置trigger
     */
    public void PlayAttackAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Animator>().SetTrigger("IsChessAttack");
            break;
        }
    }

    /*
     * 设置兵马俑的目的地
     * 传入目的地pos
     * 无返回值
     */
    public void SetDestination(Vector2Int pos)
    {
        destination = pos;
    }

    /*
     * 得到兵马俑的下一个位置
     * 返回nextDestination
     */
    public Vector2Int GetNextDestination()
    {
        return nextDestination;
    }

    /*
     * 根据位置去移动兵马俑
     * 传入移动的下一个位置pos
     * 无返回值
     * 调用运动管理器方法
     */
    public void MoveToPosition(Vector2Int pos)
    {
        isMoving = true;
        action_manager.Move(gameObject, pos);
    }

    /*
     * 得到兵马俑是否正在移动
     * 返回bool类型
     */
    public bool GetMoving()
    {
        return isMoving;
    }

    /*
     * 取消兵马俑的攻击状态
     */
    public void StopAttackStatus()
    {
        isAttack = false;
    }

    /*
     * 得到兵马俑攻击状态
     * 返回bool类型
     */
    public bool GetAttackStatus()
    {
        return isAttack;
    }

    /*
     * 设置棋子的阵营
     * 传入棋子设置的阵营
     * 同时设置血条颜色,这里使用find函数查找血条所以有修改父子关系可能会报错
     */ 
    public void SetChessSide(Side side)
    {
        chessSide = side;
        Transform fill = gameObject.transform.Find("Canvas/Slider/Fill Area/Fill");
        if (side == Side.playerA)
        {
            fill.GetComponent<Image>().color = Color.green;
        }
        else
        {
            fill.GetComponent<Image>().color = Color.red;
        }
    }

    /*
     * 自动攻击函数
     * 没有设置自动攻击宫殿,自动攻击攻击范围内血最少的棋子
     */ 
    public void AutoAttacks()
    {
        if (!isMoving && !isAttack && chessType != ChessType.Castle)
        {
            float MinBlood = 9999;
            //检测周围是否有棋子
            GameObject victim = null;
            foreach (Vector2Int pos in attackRange)
            {
                Vector2Int temp = currentPosition + pos;
                if (temp.x >= 0 && temp.y >= 0 && temp.x <= 9 && temp.y <= 13)
                {
                    Tile tile = mapController.GetTileWithPosition(temp);
                    if (tile.tileState == TileState.Occupied && tile.side == Side.playerB)
                    {
                        GameObject tempGo = tile.occupyChess ?? null;
                        if (tempGo == null) { return; }

                        if (MinBlood > tile.occupyChess.GetComponent<Chess>().GetBlood())
                        {
                            victim = tile.occupyChess;
                            MinBlood = tile.occupyChess.GetComponent<Chess>().GetBlood();
                        }
                    }
                }
            }
            if (victim != null)
            {
                Singleton<PlayerController>.Instance.Attack(gameObject, victim);
            }
        }
    }
}
