using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public static class ListExtension
{
    public static void RemoveAt_Disorder<T>(this List<T> list, int index)
    {
        int lastIndex = list.Count - 1;
        list[index] = list[lastIndex];
        list.RemoveAt(lastIndex);
    }

    public static void Remove_Disorder<T>(this List<T> list, T item)
    {
        int index = list.IndexOf(item);
        if (index >= 0)
        {
            list.RemoveAt_Disorder(index);
        }      
    }
}


public class ListShuffler
{
    public static void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public enum 游戏流程阶段
{
   
    准备阶段,
    开始回合,  
    选取角色,
    角色行动,
    展示攻击范围,
    展示移动范围,
    展示系统技能支援范围,
    释放系统支援技能,
    移动后选项,
    角色动作结束,
    交换队伍,
    游戏结算
}

public enum 战场环境
{
    白天,
    夜晚,
    极昼,
    极夜,
    混沌
}

public enum GameManageTestMode
{
    NoTest,
    Test,
    
}

//胜利条件
public enum GameVictoryConditions
{
    DefeatTheEnemys,               //击败所有敌人
    Escape,                        //脱离战场
    StickToTheRounds               //坚守某个单位至某个回合
}

//关卡Boss模式
public enum GameBossType
{
    Ordinary,                      //普通敌人
    Giant                          //巨大敌人（占四格位置）
}

public enum GameDifficulty
{
    Normal,
    Hard,
    Special
}


public class GameManage : MonoBehaviour
{
    //游戏测试模式
    public GameManageTestMode _游戏测试模式;
    //胜利条件
    public GameVictoryConditions _gamevictoryconditions;
    //安置点相机位置00
    public GameObject _placementpoint00;
    //安置点相机位置01
    public GameObject _placementpoint01;

    //脱离模式信息
    public EscapeInfo _escapeInfo;
    //设定脱离成功时队员数量
    public int _escapeunitnum;
    //设定脱离的队员数量
    public int _escapenum;
    //获得目标的Transform
    Transform _currentTarget;
    //友军，敌人，中立单位
    private GameObject[] players;
    private GameObject[] enemys;
    private GameObject[] neutralitys;
    private GameObject[] buildings;

    
    public List<MyTurnUnit> _vipTarget;            //VIP单位
    private List<MyTurnUnit>player;                //友方队伍
    private List<MyTurnUnit>enemy;                 //敌方队伍
    private List<MyTurnUnit>team;                  //当前队伍
    private List<MyTurnUnit>neutralityteam;        //中立单位
    public  List<MyTurnUnit> targetTeam;           //攻击目标队伍
    public  List<MyTurnUnit> targetTeam01;          //群体攻击目标队伍
    public  List<MyTurnUnit> _allUnitTeam;
    public  List<MyTurnUnit> targetEnemyteam;    //决定胜负条件的敌方目标队伍   listCustomer.OrderBy(s => s.id).ToList();

    public List<Building> _buildinglist;


    //桥A  bool开关.地块数字.表
    public bool bridgeA;
    public List<int> _bridge00;
    public List<MyTurnCell> _bridgeCell00;
    //桥B
    public bool bridgeB;
    public List<int> _bridge01;
    public List<MyTurnCell> _bridgeCell01;
    //桥C
    public bool bridgeC;
    public List<int> _bridge02;
    public List<MyTurnCell> _bridgeCell02;
    //通信塔支援格
    public List<int> _signalnum00;
    public List<MyTurnCell> _signalCells00;

    public 游戏流程阶段 _currentGameState;

    public List<Light> 开局光源组;

    //日夜环境变换
    public 战场环境 _battlefieldenvironment;

    public EscOption _escoption;

    MyTurnMouse mtm;

    public int 场景编号;

    private bool optionopen;

    private int index; 

    private float timer =0;

    public float 自动战斗等待时间;
    
    public int turncount;

    public int MaxTurnCount;

    public int EnemyLV;

    public int StrengthenLV;
    public int _targetenemynumber;

    public int 队长等级;
    public float 生命倍率;       //关卡设置生命倍率
    public float turnclock;

    public int [] 战略支援技能表 = new int [3];
    public Skill _gmsupportskill;

    public int 初始战略点数;
    public int 当前战略点数;
    public int 队伍产生点数;
    
    int [] 生产费用统计 = new int [8];
    int [] 消耗费用统计 = new int [8];

    public int 展示队伍消耗费用;
    public int 队长连接值;
    public int 战场环境连接值;
    
    public int 支援回合倍率;     //支援回合倍率

    public float SP下降倍率;

    public int 环境DeBuff;
    private bool 自动战斗;
    public bool 允许观战;
    
    public bool 补给许可;
    
    [SerializeField] GameObject HP增加;
    [SerializeField] GameObject CP增加;
    [SerializeField] GameObject SP增加;
    [SerializeField] GameObject SUP攻击力特效;
    [SerializeField] GameObject SUP防御力特效;
    [SerializeField] GameObject SUP穿透力特效;
    [SerializeField] GameObject SUP暴击率特效;
    [SerializeField] GameObject SUP精神力特效;
    
    public bool AutoFight
    {
        set{自动战斗=value;}
        get{return 自动战斗;}
    }

    public List<MyTurnUnit> GetPlayer
    {
    set{player = value;}
    get{return player;}
    }

    public List<MyTurnUnit> GetEnemy
    {
    set{enemy = value;}
    get{return enemy;}
    }
    
    public List<MyTurnUnit> GetTeam
    {
    set{team = value;}
    get{return team;}
    }

    public List<MyTurnUnit> GetTarGetTeam
    {
    set{targetTeam = value;}
    get{return targetTeam;}
    }

