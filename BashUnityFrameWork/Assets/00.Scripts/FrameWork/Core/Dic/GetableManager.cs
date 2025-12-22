using UnityEngine;

public class GetableManager : BaseGameCompo,IGetCompoable<SubSystem>
{
    [HideInInspector]
    public SubSystem Mom;

    public virtual void Init(SubSystem mom)
    {
        mom.AddCompoDic(this.GetType(), this);
        Mom = mom;
    }

    public void RegisterEvents(SubSystem main)
    {

    }

    public virtual void UnregisterEvents(SubSystem main)
    {

    }
    private void Start()
    {
        if(Mom == null)
        {
            //Transform trm = GetComponent<Transform>();

            //while(trm.parent != null && trm.gameObject.GetComponent<GameManager>())
            //{
            //    trm = trm.parent;
            //}
            //Init(trm.GetComponent<GameManager>());
            if (SubSystem.Instance != null)
                Init(SubSystem.Instance);

        }
    }
}

public class GetableManagerParent<T> : GetCompoParentSample<T>, IGetCompoable<SubSystem> where T : IGetCompoParent<T>
{
    [HideInInspector]
    public SubSystem Mom;

    public virtual void Init(SubSystem mom)
    {
        mom.AddCompoDic(this.GetType(), this);
        Mom = mom;
    }

    public void RegisterEvents(SubSystem main)
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterEvents(SubSystem main)
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        if (Mom == null)
        {
            //Transform trm = GetComponent<Transform>();

            //while(trm.parent != null && trm.gameObject.GetComponent<GameManager>())
            //{
            //    trm = trm.parent;
            //}
            //Init(trm.GetComponent<GameManager>());
            if (SubSystem.Instance != null)
                Init(SubSystem.Instance);

        }
    }
}
