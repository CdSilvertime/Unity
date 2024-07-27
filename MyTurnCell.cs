using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum CellStateEnum
{
    普通,
    可使用,
    可移动,
    可攻击,
    可准备,
    可援助,
    可撤离
}

public enum CellName
{
    普通,
    草地,
    平地,
    高台,
    水面,
    障碍物无攻击阻挡,
    障碍物有攻击阻挡


}

///<summary>
///
///</summary>
public class MyTurnCell : MonoBehaviour
{
    Material _selfMat;
    Color _originColor;

    [SerializeField]
    Color _zerocolor;
    [SerializeField]
    Color _highColor;
    [SerializeField]
    Color _warnColor01;
    [SerializeField]
    Color _warnColor02;
    [SerializeField]
    Color _rangeColor01;
    [SerializeField]
    Color _rangeColor02;
    [SerializeField]
    Color _readyColor01;
    [SerializeField]
    Color _readyColor02;
    [SerializeField]
    Color _escapeColor01;
    [SerializeField]
    Color _escapeColor02;

    [SerializeField]
    Color _support;   //支援色

    [SerializeField]
    Color _hathpace;   //高台色
    
    [SerializeField]
    Color _gold;   //高台色

    public MyTurnMap gmmap;
    public float duration;
    private float t =0;
    private float i =0;
    bool OnOff;
    public CellName _cellname;
    public int CellNum;
    public int WaterNum;
    public CellStateEnum CellState { get; private set; }
    CellStateEnum _lastState;
    public int G;
    public int F;
    public int P;                             //玩家队单位提供P值
    public int E;                             //敌人队单位提供E值
    public int _row;
    public int _col;
    public MyTurnCell Parent;
    public MyTurnCell RangeParent;
    public bool _openChest;            //判定宝箱开启选项出现
    public bool _mineOpen;             //判定地雷状态激活
    public bool IsUnit;                //判定是否有Unit在该格
    public bool IsItem;
    public bool IsObstacle;            //决定是否为障碍物 
    public bool IsWater;               //决定是否为水域.
    public bool NormalCell;
    public bool UnitCanSet;            //己方可准备的格子
    public bool IsSurround;            //判定格子是否被包围
    public bool UnitSurround;          //判定单位是否被包围
    public bool _unitEscape;
    public bool _boundary;             //判断边界
    public bool 是否高台;
    public bool 攻击阻挡;
    public bool 建筑战略支援;
    public bool 战略支援;
    public bool 显示号码;
    public float _fontsize;
    public float _showcellnum;
    MyTurnUnit _currentUnit;
    MyTurnItem _currentItem;
    TextMeshPro _selfText;
    GameManage gm;
    
    public List<MyTurnPoint> _pointList;

   


    void Start()
    {
        gm = GameObject.Find("GameManage").GetComponent<GameManage>();
        _selfMat = GetComponent<MeshRenderer>().material;
        _selfText = transform.Find("CellNum").GetComponent<TextMeshPro>();
        _openChest=false;
        IsSurround=false; 
        _originColor = _selfMat.color;       
        
    }


    public void SwitchState(CellStateEnum cellState)
    {
        _lastState = CellState;
        switch (cellState)
        {
            case CellStateEnum.普通:
            {
                Normal();
            }
            break;
            case CellStateEnum.可使用:
            {
                High();
            }break;
            case CellStateEnum.可移动:
            {
                ShowRange();
            }
            break;
            case CellStateEnum.可攻击:
            {
                Warn();
            }
            break;
            case CellStateEnum.可准备:
            {
                Ready();
            }
            break;
            case CellStateEnum.可援助:
            {
                Escape();
            }
            break;
            case CellStateEnum.可撤离:
            {
                Escape();
            }
            break;
        }
        CellState = cellState;
    }



    public void High()
    {
        _selfMat.color = _highColor;
    }

    public void Normal()
    {
        _selfMat.color = _originColor;
    }

