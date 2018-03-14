using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LittleFsm<T> where T : struct
{
    [SerializeField]
    protected T m_PrevState;
    [SerializeField]
    protected T m_CurState;
    [SerializeField]
    public float m_StateTimer = 0;

    public T GetCurState { get { return m_CurState; } }
    //状态字典
    protected Dictionary<T, StateFunc> m_StateFunc = new Dictionary<T, StateFunc>();

    //注册状态
    public void RegistState(T state, StateFunc.EnterFunc enter, StateFunc.UpdateFunc update, StateFunc.ExitFunc exit)
    {
        StateFunc func = new StateFunc
        {
            enterFunc = enter,
            updateFunc = update,
            exitFunc = exit
        };

        this.m_StateFunc.Add(state, func);
        //Debug.Log("State:" + state);
    }

    public void UpdateState(float delta)
    {
        if (this.m_StateFunc.ContainsKey(this.m_CurState) && (this.m_StateFunc[this.m_CurState].updateFunc != null))
        {
            this.m_StateFunc[this.m_CurState].updateFunc(delta);
        }
        m_StateTimer += delta;
    }

    public void SetState(T state, object param = null)
    {
        if (m_CurState.Equals(state)) return;
        T curState = this.m_CurState;
        this.m_PrevState = curState;
        this.m_CurState = state;

        if (this.m_StateFunc.ContainsKey(curState) && (this.m_StateFunc[curState].exitFunc) != null)
        {
            this.m_StateFunc[curState].exitFunc();
        }

        if (this.m_StateFunc.ContainsKey(this.m_CurState) && (this.m_StateFunc[this.m_CurState].enterFunc) != null)
        {
            this.m_StateFunc[this.m_CurState].enterFunc(param);
        }
        this.m_StateTimer = 0f;

    }

    public bool HasState(T state)
    {
        return m_StateFunc.ContainsKey(state);
    }

}

[System.Serializable]
public class StateFunc
{
    public delegate void EnterFunc(object param);

    public delegate void ExitFunc();

    public delegate void UpdateFunc(float delta);

    public EnterFunc enterFunc;
    public ExitFunc exitFunc;
    public UpdateFunc updateFunc;
}