    public int GetIndex
    {
        set{index = value;}
        get{return index;}
    }

    public float GetTimer
    {
        set{timer = value;}
        get{return timer;}
    }

public static GameManage Instance;

//系统信息显示
public GameInfoManger _gameinfo;

public UIManager _uimanager;

Buff _newbuff;

public List<BuffIcon> _buff图标组;

public List<Item> _itemlist;

public int 地雷激活回合;


MyTurnMap gmmp;

public int 特殊规则A;
public int 特殊规则B;

//关卡难度选择
public GameDifficulty _gamedifficulty;

//VIP列表
public VIPCardListManager _vipcardlistmanager;

Tag _newTag;

public List<Tag> _tags;

//显示直播模式标识
public Image _liveModeImage;
//直播模式
public bool _liveMode;
//去中心化
public bool _decentralizationMode;

//回合结束后的转场
public AnimationCurve _showcurveForturnchange;
public AnimationCurve _hidecurveForturnchange;
public float animationSpeed;
public GameObject panel;


//随机宝箱位置
public bool _randomChestLocation;
//随机地雷位置
public bool _randomMineLocation;


//空降部队A
public List<MyTurnUnit> _airborneUnitsForce00;
//空降区域A
public List<int> _airborneCellList00;
//空降部队B
public List<MyTurnUnit> _airborneUnitsForce01;
//空降区域B
public List<int> _airborneCellList01;
//空降部队C
public List<MyTurnUnit> _airborneUnitsForce02;
//空降区域C
public List<int> _airborneCellList02;





public UnitPushingBox _unitPushingBox;


void Awake()
{
    HideStartLight();
    _escapeInfo.gameObject.SetActive(false);
    _vipcardlistmanager.gameObject.SetActive(false);

    GameData.Instance.SetGameDifficulty(this); 
    _tags = null;
    
}

void Start()
{
    if(_gamedifficulty == GameDifficulty.Normal)
    {
        StrengthenLV = 0;
        _liveMode = false;
    }
    else if(_gamedifficulty == GameDifficulty.Hard)
    {
        StrengthenLV = 5;
        _liveMode = false;
    }
    else if(_gamedifficulty == GameDifficulty.Special)
    {
        StrengthenLV = 0;
        _liveMode = true;
    }

    Instance=this;
    自动战斗=false;  
    timer = 0;
    _escapenum = 0;
    
    mtm = GameObject.Find("GameManage").GetComponent<MyTurnMouse>();
    gmmp = transform.Find("GameSceneMap").GetComponent<MyTurnMap>();
    _newbuff = transform.Find("Buff").GetComponent<Buff>();
    _newTag = transform.Find("Tag").GetComponent<Tag>();

    if(_battlefieldenvironment == 战场环境.夜晚 || _battlefieldenvironment == 战场环境.极夜)
    {
        SetStartLight();
    }
    当前战略点数 = 初始战略点数;
    补给许可 = true;

    if(_游戏测试模式 == GameManageTestMode.NoTest)
    {
        GameData.Instance.SetGameManagerItemList(this);
    }
     
    //显示直播模式标识
    if(_liveMode == true)
    {
        _liveModeImage.gameObject.SetActive(true);
    }
    else
    {
        _liveModeImage.gameObject.SetActive(false);
    }

    if(_bridge00.Count > 0)
    {
        foreach(var o in _bridge00)
        {
           _bridgeCell00.Add( gmmp._cellList[o]);
        }
    }
   
  
}
    public void SetTeam()
    {
        players =GameObject.FindGameObjectsWithTag("PlayerLive");
        enemys =GameObject.FindGameObjectsWithTag("EnemyLive"); 
        neutralitys =GameObject.FindGameObjectsWithTag("Neutrality"); 
        buildings = GameObject.FindGameObjectsWithTag("Building");
        player =new List<MyTurnUnit>();
        enemy =new List<MyTurnUnit>();
        team = new List<MyTurnUnit>();
        targetTeam = new List<MyTurnUnit>();
        targetEnemyteam = new List<MyTurnUnit>();
        targetTeam01 = new List<MyTurnUnit>();
        _allUnitTeam = new List<MyTurnUnit>();
        neutralityteam =new List<MyTurnUnit>();
        _buildinglist = new List<Building>();
    
        foreach (var o in players)
        {
            var unit = o.GetComponent<MyTurnUnit>();
            
            print(o+"加入玩家");
            player.Add(unit);
            _allUnitTeam.Add(unit);      
        }  
       
        foreach (var o in enemys)
        {   
            var unit = o.GetComponent<MyTurnUnit>();
            print(o+"加入敌人");          
            _allUnitTeam.Add(unit);
            
            if(unit.侦查状态 != InvestigateState.伪装)              //加入敌方 自主行动/群攻/目标 队伍
            {
                enemy.Add(unit);
                targetTeam01.Add(unit);
                targetEnemyteam.Add(unit);               
            }
        }
        foreach (var o in neutralitys)
        {   
            var unit = o.GetComponent<MyTurnUnit>();
            print(o+"加入中立");
            neutralityteam.Add(unit);
            targetTeam01.Add(unit);
            _allUnitTeam.Add(unit);
        }
        foreach(var o in buildings)
        {
            var unit = o.GetComponent<Building>();
            _buildinglist.Add(unit);

        }

        var a = targetEnemyteam.OrderBy(s => s.排序优先级).ToList();             //目标队伍进行排序
        a.Reverse();                                                     
        targetEnemyteam = a;
        var b = player.OrderBy(s => s.排序优先级).ToList();                      //玩家队伍进行排序
        b.Reverse(); 
        player = b;

        if(_decentralizationMode == false)
        {
            player[0]._teamleader = TeamLeader.队长;
           
        }
        else
        {
            foreach(var o in player)
            {
                o._teamleader = TeamLeader.队长;
            }
        }
        
        team = player;                               //执行队伍为玩家队伍
        targetTeam = enemy;
        _targetenemynumber = enemy.Count;

        if( _gamevictoryconditions == GameVictoryConditions.Escape )
        {
            _escapeunitnum = player.Count;
            _escapeInfo.gameObject.SetActive(true);
            _escapeInfo._escapeNumInfo.text = "" + _escapenum + "/" + _escapeunitnum;
        }

        if( _gamevictoryconditions == GameVictoryConditions.StickToTheRounds )
        {
            _vipcardlistmanager.gameObject.SetActive(true);
        }


        UIManager.Instance.HideAllSettlenments();         //关闭所有安置点；
        mtm.SwitchState(TurnStateEnum.设置队伍);

    } 

