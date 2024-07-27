
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum CharacterLock
{
    锁定,
    解锁
}
public enum UnitSize
{
    Normal,
    Giant,
}
//阵营
public enum CampSortEnum
{
    玩家,
    敌人,
    杂物,
    建筑,
    中立
}
//势力
public enum Forces
{
    游戏测试,
    十九号事务所,
    猛狼队,
    生命研究所,
    和胜都,
    第四机动装甲师,
    联邦国防部,
    美蓝商业联合会,
    丧尸权益保护委员会,
    武装女仆团

}
//种族
public enum UnitRace
{
    人形,
    野兽,
    合成体,
    机械体,
    意识体
}

//职业
public enum UnitCareer
{
    新手,
    喽啰,
    成年野兽,
    突击手,
    武师,
    侦查兵,
    机枪兵,
    重装机枪兵,
    狙击手,
    暗杀者,
    炮手,
    间谍,
    支援兵,
    召唤物与陷阱,
    宝箱猎人,
    偶像,
    神枪手,
    执法者,
    机甲
}

//移动模式
public enum MoveTypeEnum
{   
    地面行走,
    近地悬浮,
    空中飞行,
    水面航行,
}  

//攻击模式
public enum AttackType
{
    近战,
    弹道,
    炮击,
    自爆,
    光束,
    精神
}
//行动模式
public enum AIType
{
    待机,
    巡逻,
    索敌进攻
}

//索敌方式
public enum GetTargetUnitEnum
{      
    直接距离,
    移动距离,
    血量最低,
    据点突袭,
    待机
}

public enum GetTargetUnitCell
{
    直线距离,
    移动距离
}

//侦查状态
public enum InvestigateState
{
    正常,
    被点亮,
    隐匿,
    伪装,
    心灵标记,
    准备登场
}
//爆发状态
public enum ExplosionType
{
    正常,
    普通爆发,
    兽神爆发,
    机神爆发
}
//行动力状态
public enum CommitmentState
{
    绿色,
    黄色,
    红色,
}

//自动进行回合
public enum AutoTurnStateEnum
{   
    选取角色,
    角色行动,
    展示攻击范围,
    展示技能范围,
    展示移动范围,
    移动后行动,
    进行移动,
    角色动作结束,
    角色死亡 
}
//OutLine状态
public enum StateEnum
{   
    闲置,
    预选,
    选中,
    攻击目标,
    地雷伪装
}
//语音谈话状态
public enum TalkVoiceState
{
    无,
    待机,
    攻击,
    受伤,
    受伤至危急状态,
    使用技能,
    受到治疗,
    危急状态受到治疗,
    死亡,
    复活,
}
//连接状态
public enum LinkState
{
    无连接,
    绿色状态,
    黄色状态,
    红色状态,
}
//职务
public enum TeamLeader
{
    普通,
    队员,
    队长,
    精英,
    危险
}
//支援模式
public enum SupportType
{
    关闭,
    HP增加,
    CP增加,
    SP增加,
    攻击力增加,
    防御力增加,
    穿透力增加,
    暴击率增加,
    精神力增加
}

public enum HealthState
{
    正常,
    危急
}

public enum UnitRenderingMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent
}

[RequireComponent(typeof(Outline))]
public class MyTurnUnit : MonoBehaviour
{

    public  CharacterLock _characterlock;
    public UnitSize _unitSize;
    public bool 选中;
    public int 头像编号;
    public int 排序优先级;
    public CampSortEnum CampSort;
    public Forces 势力;
    public UnitRace  种族;
    public UnitCareer 职业;
    public MoveTypeEnum _currentMoveType;

    public bool _shodowUnit;                  //强化隐匿

    Color _originalcolor;
    Color _newcolor;
    public Material _selfUnitMat;                      //单位材质
    public UnitRenderingMode renderingMode;                //材质模式

    public string _movetype;
    public AutoTurnStateEnum _currentAutoTurnState;
    public GetTargetUnitEnum 索敌方式;
    public AttackType 攻击方式;
    public GetTargetUnitCell 格子选择;
    public AIType 行动模式;
    public InvestigateState 侦查状态;
    public CommitmentState 行动力状态;
    public StateEnum _currentState;
    public MyTurnCell CurrentCell; 
    public MyTurnPoint CurrentPoint; 
    

    
    public MyTurnCell targetCell;
    public MyTurnPoint targetPoint;

    public Bullet _currentbullet;

    public float _bulletSpeed;

    public GameObject _firePoint;

    public Vector3 _firePoistion;

    public int mapCol;

    public int transformNum;
    public int transformRotationNum;
    public MyTurnCell _moveTotargetCell;
    public MyTurnPoint _moveTotargetPoint;
    public List<MyTurnUnit> _targetUnitList ;
    public Skill _currentskill;
    public int[] 技能组; 
    
    public string 单位名字;
    public float MixHP = 100; 
    public int LV = 1;
    public int attckrange;
    public int 固定伤害范围 ;
    public int DamageRange;

    public int Pvalue;
    public int Evalue;
    public float MAXHP;
    public float HP;   
    public float MAXCP;
    public float CP;

    public float MAXSP;
    public float SP;   
    public float 生命成长率 = 1;
    public float 道具HP上限;
    public float 攻击成长率 = 1;
    public float 防御成长率 = 1;
    
    public float 消耗行动力;
    public float 最大恢复行动力;
    public float 当前恢复行动力;

    public float 道具CP上限;
    public float BuffCP;

    public float 道具SP上限;

    public string 攻击类型;
    public float 基础攻击力;           //0级基础攻击力
    public float 道具攻击力;           //道具攻击力
    public float 战斗攻击力;           //进入战斗时根据等级计算出的面板攻击力 战斗攻击力 = 基础攻击力+LV*攻击成长率
    public float 最大攻击力;           //在接受了Buff加成下的面板攻击力  最大攻击力 = 战斗攻击力+Buff攻击力
    public float Buff攻击力;           //由Buff提供的攻击力加成

    public string 护甲类型;
    public float 基础防御力;
    public float 道具防御力;
    public float 战斗防御力;
    public float 最大防御力;
    public float Buff防御力;

    public string 穿透类型;
    public float 基础穿透力;
    public float 道具穿透力;
    public float 战斗穿透力;
    public float 最大穿透力;
    public float Buff穿透力;

    public float 基础暴击率;
    public float 道具暴击率;
    public float 战斗暴击率;
    public float 最大暴击率;
    public float Buff暴击率;

    public float 基础精神力;
    public float 道具精神力;
    public float 战斗精神力;
    public float 最大精神力;
    public float Buff精神力;

    public float 基础暴击伤害倍率;
    public float 最大暴击伤害倍率;

    public bool 被暴击;
    Outline _selfOutLine;
    public Animator _selfAnim;
    GameManage gm;
    MyTurnMap gmmap;
    MyTurnMouse mtm;
    
    //游戏系统信息管理
    GameInfoManger gim;

    Material _selfMat;
   
    public int 基础移动范围;
   
    public int moverange;     //实际移动范围

    public int 生存回合数;
    public int 击杀敌人数;
   
    public float attackdamge;

    public float 模型消失时间;
    public TalkVoiceState 谈话语音状态;
    public float 谈话语音时间;
    public MyTurnUnit targetUnit;
    public HealthBar _healthBar;
    public DamageNum _damageNum;