    public void ShowRange()
    {
       if(!OnOff)
        {
          i=0;   
          t += Time.deltaTime*0.7f / duration;
          _selfMat.color = Color.Lerp(_rangeColor01, _rangeColor02, t);            //可移动范围闪烁
          if(t>=1)
          OnOff=true;
        }
        else
        {
            t=0;
            i += Time.deltaTime*0.7f / duration;
            _selfMat.color = Color.Lerp(_rangeColor02, _rangeColor01, i);
            if(i>=1)
            OnOff=false;
        }
    }

    public void Warn()
    {
       if(!OnOff)
        {
          i=0;   
          t += Time.deltaTime*0.7f/ duration;
          _selfMat.color = Color.Lerp(_warnColor01, _warnColor02, t);
          if(t>=1)
          OnOff=true;  
        }
        else
        {
            t=0;
            i += Time.deltaTime*0.7f/ duration;
            _selfMat.color = Color.Lerp(_warnColor02, _warnColor01, i);
            if(i>=1)
            OnOff=false;
        }
    }

    
    public void Ready()
    {
       if(!OnOff)
        {
          i=0;   
          t += Time.deltaTime*1f/ duration;
          _selfMat.color = Color.Lerp(_readyColor01, _readyColor02, t);
          if(t>=1)
          OnOff=true;  
        }
        else
        {
            t=0;
            i += Time.deltaTime*1f/ duration;
            _selfMat.color = Color.Lerp(_readyColor02, _readyColor01, i);
            if(i>=1)
            OnOff=false;
        }
    }

    public void Escape()
    {
       if(!OnOff)
        {
          i=0;   
          t += Time.deltaTime*0.7f/ duration;
          _selfMat.color = Color.Lerp(_escapeColor01, _escapeColor02, t);
          if(t>=1)
          OnOff=true;  
        }
        else
        {
            t=0;
            i += Time.deltaTime*0.7f/ duration;
            _selfMat.color = Color.Lerp(_escapeColor02, _escapeColor01, i);
            if(i>=1)
            OnOff=false;
        }
    }


    public void Hide()
    {
        _selfMat.color = Color.clear;
    }

//显示Cell编号和颜色


void ShowUnitCanSetCell()
{
    if(gm._currentGameState == 游戏流程阶段.准备阶段)
    {
        if(this.UnitCanSet == true)
        {
            this.Ready();
            
        }
    }
    else
    {
        if(NormalCell==true)
        {
            this.Normal(); 
        }
    }   
}
void HideUnitCanSetCell()
{
    if( UnitCanSet == true)
    {
        this.Normal();
    }
}

void ShowUnitEscapeCell()
{
    if(_unitEscape == true)
    {
        this.Escape();

    }
}
 
public void ShowSupport()
{
    _selfText.text = "□";
    _selfText.color = _support;
    
}

public void ShowHathpace()
{
     _selfText.text = "H";
    _selfText.color = _hathpace;

}


//上两排代码注释后可以看格子编号

public void CheckCell()
{
    if(_boundary )
    {
        _selfText.text = ""+CellNum +"\n"
        +"P值:"+P +"\n" + "E值:" +E;
        _selfText.fontSize = _fontsize;      
        _zerocolor.a = _showcellnum;             
        _selfText.color = _zerocolor;        
    }
    else
    {
        if(显示号码 == true)
        {
            _selfText.text = ""+ CellNum +"\n"
            +"P值:"+P +"\n" + "E值:" +E;
            _selfText.fontSize = _fontsize;             
            _selfText.color = Color.black;

        }
        else
        {
            _selfText.text = ""+ CellNum +"\n"
            +"P值:"+P +"\n" + "E值:" +E;
            _selfText.fontSize = 8f;      
            _zerocolor.a = _showcellnum;             
            _selfText.color = _zerocolor;
            if( 战略支援 )
            {
                ShowSupport();
            }
        }
    }
}

//"P值:"+P +"\n" + "E值:" +E;

void Update()
{        
 
    if(gm._currentGameState == 游戏流程阶段.准备阶段)
    {
        ShowUnitCanSetCell();
    }

    if(gm._currentGameState != 游戏流程阶段.准备阶段)
    {
        ShowUnitEscapeCell();

    }
    
    CheckCell();


}
   
}