    //生命倍率
    public void SetInitialTeamHp()
    {
        foreach(var o in team)
        {
            o.HP = o.HP*生命倍率;
        }
    }
           
    public void ChangeTeam()
    {
        index=0;    
        if(team == player)
        {  
            补给许可 = false;       
            team = enemy;
            targetTeam=player;
            foreach(var o in team)
            {           
                if( o.侦查状态 == InvestigateState.伪装 || o == null)
                {
                    o.GetMove = false;                  
                }
                else
                {
                    o.GetMove=true;                
                }                                                                       
            }
            print("行动队伍切换"); 
            Announce(); 
        }
        else
        {      
            if(!自动战斗)
            {
                if(队长连接值 == 0)
                {
                    补给许可 = false;
                  
                }
                else
                {
                    补给许可 = true;
                }
                队伍产生点数 = 0;                                 // 队伍产生点数        先置零再从team中每个成员处+生产战略点数，最后再: 当前战略点数 += 队伍产生点数。         
                team = player;
                targetTeam = enemy;
                turncount++;
                turnclock++;
                ChangeBattleFieldDayNightState();
                if(_liveMode == true)
                {
                    _gameinfo._audiencelistManager.GetMoreAudience();
                }
                
                if(turnclock >= 24)
                {
                    turnclock = 0;
                }           
                foreach(var o in team)
                {       
                    o.GetMove=true;
                    CheckBuff(o);
                    队伍产生点数 += o.生产战略点数;
                    o.生存回合数 = o.生存回合数 +1;
                    if(o.爆发类型 != ExplosionType.正常)
                    {
                        o.SP = o.SP - 30*SP下降倍率;
                        if(o.SP < 0 )
                        {
                            o.SP = 0;
                            o.爆发类型 = ExplosionType.正常;
                            var a = _gameinfo.系统信息文本.text;
                            _gameinfo.系统信息文本.text =" [回合" +turncount+"]" +o.单位名字+"解除全面爆发状态\n"; 
                            _gameinfo.系统信息文本.text  += a;
                        }                     
                    }

                    if(o.SP == o.MAXSP && o.爆发类型 == ExplosionType.正常)
                    {
                        o.爆发类型 = ExplosionType.普通爆发;
                        SetUnitExplosionState(o);
                        var a = _gameinfo.系统信息文本.text;
                        _gameinfo.系统信息文本.text =" [回合" +turncount+"]" +o.单位名字+"进入全面爆发状态\n"; 
                        _gameinfo.系统信息文本.text  += a;
                    }
                }
                foreach(var o in targetTeam)
                {
                   CheckBuff(o);
                    o.生存回合数 = o.生存回合数 +1;
                }

                if(补给许可 == true)
                {
                    当前战略点数 += 队伍产生点数;

                }
               
                mtm._currentSelectUnit=team[0];
                var b = _gameinfo.系统信息文本.text;
                _gameinfo.系统信息文本.text =" [回合" +turncount+"]开始\n"; 
                _gameinfo.系统信息文本.text  += b;         
                print("行动队伍切换"); 
                
                //StartCoroutine(HideTurnChangePanel(panel));  
            }
            else
            {
                if(队长连接值 == 0)
                {
                    补给许可 = false;
                  
                }
                else
                {
                    补给许可 = true;

                }
                队伍产生点数 = 0;
                team = player;
                targetTeam = enemy;
                turncount++;
                turnclock++;
                ChangeBattleFieldDayNightState();
                if(_liveMode == true)
                {
                    _gameinfo._audiencelistManager.GetMoreAudience();
                }
                
                if(turnclock >= 24)
                {
                    turnclock = 0;
                }

                foreach(var o in team)
                {
                    o.GetMove=true;
                    CheckBuff(o);
                    队伍产生点数 += o.生产战略点数;
                    o.生存回合数 = o.生存回合数 +1;                                                  
                }
                foreach(var o in targetTeam)
                {
                    CheckBuff(o);
                    o.生存回合数 = o.生存回合数 +1;
                }
                if(补给许可 == true)
                {
                    当前战略点数 += 队伍产生点数;
                }
                
                print("行动队伍切换");
                var a = _gameinfo.系统信息文本.text;
                _gameinfo.系统信息文本.text =" [回合" +turncount+"]开始\n"; 
                _gameinfo.系统信息文本.text  += a;
          
                //StartCoroutine(HideTurnChangePanel(panel));
                Announce(); 
            }  
        }                                  
    }

    public void ClearAttackMack(List<MyTurnUnit>  units)
    {
        foreach(var o in units)
        {   
            if( o._currentState == StateEnum.攻击目标)
            {
                o.DisSelect();
            }                   
        }
    }
   