    PlayerCard _playerCard;

    MaskRotation _mask;

    ShadowVFX _shadowvfx;

    ExplosionVFX _explosionvfx;

    private bool unitselect;     //单位是否选择

    public bool GetUnitselect    //角色状态卡使用
    {
        set { unitselect=value; }
        get { return unitselect;}

    }
    private bool canMove;
    public bool GetMove
    {
        set { canMove=value; }
        get { return canMove;}
    }

    private bool AutocanMove;
    public bool GetAutoMove
    {
        set { AutocanMove=value; }
        get { return AutocanMove;}
    }
    //角色是否被包围
    private bool surround;

    public float GetHP
    {
        set{ MAXHP=value;}
        get{ return MAXHP;}
    }

private Vector3 targetPos;
public float maxdistance;
public float timer;
public  float idleTime;

public float damage;

public float waittimer;

public string[] 对话模组;

/*
0:开始 1:行动 2:受伤 3:受伤至危急状态   4:受到治疗 5:危急状态受到治疗 6:支援 7:死亡 8:复活
*/

public float GetUnitDamage
{
    set{ damage = value;}
    get{ return damage;}
}

public float GetTimer
{
    set{ timer=value;}
    get{ return timer;}
}

public LinkState _linkstate;
public int 生产战略点数;
public int 被击杀产生战略点数;
public int 击杀倍率;
public int 基础消耗战略点;
public int 消耗倍率;
public int 最终消耗费用;
public int 支援距离;
public int linklevel;
public int 环境连接值;
public int 区域连接值;
public int 控制连接值;
public int 战略支援类型;
public TeamLeader _teamleader;
public SupportType _supporttype;
public List<int> Buff组; 
public List<Buff> _bufflist;
public ExplosionType 爆发类型;
public HealthState 生命状态;
public List<Item>  _itemlist;

public List<UnitBodyEntity> _unitbodyentitylist;

public UnitTower _unitTower;
public bool fire;

public bool _falltoDeath;            //摔落致死
public bool _uptoDeath;              //上升致死
public bool _selfexplosion;           //自爆致死

public MyTurnCell _powerCell;
public MyTurnCell _destinationCell;


    void Start()
    {
        gm = GameObject.Find("GameManage").GetComponent<GameManage>();
        mtm = GameObject.Find("GameManage").GetComponent<MyTurnMouse>();
        gmmap = GameObject.Find("GameManage/GameSceneMap").GetComponent<MyTurnMap>();
        gim  = GameObject.Find("Canvas/游戏信息").GetComponent<GameInfoManger>();

        _targetUnitList = new List<MyTurnUnit>();       
        canMove=false;
        AutocanMove=false;
        mapCol = gmmap.col -3;

        if(CampSort ==CampSortEnum.玩家)
        {
            _unitbodyentitylist[1].gameObject.SetActive(false);
            _unitbodyentitylist[2].gameObject.SetActive(false);  
        }

        if(CampSort ==CampSortEnum.敌人)
        {
            LV = gm.EnemyLV + gm.StrengthenLV; 
           
        }

        MAXHP = MixHP +LV*生命成长率+道具HP上限;
        HP = MAXHP;
        CP = MAXCP;
        SP = 0;
        
        战斗攻击力 = 基础攻击力+LV*攻击成长率+道具攻击力;
        最大攻击力 = 战斗攻击力;

        战斗防御力 = 基础防御力+LV*防御成长率+道具防御力;
        最大防御力 = 战斗防御力;

        战斗穿透力 = 基础穿透力+道具穿透力;
        最大穿透力 = 战斗穿透力;

        战斗暴击率 = 基础暴击率+道具暴击率;
        最大暴击率 = 战斗暴击率;

        战斗精神力 = 基础精神力+道具精神力;
        最大精神力 = 战斗精神力;

        最大暴击伤害倍率 = 基础暴击伤害倍率;

        maxdistance=0.1f;
        targetPos = Vector3.zero; 
        idleTime=1f;

        if(_shodowUnit == true)
        {
            _originalcolor = _selfUnitMat.color;
            _newcolor= new(1,1,1,0);

        }

       _selfOutLine = GetComponent<Outline>(); 
        if(this.职业 != UnitCareer.召唤物与陷阱  )
        {
            _selfOutLine.outlineWidth = 4;

        }
        else
        {
            _selfOutLine.outlineWidth = 0;

        }

        Maskbar.Instance.SetTargetMask(this);
        moverange = 基础移动范围;
        if(gm._decentralizationMode == false)
        {
            支援距离 = moverange ;
        }
        else
        {
            支援距离 = 1;
        }
        谈话语音时间 = 0;
        _selfAnim = GetComponent<Animator>();
        UIManager.Instance.CreateBar(this);
        ShadowMask.Instance.SetTargetShadowMask(this);               
    }
 

    //设置单位到地块上（给地块周围赋P值和E值）
    public void SetCell(MyTurnCell cell)
    {
        if(this.职业 != UnitCareer.召唤物与陷阱)
        {
            MyTurnMap.Instance.SetCellPEvalue(cell,this);
        }      
        CurrentCell = cell;
        transform.position = cell.transform.position;     
    }

    public void SetPoint(MyTurnPoint point)
    {
        CurrentPoint = point;
        CurrentCell = point._currentCellList[0];
        transform.position = point.transform.position;
    }


    public void GetOutLine()
    {
        _selfOutLine = GetComponent<Outline>();
        if (_selfOutLine==null)
        {
            _selfOutLine = transform.GetComponentInChildren<Outline>();
        }
    }

    

    public void MoveToCell(List<MyTurnCell> path)  //移动到Cell
    {
        StartCoroutine(MoveCor(path));        
    }

    public void AutoMoveToCell(List<MyTurnCell> path)  //移动到Cell
    {
        StartCoroutine(AutoMoveCor(path));        
    }
    public void AutoMoveToPoint(List<MyTurnPoint> path)  //移动到点
    {
        StartCoroutine(AutoMoveCor(path));

    }



   //玩家控制移动结束时，最后一步加载 MyTurnMouse.Instance.SwitchState(TurnStateEnum.移动后选项)
    IEnumerator MoveCor(List<MyTurnCell> path)                     //使用协程，计算移动路径
    {
        _selfAnim.SetBool("Run",true);     
        while (path.Count > 0)
        {
            float workTime = 0;
            Vector3 originPos = transform.position;
            Vector3 desPos = path[0].transform.position;
            GameObject tmp = new GameObject();
            tmp.transform.position = transform.position;
            tmp.transform.LookAt(desPos);
            Quaternion originRot = transform.rotation;
            Quaternion desRot = tmp.transform.rotation;
            Destroy(tmp);
            while (true)
            {
                workTime += Time.deltaTime*4.5f;             // 调整协程完成每一步的时间 ，数值越大越快
                transform.position = Vector3.Lerp(originPos, desPos, workTime);
                transform.rotation = Quaternion.Lerp(originRot, desRot, workTime * 3f);
                ModelUpdating();                                    //模型转向修正，防止走向地形差时发生X轴转向。       
                if (workTime >= 1)
                {
                    CurrentCell = path[0];
                    path.RemoveAt(0);
                    break;
                }
                yield return null;                  
            }
        }
        _selfAnim.SetBool("Run", false);
        MyTurnMap.Instance.ClearPath();
        MyTurnMouse.Instance.SwitchState(TurnStateEnum.移动后选项);
    }    


