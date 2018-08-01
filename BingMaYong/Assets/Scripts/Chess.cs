using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Direction
{//这个枚举设置棋子的方向
    South,
    Southwest,
    West,
    Northwest,
    North,
    Northeast,
    East,
    Southeast
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

    public  GameObject victim = null;                                        //被攻击者
    public List<GameObject> attacker = new List<GameObject>();//正在攻击自己的人的列表
    public int attackCapacity;                                          //攻击容量，用于AI
    public int attackValue;//攻击权值，配合attackCapacity，只在AI中使用

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
    public float defence;                    //护甲值
    public float forInfantryChessHurt;    //特殊情况下会给步兵俑伤害
    public float blood;                   //棋子的血量

    public Direction direction;
    public float speed;    //速度，action开始时会被获取
    /*
     * 得到兵马俑的当前位置
     * 返回Vector2Int类型的当前位置
     */
     public float GetSpeed()
    {
        return speed;
    }
    public void BeingAttackedBy(GameObject newAttacker)
    {
        foreach(var i in attacker)
        {
            if (i == newAttacker) return;//如果新物体已经在attcker列表里面了，则中断
        }
        attacker.Add(newAttacker);
    }
    public void FreeAttacker()
    {
        foreach (var i in attacker)//对于所有攻击死亡者的人，解除攻击者对死亡者的关注
        {
            Debug.Log("取消关注");
            GameObject tempGo = i ?? null;
            if (tempGo == null) { continue; }
            OnWalk -= i.GetComponent<Chess>().HandleOnWalk;//取消关注
        }
    }
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
    public Vector2Int GetAttackTargetLocations(Vector2Int victimPos)//!!!!!!!!!!!!!!!!!!!bug！！！！！！！！！！！！！！！！//强行解决
    {



        Vector2Int targetLocations = currentPosition;
        List<Vector2Int> chioceRange = attackRange;
        List<Vector2Int> temp = new List<Vector2Int>();
        int minPath = 9999;
        //对战车进行特殊处理,攻击目标只能是方向的正前方
        if (chessType == ChessType.Car)
        {
            if (direction == Direction.North)
                temp.Add(new Vector2Int(0, 1));
            else if (direction == Direction.South)
                temp.Add(new Vector2Int(0, -1));
            else if (direction == Direction.East)
                temp.Add(new Vector2Int(-1, 0));
            else
                temp.Add(new Vector2Int(1, 0));
            chioceRange = temp;
        }
        foreach (Vector2Int vec in chioceRange)
        {
            //坐标合法
            if((victimPos - vec).x >= 0 && (victimPos - vec).y >= 0 && (victimPos - vec).x <= 9 && (victimPos - vec).y <= 13)
            {
                Tile targetTile = Singleton<MapController>.Instance.GetTileWithPosition(victimPos - vec);
                if(victimPos - vec == currentPosition)
                    return currentPosition;
                //能攻击的点没有被占领
                if (MapController.instance.CanWalk(targetTile.tilePosition)&&!MapController.instance.IsInOrder(targetTile.tilePosition))//可以用函数
                {
                    int count = Singleton<MapController>.Instance.GetPathListCount(currentPosition, victimPos - vec);
                   // Debug.Log("步数" + count + "目的地" + (victimPos - vec));
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
    public float GetChessHurt(GameObject victim)//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!攻击数值部分！！！！！！！！！！！！！！！！！！！！！
    {
        //判断攻击时候对象是否已经被摧毁
        GameObject tempGo = victim ?? null;
        if (tempGo == null) { Debug.Log("不要鞭尸了"); return normalAttackHurt; }

        if (chessType == ChessType.Car)//车打人
        {
            if (this.GetComponent<CarChess>().IsFront(victim.GetComponent<Chess>().GetCurrentPosition()))
            {
                return normalAttackHurt;//很大量的伤害---无视防御
            }
            else
            {
                return 0;//只能对前方造成伤害
            }
        }
        if(victim.gameObject.GetComponent<Chess>().chessType == ChessType.Car)//“我”打车
        {//步兵在侧面打车造成超量攻击
            if (victim.gameObject.GetComponent<CarChess>().IsFront(GetCurrentPosition()))//此时造成伤害有限
            {
                return 0.5f * normalAttackHurt;
            }
            else
            {
                return 1.5f * normalAttackHurt;
            }

        }

        return normalAttackHurt-victim.GetComponent<Chess>().defence;//返回攻击减去防御的值，注意如果是负数会报错的

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
           // Debug.Log("到达终点");
            isMoving = false;
            StopMoveAnimation();

            //之后可以智能判断周边是否需要攻击
            MapController.instance.RedoOrder(destination);//取消对目的地的锁
            if (isAttack)
            {
                Singleton<PlayerController>.Instance.Attack(victim, this.gameObject);
                //如果是攻击状态在移动结束后需要攻击
                //得到伤害
                float hurt = GetChessHurt(victim);
                //攻击
                if (action_manager == null)
                {
                    //Debug.Log("actionManager NULL");
                    action_manager = gameObject.AddComponent<RoleActionManager>();
                }
                action_manager.Attack(gameObject, victim, hurt);
            }
            else
            {
                //自动攻击
                //AutoAttacks();
            }
        }
        else
        {
            //得到下一个位置
            if(chessType == ChessType.Car)
            {
                nextDestination = gameObject.GetComponent<CarChess>().GetNextStep(GetCurrentPosition(), destination);
            }
            else
                nextDestination = mapController.GetNextStep(GetCurrentPosition(), destination);
            //Debug.Log("下一个移动到的位置" + nextDestination);
            //如果该位置是合法的，走向该位置
            if (MapController.instance.CanWalk(nextDestination))
            {
                //移动到该位置
                MoveToPosition(nextDestination);
                if (this.OnWalk != null)
                {
                    this.OnWalk(currentPosition);//表示占领当前position
                }
            }
            else
            {
         //       Debug.Log("该位置不合法，应该停在当前位置");
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
        victim.GetComponent<Chess>().OnWalk += HandleOnWalk;           //监听被攻击者事件
        victim.GetComponent<Chess>().BeingAttackedBy(this.gameObject);//被攻击者记录攻击者
        isAttack = true;
    }
    //如果攻击目标移动则重新选择攻击位置
    public void HandleOnWalk(Vector2Int pos)
    {
        //判断对象是否已经被摧毁
        //物体已经被销毁但依旧监听
        Chess tempGo = this ?? null;
        if (tempGo == null) { return ; }

        Vector2Int vicPos = gameObject.GetComponent<Chess>().GetAttackTargetLocations(pos);
        MapController.instance.RedoOrder(destination);
        MapController.instance.OrderPosition(vicPos);//预定下一个位置
        gameObject.GetComponent<Chess>().SetDestination(vicPos);
    }
    /*
     * 减少自身的血量
     * 传入攻击者的伤害hurt
     * 无返回值
     * 调用了Chess的方法
     */
    public void ReduceBoold(float hurt)
    {
      //  Debug.Log("扣血" + hurt);
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
        //不再监听!!!!!!!!
        if(victim != null)
            victim.GetComponent<Chess>().OnWalk -= HandleOnWalk;
        isAttack = false;
    }
    public GameObject dieParticle;//死亡的粒子效果
    public void Die()
    {
        GameObject temp=Instantiate(dieParticle);//播放死亡粒子效果
        temp.transform.position = this.transform.position+new Vector3(0,0,-3);

        ReleaseCurrentPosition();//释放当前位置
        FreeAttacker();//释放所有监听
        ChessController.instance.RemoveChess(this.GetComponent<Chess>());//移除列表
     //   Destroy(this);

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
    public virtual void AutoAttacks()//
    {
        if (!isMoving && !isAttack && chessType != ChessType.Castle)
        {
            float MinBlood = 9999;
            //检测周围是否有棋子
            GameObject victim = null;

            List<Vector2Int> chioceRange = attackRange;
            List<Vector2Int> tempList = new List<Vector2Int>();
            //对战车进行特殊处理,攻击目标只能是方向的正前方
            if (chessType == ChessType.Car)
            {
                if (direction == Direction.North)
                    tempList.Add(new Vector2Int(0, 1));
                else if (direction == Direction.South)
                    tempList.Add(new Vector2Int(0, -1));
                else if (direction == Direction.East)
                    tempList.Add(new Vector2Int(-1, 0));
                else
                    tempList.Add(new Vector2Int(1, 0));
                chioceRange = tempList;
            }

            foreach (Vector2Int pos in chioceRange)
            {
                Vector2Int temp = currentPosition + pos;
                if (temp.x >= 0 && temp.y >= 0 && temp.x <= 9 && temp.y <= 13)
                {
                    Tile tile = mapController.GetTileWithPosition(temp);
                    if (tile.tileState == TileState.Occupied && tile.side == Side.playerB)
                    {
                        //检测可以攻击的棋子是否已经死亡
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
    /*
     * 战车重写该方法,找寻下一个移动的位置
     */ 
    public virtual Vector2Int GetNextStep(Vector2Int currentPos, Vector2Int destination)
    {
        return currentPos;
    }

    /*
     * 跪射俑如果是斜角攻击完之后转向到北方
     */
    public void RotateToNorth()
    {
        if (direction == Direction.Northeast || direction == Direction.Northwest
    || direction == Direction.Southeast ||direction == Direction.Southwest)
        {
            int value = -(direction - Direction.North) * 45;
            transform.Rotate(new Vector3(0, 0, 1), value);//旋转角色
            direction = Direction.North;
        }
    }
}