    //胜负条件判定
    public void TeamWinOrLose(GameVictoryConditions gameVictoryConditions)                                                  
    {

        _gamevictoryconditions = gameVictoryConditions;
        switch(_gamevictoryconditions)
        {

            //玩家/敌人(目标)阵营存活人数，回合数大于最大回合
            case GameVictoryConditions.DefeatTheEnemys:
                
                if(player.Count<=0 || targetEnemyteam.Count<=0 || turncount > MaxTurnCount)            
                {
                    SwitchGameState(游戏流程阶段.游戏结算);
                }  
            break;

            //友军脱离目标数足够后
            case GameVictoryConditions.Escape:
                
                if( _escapenum == _escapeunitnum ||  player.Count<=0 ||  targetEnemyteam.Count<=0 || turncount > MaxTurnCount)
                {
                    SwitchGameState(游戏流程阶段.游戏结算);

                }
            break;

            //守护目标或据点一定回合后
            case GameVictoryConditions.StickToTheRounds:
                
                if(  player.Count<=0 ||  targetEnemyteam.Count<=0 || turncount > MaxTurnCount)
                {
                    SwitchGameState(游戏流程阶段.游戏结算);

                }
            break;



        }
                            
    }






    public void Announce()  //发布命令的消息
    {

        if(team[index].GetMove == false || team[index].GetHP <= 0 || team[index] == null)
        {
            index++;
            if( index >= team.Count)
            {
                ChangeTeam();  
            }
            else
            {
                Announce();  
            }         
        }  
        else
        {  
            team[index].GetAutoMove=true;
            team[index].GetTimer = 0 ;
            team[index]._currentAutoTurnState=AutoTurnStateEnum.选取角色;
            print("目前行动者"+team[index]);
            //夜晚时间段内，敌方单位隐藏血条UI
            if(team == enemy)
            {
               if(team[index].攻击方式 != AttackType.自爆)
               {
                    if(turnclock == 6)
                    {
                        team[index].侦查状态 = InvestigateState.正常;
                    }
                    else if(turnclock == 18)
                    {
                        team[index].侦查状态 = InvestigateState.隐匿;
                    
                    }
                }             
            }  
        }                      
    }

