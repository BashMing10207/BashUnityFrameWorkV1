using System;

public interface IGetCompoParent<T> where T : IGetCompoParent<T>
{
    void Init();
    void RegisterEvents();
    void UnregisterEvents();

    public void AddCompoDic(System.Type type, IGetCompoable<T> compo);
    public void RemoveCompoDic<K>() where K : IGetCompoable<T>;
    public void ClearCompoDic();
    public K GetCompo<K>(bool isIncludeChild = false) where K : IGetCompoable<T>;
    //public U GetCompo<U>() where U : IGetCompoable<T>;
    public bool HasCompo(Type type);
    public int CompoCount();
    public void ForEachCompo(System.Action<IGetCompoable<T>> action);
    public void ForEachCompo<U>(System.Action<U> action) where U : IGetCompoable<T>;
}
