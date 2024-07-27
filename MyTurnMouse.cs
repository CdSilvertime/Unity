using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;


public enum TurnStateEnum
{   
    放置角色,
    设置队伍,
    选取角色,
    角色行动,
    展示角色攻击范围,
    进行群体攻击,
    展示角色技能列表,
    展示角色技能范围,
    展示系统支援技能范围,
    进行技能释放,
    展示角色移动范围,
    选定角色行走路线,
    移动后选项,
    返回原处,
    选定角色攻击目标,
    角色休整状态,
    角色动作结束,
    敌方行动回合

}

public enum GameMouseTestMode
{
   
    Test,
    NoTest
    
}


public class MyTurnMouse : MonoBehaviour
{

    public GameMouseTestMode _测试模式;
    Transform _currentTarget;
    public GameManage gm;
    public MyTurnMap gmmp;
    public MyTurnCell _currentCell;     
    private BattleWnd _BattleWnd;    
    private Monitor monitor;
    MyTurnCell _backCell;
    public MyTurnUnit _currentUnit;
    public MyTurnUnit _currentSelectUnit;
    public List<MyTurnUnit> _playerList;

    public MyTurnUnit _currentSetUnit;

    public TurnStateEnum _currentState;
    public static MyTurnMouse Instance;

    

    public GameInfoManger _gameinfo;
    private float timer;
    public UnitAttackInfo _unitattackinfo;
 
    void Awake() 
    {
        Instance = this; 
       
        _BattleWnd = GameObject.Find("BattleWnd").GetComponent<BattleWnd>();
        monitor = GameObject.Find("Monitor").GetComponent<Monitor>(); 

        //通过GameData这种方式设置队伍
        if(_测试模式 == GameMouseTestMode.NoTest)
        {
            GameData.Instance.SetPlayerTeam(this); 
        }
                                               
    }
    void start()
    {
        _currentSetUnit = _playerList[0];
        
    }
    
    
    public void AuthorizationLicense()
    {
        _BattleWnd.moveLicense = true;
        _BattleWnd.attackLicense = true;        
    }



    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (_currentState)
            {
                case TurnStateEnum.放置角色:
                   
                    if (_currentCell != null)
                    { 
                        if(_currentCell.UnitCanSet==true)
                        {
                            
                            MyTurnUnit clone = Instantiate(_currentSetUnit);
                            clone.SetCell(_currentCell);
                            
                            _currentCell.IsUnit=true;
                            _currentCell.IsObstacle=true;
                            _currentCell.UnitCanSet=false;
                            UIManager.Instance.RemoveTeamForSet();
                                                
                        }                      
                    }
                    break;

                case TurnStateEnum.设置队伍:
                    {                       
                        foreach(var o in gm.GetTeam)
                        {
                            o.GetMove=true;
                            o.HP= o.HP*gm.生命倍率;                  //关卡设置的生命倍率                     
                        } 
                        
                        _BattleWnd.moveLicense = true;
                        _BattleWnd.attackLicense = true;
                        _currentSelectUnit = gm.GetTeam[0]; 
                       
                        gm.队长等级 = gm.GetTeam[0].LV ;
                        var a = _gameinfo.系统信息文本.text;
                        _gameinfo.系统信息文本.text =" [回合1]开始\n"; 
                        _gameinfo.系统信息文本.text  += a; 
                        GameManage.Instance.SwitchGameState(游戏流程阶段.开始回合);          
                        SwitchState(TurnStateEnum.选取角色);                                  
                    }
                      break;

                case TurnStateEnum.选取角色:
                    if (_currentUnit !=null && _currentUnit.GetMove == true && gm.GetTeam == gm.GetPlayer && !gm.AutoFight)    //  && gm.GetTeam == gm.GetPlayer
                    {   
                        if(_currentSelectUnit != null)
                        {
                            _currentUnit.DisSelect();
                        }
                        _backCell=null;                                                                                              
                        _currentSelectUnit = _currentUnit;
                        _currentUnit.CurrentCell._openChest=true;                         
                        _currentSelectUnit.CurrentCell.IsObstacle=false;
                        foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                        {
                            o.IsObstacle=false;
                        }

                        _currentSelectUnit._targetUnitList.Clear();
                        _currentSelectUnit.Select();
                        _currentSelectUnit.消耗行动力 = 0;
                        _currentSelectUnit.当前恢复行动力 = 0;
                        UIManager.Instance.ShowBattleWnd(_currentSelectUnit);
                        _currentSelectUnit.DamageRange = _currentSelectUnit.固定伤害范围;
                        SwitchState(TurnStateEnum.角色行动); 
                    }                     
                    break;
               
                case TurnStateEnum.展示角色移动范围: 
                    { 
                        if(_currentUnit != null && _currentCell == _currentUnit.CurrentCell )
                        {
                            if ( _currentUnit != _currentSelectUnit && _currentUnit.GetMove == true)
                            { 
                                _backCell=null;
                                _currentUnit.DisSelect(); 
                                _currentSelectUnit.DisSelect(); 
                                _currentUnit= _currentTarget.GetComponent<MyTurnUnit>(); 
                                _currentUnit.Select();                
                                _currentSelectUnit = _currentUnit;
                                //MyTurnMap.Instance.ClearAll();
                                MyTurnMap.Instance.ClearPath();
                                UIManager.Instance.ShowBattleWnd(_currentSelectUnit);
                                SwitchState(TurnStateEnum.选取角色);   
                            }                                                
                        }                                       
                        else if (_currentCell != null  && !_currentCell.NormalCell )     //只有在_currentCell不为空，并且NormalCell属性不为ture时才可以选定角色行走路线
                        {           
                            _backCell=_currentSelectUnit.CurrentCell; 
                            MyTurnMap.Instance.GeneratePath(_currentSelectUnit , _currentCell , gmmp._pointList[0]);
                            UIManager.Instance.HideBattleWnd();
                            _currentSelectUnit.CurrentCell.IsUnit=false;
                            foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                            {
                                o.IsUnit=false;
                            }


                            _currentSelectUnit.CurrentCell._openChest=false;    
                            SwitchState(TurnStateEnum.选定角色行走路线);  
                        }
                    }
                    break;

                case TurnStateEnum.展示角色攻击范围:
                    {  
                        if(_currentSelectUnit.攻击方式 == AttackType.炮击)
                        {
                            if(!_currentCell.NormalCell)
                            {

                                var tmp= MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit.固定伤害范围,_currentCell);
                                _currentSelectUnit.GetTargetUnitSlist(gm.targetTeam01,tmp); 
                                _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1;
                                if(_currentSelectUnit._targetUnitList.Count > 0)
                                {                               
                                    SwitchState(TurnStateEnum.进行群体攻击); 
                                }
                            }                       
                        }
                        else
                        {
                            if( !_currentCell.NormalCell)   //格子必须是非普通，即_UnitAttckCellList内的格子
                            {
                                if(gmmp._UnitAttckCellList.Contains(_currentUnit.CurrentCell))   //角色的攻击范围格子必须包含目标当前位置格子
                                {
                                    if( _currentUnit.CampSort != _currentSelectUnit.CampSort && _currentUnit.CampSort != CampSortEnum.建筑 )    //攻击角色的阵营与目标角色阵营不一样
                                    {
                                        _currentSelectUnit.谈话语音状态 = TalkVoiceState.攻击;
                                        _currentSelectUnit.谈话语音时间 = 2.0f;
                                        _currentSelectUnit.Attack(_currentUnit);
                                        _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1;                      
                                        if(_currentUnit.HP <= 0)
                                        {
                                            _currentUnit.CurrentCell.IsUnit=false;
                                            foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                                            {
                                                o.IsUnit=false;
                                            }
                                            _currentUnit.CurrentCell._openChest=false;
                                            _currentSelectUnit.GetMove = false;
                                            SwitchState(TurnStateEnum.角色动作结束); 
                                        }  
                                        else
                                        {
                                            if(_currentSelectUnit._currentMoveType == MoveTypeEnum.空中飞行)
                                            {
                                                
                                                UnitEnding(_currentSelectUnit);
                                            } 
                                            else
                                            {
                                                _currentSelectUnit.CurrentCell.IsObstacle=true;
                                                foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                                                {
                                                    o.IsObstacle=true;
                                                }
                                                
                                                UnitEnding(_currentSelectUnit);
                                            }                                          
                                        }                                 
                                    }
                                }
                            }
                            else
                            {
                                SwitchState(TurnStateEnum.展示角色攻击范围);
                            }                          
                        }
                    }
                    break;

