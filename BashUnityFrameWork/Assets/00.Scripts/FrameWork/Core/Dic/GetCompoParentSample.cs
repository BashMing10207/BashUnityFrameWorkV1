using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetCompoParentSample<T> : BaseGameCompo, IGetCompoParent<T> where T : IGetCompoParent<T>
{
    // [변경] 키를 (Type, string) 튜플에서 Type 단일 키로 변경
    protected Dictionary<Type, IGetCompoable<T>> _components = new Dictionary<Type, IGetCompoable<T>>();

    private readonly List<ILifeCycleable<T>> _lifeCycles = new();

    protected virtual void Awake()
    {
        Init();
    }

    public void RegisterEvents()
    {
        foreach (var compo in _components.Values)
        {
            if (this is T parent)
            {
                compo.RegisterEvents(parent);
            }
        }
    }

    public void UnregisterEvents()
    {
        foreach (var compo in _components.Values)
        {
            if (this is T parent)
            {
                compo.UnregisterEvents(parent);
            }
        }
    }

    // [변경] name 파라미터 제거
    public virtual void AddCompoDic(Type type, IGetCompoable<T> compo)
    {
        // 이미 해당 타입이 존재하면 추가하지 않음 (덮어쓰기를 원하면 인덱서 [] 사용)
        if (!_components.ContainsKey(type))
        {
            _components.Add(type, compo);
        }
        else
        {
            Debug.LogWarning($"[GetCompoParent] Component of type {type.Name} already exists.");
        }
    }

    // [변경] name 파라미터 제거 및 로직 단순화
    public virtual void RemoveCompoDic<K>() where K : IGetCompoable<T>
    {
        Type type = typeof(K);

        // 정확히 일치하는 타입이 있는 경우 삭제
        if (_components.TryGetValue(type, out var compo))
        {
            compo.UnregisterEvents(this is T parent ? parent : default);
            _components.Remove(type);
            return;
        }

        // [선택 사항] 상속 관계까지 찾아서 지워야 한다면 아래 로직 유지, 
        // 보통 딕셔너리 관리에서는 정확한 타입 키 삭제를 선호하므로 위에서 처리됨.
        // 만약 Interface로 등록된 것을 구현체 타입으로 지우려 할 때를 대비한다면:
        foreach (var kvp in _components)
        {
            if (typeof(K).IsAssignableFrom(kvp.Key))
            {
                kvp.Value.UnregisterEvents(this is T parent ? parent : default);
                _components.Remove(kvp.Key);
                return; // 하나 지우고 리턴
            }
        }
    }

    // [변경] name 파라미터 제거
    public virtual void AddRealCompo<K>() where K : Component, IGetCompoable<T>
    {
        // 이미 존재하면 추가하지 않고 리턴
        if (HasCompo(typeof(K))) return;

        K instance = gameObject.AddComponent<K>();
        _components.Add(instance.GetType(), instance);
    }

    // [변경] name 파라미터 제거, 딕셔너리 조회 최적화
    public virtual K GetCompo<K>(bool isIncludeChild = false) where K : IGetCompoable<T>
    {
        Type type = typeof(K);

        // 1. O(1) 조회: 정확한 타입 매칭 시도
        if (_components.TryGetValue(type, out var compo))
        {
            return (K)compo;
        }

        // 2. O(N) 조회: 인터페이스나 부모 클래스로 찾을 경우 (isIncludeChild가 true일 때만 검색)
        // 혹은 GetCompo<Interface> 형태로 호출했을 때를 대비해 기본적으로 검색할 수도 있음
        // 여기서는 기존 로직의 isIncludeChild 의도를 존중
        if (!isIncludeChild) 
        {
            // O(N) 시도: 딕셔너리 키(구현체)가 K(인터페이스)를 상속받았는지 확인
            foreach (var kvp in _components)
            {
                if (type.IsAssignableFrom(kvp.Key))
                    return (K)kvp.Value;
            }
            return default;
        }

        // isIncludeChild가 true일 경우의 추가 로직이 필요하다면 여기에 작성
        // (현재 구조에서는 위 루프가 그 역할을 겸함)

        return default;
    }

    public void ClearCompoDic()
    {
        _components.Clear();
    }

    // [변경] name 파라미터 제거
    public virtual U GetOrAddCompo<U>() where U : Component, IGetCompoable<T>
    {
        U compo = GetCompo<U>();

        if (compo == null)
        {
            AddRealCompo<U>();
            compo = GetCompo<U>();
        }

        return compo;
    }

    public int CompoCount()
    {
        return _components.Count;
    }

    public void ForEachCompo(Action<IGetCompoable<T>> action)
    {
        foreach (var component in _components.Values)
        {
            action(component);
        }
    }

    public void ForEachCompo<U>(Action<U> action) where U : IGetCompoable<T>
    {
        foreach (var component in _components.Values)
        {
            if (component is U typedComponent)
            {
                action(typedComponent);
            }
        }
    }

    public virtual void Init()
    {
        // 기존 로직 유지: 자식들 초기화
        IGetCompoable<T>[] babies = GetComponentsInChildren<IGetCompoable<T>>(true);
        for (int i = 0; i < babies.Length; i++)
        {
            babies[i].Init(this is T parent ? parent : default);
            
            // [팁] 만약 Init 단계에서 자동으로 딕셔너리에 등록하길 원한다면:
            // AddCompoDic(babies[i].GetType(), babies[i]);
        }

        _lifeCycles.Clear();
        ILifeCycleable<T>[] babies2 = GetComponentsInChildren<ILifeCycleable<T>>(true);
        for (int i = 0; i < babies2.Length; i++)
        {
            var lc = babies2[i];
            lc.Init(this is T parent ? parent : default);
            _lifeCycles.Add(lc);
        }

        RegisterEvents();

        foreach (var lc in _lifeCycles)
            lc.AfterInit();
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;
        foreach (var lc in _lifeCycles)
            lc.Tick(dt);
    }

    protected virtual void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;
        foreach (var lc in _lifeCycles)
            lc.TickFixed(fdt);
    }

    protected virtual void OnDestroy()
    {
        foreach (var lc in _lifeCycles)
            lc.BeforeDestroy();

        UnregisterEvents();
    }

    // [변경] 단순화된 HasCompo
    public bool HasCompo(Type type)
    {
        // 1. 정확한 키 확인 (O(1))
        if (_components.ContainsKey(type)) return true;

        // 2. 상속 관계 확인 (O(N)) - 인터페이스로 물어봤을 때 구현체가 있는지
        foreach (var keyType in _components.Keys)
        {
            if (type.IsAssignableFrom(keyType))
                return true;
        }
        return false;
    }

    // HasCompo(string name) 및 HasCompo(Type, string) 오버로드는 삭제됨
}