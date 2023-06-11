namespace Nailgod;
public class State
{
    private List<FsmStateAction> actions = new List<FsmStateAction>();
    private float timer;
    private float timeLimit = float.MaxValue;
    public State(GameObject gameObject)
    {
    }
    public string Name()
    {
        return GetType().Name;
    }
    protected void LogWarn(string message)
    {
        Nailgod.instance.LogWarn(Name() + ": " + message);
    }
    protected void LogError(string message)
    {
        Nailgod.instance.LogError(Name() + ": " + message);
    }
    protected void ExitAfterSeconds(float timeLimit_)
    {
        timeLimit = timeLimit_;
    }
    protected void AddAction(FsmStateAction fsmStateAction)
    {
        actions.Add(fsmStateAction);
    }
    public virtual void OnEnter(GameObject gameObject)
    {
    }
    public void OnEnterInternal(GameObject gameObject)
    {
        timer = 0;
        OnEnter(gameObject);
        foreach (var action in actions)
        {
            action.OnEnter();
        }
    }
    public virtual bool OnFixedUpdate(GameObject gameObject)
    {
        return true;
    }
    public bool OnFixedUpdateInternal(GameObject gameObject)
    {
        timer += Time.deltaTime;
        bool repeat = OnFixedUpdate(gameObject);
        foreach (var action in actions)
        {
            action.OnFixedUpdate();
        }
        return timer < timeLimit && repeat;
    }
    public virtual void OnUpdate(GameObject gameObject)
    {
    }
    public void OnUpdateInternal(GameObject gameObject)
    {
        OnUpdate(gameObject);
        foreach (var action in actions)
        {
            action.OnUpdate();
        }
    }
    public virtual string OnExit(GameObject gameObject, bool interrputed)
    {
        return null;
    }
    public string OnExitInternal(GameObject gameObject, bool interrputed)
    {
        string nextStateName = OnExit(gameObject, interrputed);
        foreach (var action in actions)
        {
            action.OnExit();
        }
        return nextStateName;
    }
}
public class StateMachine : MonoBehaviour
{
    private string initialStateName;
    private string currentStateName;
    private enum Status
    {
        Head,
        Body,
        Tail,
    }
    private Status status;
    private Dictionary<string, State> states = new Dictionary<string, State>();
    private PlayMakerFSM parentFSM;
    private string parentStateName;
    public string Name()
    {
        return GetType().Name;
    }
    protected void LogWarn(string message)
    {
        Nailgod.instance.LogWarn(name + "." + Name() + ": " + message);
    }
    protected void LogError(string message)
    {
        Nailgod.instance.LogError(name + "." + Name() + ": " + message);
    }
    protected void AddState(State state)
    {
        states[state.Name()] = state;
    }
    protected void SetInitialState(State state)
    {
        initialStateName = state.Name();
        currentStateName = state.Name();
        status = Status.Head;
    }
    protected void AddSetInitialState(State state)
    {
        AddState(state);
        SetInitialState(state);
    }
    protected void SetParent(PlayMakerFSM parentFSM_, string parentState_)
    {
        parentFSM = parentFSM_;
        parentStateName = parentState_;
    }
    private bool CheckCurrentStateName()
    {
        if (currentStateName == null)
        {
            LogError("State is null.");
            return false;
        }
        if (!states.ContainsKey(currentStateName))
        {
            LogError("State " + currentStateName + " is not found.");
            return false;
        }
        return true;
    }
    private void FixedUpdate()
    {
        if (!CheckCurrentStateName())
        {
            return;
        }
        var state = states[currentStateName];
        if (parentFSM != null && parentFSM.ActiveStateName != parentStateName)
        {
            if (status != Status.Head)
            {
                LogWarn("Interrupting " + currentStateName);
                state.OnExitInternal(gameObject, true);
            }
            currentStateName = initialStateName;
            status = Status.Head;
        }
        else if (status == Status.Head)
        {
            LogWarn("Entering " + currentStateName);
            state.OnEnterInternal(gameObject);
            status = Status.Body;
        }
        else if (status == Status.Body)
        {
            if (!state.OnFixedUpdateInternal(gameObject))
            {
                status = Status.Tail;
            }
        }
        else
        {
            LogWarn("Leaving " + currentStateName);
            currentStateName = state.OnExitInternal(gameObject, false);
            status = Status.Head;
        }
    }
    private void Update()
    {
        if (!CheckCurrentStateName())
        {
            return;
        }
        var state = states[currentStateName];
        if (parentFSM != null && parentFSM.ActiveStateName != parentStateName)
        {
        }
        else if (status == Status.Body)
        {
            state.OnUpdateInternal(gameObject);
        }
    }
}