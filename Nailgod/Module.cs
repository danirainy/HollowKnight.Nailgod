namespace Nailgod;
public abstract class Module
{
    public static Module instance;
    public static System.Random random = new();
    public Module()
    {
        instance = this;
        Nailgod.instance.modules.Add(this);
    }
    public string Name()
    {
        return GetType().Name;
    }
    public void LogWarn(string message)
    {
        Nailgod.instance.LogWarn(Name() + ": " + message);
    }
    public void LogError(string message)
    {
        Nailgod.instance.LogError(Name() + ": " + message);
    }
    public bool InArena()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GG_Sly";
    }
    public GameObject SlyBoss()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (var obj in scene.GetRootGameObjects())
        {
            LogWarn("checking" + obj.name);
            if (obj.name == "Battle Scene")
                return obj.Find("Sly Boss").gameObject;
        }
        return null;

    }
    public virtual List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)> { };
    }
    public virtual void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
    }
    public virtual void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
    }
    public virtual void BeginApplyMusicCue(On.AudioManager.orig_BeginApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
    {
    }
    public virtual void HeroUpdateHook()
    {
    }
    public virtual string LanguageGetHook(string key, string sheetTitle, string orig)
    {
        return orig;
    }
}