                case TurnStateEnum.展示角色技能列表:
                    
                    break;

                case TurnStateEnum.展示角色技能范围:
                    if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方减益类)
                    {
                        if( !_currentCell.NormalCell)   //格子必须是非普通，即_UnitAttckCellList内的格子
                        {
                            if(gmmp._UnitAttckCellList.Contains(_currentUnit.CurrentCell))   //角色的攻击范围格子必须包含目标当前位置格子
                            {
                                if( _currentUnit.CampSort != _currentSelectUnit.CampSort )    //攻击角色的阵营与目标角色阵营不一样
                                {
                                    _currentSelectUnit.SkillTo(_currentUnit);
                                    _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1;  
                                    if(_currentUnit.HP <= 0)
                                    {
                                        _currentUnit.CurrentCell.IsUnit=false;
                                        foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                                        {
                                            o.IsUnit=false;
                                        }
                                        _currentUnit.CurrentCell._openChest=false;
                                        _currentSelectUnit.GetMove = false;
                                        SwitchState(TurnStateEnum.角色动作结束); 
                                    }  
                                    else
                                    {
                                        if(_currentSelectUnit._currentMoveType == MoveTypeEnum.空中飞行)
                                        {
                                           
                                            UnitEnding(_currentSelectUnit);
                                        } 
                                        else
                                        {
                                            _currentSelectUnit.CurrentCell.IsObstacle=true;
                                          
                                           UnitEnding(_currentSelectUnit);
                                        } 
                                    }                                 
                                }
                            }
                        }
                        else
                        {
                            SwitchState(TurnStateEnum.展示角色技能范围);
                        }                          

                    }
                    else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方群体减益类)
                    {
                        if(!_currentCell.NormalCell)
                        {
                            var tmp= MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);
                            _currentSelectUnit.GetTargetUnitSlist(gm.targetTeam01,tmp);
                            _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1;  
                            if(_currentSelectUnit._targetUnitList.Count > 0)
                            {                               
                                SwitchState(TurnStateEnum.进行技能释放); 
                            }
                        }
                    }
                    else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.己方增益类)
                    {
                        if( !_currentCell.NormalCell)                  //格子必须是非普通，即_UnitAttckCellList内的格子
                        {
                            if(gmmp._UnitAttckCellList.Contains(_currentUnit.CurrentCell))   //角色的攻击范围格子必须包含目标当前位置格子
                            {
                                if( _currentUnit.CampSort == _currentSelectUnit.CampSort && _currentCell == _currentUnit.CurrentCell)    //攻击角色的阵营与目标角色阵营不一样
                                {
                                    _currentSelectUnit.SkillTo(_currentUnit); 
                                    _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1;                                                      
                                    if(_currentSelectUnit._currentMoveType == MoveTypeEnum.空中飞行)
                                    {
                                     
                                        UnitEnding(_currentSelectUnit);

                                    } 
                                    else
                                    {
                                        _currentSelectUnit.CurrentCell.IsObstacle=true;
                                        foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                                        {
                                            o.IsObstacle = true;

                                        }
                                     
                                        UnitEnding(_currentSelectUnit);
                                    }                                                                         
                                }
                            }
                            else
                            {
                                SwitchState(TurnStateEnum.展示角色技能范围);
                            }
                        }

                    }                                              
                    else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.己方群体增益类)
                    {
                        if(!_currentCell.NormalCell)
                        {

                            var tmp= MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);
                            _currentSelectUnit.GetTargetUnitSlist(gm.GetTeam,tmp);
                            _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1; 
                            if(_currentSelectUnit._targetUnitList.Count > 0)
                            {
                                SwitchState(TurnStateEnum.进行技能释放); 
                            }                                                

                        }
                    }
                    else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.系统收益类)
                    {

                    }
                    else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
                    {
                        if(!_currentCell.NormalCell)
                        {
                            var tmp= MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);
                            _currentSelectUnit.GetTargetUnitSlist(gm._allUnitTeam,tmp);                                //*重点分类
                            _currentSelectUnit.消耗行动力 = _currentSelectUnit.消耗行动力 +1;                               
                            SwitchState(TurnStateEnum.进行技能释放); 
                        }
                    }
                    break;
                case TurnStateEnum.选定角色行走路线:
                   
                    break;
                case TurnStateEnum.移动后选项:
                    break;
                case TurnStateEnum.选定角色攻击目标:
                    break;
                case TurnStateEnum.角色动作结束:

                    break;
            }
        }              
        if (Input.GetMouseButtonDown(1))
        {
            switch (_currentState)
            {
                case TurnStateEnum.放置角色:
                    break;
                case TurnStateEnum.选取角色:
                    UIManager.Instance.HideBattleWnd();
                   
                    break;
                case TurnStateEnum.角色行动:
                    _currentSelectUnit.CurrentCell.IsUnit=true;
                    foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                    {
                        o.IsUnit = true;

                    }
                    
                    if(_currentSelectUnit._currentMoveType != MoveTypeEnum.空中飞行)
                    {
                        _currentSelectUnit.CurrentCell.IsObstacle=true;
                        foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                        {
                            o.IsObstacle = true;

                        }

                        
                    }
                    _currentSelectUnit.DisSelect();
                    UIManager.Instance.HideBattleWnd();        
                    SwitchState(TurnStateEnum.选取角色);

                    break;    
                case TurnStateEnum.展示角色攻击范围:
                    if(_BattleWnd.moveLicense == true)
                    {                      
                        _currentSelectUnit.Select();                       
                        GameManage.Instance.ClearAttackMack(gm._allUnitTeam);
                        SwitchState(TurnStateEnum.角色行动);
                    }
                    else
                    {
                        _currentSelectUnit.Select(); 
                        GameManage.Instance.ClearAttackMack(gm._allUnitTeam);
                        SwitchState(TurnStateEnum.移动后选项);
                    }
                    break;
                case TurnStateEnum.展示角色技能列表:
                    if(_BattleWnd.moveLicense == true)
                    {
                        //MyTurnMap.Instance.ClearAll();
                        MyTurnMap.Instance.ClearPath();
                        _currentSelectUnit.HideSkillList();                        
                        UIManager.Instance.ShowBattleWnd(_currentSelectUnit);
                        SwitchState(TurnStateEnum.角色行动);
                    }
                    else
                    {
                        //MyTurnMap.Instance.ClearAll();
                        MyTurnMap.Instance.ClearPath();
                        _currentSelectUnit.HideSkillList();                        
                        SwitchState(TurnStateEnum.移动后选项);
                    }
                    break;

                case TurnStateEnum.展示角色技能范围:
                    if(_BattleWnd.moveLicense == true)
                    {
                        //MyTurnMap.Instance.ClearAll();
                        MyTurnMap.Instance.ClearPath();
                        _currentSelectUnit.HideSkillList();
                        _currentSelectUnit.Select();                        
                        UIManager.Instance.ShowBattleWnd(_currentSelectUnit);
                        GameManage.Instance.ClearAttackMack(gm._allUnitTeam);
                        SwitchState(TurnStateEnum.角色行动);
                    }
                    else
                    {
                        //MyTurnMap.Instance.ClearAll();
                        MyTurnMap.Instance.ClearPath();
                        _currentSelectUnit.HideSkillList();
                        _currentSelectUnit.Select();                        
                        GameManage.Instance.ClearAttackMack(gm._allUnitTeam);
                        SwitchState(TurnStateEnum.移动后选项);
                    }
                    break;

                case TurnStateEnum.展示角色移动范围:
                    //MyTurnMap.Instance.ClearAll();
                    MyTurnMap.Instance.ClearPath();        
                    UIManager.Instance.ShowBattleWnd(_currentSelectUnit);
                    SwitchState(TurnStateEnum.角色行动);
                    break;
                case TurnStateEnum.选定角色行走路线:
                    break;
                case TurnStateEnum.移动后选项: 
                    break;    
                case TurnStateEnum.选定角色攻击目标:
                    SwitchState(TurnStateEnum.展示角色攻击范围);
                    break;
                case TurnStateEnum.角色动作结束:    
                    break;
            }
        }
    }
    public void SwitchState(TurnStateEnum targetState)
    {
        _currentState = targetState;
        print(targetState);
        switch (_currentState)
        {
            case TurnStateEnum.放置角色:
                
                break;
            case TurnStateEnum.设置队伍:
                if(gm.targetEnemyteam!=null)
                {
                    UIManager.Instance.ShowEnemyCard(gm);

                }
                break; 
           
            case TurnStateEnum.选取角色:
                Monitor.Instance._cameraMode = CameraMode.跟踪模式;
                UIManager.Instance.ShowPlayerCard(gm);
                if(gm._currentGameState != 游戏流程阶段.交换队伍)
                {
                    GameManage.Instance.SwitchGameState(游戏流程阶段.选取角色); 
                }
                break;
            case TurnStateEnum.角色行动:
                
                Monitor.Instance._cameraMode = CameraMode.跟踪模式;
                //MyTurnMap.Instance.ClearAll();
                MyTurnMap.Instance.ClearPath();
                UIManager.Instance.ShowBattleWnd(_currentSelectUnit);                        //展示移动后选项
                _currentSelectUnit.Select(); 
                
                GameManage.Instance.SwitchGameState(游戏流程阶段.角色行动);
                
                break;    
            case TurnStateEnum.展示角色攻击范围:
                _currentSelectUnit.DamageRange = _currentSelectUnit.固定伤害范围;
                _currentSelectUnit.Select();
                UIManager.Instance.HideBattleWnd(); 
                UIManager.Instance.ShowPlayerCard(gm);
                break;
            case TurnStateEnum.进行群体攻击:             
                _currentSelectUnit.CurrentCell.IsUnit=true;
                _currentSelectUnit.CurrentCell.IsObstacle=false;
                _currentSelectUnit.CurrentCell.NormalCell = false;
                _currentSelectUnit.AttacktoAllTarget(_currentSelectUnit._targetUnitList,_currentCell);                 
                break;
            case TurnStateEnum.展示角色技能列表:
                UIManager.Instance.HideBattleWnd(); 
                _currentSelectUnit.ShowSkillList();               
                break; 
            case TurnStateEnum.展示角色技能范围:
                _currentSelectUnit.Select(); 

               
                
                break;
            case TurnStateEnum.展示系统支援技能范围:

                break;
            case TurnStateEnum.进行技能释放:
                _currentSelectUnit.CurrentCell.IsUnit=true;
                _currentSelectUnit.CurrentCell.IsObstacle=false;
                foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                {
                    o.IsUnit=true;
                    o.IsObstacle=false;
                }
                _currentSelectUnit.CurrentCell.NormalCell = false; 
                if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
                {                  
                   if(_currentSelectUnit._currentskill.编号== SkillNum.七号技能 || _currentSelectUnit._currentskill.编号== SkillNum.五十号技能)
                   {
                        MyTurnMap.Instance.TargetCellInvestigate(MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell));
                   }                                     
                }                                                                    
                _currentSelectUnit.SkilltoAllTarget(_currentSelectUnit._targetUnitList,_currentCell);
                break;
            case TurnStateEnum.展示角色移动范围:
                _currentSelectUnit.CurrentCell.IsUnit=true;
                _currentSelectUnit.CurrentCell.IsObstacle=false;
                foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                {
                    o.IsUnit=true;
                    o.IsObstacle=false;
                }
                _currentSelectUnit.CurrentCell.NormalCell = false;
                _currentSelectUnit.Select();
                MyTurnMap.Instance.GenerateRange(_currentSelectUnit);               
                UIManager.Instance.HideBattleWnd();
                break;                
            case TurnStateEnum.选定角色行走路线:
            
                _currentSelectUnit.CurrentCell.IsUnit=false;
                foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                {
                    o.IsUnit=false;
                    
                }
                _currentSelectUnit.CurrentCell._openChest=false; 
                _currentSelectUnit.消耗行动力 = gmmp.Path.Count; 
                 
                _currentSelectUnit.MoveToCell(MyTurnMap.Instance.Path);                    
                break;
            case TurnStateEnum.移动后选项:
                Monitor.Instance._cameraMode = CameraMode.跟踪模式;
                _BattleWnd.moveLicense = false;
                //MyTurnMap.Instance.ClearAll();
                MyTurnMap.Instance.ClearPath();
                UIManager.Instance.ShowBattleMidWnd(_currentSelectUnit); 
               
                _currentSelectUnit.Select();                 
                GameManage.Instance.SwitchGameState(游戏流程阶段.移动后选项); 
                break;
            case TurnStateEnum.返回原处:
                if(_backCell!=null)
                {
                    //_currentSelectUnit.SetCell(_backCell);

                    _currentSelectUnit.CurrentCell = _backCell;
                    _currentSelectUnit.transform.position = _backCell.transform.position;
                    _currentSelectUnit.消耗行动力 = 0;
                    _backCell._openChest=true;
                    _BattleWnd.moveLicense = true;
                       
                }             
                UIManager.Instance.HideBattleWnd();
                SwitchState(TurnStateEnum.展示角色移动范围);
                break;
            case TurnStateEnum.角色休整状态:
                if(_currentSelectUnit._currentMoveType == MoveTypeEnum.空中飞行)
                {
                    _currentSelectUnit.当前恢复行动力 =  _currentSelectUnit.最大恢复行动力;
                    UnitEnding(_currentSelectUnit);
                } 
                else
                {
                    _currentSelectUnit.当前恢复行动力 =  _currentSelectUnit.最大恢复行动力;
                    _currentSelectUnit.CurrentCell.IsObstacle=true;
                    foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                    {
                        o.IsObstacle=true;
                    }
                    UnitEnding(_currentSelectUnit);
                } 
                break;
            case TurnStateEnum.角色动作结束:
                GameManage.Instance.ClearAttackMack(gm._allUnitTeam);         
                if(_currentSelectUnit._currentMoveType == MoveTypeEnum.空中飞行)
                {
                   
                    UnitEnding(_currentSelectUnit); 
                } 
                else
                {
                    _currentSelectUnit.CurrentCell.IsObstacle=true;
                    foreach(var o in _currentSelectUnit.CurrentCell._pointList)
                    {
                        o.IsObstacle=true;
                    }
                    
                    UnitEnding(_currentSelectUnit);                                                   
                   
                }                
                break;
            case TurnStateEnum.敌方行动回合:               
                if(gm.GetTeam == gm.GetPlayer)
                {
                    SwitchState(TurnStateEnum.选取角色);
                }
                break;    
        }
    }



    private void UnitEnding(MyTurnUnit unit)
    {
        if(_BattleWnd.moveLicense == false)
        {
            gmmp.ClearCellPEvalue(_backCell,unit);
            gmmp.SetCellPEvalue(unit.CurrentCell,unit);
        }
        
        AuthorizationLicense();                                                        //对按钮效果进行重新许可
        unit.DisSelect();
        unit.CurrentCell.IsUnit=true;  
        foreach(var a in _currentSelectUnit.CurrentCell._pointList)
        {
            a.IsUnit=true;
        }
        unit.GetMove = false;
        unit.DamageRange = unit.固定伤害范围;
        if(unit.消耗行动力 >0)
        {
            var a = _gameinfo.系统信息文本.text;
            _gameinfo.系统信息文本.text =" [" + "回合" +gm.turncount  +"]"+ unit.单位名字+"消耗了"+unit.消耗行动力+"CP\n"; 
            _gameinfo.系统信息文本.text += a; 

        }
        
        var o = unit.当前恢复行动力 - unit.消耗行动力;

        if( o > 0)
        {
            var a = _gameinfo.系统信息文本.text;
            _gameinfo.系统信息文本.text =" [" + "回合" +gm.turncount  +"]"+unit.单位名字 +"回复了"+ o +"CP\n"; 
            _gameinfo.系统信息文本.text += a; 
        }

        unit.CP = unit.CP + o;
        unit.GetSwitchCommitment();
        
        if(_currentSelectUnit.CP > _currentSelectUnit.MAXCP)
        {
            _currentSelectUnit.CP = _currentSelectUnit.MAXCP;
        }          
        unit._healthBar.timer = 12f;
        unit._healthBar.消耗行动力 = o;
        if(gm._decentralizationMode == false)
        {
            if(_currentSelectUnit._teamleader == TeamLeader.队长)
            {
                MyTurnMap.Instance.SupportRangeUpdate(_currentSelectUnit);
                gm.队长连接值 = _currentSelectUnit.linklevel;
            }

        }
        else
        {
            _backCell.战略支援 = false;
            unit.CurrentCell.战略支援=true; 
            if(_backCell != null)
            {
                gmmp._supportCellList.Remove(_backCell);

            }
            
            gmmp._supportCellList.Add(unit.CurrentCell);
        }     
              
        //MyTurnMap.Instance.ClearAll();
        MyTurnMap.Instance.ClearPath();
        UIManager.Instance.HideBattleWnd();
        unit = null;
        if(_gameinfo._infoMode == InfoMode.直播)
        {   
            _gameinfo._audiencelistManager.SetAudienceMessage(_gameinfo._audiencelistManager._audienceinfoList);
            _gameinfo.互动许可 = true;
            _gameinfo._hideinteractionButton.gameObject.SetActive(false);
        }
         
        GameManage.Instance.GetSupportCostPoint();                
        GameManage.Instance.SwitchGameState(游戏流程阶段.选取角色);
        gm.GetIndex++;  
        print(gm.GetIndex);
        print(gm.GetTeam.Count);           
        if( gm.GetIndex == gm.GetTeam.Count )
        {             
            GameManage.Instance.SwitchGameState(游戏流程阶段.交换队伍);
            print(gm._currentGameState);  
            SwitchState(TurnStateEnum.选取角色);    
        }
        else
        {   
            SwitchState(TurnStateEnum.选取角色);
        }                 
    }

    
