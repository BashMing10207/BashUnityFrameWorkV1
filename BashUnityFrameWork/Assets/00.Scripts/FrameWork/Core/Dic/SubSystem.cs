using UnityEngine;

public class SubSystem : GetCompoParentSample<SubSystem>
{
    private static SubSystem _instance;

    public static SubSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("SubSystem", typeof(SubSystem)).GetComponent<SubSystem>();
            }
            else
            {

            }

            return _instance;
        }
    }

    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    public override void Init()
    {
        if( _instance == null )
        {
            _instance = this;

        }
        else
        {
           
        }
        base.Init();
    }

    public override K GetCompo<K>(bool isIncludeChild = false)
    {

        //if (_components.TryGetValue(typeof(K), out var compo))
        //{
            return base.GetCompo<K>(isIncludeChild);
        //}
        //Create Compo when No Compo HEHEHA
    }
}

    //public static GameManager Instance
    //{
    //    get
    //    {
    //        
    //if(Instance == null)
    //    {
    //       Instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();
    //    }

//        return Instance;
//    }

//    private set;
//}