    //自动移动结束时 最后一步加载  SwitchAutoState(AutoTurnStateEnum.移动后行动);
    IEnumerator AutoMoveCor(List<MyTurnCell> path)               
    {  
        _selfAnim.SetBool("Run",true);       
        while (path.Count > 0)
        {
            float workTime = 0;
            Vector3 originPos = transform.position;
            Vector3 desPos = path[0].transform.position;
            GameObject tmp = new GameObject();
            tmp.transform.position = transform.position;
            tmp.transform.LookAt(desPos);
            Quaternion originRot = transform.rotation;
            Quaternion desRot = tmp.transform.rotation;
            Destroy(tmp);
            while (true)
            {
                workTime += Time.deltaTime*4f;             // 调整协程完成每一步的时间 ，数值越大越快
                transform.position = Vector3.Lerp(originPos, desPos, workTime);
                transform.rotation = Quaternion.Lerp(originRot, desRot, workTime * 2.5f);
                ModelUpdating();                          //模型转向修正，防止走向地形差时发生X轴转向。
                if(workTime >= 1f)
                {
                    CurrentCell = path[0];
                    path.RemoveAt(0);
                    break;
                }
                yield return null;                   
            }
        }
        _selfAnim.SetBool("Run", false);
        MyTurnMap.Instance.ClearPath();
        SwitchAutoState(AutoTurnStateEnum.移动后行动);                   
    }

    IEnumerator AutoMoveCor(List<MyTurnPoint> path)               
    {  
        _selfAnim.SetBool("Run",true);       
        while (path.Count > 0)
        {
            float workTime = 0;
            float ratateTime = 0;
            Vector3 originPos = transform.position;
            Vector3 desPos = path[0].transform.position;
            transformRotationNum = GetTransformRotationNum(this.CurrentPoint,path[0],transformRotationNum);
            GameObject tmp = new GameObject();
            tmp.transform.position = transform.position;
            tmp.transform.rotation = transform.rotation;
            tmp.transform.LookAt(desPos);
            Quaternion originRot = transform.rotation;
            Quaternion towerRot = _unitTower.transform.rotation;
            Quaternion desRot = tmp.transform.rotation;
            Destroy(tmp);
            while (true)
            {
                ratateTime += Time.deltaTime*1.2f;
                _unitTower.transform.rotation = Quaternion.Lerp(towerRot, desRot, ratateTime);
                if( transformNum  == transformRotationNum || transformNum == 0)
                {
                    workTime += Time.deltaTime*4.5f;
                    transform.position = Vector3.Lerp(originPos, desPos, workTime);
                    if(workTime >= 1f)
                    {
                        transformNum = transformRotationNum;
                        CurrentPoint = path[0];
                        path.RemoveAt(0);
                        break;
                    }
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(originRot, desRot, ratateTime);
                    if( ratateTime >= 1f)
                    {
                        workTime += Time.deltaTime*4.5f;
                        transform.position = Vector3.Lerp(originPos, desPos, workTime);
                        if(workTime >= 1f)
                        {
                            transformNum = transformRotationNum;
                            CurrentPoint = path[0];
                            path.RemoveAt(0);
                            break;
                        }
                    }
                }
                yield return null;                   
            }
        }
        _selfAnim.SetBool("Run", false);
        MyTurnMap.Instance.ClearPointPath();
        SwitchAutoState(AutoTurnStateEnum.移动后行动);   
    }


    public int GetTransformRotationNum(MyTurnPoint from ,MyTurnPoint to ,int Num)
    {
        var o = from._pointNum - to._pointNum;
        if(o == mapCol)
        {
            o = 1;
        }
        else if (o== 1)
        {
            o = 2;
        }
        else if( o == mapCol*-1)
        {
            o = 3;
        }
        else if(o== -1)
        {
            o = 4;
        }       
        Num = o ;
        return Num;
    }

    IEnumerator UnitTowerRotate(UnitTower _tower,MyTurnUnit unit)
    {
        float ratateTime = 0;
        Vector3 desPos =unit.transform.position;
        GameObject tmp = new GameObject();
        tmp.transform.position = transform.position;
        tmp.transform.LookAt(desPos);
        Quaternion originRot = _tower.transform.rotation;
        Quaternion desRot = tmp.transform.rotation;
        Destroy(tmp);
        while(originRot != desRot)
        {
            ratateTime += Time.deltaTime;
            _tower.transform.rotation = Quaternion.Lerp(originRot, desRot, ratateTime);
            if(ratateTime >=1f)       
            {
                fire = true;
                break;
            } 
            yield return null;
        }
    }
    //炮塔旋转:子物件角度旋转受该单位Scale影响，如果Scale三个数字不一样，那该单位子物件旋转会有偏差

   

    internal void Attack(MyTurnUnit target)
    {  
                 
        if(this.侦查状态 == InvestigateState.隐匿)
        {
           this.侦查状态 = InvestigateState.正常;
        }
        if(_unitSize ==  UnitSize.Giant)
        {
            if(fire == false)
            {
                StartCoroutine(UnitTowerRotate(_unitTower,targetUnit));
            }
            else
            {
                switch(攻击方式)
                {
                    case AttackType.近战:      
                    CreateBullet(this,target,this.CurrentCell); 
                    break;
                    case AttackType.弹道:
                    CreateBullet(this,target,this.CurrentCell);          
                    break;
                    case AttackType.炮击:
                    break;
                    case AttackType.自爆:
                    CreateBullet(this,target,this.CurrentCell);
                    target.DisSelect();
                    GetAttackDamge(this);
                    var d = attackdamge*-1.2f;
                    this.GetDamage(this.CurrentCell,d);
                    _selfexplosion = true;
                    break;
                    case AttackType.光束:
                    CreateBullet(this,target,this.CurrentCell);
                    break;
                }
               
            }

        }
        else
        {
            fire = true;
            
            transform.LookAt(target.transform);
            if(fire == true)
            {
                switch(攻击方式)
                {
                    case AttackType.近战:      
                    CreateBullet(this,target,this.CurrentCell); 
                    break;
                    case AttackType.弹道:
                    CreateBullet(this,target,this.CurrentCell);          
                    break;
                    case AttackType.炮击:
                    break;
                    case AttackType.自爆:
                    CreateBullet(this,target,this.CurrentCell);
                    target.DisSelect();
                    GetAttackDamge(this);
                    var d = attackdamge*-1.2f;
                    this.GetDamage(this.CurrentCell,d);
                    _selfexplosion = true;
                    break;
                    case AttackType.光束:
                    CreateBullet(this,target,this.CurrentCell);
                    break;
                }
            } 
        }
            
              
    }