private void MouseDetect()
{

        if (!EventSystem.current.IsPointerOverGameObject())      //防止射线检测穿透UI
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mouseRay, out hitInfo))
            {
                _currentTarget = hitInfo.transform;
                MyTurnCell cell = _currentTarget.GetComponent<MyTurnCell>(); 
                MyTurnUnit unit = _currentTarget.GetComponent<MyTurnUnit>();
                if (cell != null)
                {
                    switch (_currentState)
                    {
                        case TurnStateEnum.放置角色:
                        {
                            if (this._currentCell != null && this._currentCell != cell)
                            {   
                                                          
                                this._currentCell.SwitchState(CellStateEnum.普通);                                
                            }

                            this._currentCell = cell;
                            cell.SwitchState(CellStateEnum.可使用);
                        }
                        break;
                        case TurnStateEnum.选取角色:
                        {                           
                            if ( _currentCell != null  )     //_currentCell != null && || cell != _currentUnit.CurrentCell
                            {
                                _currentCell.SwitchState(CellStateEnum.普通);  

                                /*
                                if(cell != _currentCell || _currentCell!= unit.CurrentCell)
                                {
                                    
                                    _currentCell.SwitchState(CellStateEnum.普通);   
                                } 
                                else
                                {
                                    _currentCell.SwitchState(CellStateEnum.普通);  
                                   
                                } */                           
                            }
                           
                                     
                            _currentCell = cell;
                            cell.SwitchState(CellStateEnum.可使用);
                        }
                        break;
                        case TurnStateEnum.展示角色移动范围:
                       {
                            MyTurnMap.Instance.GenerateRange(this._currentSelectUnit);
                            if(_currentCell != null)
                            {
                                _currentCell.SwitchState(CellStateEnum.普通);
                            }
                            this._currentCell = cell;
                            cell.SwitchState(CellStateEnum.可使用);
                       }         
                        break;

                        case TurnStateEnum.展示角色攻击范围:
                        {
                            
                            if(this._currentSelectUnit.职业 == UnitCareer.炮手 || this._currentSelectUnit.职业 == UnitCareer.狙击手 )
                            {
                                //移动前后攻击范围变动
                                if( _BattleWnd.moveLicense == true )
                                {
                                    MyTurnMap.Instance.GenerateAttackRange(this._currentSelectUnit,this._currentSelectUnit.attckrange);
                                    if(this._currentSelectUnit.职业 == UnitCareer.炮手)
                                    {
                                        MyTurnMap.Instance.GetTargetUnitsCell(this._currentSelectUnit,this._currentSelectUnit.固定伤害范围,this._currentCell);

                                    }
                                    
                                }
                                else
                                {
                                    MyTurnMap.Instance.GenerateAttackRange(this._currentSelectUnit,1);
                                    if(this._currentSelectUnit.职业 == UnitCareer.炮手)
                                    {
                                        MyTurnMap.Instance.GetTargetUnitsCell(this._currentSelectUnit,this._currentSelectUnit.固定伤害范围,this._currentCell);

                                    }
                                }
                            }
                            else
                            {
                                MyTurnMap.Instance.GenerateAttackRange(this._currentSelectUnit,this._currentSelectUnit.attckrange);

                            }
                            if(_currentCell != null)
                            {
                                _currentCell.SwitchState(CellStateEnum.普通);
                            }
                           
                            this._currentCell = cell;
                            cell.SwitchState(CellStateEnum.可使用);
                        }
                        break;

                        //当前技能编辑
                        case TurnStateEnum.展示角色技能范围:
                        {                            
                            MyTurnMap.Instance.GenerateSkillRange(_currentSelectUnit, _currentSelectUnit._currentskill);
                            if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方群体减益类 || _currentSelectUnit._currentskill._skillcampsort == SkillCampSort.己方群体增益类 || _currentSelectUnit._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
                            {
                                MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);
                            }
                            if(_currentCell != null)
                            {
                                _currentCell.SwitchState(CellStateEnum.普通);
                            }
                            _currentCell = cell;
                            cell.SwitchState(CellStateEnum.可使用);                           
                        }
                        break;
                    }
                }
                
                //***********************************持续更新unit的移动范围和攻击范围内地块状态

                if (unit != null)
                {    
                    switch (_currentState)
                    {
                        case TurnStateEnum.选取角色:
                        {
                            _currentCell.SwitchState(CellStateEnum.普通); 

                            if (_currentUnit != null )      
                            {
                                if(_currentUnit != unit )
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);

                                }
                                else
                                {
                                    _currentCell = _currentUnit.CurrentCell;
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.可使用);
                                }
                            }

                               
                            _currentUnit = unit;
                            _currentUnit.PreSelect();
                            _currentCell = _currentUnit.CurrentCell;
                            _currentUnit.CurrentCell.SwitchState(CellStateEnum.可使用);
                        }
                        break;
                        case TurnStateEnum.角色行动:

                        break;
                        case TurnStateEnum.展示角色移动范围:
                        {
                            if (_currentUnit != null && _currentUnit != unit && _currentUnit != _currentSelectUnit)
                            {
                                _currentUnit.DisSelect();
                                _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);
                            }
                                _currentUnit = unit;
                               
                                _currentCell= _currentUnit.CurrentCell;                       
                                MyTurnMap.Instance.GenerateRange(_currentSelectUnit);
                                _currentUnit.CurrentCell.SwitchState(CellStateEnum.可使用);
                        }                   
                        break;

                        case TurnStateEnum.展示角色攻击范围:
                        {
                           
                            if(_currentSelectUnit.职业 == UnitCareer.炮手||_currentSelectUnit.职业 == UnitCareer.狙击手)
                            {
                                if (_currentUnit != null &&  _currentUnit != unit  && _currentUnit != _currentSelectUnit)
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);

                                }

                                _currentUnit = unit;
                                if(_currentUnit.CampSort != CampSortEnum.建筑 )
                                {
                                    if(_currentUnit.CampSort != _currentSelectUnit.CampSort)
                                    {
                                        _currentUnit.SelectAttack();
                                        _currentCell = _currentUnit.CurrentCell;

                                    }
                                    else
                                    {
                                        _currentUnit.DisSelect();
                                        _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);

                                    }
                                    
                                }
                                else
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);

                                }
                            
                                _currentCell = _currentUnit.CurrentCell;
                                if(_BattleWnd.moveLicense == true)
                                {
                                    MyTurnMap.Instance.GenerateAttackRange(_currentSelectUnit, this._currentSelectUnit.attckrange );
                                    if(_currentSelectUnit.职业 == UnitCareer.炮手)
                                    {
                                        MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit.固定伤害范围,_currentCell);
                                    }
                                    
                                }
                                else
                                {
                                    MyTurnMap.Instance.GenerateAttackRange(_currentSelectUnit, 1 );
                                    if(_currentSelectUnit.职业 == UnitCareer.炮手)
                                    {
                                        MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit.固定伤害范围,_currentCell);
                                    }
                                }
                                
                            }
                            else
                            {
                                if (_currentUnit != null && _currentUnit != unit && _currentUnit != _currentSelectUnit)
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);
                                }
                                  _currentUnit = unit;

                                if(_currentUnit.CampSort != _currentSelectUnit.CampSort && _currentUnit.CampSort != CampSortEnum.建筑)
                                {
                                    _currentUnit.SelectAttack();
                                }
                             

                                MyTurnMap.Instance.GenerateAttackRange(_currentSelectUnit,this._currentSelectUnit.attckrange );
                                _currentCell=_currentUnit.CurrentCell;
                                _currentUnit.CurrentCell.SwitchState(CellStateEnum.可使用);

                            }  
                                                  
                        }
                        break;

                        case TurnStateEnum.展示角色技能范围:
                        {

                            if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方群体减益类  )    //_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方减益类 || 
                            {

                                if (_currentUnit != null &&  _currentUnit != unit  && _currentUnit != _currentSelectUnit)   //
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);
                                }
                                _currentUnit = unit;
                                if(_currentUnit.CampSort != _currentSelectUnit.CampSort  )
                                {
                                    _currentUnit.SelectAttack();
                                    _currentCell = _currentUnit.CurrentCell;

                                }
                               
                                _currentCell = _currentUnit.CurrentCell;
                               
                                MyTurnMap.Instance.GenerateSkillRange(_currentSelectUnit, _currentSelectUnit._currentskill);
                                MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);                       
                            }
                            else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.己方群体增益类   )     //      && _currentUnit != _currentSelectUnit
                            {
                                if (_currentUnit != null &&  _currentUnit != unit  && _currentUnit != _currentSelectUnit )
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);
                                }
                                _currentUnit = unit;
                               
                                if( _currentUnit.CampSort == _currentSelectUnit.CampSort  )
                                {
                                    _currentUnit.SelectAttack();
                                    _currentCell = _currentUnit.CurrentCell;

                                }
                             
                                _currentCell = _currentUnit.CurrentCell;
                          
                                MyTurnMap.Instance.GenerateSkillRange(_currentSelectUnit, _currentSelectUnit._currentskill);
                                MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);
                                                            
                            }
                            else  if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.己方增益类  )
                            {

                                if (_currentUnit != null  &&  _currentUnit != unit    )     //    && _currentUnit != unit         && _currentUnit != _currentSelectUnit
                                {
                                  
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);

                                }
                                _currentUnit = unit;
                                if(_currentUnit.CampSort == _currentSelectUnit.CampSort )
                                {
                                    _currentUnit.SelectAttack();
                                }
                               
                                MyTurnMap.Instance.GenerateSkillRange(_currentSelectUnit, _currentSelectUnit._currentskill);
                                _currentCell = _currentUnit.CurrentCell;
                                _currentUnit.CurrentCell.High();
                            } 
                            else if( _currentSelectUnit._currentskill._skillcampsort == SkillCampSort.敌方减益类  )
                            {
                                if (_currentUnit != null  && _currentUnit != unit  && _currentUnit != _currentSelectUnit)     //    && _currentUnit != unit
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);
                                }
                                _currentUnit = unit;
                                if(_currentUnit.CampSort != _currentSelectUnit.CampSort)
                                {
                                    _currentUnit.SelectAttack();
                                }
                                                          
                                MyTurnMap.Instance.GenerateSkillRange(_currentSelectUnit, _currentSelectUnit._currentskill);
                                _currentCell = _currentUnit.CurrentCell;
                                _currentUnit.CurrentCell.High();
                            } 
                            else if(_currentSelectUnit._currentskill._skillcampsort == SkillCampSort.群体系统收益类)
                            {
                                if (_currentUnit != null &&  _currentUnit != unit  && _currentUnit != _currentSelectUnit )
                                {
                                    _currentUnit.DisSelect();
                                    _currentUnit.CurrentCell.SwitchState(CellStateEnum.普通);
                                }
                                _currentUnit = unit;
                               
                                if( _currentUnit.CampSort != _currentSelectUnit.CampSort  )
                                {
                                    _currentUnit.SelectAttack();
                                    _currentCell = _currentUnit.CurrentCell;

                                }
                              
                                _currentCell = _currentUnit.CurrentCell;                          
                                MyTurnMap.Instance.GenerateSkillRange(_currentSelectUnit, _currentSelectUnit._currentskill);
                                MyTurnMap.Instance.GetTargetUnitsCell(_currentSelectUnit,_currentSelectUnit._currentskill.技能释放范围,_currentCell);
                            }
                        }
                        break; 
                    }
                }
                else                                              //测试预选状态
                {
                    if ( _currentUnit != null &&_currentUnit!=_currentSelectUnit  )      //_currentUnit != null &&
                    {
                        _currentUnit.DisSelect();
                    } 
                    else
                    {
                        return;
                    }                 
                }
            }    
            else
            {
                if (_currentCell != null)
                {
                    _currentCell.Normal();
                    _currentCell = null;
                }
            }
        } 
    
}
    

    public void SetMyTurnMouseToPlacingCharacters()
    {
        _currentState = TurnStateEnum.放置角色;
    }
    

 
      
    void Update()
    {
       

        if(gm._currentGameState != 游戏流程阶段.展示系统技能支援范围)
        {
            MouseDetect();
            MouseInput();

        }

        if(_currentSelectUnit!=null)
        {  
            if(monitor._cameraMode == CameraMode.跟踪模式)
            {
                Monitor.Instance.FollowTo(_currentSelectUnit); 
            }                         
        } 
      

        if(_currentState == TurnStateEnum.展示角色移动范围)
        {
            if(_currentCell!=null && !_currentCell.NormalCell)
            {
                PointToLine.Instance.ShowCellToLine(_currentSelectUnit,_currentCell);
            }
            else
            {
                PointToLine.Instance.PointToLineHide();

            }
        }

        if(_currentState == TurnStateEnum.展示角色攻击范围)
        {
            if(_currentSelectUnit.攻击方式 == AttackType.炮击)
            {
                if(_currentCell!=null && !_currentCell.NormalCell    )
                {
                    PointToLine.Instance.ShowCellToLine(_currentSelectUnit,_currentCell);
                }
                else
                {
                    PointToLine.Instance.PointToLineHide();

                }
                
            }
            else
            {
                if( _currentUnit!=null  && _currentUnit.CampSort != CampSortEnum.建筑 )
                {
                    if(_currentCell != null && !_currentCell.NormalCell && gmmp._UnitAttckCellList.Contains(_currentUnit.CurrentCell))
                    {
                        PointToLine.Instance.ShowAttackPointToLine(_currentSelectUnit,_currentUnit);

                    }
                    else
                    {
                        PointToLine.Instance.PointToLineHide();       //目前修改

                    }
                }  
                else
                {
                    PointToLine.Instance.PointToLineHide();

                }         
            }           
        }

        if(_currentState == TurnStateEnum.展示角色攻击范围 || _currentState == TurnStateEnum.展示角色技能范围 ||_currentState == TurnStateEnum.展示系统支援技能范围)
        {
            if(_currentUnit.CampSort != CampSortEnum.建筑)
            {
                if(_currentState == TurnStateEnum.展示角色攻击范围 )
                {                               
                    _unitattackinfo.ShowUnitAttackInfo(_currentSelectUnit,_currentUnit);


                }
                else
                {
                    _unitattackinfo.ShowUnitSkillInfo(_currentSelectUnit);

                }

            }
            else
            {
                _unitattackinfo.HideUnitAttackInfo();

            }
        }
        else
        {
            _unitattackinfo.HideUnitAttackInfo();
        }

        
        if(_currentState == TurnStateEnum.选定角色行走路线 || _currentState == TurnStateEnum.角色动作结束 || _currentState == TurnStateEnum.选取角色 || _currentState == TurnStateEnum.角色行动  || _currentState == TurnStateEnum.移动后选项)
        {
            PointToLine.Instance.PointToLineHide();

            if(_currentState == TurnStateEnum.角色动作结束 || _currentState == TurnStateEnum.选取角色 || _currentState == TurnStateEnum.角色行动 )   
            {
                if(gm.targetEnemyteam!=null)
                {
                    UIManager.Instance.ShowEnemyCard(gm);

                }
            }            
        }


    }

}