   public void SwitchGameState(游戏流程阶段 targetState)                          //游戏流程管理
    {       
        _currentGameState = targetState;
        switch (_currentGameState)
        {
            case 游戏流程阶段.准备阶段:
                
                break;
            case 游戏流程阶段.开始回合:
                HideStartLight();
                break;
            case  游戏流程阶段.选取角色:
               
                break;
            case  游戏流程阶段.角色行动:
                  
                break;
            case  游戏流程阶段.展示攻击范围:
               
                break;
            case  游戏流程阶段.展示移动范围:
               
                break;
            case  游戏流程阶段.移动后选项:
               
                break;
            case  游戏流程阶段.角色动作结束:
               
                break;
            case  游戏流程阶段.交换队伍:              //队伍更新                                            
                if(team == enemy )
                {
                    if(timer < 1.3f)
                    {
                        StartCoroutine(ShowTurnChangePanel(panel));

                    }
                    else
                    {
                        StartCoroutine(HideTurnChangePanel(panel));
                    } 
                }
                                                     
                if(timer > 1.5f)
                {
                    
                    ChangeTeam();
                    _currentGameState = 游戏流程阶段.选取角色;
                }
                       
                break;    
            case  游戏流程阶段.游戏结算:
                
                break;                        
        }
    }

//查询敌方行动队伍
public void PrintTargetEnemyTeam(List<MyTurnUnit> targetenmeyteam)
{
    foreach(var o in targetenmeyteam)
    {
        print("当前敌方行动队伍中有" + o);
    }

}

//关闭开局光源组
public void HideStartLight()
{
    foreach(var o in 开局光源组)
    {
        o.gameObject.SetActive(false);
    }
}

//开启开局光源组
public void SetStartLight()
{
    foreach(var o in 开局光源组)
    {
        o.gameObject.SetActive(true);
    }

}

public void BackToGameStrat()
{
    SceneManager.LoadScene(0);
}

public void BackToThisGameSences()
{
    SceneManager.LoadScene(场景编号);
}


public void GameturntoGamelobby()
{
    SceneManager.LoadScene(1);
}


//退出游戏
public void ExitGame()
{
    Application.Quit();

}


public void SetGameSpecialRule() 
{

    if( 特殊规则A == 1 )
    {
        生命倍率= 0.5f;
    }
    else if( 特殊规则A == 2 )
    {

    }

}



//一键设置支援类型为关闭


//设置空降部队降落模式
public void SetAirborneUnitsMove(MyTurnUnit unit, MyTurnCell dscell)
{

    UnitPushingBox clonebox = Instantiate(_unitPushingBox);   //_unitpushingbox
    clonebox.transform.position= dscell.transform.position+ Vector3.up*7.2f;
    clonebox._unit = unit;
    unit.transform.position = clonebox.transform.position;
    clonebox._targetCell = dscell;
    dscell.IsUnit = true;
    dscell.IsObstacle = true;
    clonebox._moveType = MoveType.空降;
    MyTurnMap.Instance.SetCellPEvalue(dscell,unit);
    clonebox.UnitMoving = true;
    
}

//空降部队A降落
public void AirborneUnitsForce00()
{
    ListShuffler.Shuffle(_airborneCellList00);

    var _celllist = new List<MyTurnCell>();

    foreach(var o in _airborneCellList00)
    {
        if(gmmp._cellList[o].IsUnit == true || gmmp._cellList[o].IsObstacle == true)
        {
            continue;
        }
        else
        {
            _celllist.Add(gmmp._cellList[o]);
        }
    }

    for(int i = 0; i<_airborneUnitsForce00.Count; i++)
    {
        MyTurnUnit clone = Instantiate(_airborneUnitsForce00[i]);
        clone.LV= EnemyLV + StrengthenLV;
        clone.transform.LookAt(player[0].transform);
        enemy.Add(clone);
        targetTeam01.Add(clone);
        clone.transform.localRotation *= Quaternion.Euler(0,90,0);
        SetAirborneUnitsMove(clone, _celllist[i]);  
    }

}
public void TowerAShowDrawLines()
{
    _buildinglist[0].SetDrawLines();
}





//桥A状态变化
public void ChangeBridgeAState()
{
    if(bridgeA == false)
    {
        MyTurnMap.Instance.SetBridgeCubeUP(_bridgeCell00,gmmp._cubeList);
        DestroyBridgeUnitByUp(_bridgeCell00,_allUnitTeam);
        bridgeA = true;
    }
    else
    {
        MyTurnMap.Instance.SetBridgeCubeDown(_bridgeCell00,gmmp._cubeList);
        DestroyBridgeUnitByDown(_bridgeCell00,_allUnitTeam);
        bridgeA = false;
    }
}

//消灭桥面上的单位
private void DestroyBridgeUnitByUp(List<MyTurnCell> _cells,List<MyTurnUnit> _units)
{
    for(int i=0; i <_units.Count ; i++)
    {
        if(_units[i]._currentMoveType == MoveTypeEnum.空中飞行|| _units[i]._currentMoveType == MoveTypeEnum.近地悬浮)
        {
            continue;
        }
        else
        {
            for(int j=0; j <_cells.Count ; j++)
            {
                if(_units[i].CurrentCell == _cells[j])
                {
                    _units[i]._uptoDeath = true;
                    var b = (_units[i].MixHP  +_units[i].LV*_units[i].生命成长率+_units[i].道具HP上限)*-1.5f;           
                    var a = _gameinfo.系统信息文本.text;
                    _gameinfo.系统信息文本.text =" [回合"+ turncount+"]" + _units[i].单位名字 + "被不明伤害击倒了。" + "\n"; 
                    _gameinfo.系统信息文本.text += a; 
                    
                    _units[i].GetDamage(_units[i].CurrentCell,b); 
                }
            }
        }  
    }
}

private void DestroyBridgeUnitByDown(List<MyTurnCell> _cells,List<MyTurnUnit> _units)
{
    for(int i=0; i <_units.Count ; i++)
    {
        if(_units[i]._currentMoveType == MoveTypeEnum.空中飞行|| _units[i]._currentMoveType == MoveTypeEnum.近地悬浮)
        {
            continue;
        }
        else
        {
            for(int j=0; j <_cells.Count ; j++)
            {
                if(_units[i].CurrentCell == _cells[j])
                {
                    _units[i]._falltoDeath = true;
                    var b = (_units[i].MixHP  +_units[i].LV*_units[i].生命成长率+_units[i].道具HP上限)*-1.5f;           
                    var a = _gameinfo.系统信息文本.text;
                    _gameinfo.系统信息文本.text =" [回合"+ turncount+"]" + _units[i].单位名字 + "被不明伤害击倒了。" + "\n"; 
                    _gameinfo.系统信息文本.text += a; 
                    _units[i].GetDamage(_units[i].CurrentCell,b); 
                }
            }
        }  
    }
}


//向队员设置补给支援
public void SetSupport(MyTurnUnit unit)
{
    if(unit.战略支援类型 == 1  && unit.linklevel != 0 )
    {
       
        _newbuff._num = 1 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 5*队长等级;
        _newbuff.编号 = BuffNum.一号Buff;
        _newbuff.效果类型 = BuffType.恢复数值类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
        
    }
    else if(unit.战略支援类型 == 2 && unit.linklevel != 0)
    {     
        _newbuff._num = 2 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 10;
        _newbuff.编号 = BuffNum.二号Buff;
        _newbuff.效果类型 = BuffType.恢复数值类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
       
    }
    else if(unit.战略支援类型 == 3 && unit.linklevel != 0)
    {     
        _newbuff._num = 3 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 10 ;  
        _newbuff.编号 = BuffNum.三号Buff;
        _newbuff.效果类型 = BuffType.恢复数值类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
        
    }
    else if(unit.战略支援类型 == 4 && unit.linklevel != 0)
    {      
        _newbuff._num = 4 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 5*队长等级;
        _newbuff.编号 = BuffNum.四号Buff;
        _newbuff.效果类型 = BuffType.属性增强类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
        
    }
    else if(unit.战略支援类型 == 5 && unit.linklevel != 0)
    {     
        _newbuff._num = 5 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 5*队长等级;
        _newbuff.编号 = BuffNum.五号Buff;
        _newbuff.效果类型 = BuffType.属性增强类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
        
    }
    else if(unit.战略支援类型 == 6 && unit.linklevel != 0)
    {     
        _newbuff._num = 6 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 20;
        _newbuff.编号 = BuffNum.六号Buff;
        _newbuff.效果类型 = BuffType.属性增强类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
        
    }
    else if(unit.战略支援类型 == 7 && unit.linklevel != 0)
    {       
        _newbuff._num = 7 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 20;
        _newbuff.编号 = BuffNum.七号Buff;
        _newbuff.效果类型 = BuffType.属性增强类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
        
    }
    else if(unit.战略支援类型 == 8 && unit.linklevel != 0)
    {         
        _newbuff._num = 8 ;
        _newbuff._lv = 队长等级;
        _newbuff.基本数 = 20;
        _newbuff.编号 = BuffNum.八号Buff;
        _newbuff.效果类型 = BuffType.属性增强类;
        _newbuff.结果类型 = BuffResult.增益;
        _newbuff.持续回合 = 1*支援回合倍率;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
      
    }    
}



//设置玩家队伍补给支援
public void SetPlayerTeamSupport()
{
    if(补给许可 == true   )
    {
        if( 当前战略点数 < 展示队伍消耗费用 && 队长连接值 == 0  )
        {
            补给许可 = false;
             
        }
        else
        {
            foreach(var o in player)
            {
                if(o.战略支援类型 !=0 )
                {
                    SetSupport(o);
                }
            }
            当前战略点数 -= 展示队伍消耗费用;
            补给许可 = false; 
        }      
    }    
}


//实时计算队伍支援消耗点数
public void GetSupportCostPoint()
{
    
    for(int i = 0 ; i < Math.Max(消耗费用统计.Length,player.Count) ; i++)
    {
        if( i < player.Count)
        {
            消耗费用统计[i] = player[i].最终消耗费用;
        }
        else
        {
            消耗费用统计[i] = 0;
        }
    }   
    展示队伍消耗费用 = 消耗费用统计.Sum();
}


//设置角色爆发Buff
public void SetUnitExplosionState(MyTurnUnit unit)
{
    if(unit.爆发类型 == ExplosionType.普通爆发)
    {
        _newbuff._num = 33 ;
        _newbuff._lv = unit.LV;
        _newbuff.基本数 = 5*unit.LV;
        _newbuff.编号 = BuffNum.三十三号Buff;
        _newbuff.效果类型 = BuffType.特殊机制类;
        _newbuff.结果类型 = BuffResult.增益;
        int result = (int)Math.Ceiling(100/(30*SP下降倍率));                             //通过Math.Ceiling方法获得 SP下架倍率提升后得到的Explosion持续回合
        _newbuff.持续回合 = result;
        _newbuff.终止回合 = turncount + _newbuff.持续回合;
        CreateBuff(unit);
    }
}


//切换环境状态
public void ChangeBattleFieldDayNightState()
{
    if(_battlefieldenvironment == 战场环境.白天 || _battlefieldenvironment == 战场环境.夜晚)
    {
        if(turnclock == 6)
        {
            _battlefieldenvironment = 战场环境.白天;

        }
        else if(turnclock == 18)
        {
            _battlefieldenvironment = 战场环境.夜晚;

        }
    }
}


//创建新Buff
public void CreateBuff(MyTurnUnit unit)
{
    Buff clonebuff = Instantiate(_newbuff);
    clonebuff.transform.SetParent(unit.transform, true);
    clonebuff.gameObject.SetActive(true);
    clonebuff.transform.position = unit.transform.position;
    clonebuff.GetComponent<Buff>().SetTarget(unit);
    unit._bufflist.RemoveAll(x => x == null);
    unit._bufflist.Add(clonebuff);
    unit._healthBar.Buff组.RemoveAll(x => x == null);
    unit._healthBar.Buff组.Add(clonebuff);
    unit._healthBar.ShowBuffList();
    
}

//创建新tag
public void CreateTag()
{
    Tag clonetag = Instantiate(_newTag);
    clonetag.transform.SetParent(transform,true);
    clonetag.gameObject.SetActive(true);
    _tags.Add(clonetag);
}


//Buff检查
public void CheckBuff(MyTurnUnit unit)
{   
    foreach(var o in unit._bufflist)
    {     
        o.持续回合--;        
        if( o.持续回合 > 0 )    //只要持续回合大于0，每回合都调用一次
        {
            if( o.效果类型 == BuffType.恢复数值类   )
            {
                o.SetSwithBuffState(o.编号,unit);
            } 
        } 
        if( o.持续回合 == 0)
        {
            if(o.效果类型 == BuffType.属性增强类)
            {
                o.CancelSwithBuffState(o.编号,unit);
            }
            else if(o.效果类型 == BuffType.特殊机制类)
            {
                o.CancelSwithBuffState(o.编号,unit);
            }
        }
    }
    unit._bufflist.RemoveAll(x => x == null);
    unit._healthBar.Buff组.RemoveAll(x => x == null);
    unit._healthBar.ShowBuffList();    
}