    //创造子弹
    public void CreateBullet(MyTurnUnit attacker,MyTurnUnit target,MyTurnCell powercell)
    {
        attacker._firePoistion = attacker._firePoint.transform.position;
        attacker._selfAnim.SetTrigger("Attack");
        Bullet clone= Instantiate(attacker._currentbullet); 
        clone.moveSpeed = attacker._bulletSpeed;          
        clone.transform.position= attacker._firePoistion;
        clone.transform.LookAt(target.transform);
        clone._attackUnit = attacker;
        clone._targetUnit = target;
        clone._powerCell = powercell;
        clone.target=target.transform;
        clone.IsMoving=true; 
        attacker.GetSPChange(attacker,target);
        attacker._selfAnim.SetTrigger("Stop");
    
    }

    //攻击伤害公式
    public float GetAttackDamge(MyTurnUnit unit)
    {
        attackdamge = this.最大攻击力 - unit.最大防御力*(1- this.最大穿透力/100f);
        return attackdamge;
    }

    public void AttacktoAllTarget(List<MyTurnUnit> path,MyTurnCell target)  
    {
        transform.LookAt(target.transform);
        _firePoistion = _firePoint.transform.position;
        _selfAnim.SetTrigger("Attack");
        Bullet clone= Instantiate(_currentbullet);           
        clone.transform.position= _firePoistion;
        clone.transform.LookAt(target.transform);
        clone._attackUnit = this; 
        clone.target=target.transform;
        clone._powerCell = target;
        clone._targetUnitList = this._targetUnitList;
        clone.IsMoving=true; 
        _selfAnim.SetTrigger("Stop"); 
        if(this.爆发类型 == ExplosionType.正常)
        {
            this.SP = this.SP + 15;
            if(this.SP > this.MAXSP)
            {
                this.SP = this.MAXSP;
            }
        }
        MyTurnMouse.Instance.SwitchState(TurnStateEnum.角色动作结束);
    }
 
 
    public List<MyTurnUnit> GetTargetUnitSlist(List<MyTurnUnit> teamUnits, List<MyTurnCell> targetCells)
    {   
        _targetUnitList = teamUnits.Where(unit => targetCells.Contains(unit.CurrentCell)).ToList();
        return _targetUnitList;
    }

    

    internal void SkillTo(MyTurnUnit target)
    {

        if(_currentskill._skillcampsort==SkillCampSort.敌方群体减益类 || _currentskill._skillcampsort==SkillCampSort.己方群体增益类)
        {
            if(_currentskill._skillcampsort==SkillCampSort.己方群体增益类)
            {
                damage=_currentskill.技能伤害;         
                if(this != target )
                {
                    if(target.生命状态 == HealthState.正常)
                    {
                        target._healthBar.talktimer03=12f;
                    }
                    else if(target.生命状态 == HealthState.危急)
                    {
                        target._healthBar.talktimer05=12f;
                    }          
                }            
                               
                target.GetDamage(this.CurrentCell,damage);
               
                _selfAnim.SetTrigger("Stop");  
            }
            else if(_currentskill._skillcampsort==SkillCampSort.敌方群体减益类)
            {
                var o=_currentskill.技能伤害+this.最大攻击力-(target.最大防御力)*(1-最大穿透力/100f);
                if( o < 0)
                {
                    o = 5;
                }
                damage = o-o*2;                                   
                           
                target.GetDamage(this.CurrentCell,damage);
               
                if(target.HP <= 0)
                {
                    var a = gim.系统信息文本.text;
                    gim.系统信息文本.text =" [回合" +gm.turncount+"]"+this.单位名字+"击倒了"+target.单位名字+"\n"; 
                    gim.系统信息文本.text += a; 
                    this.击杀敌人数 = this.击杀敌人数 + 1; 

                    if(this.CampSort == CampSortEnum.玩家)
                    {
                        this._healthBar.talktimer02=11f;
                    }   
                }
                _selfAnim.SetTrigger("Stop");  
                target._selfAnim.SetTrigger("Damge"); 
                target._selfAnim.SetTrigger("Stop");
            }
        }
        else if(_currentskill._skillcampsort==SkillCampSort.己方增益类 || _currentskill._skillcampsort==SkillCampSort.敌方减益类)
        {

            if(_currentskill._skillcampsort==SkillCampSort.己方增益类)
            {
                if(target.种族 == UnitRace.机械体)    //HP恢复类技能对机械体单位无效，但战略支援的技能有效
                {
                    if(_currentskill._skilleffect == SkillEffect.HP恢复)
                    {

                        //显示治疗无效文本代码"治疗无效"
                        //target._healthBar.talktimer03=12f;

                    }
                    else
                    {
                        _selfAnim.SetTrigger("Attack");
                        Skill clone=Instantiate(_currentskill);
                        clone.transform.position= _firePoistion;
                        clone.transform.LookAt(target.transform);
                        clone.target=target.transform;
                        clone.IsMoving=true;   
                        damage=_currentskill.技能伤害;          
                        if(this != target )
                        {
                            if(target.生命状态 == HealthState.正常)
                            {
                                target._healthBar.talktimer03=12f;
                            }
                            else if(target.生命状态 == HealthState.危急)
                            {
                                target._healthBar.talktimer05=12f;
                            }
                            
                        }                      
                                     
                        target.GetDamage(this.CurrentCell,damage);
                       
                        _selfAnim.SetTrigger("Stop");
                    }

                }
                else
                {
                    _selfAnim.SetTrigger("Attack");
                    Skill clone=Instantiate(_currentskill);
                    clone.transform.position= _firePoistion;
                    clone.transform.LookAt(target.transform);
                    clone.target=target.transform;
                    clone.IsMoving=true;   
                    damage=_currentskill.技能伤害;          
                    if(this != target )
                    {
                        if(target.生命状态 == HealthState.正常)
                        {
                            target._healthBar.talktimer03=12f;
                        }
                        else if(target.生命状态 == HealthState.危急)
                        {
                            target._healthBar.talktimer05=12f;
                        }        
                    }                      
                    target._healthBar._damage=damage;                  
                    target.GetDamage(this.CurrentCell,damage);
                    target._damageNum._damage=damage; 
                    _selfAnim.SetTrigger("Stop"); 
                }               
            }
            else if(_currentskill._skillcampsort==SkillCampSort.敌方减益类)
            {
                transform.LookAt(target.transform);
                _selfAnim.SetTrigger("Attack");
                Skill clone=Instantiate(_currentskill);
                clone.transform.position= _firePoistion;
                clone.transform.LookAt(target.transform);
                clone.target=target.transform;
                clone.IsMoving=true;   
                var o=_currentskill.技能伤害+最大攻击力-(target.最大防御力)*(1-最大穿透力/100f);
                if( o < 0)
                {
                    o = 5;
                }
                damage = o-o*2;                
                        
                target.GetDamage(CurrentCell,damage);
               
                if(target.HP <= 0)
                {
                    var a = gim.系统信息文本.text;
                    gim.系统信息文本.text =" [回合" +gm.turncount+"]"+this.单位名字+"击倒了"+target.单位名字+"\n"; 
                    gim.系统信息文本.text += a; 
                    击杀敌人数 = 击杀敌人数 + 1; 

                    if(CampSort == CampSortEnum.玩家)
                    {
                        _healthBar.talktimer02=11f;
                    }   
                }
                _selfAnim.SetTrigger("Stop");  
                target._selfAnim.SetTrigger("Damge"); 
                target._selfAnim.SetTrigger("Stop");
            }
        }
        else if(_currentskill._skillcampsort==SkillCampSort.系统收益类)
        {         
            _selfAnim.SetTrigger("Attack");
            Skill clone=Instantiate(_currentskill);
            clone.transform.position= _firePoistion;
            clone.transform.LookAt(target.transform);
            clone.target=target.transform;
            clone.IsMoving=true;   
            _selfAnim.SetTrigger("Stop");  
           
        }
        else if(_currentskill._skillcampsort==SkillCampSort.群体系统收益类)
        {
            if(_currentskill.编号== SkillNum.七号技能 ||_currentskill.编号== SkillNum.五十号技能)
            {

                target.侦查状态 = InvestigateState.被点亮;
                target.CurrentCell._mineOpen =true; 
                _selfAnim.SetTrigger("Stop"); 
            }                                  
        }        
    }


    public void SkilltoAllTarget(List<MyTurnUnit> path,MyTurnCell target)  
    {
        Skill clone= Instantiate(_currentskill);
        transform.LookAt(target.transform);            
        clone.transform.position= _firePoistion;
        clone.transform.LookAt(target.transform);
        clone.target=target.transform;
        clone.IsMoving=true;
        if(clone != null)
        {
            StartCoroutine(SkillAllTarget(path)); 
            if(gm._currentGameState == 游戏流程阶段.释放系统支援技能 || gm._currentGameState == 游戏流程阶段.展示系统技能支援范围 )
            {
                GameManage.Instance.ClearAttackMack(gm._allUnitTeam);
                //MyTurnMap.Instance.ClearAll();
                MyTurnMap.Instance.ClearPath();
                GameManage.Instance.SwitchGameState(游戏流程阶段.角色行动);
            } 
            else
            {

               MyTurnMouse.Instance.SwitchState(TurnStateEnum.角色动作结束);
            }                    
        }              
    }
  
    IEnumerator SkillAllTarget(List<MyTurnUnit> path) 
    { 
        this._selfAnim.SetTrigger("Attack"); 
        while (path.Count > 0)
        { 
            yield return new WaitForSeconds(0.001f);
            SkillTo(path[0]);
            path[0].DisSelect();
            path.RemoveAt(0);
        }
        this._selfAnim.SetTrigger("Stop"); 
    }


    //设置伤害
    public void GetDamage(MyTurnCell attacker,float damage)
    {   
        this._healthBar._damage= damage;

        if(damage < 0 && CampSort != CampSortEnum.杂物 && _unitSize != UnitSize.Giant)
        {
            transform.LookAt(attacker.transform);           //转向面对伤害来源
            //_selfAnim.SetTrigger("Damge");
        }
        UIManager.Instance.CreateDamageNum(this);

        HP += damage;
        if(HP > MAXHP)
        {
            HP = MAXHP;
        } 
        _healthBar.ShowHealth();

        if(CampSort == CampSortEnum.玩家 || CampSort == CampSortEnum.敌人)
        {
            if(生命状态 == HealthState.正常)
            {
                if( HP < MAXHP*0.3f && HP > 0)
                {
                    生命状态 = HealthState.危急;
                    //角色模型设置为大破状态
                    _healthBar.talktimer04=11f;
                }
            } 
            if( 生命状态 == HealthState.危急)
            {
                if(HP > MAXHP*0.3f)
                {
                    生命状态 = HealthState.正常;
                    _healthBar.talktimer05=11f;
                }               
            }
        } 
        
        this._damageNum._damage= damage;

        if(HP<=0)
        {  
            CurrentCell.IsUnit = false;
            CurrentCell.IsObstacle = false;
            CurrentCell.IsSurround = false;
            if(CampSort == CampSortEnum.杂物)
            {
                foreach(var o in CurrentCell._pointList)
                {
                    o.IsUnit = false;
                    o.IsObstacle  = false;
                    o.IsObstaclePlus = false;
                }
            }
            else
            {
                foreach(var o in CurrentCell._pointList)
                {
                    o.IsUnit = false;
                    o.IsObstacle  = false;
                }

            }

            if(_unitSize == UnitSize.Giant)
            {
                CurrentPoint.IsUnit = false;
                CurrentPoint.IsObstacle = false;
                foreach(var o in CurrentPoint._currentCellList)
                {
                    o.IsUnit = false;
                    o.IsObstacle  = false;
                }
            }
            


            if(this.侦查状态 != InvestigateState.被点亮)
            {
                gmmap.ClearCellPEvalue(CurrentCell,this);
            }
               
            if(CampSort == CampSortEnum.玩家 )
            {
                if(gm._decentralizationMode == true)
                {
                    CurrentCell.战略支援 = false;
                    gmmap._supportCellList.Remove(this.CurrentCell);
                }
                else
                {
                    if(_teamleader == TeamLeader.队长)
                    {
                        gm.队长连接值 = 0;
                        gm.补给许可 = false;
                        gmmap.SupportRangeClear();
                    }
                }
            } 
            _healthBar.HealthDestroy();
            _damageNum.HideDamageNum();
            gm.GetTeam.Remove(this);
            gm.targetTeam.Remove(this);                      // * 通过血量归零判定，移除 当前队伍/目标/敌人+中立/目标敌人 列表中的Unit (包括对攻击对象造成伤害，自爆对己方队员造成伤害时)
            gm.targetTeam01.Remove(this);
            gm.targetEnemyteam.Remove(this);
            gm._allUnitTeam.Remove(this);

                                                    
            if(_falltoDeath == true || _uptoDeath == true)                          
            {
                if(_falltoDeath == true )
                {
                    StartCoroutine(UnitDown(this,10f,8.8f));

                }
                else if( _uptoDeath == true )
                {
                    StartCoroutine(UnitDown(this,-1f,1f));
                }
            }
     
            
        
            if(CampSort == CampSortEnum.玩家 && gm._gamevictoryconditions== GameVictoryConditions.Escape)
            {
                GameManage.Instance.SwitchGameState(游戏流程阶段.游戏结算);           
            }
            
            Destroy(gameObject,模型消失时间);
        
        }            
    }