   //显示转场回合数
   IEnumerator ShowTurnChangePanel(GameObject gameObject)
   {
        float tctimer = 0;
        while(tctimer <= 1)
        {
            gameObject.transform.localScale = Vector3.one*_showcurveForturnchange.Evaluate(tctimer);
            tctimer += Time.deltaTime*animationSpeed;
            yield return null;
        }

   }

   IEnumerator HideTurnChangePanel(GameObject gameObject)
   {
        float tctimer = 0;
        while(tctimer <= 1)
        {
            gameObject.transform.localScale = Vector3.one*_hidecurveForturnchange.Evaluate(tctimer);
            tctimer += Time.deltaTime*animationSpeed;
            yield return null;
        }
   }


private void GameManageMouseInput()
{
    if (Input.GetMouseButtonDown(0))
    {
        switch (_currentGameState)
        {
            case 游戏流程阶段.展示系统技能支援范围:
        
            if(player[0]._currentskill._skillcampsort == SkillCampSort.敌方减益类)
            {
                if( !mtm._currentCell.NormalCell)   //格子必须是非普通，即_UnitAttckCellList内的格子
                {
                    if(gmmp._UnitAttckCellList.Contains(mtm._currentUnit.CurrentCell))   //角色的攻击范围格子必须包含目标当前位置格子
                    {
                        if( mtm._currentUnit.CampSort != CampSortEnum.玩家 )    //攻击角色的阵营与目标角色阵营不一样
                        {

                            当前战略点数 -=  player[0]._currentskill.消耗费用;                               
                            player[0].SkillTo(mtm._currentUnit);
                            SwitchGameState(游戏流程阶段.角色行动);                               
                        }
                    }
                }
                else
                {
                    SwitchGameState(游戏流程阶段.角色行动);
                }                          

            }
            else if(player[0]._currentskill._skillcampsort == SkillCampSort.敌方群体减益类)
            {
                if(!mtm._currentCell.NormalCell)
                {
                    var tmp= MyTurnMap.Instance.GetTargetUnitsCell(player[0], player[0]._currentskill.技能释放范围,mtm._currentCell);
                    player[0].GetTargetUnitSlist(targetTeam01,tmp);
                           
                    if(player[0]._targetUnitList.Count > 0)
                    {  
                        当前战略点数 -=  player[0]._currentskill.消耗费用;
                        player[0].SkilltoAllTarget(player[0]._targetUnitList,mtm._currentCell);
                                                                       
                    }
                }   
            }
            else if(player[0]._currentskill._skillcampsort == SkillCampSort.己方增益类)
            {
                        if( !mtm._currentCell.NormalCell)                  //格子必须是非普通，即_UnitAttckCellList内的格子
                        {
                            if(gmmp._UnitAttckCellList.Contains(mtm._currentUnit.CurrentCell))   //角色的攻击范围格子必须包含目标当前位置格子
                            {
                                if(player[0].CampSort == mtm._currentUnit.CampSort && mtm._currentCell == mtm._currentUnit.CurrentCell)    //攻击角色的阵营与目标角色阵营不一样
                                {
                                    player[0].SkillTo(mtm._currentUnit);
                                    当前战略点数 -=  player[0]._currentskill.消耗费用;  
                                    SwitchGameState(游戏流程阶段.角色行动);
                                                                 
                                }
                            }
                            else
                            {
                               SwitchGameState(游戏流程阶段.角色行动);
                            }
                        }

            }                                              
            else if( player[0]._currentskill._skillcampsort == SkillCampSort.己方群体增益类)
            {
                        if(!mtm._currentCell.NormalCell)
                        {

                            var tmp= MyTurnMap.Instance.GetTargetUnitsCell(player[0],player[0]._currentskill.技能释放范围,mtm._currentCell);
                            player[0].GetTargetUnitSlist(player,tmp);
                            
                            if(player[0]._targetUnitList.Count > 0)
                            {
                                当前战略点数 -=  player[0]._currentskill.消耗费用;  
                                player[0].SkilltoAllTarget(player[0]._targetUnitList,mtm._currentCell);
                            }                                                

                        }
            }
            else if(player[0]._currentskill._skillcampsort == SkillCampSort.系统收益类)
            {

            }
            else if(player[0]._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
            {
                if(!mtm._currentCell.NormalCell)
                {
                    var tmp= MyTurnMap.Instance.GetTargetUnitsCell(player[0],player[0]._currentskill.技能释放范围,mtm._currentCell);
                    player[0].GetTargetUnitSlist(_allUnitTeam,tmp);                                //*重点分类
                    if(player[0]._currentskill.编号 == SkillNum.七号技能 || player[0]._currentskill.编号 == SkillNum.五十号技能)
                    {
                        MyTurnMap.Instance.TargetCellInvestigate(MyTurnMap.Instance.GetTargetUnitsCell(player[0],player[0]._currentskill.技能释放范围,mtm._currentCell));
                    } 
                    当前战略点数 -=  player[0]._currentskill.消耗费用;
                    player[0].SkilltoAllTarget(player[0]._targetUnitList,mtm._currentCell); 

                }
            }
                      
            break;
            case 游戏流程阶段.释放系统支援技能:
           
            SwitchGameState(游戏流程阶段.角色行动);   
            break;

        }
    }
    if (Input.GetMouseButtonDown(1))
    {
        switch (_currentGameState)
        {
            case 游戏流程阶段.展示系统技能支援范围:
            ClearAttackMack(_allUnitTeam);
            
            MyTurnMap.Instance.ClearPath();
            Monitor.Instance._cameraMode = CameraMode.跟踪模式;
            SwitchGameState(游戏流程阶段.角色行动);
            break;

        }
    }
}


    private void GameManageMouseDetect()
    {
        if (!EventSystem.current.IsPointerOverGameObject())      //防止射线检测穿透UI
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mouseRay, out hitInfo))
            {
                _currentTarget = hitInfo.transform;

                MyTurnCell cell = _currentTarget.GetComponent<MyTurnCell>();               
                if (cell != null)
                {
                   MyTurnMap.Instance.GenerateSkillRange(player[0], player[0]._currentskill);
                   if(player[0]._currentskill._skillcampsort == SkillCampSort.敌方群体减益类 || player[0]._currentskill._skillcampsort == SkillCampSort.己方群体增益类 || player[0]._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
                    {
                        MyTurnMap.Instance.GetTargetUnitsCell(player[0],player[0]._currentskill.技能释放范围,mtm._currentCell);
                    }
                    mtm._currentCell = cell;
                    cell.SwitchState(CellStateEnum.可使用);       
                    
                }

                MyTurnUnit unit = _currentTarget.GetComponent<MyTurnUnit>();               

                if (unit != null)
                {    
                    if(player[0]._currentskill._skillcampsort == SkillCampSort.敌方群体减益类  )    //_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方减益类 || 
                    {

                        if (mtm._currentUnit != null )   //&&  mtm._currentUnit != unit  && mtm._currentUnit != player[0]
                        {
                            mtm._currentUnit.DisSelect();
                        }
                            mtm._currentUnit = unit;
                        if(player[0].CampSort != mtm._currentUnit.CampSort)
                        {

                            mtm._currentUnit.SelectAttack();
                            mtm._currentCell = mtm._currentUnit.CurrentCell;

                        }
                        else
                        {
                            //mtm._currentUnit.PreSelect();
                        }
                        mtm._currentCell =  mtm._currentUnit.CurrentCell;
                        MyTurnMap.Instance.GenerateSkillRange(player[0],player[0]._currentskill);
                        MyTurnMap.Instance.GetTargetUnitsCell(player[0], player[0]._currentskill.技能释放范围,mtm._currentCell);                      
                    }
                    else if(player[0]._currentskill._skillcampsort == SkillCampSort.己方群体增益类   )     //      && _currentUnit != _currentSelectUnit
                    {
                        if (mtm._currentUnit != null &&  mtm._currentUnit != unit  && mtm._currentUnit != mtm._currentSelectUnit )
                        {
                            mtm._currentUnit.DisSelect();
                        }
                        mtm._currentUnit = unit;
                               
                        if( mtm._currentUnit.CampSort == player[0].CampSort  )
                        {
                            mtm._currentUnit.SelectAttack();
                            mtm._currentCell = mtm._currentUnit.CurrentCell;

                        }
                        else
                        {
                            mtm._currentUnit.PreSelect();
                        }
                        mtm._currentCell = mtm._currentUnit.CurrentCell;
                          
                        MyTurnMap.Instance.GenerateSkillRange(player[0],player[0]._currentskill);
                        MyTurnMap.Instance.GetTargetUnitsCell(player[0], player[0]._currentskill.技能释放范围,mtm._currentCell);   
                                                            
                    }
                    else  if(player[0]._currentskill._skillcampsort == SkillCampSort.己方增益类  )
                     {

                        if (mtm._currentUnit != null  &&  mtm._currentUnit != unit    )     //    && _currentUnit != unit         && _currentUnit != _currentSelectUnit
                        {
                                  
                            mtm._currentUnit.DisSelect();
                        }
                            mtm._currentUnit = unit;
                        if(mtm._currentUnit.CampSort == player[0].CampSort )
                        {
                            mtm._currentUnit.SelectAttack();
                        }
                        else
                        {
                            mtm._currentUnit.PreSelect();  
                        }
                        MyTurnMap.Instance.GenerateSkillRange(player[0],player[0]._currentskill);
                        mtm._currentCell = mtm._currentUnit.CurrentCell;
                        mtm._currentUnit.CurrentCell.High();
                    } 
                    else if( player[0]._currentskill._skillcampsort == SkillCampSort.敌方减益类  )
                    {
                        if (mtm._currentUnit != null  && mtm._currentUnit != unit  && mtm._currentUnit != mtm._currentSelectUnit)     //    && _currentUnit != unit
                        {
                            mtm._currentUnit.DisSelect();
                        }
                        mtm._currentUnit = unit;
                        if(mtm._currentUnit.CampSort != player[0].CampSort)
                        {
                            mtm._currentUnit.SelectAttack();
                        }
                        else
                        {
                            mtm._currentUnit.PreSelect();
                        }                             
                        MyTurnMap.Instance.GenerateSkillRange(player[0],player[0]._currentskill);
                        mtm._currentCell = mtm._currentUnit.CurrentCell;
                        mtm._currentUnit.CurrentCell.High();
                    } 
                    else if(player[0]._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
                    {
                        if (mtm._currentUnit != null &&  mtm._currentUnit != unit  && mtm._currentUnit != mtm._currentSelectUnit )
                        {
                            mtm._currentUnit.DisSelect();
                        }
                        mtm._currentUnit = unit;
                               
                        if( mtm._currentUnit.CampSort != player[0].CampSort  )
                        {
                            mtm._currentUnit.SelectAttack();
                            mtm._currentCell = mtm._currentUnit.CurrentCell;

                        }
                        else
                        {
                            mtm._currentUnit.PreSelect();
                        }
                        mtm._currentCell = mtm._currentUnit.CurrentCell;                          
                        MyTurnMap.Instance.GenerateSkillRange(player[0],player[0]._currentskill);
                        MyTurnMap.Instance.GetTargetUnitsCell(player[0], player[0]._currentskill.技能释放范围,mtm._currentCell);   

                    }
                }
            }
            else                                              //测试预选状态
            {
                if ( mtm._currentUnit!=mtm._currentSelectUnit  )      //_currentUnit != null &&
                {
                    mtm._currentUnit.DisSelect();
                }                  
            }
        }    
        else
        {
            if (mtm._currentCell != null)
            {
                mtm._currentCell.Normal();
                mtm._currentCell = null;
            }
        }      
    }


void Update()
{   


    ChangeBattleFieldDayNightState();
    SwitchGameState(_currentGameState);
    
    if(player.Count > 0 && _currentGameState != 游戏流程阶段.准备阶段)
    {
        UIManager.Instance.ShowPlayerCard(this);
    }
    


    if(_currentGameState== 游戏流程阶段.交换队伍)
    {
        timer += Time.deltaTime;
    }
    else
    {
        timer=0;
    }

    if(_currentGameState == 游戏流程阶段.展示系统技能支援范围 )
    {
        GameManageMouseDetect();
        GameManageMouseInput();

    }
    if(turncount > 1)
    {
        TeamWinOrLose(_gamevictoryconditions);
        if(_currentGameState == 游戏流程阶段.游戏结算)
        {
            GameScore.Instance.ShowGameScore();
        } 
      
    }


    
    if(Input.GetKeyDown(KeyCode.Escape))
    {
        if(optionopen==false)
        {
            optionopen=true;
            _escoption.ShowEsc();
        }
        else
        {
            optionopen=false;
            _escoption.HideEsc();

        } 
    } 

    
    if(Input.GetKeyDown(KeyCode.Space))
    {
        enemy[0].CP = enemy[0].CP + 100;
    }
    
   

}

}