    public IEnumerator UnitDown(MyTurnUnit _unit,float num,float moveSpeed)     //起点，终点，移动者，移动幅度，移动速度
    {
        Vector3 startPosition = _unit.transform.position;
        Vector3 endPosition = _unit.transform.position + Vector3.down*num ;
        float distance = Vector3.Distance(startPosition, endPosition);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / distance;
            _unit.transform.position = Vector3.Lerp(startPosition, endPosition, t);               
            yield return null;
        } 

       
    }

    IEnumerator UnitUp(MyTurnUnit _unit,float num,float moveSpeed)     //起点，终点，移动者，移动幅度，移动速度
    {
        Vector3 startPosition = _unit.transform.position;
        Vector3 endPosition = _unit.transform.position + Vector3.up*num ;
        float distance = Vector3.Distance(startPosition, endPosition);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / distance;
            _unit.transform.position = Vector3.Lerp(startPosition, endPosition, t);               
            yield return null;
        } 
    }

    public IEnumerator UnitToMove(MyTurnUnit _unit,MyTurnCell _targetCell,float moveSpeed) 
    {
        Vector3 startPosition = _unit.transform.position;
        Vector3 endPosition = _targetCell.transform.position;
        float distance = Vector3.Distance(startPosition, endPosition);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / distance;
            _unit.transform.position = Vector3.Lerp(startPosition, endPosition, t);               
            yield return null;
        } 

    }



    //设置单位脱离
    public void GetUnitEscape(MyTurnCell cell)
    {

        if(_teamleader == TeamLeader.队长)
        {
            linklevel = 0 ;
            gm.队长连接值 = 0;
            gm.补给许可 = false;
            gmmap.SupportRangeClear();
        }
           
            UIManager.Instance.HideBattleWnd();
            mtm.SwitchState(TurnStateEnum.选取角色);
            mtm.AuthorizationLicense(); 
            CurrentCell.IsUnit = false;
            CurrentCell.IsObstacle = false;
            CurrentCell.IsSurround = false;
            gm.GetTeam.Remove(this);
            gm.targetTeam.Remove(this);                      // * 通过血量归零判定，移除 当前队伍/目标/敌人+中立/目标敌人 列表中的Unit (包括对攻击对象造成伤害，自爆对己方队员造成伤害时)
            gm.targetTeam01.Remove(this);
            gm.targetEnemyteam.Remove(this);
            gm._allUnitTeam.Remove(this);
            gm._escapenum ++;
            //MyTurnMap.Instance.ClearAll();
            MyTurnMap.Instance.ClearPath();
            UIManager.Instance.HideBattleWnd();
            mtm._currentSelectUnit = null;
            GameManage.Instance.GetSupportCostPoint();                
            GameManage.Instance.SwitchGameState(游戏流程阶段.选取角色);

        if( gm.GetIndex == gm.GetTeam.Count )
        {             
            GameManage.Instance.SwitchGameState(游戏流程阶段.交换队伍);
            print(gm._currentGameState);  
            mtm.SwitchState(TurnStateEnum.选取角色);    
        }
        else
        {   
            mtm.SwitchState(TurnStateEnum.选取角色);
        }

        Destroy(gameObject,模型消失时间); 
        _healthBar.HealthDestroy();                     
    }

    public void GetSPChange(MyTurnUnit attacker,MyTurnUnit unit)
    {
        if(attacker.爆发类型 == ExplosionType.正常)
        {
            attacker.SP = attacker.SP + 15;
            if(attacker.SP > attacker.MAXSP)
            {
                attacker.SP = attacker.MAXSP;
            }
        }
        if(unit.爆发类型 == ExplosionType.正常)
        {
            unit.SP = unit.SP + 15;
            if(unit.SP > unit.MAXSP)
            {
                unit.SP = unit.MAXSP;
            }
        }
    }
   

    //锁定目标Unit的方法
    public MyTurnUnit TarGetUnitSelect(GameManage gm)
    {
        if(gm.GetTarGetTeam[0]!=null) 
        {
            if(this.索敌方式 == GetTargetUnitEnum.直接距离)
            {
                TarGetUnitMindistance(gm);
            }
            else if(this.索敌方式 == GetTargetUnitEnum.移动距离)
            {
                TarGetUnitMinMove(gm);    
            }
            else if(this.索敌方式 == GetTargetUnitEnum.血量最低)
            {
                TarGetUnitMinHP(gm);                
            }  
        }
        return targetUnit;        
    }




    public MyTurnCell TargetCellSelsect(GameManage gm)
    {
        var target = TarGetUnitSelect(gm);
        float distance = Vector3.Distance(this.transform.position, target.transform.position); 

        if( distance > 50 )
        {
            targetCell =MyTurnMap.Instance.GetNearest(this, target.CurrentCell);

        }
        else
        {
            if(this.格子选择 == GetTargetUnitCell.直线距离)
            {
                targetCell =MyTurnMap.Instance.GetNearest(this, target.CurrentCell);
            }
            else if(this.格子选择 == GetTargetUnitCell.移动距离)
            {
                targetCell=MyTurnMap.Instance.GetNearest(this,target.CurrentCell);
                var tmp = MyTurnMap.Instance.GetNearest01(this,target.CurrentCell); 
                if(tmp!=null)
                {
                    targetCell=tmp;
                }
            }
        }
  
        return targetCell;
    }

    public MyTurnPoint TargetPointSelsect(GameManage gm)
    {
        if(this.格子选择 == GetTargetUnitCell.直线距离)
        {
            targetPoint =MyTurnMap.Instance.GetNearestPoint(this, targetUnit.CurrentCell);
        }

        return targetPoint;

    }


    //直线距离选择法
    public MyTurnUnit TarGetUnitMindistance(GameManage gm) 
    { 
        //var unit = selectUnit; 
        var tmp = gm.GetTarGetTeam[0]; 
        if (tmp != null) 
        { 
            float distance = Vector3.Distance(this.transform.position, tmp.transform.position); 
            foreach (var target in gm.GetTarGetTeam) 
            { 
                MyTurnMap.Instance.SurroundBool(target.CurrentCell); 
                if (target.CurrentCell.IsSurround) { continue; }
                float targetDistance = Vector3.Distance(this.transform.position, target.transform.position);
                if (targetDistance < distance)
                {
                    distance = targetDistance;
                    tmp = target;
                }
            }
        }
        targetUnit = tmp;
        return targetUnit;
    }

    //移动距离选择法
    public MyTurnUnit TarGetUnitMinMove(GameManage gm)
    {
        //var unit = selectUnit;
        var tmp = gm.GetTarGetTeam[0];        
        MyTurnMap.Instance.ExpectedtoGeneratePath(this,this.CurrentCell,gm.GetTarGetTeam[0].CurrentCell);
        float distance = gmmap._extopath.Count;
        for(int i =0; i < gm.GetTarGetTeam.Count ; i++)
        {
            MyTurnMap.Instance.ExpectedtoGeneratePath(this,this.CurrentCell,gm.GetTarGetTeam[i].CurrentCell);
            float mindistance = gmmap._extopath.Count;
            if(mindistance < distance && !gm.GetTarGetTeam[i].CurrentCell.IsWater)   
            {    
                distance = mindistance;
                tmp = gm.GetTarGetTeam[i];                              
            }
            else
            {
                targetUnit = tmp;  
            }
        }      
        targetUnit = tmp;         
        return targetUnit; 
    }

    public MyTurnUnit TarGetUnitMinHP(GameManage gm)
    {
        //var unit =  selectUnit;
        var tmp = gm.GetTarGetTeam[0];
        if(gm.GetTarGetTeam[0]!=null) 
        {
            float distance=gm.GetTarGetTeam[0].HP;
            for(int i =0; i < gm.GetTarGetTeam.Count;i++)
            {
                float mindistance= gm.GetTarGetTeam[i].HP; 
                if(mindistance < distance)
                {      
                    distance = mindistance;
                    tmp = gm.GetTarGetTeam[i];              
                }
                else
                {
                    targetUnit = tmp;
                }
            }    
        }
        targetUnit = tmp;        
        return targetUnit;        
    }

    public void ChangeNext()
    {       
        damage=0;
        gm.GetIndex++;    
        if(gm.GetIndex < gm.GetTeam.Count)
        {
            print("同队切换队员"); 
            gm.Announce();     
        }
        else
        {
            GameManage.Instance.SwitchGameState(游戏流程阶段.交换队伍);
        }
    }

    public void SetHealthBar(HealthBar healthBar)
    {
        _healthBar = healthBar;
    }
    public void SetDamageNum(DamageNum damageNum)
    {
        _damageNum = damageNum;
    }
    public void SetMask(MaskRotation mask)
    {
        _mask= mask;
    }
    public void SetShadowMask(ShadowVFX shadow)
    {
        _shadowvfx = shadow;
    }
    public void SetExplosionVFX(ExplosionVFX explosion)
    {
        _explosionvfx = explosion;
    }   
    public void SetPlayerCard(PlayerCard playerCard)
    {
        _playerCard = playerCard;
    }   


    public void SwitchAutoState(AutoTurnStateEnum targetState)
    {
        _currentAutoTurnState = targetState;
        print(targetState);
      
        switch (_currentAutoTurnState)
        {
            case AutoTurnStateEnum.选取角色:
            break;

            case AutoTurnStateEnum.角色行动:
            break;  

            case AutoTurnStateEnum.展示攻击范围:
            break;

            case AutoTurnStateEnum.展示移动范围:
            break;

            case AutoTurnStateEnum.进行移动:
            if(_unitSize == UnitSize.Normal)
            {
                消耗行动力 = gmmap.Path.Count;
                this.AutoMoveToCell(MyTurnMap.Instance.Path);

            }
            else if(_unitSize == UnitSize.Giant)
            {
                消耗行动力 = gmmap.PointPath.Count;
                this.AutoMoveToPoint(MyTurnMap.Instance.PointPath);

            }
            
            break;

            case AutoTurnStateEnum.移动后行动:
            break; 

            case AutoTurnStateEnum.角色动作结束:           
            break;
        }
    }

    private void ChangeCommitmentState(CommitmentState Commitment)
    {
        行动力状态 = Commitment;
        switch(行动力状态)
        {
            case CommitmentState.绿色:
            moverange = 基础移动范围;
            break;
            case CommitmentState.黄色:
            moverange = 基础移动范围-1;
            break;
            case CommitmentState.红色:
            moverange = 1;
            break;
        }
    }

    public void GetSwitchCommitment()
    {
        if( this.CP >= 60 )
        {
            this.行动力状态 = CommitmentState.绿色;
        }
        else if( this.CP >= 30 && this.CP < 60 )
        {
            this.行动力状态 = CommitmentState.黄色;
        }
        else if( this.CP < 30 )
        {
            this.行动力状态 = CommitmentState.红色;
        }
    }

    //切换谈话语音状态
    private void ChangeTalkVoiceState(TalkVoiceState 谈话语音状态)
    {
         if(谈话语音状态!= TalkVoiceState.无 && 谈话语音时间 > 0)
        {
            谈话语音时间 -= Time.deltaTime;

        }
        else
        {
            谈话语音状态 = TalkVoiceState.无;
        }
    }

    private void ModelUpdating()      //模型转向修正，防止走向地形差时转向卡死。
    {
        Quaternion originRot = transform.rotation;
        originRot.x=0;
        originRot.z=0;
        transform.rotation=originRot;
    }


    //隐匿状态不显示血条
    private void SwithHealthBarState(InvestigateState targetINVState)
    {     
        侦查状态 = targetINVState;
      
        switch (侦查状态)
        {
            case InvestigateState.正常:
            this._healthBar.HealthDisplay();
            if(this._shodowUnit == true)
            {
                UnitSetMaterialRenderinglode(_selfUnitMat, UnitRenderingMode.Opaque);
                _selfUnitMat.color = _originalcolor;
                _mask.gameObject.SetActive(true);
                _selfOutLine.outlineWidth = 4;
            }
            if(this.职业 == UnitCareer.召唤物与陷阱)
            {
                GetOutLine();
            }
            break;
            case InvestigateState.隐匿:
            if(this._shodowUnit == true)
            {
                UnitSetMaterialRenderinglode(_selfUnitMat, UnitRenderingMode.Fade);
                _selfUnitMat.color = _newcolor;
                _mask.gameObject.SetActive(false);
                _selfOutLine.outlineWidth = 0;
            }
            this._healthBar.HealthHide();   
            break;
            case InvestigateState.伪装:
            this._healthBar.HealthHide();   
            break;           
            case InvestigateState.被点亮:
            GetOutLine();
            this.CurrentCell.Warn();
            this._healthBar.HealthDisplay();
            break;
            case InvestigateState.心灵标记:                
            break;
            case InvestigateState.准备登场:           
            this._healthBar.HealthHide();   
            break;
        }    
    }
    
    public void SwithTalkVoiceState(TalkVoiceState talkvoice)
    {
        谈话语音状态 = talkvoice;
        print(talkvoice);
        switch(谈话语音状态)
        {
            case TalkVoiceState.无:
            break;
            case TalkVoiceState.待机:
            break;
            case TalkVoiceState.攻击:
            break;
        }
    }

    public void ShowSkillList()
    {
        _healthBar._unitskilllist.ShowUnitSkillList();
    }

    public void HideSkillList()
    {
        _healthBar._unitskilllist.HideUnitSkillList();
    }

    public void ShowBuffList()
    {
        _healthBar._unitbufficonmanager.ShowUnitBuffList(_healthBar);
    }

    public void ShowAttackMack(MyTurnMouse gtm)
    {
        if( gtm._currentState == TurnStateEnum.展示角色攻击范围 )        
        {
            if( gtm._currentSelectUnit.攻击方式 == AttackType.炮击 ) 
            {
                if(this.CampSort !=gtm._currentSelectUnit.CampSort && this.侦查状态 != InvestigateState.伪装 )
                {
                    if(this.CurrentCell.CellState == CellStateEnum.可使用 && this.CampSort!= CampSortEnum.建筑)
                    {
                        this.SelectAttack();   
                    }
                    else
                    {
                        this.DisSelect();
                    }                                                      
                } 
            }                            
        }
        else if( gtm._currentState == TurnStateEnum.展示角色技能范围 )
        {
            if(  gtm._currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方群体减益类 )
            {
                if( this.CampSort !=gtm._currentSelectUnit.CampSort && this.侦查状态 != InvestigateState.伪装 )
                { 
                    if(this.CurrentCell.CellState == CellStateEnum.可使用)
                    {
                        this.SelectAttack();   
                    }
                    else
                    {
                        this.DisSelect();
                    }                                    
                }                
            }
            else if(  gtm._currentSelectUnit._currentskill._skillcampsort == SkillCampSort.己方群体增益类 )       
            {
                if( this.CampSort == gtm._currentSelectUnit.CampSort )
                { 
                    if(this.CurrentCell.CellState == CellStateEnum.可使用)
                    {
                        this.SelectAttack();   
                    }
                    else
                    {
                        this.DisSelect();
                    }               
                }
            }
            else if(  gtm._currentSelectUnit._currentskill._skillcampsort == SkillCampSort.群体系统收益类 )
            {
                if( this.CampSort !=gtm._currentSelectUnit.CampSort && this.侦查状态 != InvestigateState.伪装 )              // && this.侦查状态 != InvestigateState.伪装
                { 
                    if(this.CurrentCell.CellState == CellStateEnum.可使用  )
                    {                       
                        this.SelectAttack(); 
                    }
                    else
                    {
                        this.DisSelect();
                    }                                    
                }                
            }
        } 
    }


    public void ShowAttackMack01(GameManage gm, MyTurnMouse gtm )
    {
        if( gm._currentGameState == 游戏流程阶段.展示系统技能支援范围 )
        {
            if(  gm.GetPlayer[0]._currentskill._skillcampsort == SkillCampSort.敌方群体减益类 )    //gm._gmsupportskill
            {
                if( this.CampSort !=  CampSortEnum.玩家 && this.侦查状态 != InvestigateState.伪装 )
                { 
                    if(this.CurrentCell.CellState == CellStateEnum.可使用)
                    {
                        this.SelectAttack();   
                    }
                    else
                    {
                        this.DisSelect();
                    }                                    
                }                
            }
            else if(  gm.GetPlayer[0]._currentskill._skillcampsort == SkillCampSort.己方群体增益类 )       
            {
                if( this.CampSort ==  CampSortEnum.玩家)
                { 
                    if(this.CurrentCell.CellState == CellStateEnum.可使用)
                    {
                        this.SelectAttack();   
                    }
                    else
                    {
                        this.DisSelect();
                    }               
                }
            }
            else if(  gm.GetPlayer[0]._currentskill._skillcampsort == SkillCampSort.群体系统收益类 )
            {
                if( this.CampSort != CampSortEnum.玩家 && this.侦查状态 != InvestigateState.伪装 )              // && this.侦查状态 != InvestigateState.伪装
                { 
                    if(this.CurrentCell.CellState == CellStateEnum.可使用  )
                    {                       
                        this.SelectAttack(); 
                    }
                    else
                    {
                        this.DisSelect();
                    }                                    
                }                
            }
        } 
    }


    //获得连接值
    public void GetLinkLevel()
    {
        if(this.CurrentCell.战略支援 == true || this._teamleader == TeamLeader.队长)
        {
            区域连接值 =1; 
        }
        else
        {
            区域连接值 = 0;
        }      
        环境连接值 = gm.战场环境连接值;        
        linklevel = 环境连接值 + 区域连接值 + 控制连接值;
        if(this._teamleader == TeamLeader.队员 && linklevel > gm.队长连接值)
        {
            linklevel = gm.队长连接值;
        }
        else if( gm.队长连接值 == 0)
        {
            linklevel = gm.队长连接值;
        }
        GetFoundationCost();
        GetCostMultiplier();
        最终消耗费用 = 基础消耗战略点*消耗倍率;
    }
    
    public void GetFoundationCost()
    {
        if(战略支援类型 == 0)
        {
            基础消耗战略点 = 0;
        }
        else
        {
            基础消耗战略点 = 1;
        }
    }

    public void GetCostMultiplier()
    {
        if(linklevel == 0)
        {       
            消耗倍率 = 0;
        }
        else if(linklevel == 3)
        {        
            消耗倍率 = 1;
        }
        else if(linklevel == 2)
        {       
            消耗倍率 = 2;
        }
        else if(linklevel == 1)
        {       
            消耗倍率 = 3;
        }
    }


    public void PreSelect()
    {
        if(HP>0)
        {
            if (_currentState == StateEnum.选中)
            {
                return;
            }
            _selfOutLine.enabled = true;
            _selfOutLine.OutlineColor = Color.white;
            _currentState = StateEnum.预选;
            unitselect=false;
            _healthBar.SetchildCount();
            _healthBar.unitinfo.ShowStateInfo(this._healthBar);                    //预选时显示单位信息，时间为2f
        }       
    }

    public void Select()
    {    
        if(HP>0)
        {
            _selfOutLine.enabled = true; 
            _selfOutLine.OutlineColor = Color.yellow;
            _currentState = StateEnum.选中;
            unitselect=true;
            _healthBar.SetchildCount();       //调整被选中单位在UI中的优先级；
            _healthBar.unitinfo.HideStateInfo(); 
        }       
    }

    public void DisSelect()
    {
        if(HP>0)
        {
            _selfOutLine.enabled = false;
            _currentState = StateEnum.闲置;
            unitselect=false;
            _healthBar.unitinfo.HideStateInfo(); 
        }    
    }

    public void SelectAttack()
    {
        if(HP>0)
        {
            _selfOutLine.enabled = true;
            _selfOutLine.OutlineColor = Color.red;
            _currentState = StateEnum.攻击目标;
            unitselect=false;   
            _healthBar.unitinfo.HideStateInfo(); 
        }     
    }

    public void MineCamouflage()
    {
        _selfOutLine.enabled = false;
        _currentState = StateEnum.地雷伪装;
        unitselect=false;
        _healthBar.unitinfo.HideStateInfo(); 
    }

    public void SurroundBool()
    {
        MyTurnMap.Instance.SurroundBool(this.CurrentCell);
    }

    //单位隐匿状态下隐形
    public static void UnitSetMaterialRenderinglode(Material material, UnitRenderingMode renderingMode)
    {
	    switch (renderingMode)
        {
		    case UnitRenderingMode.Opaque:
		    material.SetInt("_SrcBlend",(int)UnityEngine.Rendering.BlendMode.One);
		    material.SetInt("_DstBlend",(int)UnityEngine.Rendering.BlendMode.Zero);
		    material.SetInt("_Zwrite",1);
		    material.DisableKeyword("_ALPHATEST ON");
            material.DisableKeyword("_ALPHABLEND ON");
		    material.DisableKeyword("_ALPHAPREMULTIPLY ON");
            material.renderQueue = -1;
		    break;
		    case UnitRenderingMode.Cutout:
		    material.SetInt("_SrcBlend",(int)UnityEngine.Rendering.BlendMode.One);
		    material.SetInt("_DstBlend",(int)UnityEngine.Rendering.BlendMode.Zero);
		    material.SetInt("_Zwrite",1);
		    material.EnableKeyword("_ALPHATEST ON");
		    material.DisableKeyword("_ALPHABLEND ON");
		    material.DisableKeyword("_ALPHAPREMULTIPLY ON");
		    material.renderQueue = 2450;
		    break;
		    case UnitRenderingMode.Fade:
		    material.SetInt("_SrcBlend",(int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		    material.SetInt("_DstBlend",(int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		    material.SetInt("_Zwrite",0);
		    material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");        
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
		    break;
		    case UnitRenderingMode.Transparent:
		    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
		    break;		
        }
    }




    void Update()
    {

        ModelUpdating(); 
        if(HP > 0)
        {
            SwithHealthBarState(侦查状态);
        }
        
        ShowAttackMack(mtm);
        ShowAttackMack01(gm,mtm);
        ChangeCommitmentState(行动力状态);
        ChangeTalkVoiceState(谈话语音状态);
        GetLinkLevel();

        switch (_currentState)
        {
            case StateEnum.闲置:
                DisSelect();
            break;
            case StateEnum.预选:
                PreSelect();
            break;
            case StateEnum.选中:
                Select();
            break;
            case StateEnum.攻击目标:
                SelectAttack();
            break;
            case StateEnum.地雷伪装:
                MineCamouflage();
            break;
        }

        

        if(this.CampSort == CampSortEnum.玩家 && this.CurrentCell.IsUnit == true && this.CurrentCell.IsObstacle == true)
        {
            this.CurrentCell._mineOpen = true;
        }       
    } 

}
