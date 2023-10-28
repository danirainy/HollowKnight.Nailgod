namespace Nailgod;
[Serializable]
public class Settings
{
    public bool on = true;
    public bool debugLog = true;
}
public class Nailgod : Mod, IGlobalSettings<Settings>, IMenuMod
{
    public static Nailgod instance;
    public bool ToggleButtonInsideMenu => true;
    public Settings settings_ = new();
    public List<Module> modules = new();
    private bool debug = false;
    public Nailgod() : base("Nailgod")
    {
        instance = this;
        new Animation();
        new Arena();
        new Bullet();
        new Control();
        new Dialog();
        new Nail();
        new Statue();
    }
    public override string GetVersion() => "1.0.0.1";
    public override List<(string, string)> GetPreloadNames()
    {
        List<(string, string)> preloadNames = new();
        foreach (var module in modules)
        {
            foreach (var name in module.GetPreloadNames())
            {
                preloadNames.Add(name);
            }
        }
        return preloadNames;
    }
    public new void LogWarn(string message)
    {
        if (settings_.debugLog)
        {
            base.LogWarn(message);
        }
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
        ModHooks.AfterTakeDamageHook += AfterTakeDamageHook;
        On.AudioManager.BeginApplyMusicCue += BeginApplyMusicCue;
        ModHooks.HeroUpdateHook += HeroUpdateHook;
        ModHooks.LanguageGetHook += LanguageGetHook;
        // On.HeroController.TakeDamage += HeroController_TakeDamage;
        LogWarn("Disabled parry fix.");
        if (preloadedObjects != null)
        {
            foreach (var module in modules)
            {
                module.Initialize(preloadedObjects);
            }
        }
    }
    /*
    private void HeroController_TakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, GlobalEnums.CollisionSide damageSide, int damageAmount, int hazardType)
    {
        if (settings_.on && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GG_Sly" && damageAmount > 0 && ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanTakeDamage"))
        {
            if (FSMUtility.ContainsFSM(go, "nail_clash_tink"))
            {
                List<string> slashes = new List<string>()
                {
                    "Slash",
                    "AltSlash",
                    "DownSlash",
                    "UpSlash",
                    "WallSlash",
                };
                foreach (var slash in slashes)
                {
                    var s = self.gameObject.transform.Find("Attacks").Find(slash).gameObject;
                    var col = s.GetComponent<PolygonCollider2D>();
                    if (!col.enabled)
                    {
                        continue;
                    }
                    var colother = go.GetComponent<Collider2D>();
                    if (col == null || colother == null)
                    {
                        LogError("Invalid colliders.");
                    }
                    Collider2D colliderA = col;
                    Collider2D colliderB = colother;
                    ContactFilter2D contactFilter = new ContactFilter2D();
                    contactFilter.useTriggers = true;
                    List<Collider2D> results = new List<Collider2D>();
                    int numberOfCollisions = colliderA.OverlapCollider(contactFilter, results);
                    if (numberOfCollisions > 0)
                    {
                        foreach (Collider2D result in results)
                        {
                            if (result == colliderB)
                            {
                                LogWarn("Parrying fixed.");
                                return;
                            }
                        }
                    }
                }
            }
        }
        orig(self, go, damageSide, damageAmount, hazardType);
    }
    */
    private List<Module> ActiveModules()
    {
        if (settings_.on)
        {
            return modules;
        }
        else
        {
            return new List<Module>() { };
        }
    }
    private void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        LogWarn("acc" + from.name + " to " + to.name);
        foreach (var module in ActiveModules())
        {
            module.ActiveSceneChanged(from, to);
        }
        if (settings_.on && debug && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GG_Sly")
        {
            var slyBoss = GameObject.Find("Battle Scene").Find("Sly Boss").gameObject;
            var fsm = slyBoss.LocateMyFSM("Control");
            fsm.GetAction<Wait>("Docile", 0).time = 0;
            fsm.GetAction<Wait>("Call", 1).time = 0;
            fsm.GetAction<Wait>("Catch", 4).time = 0;
            fsm.GetAction<Wait>("Roar", 10).time = 0.01f;
        }
    }
    private int AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        return settings_.on && debug && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GG_Sly" ? 0 : damageAmount;
    }
    private IEnumerator BeginApplyMusicCue(On.AudioManager.orig_BeginApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
    {
        foreach (var module in ActiveModules())
        {
            module.BeginApplyMusicCue(orig, self, musicCue, delayTime, transitionTime, applySnapshot);
        }
        yield return orig(self, musicCue, delayTime, transitionTime, applySnapshot);
    }
    private void HeroUpdateHook()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GG_Sly");
        }
        foreach (var module in ActiveModules())
        {
            module.HeroUpdateHook();
        }
    }
    private string LanguageGetHook(string key, string sheetTitle, string orig)
    {
        foreach (var module in ActiveModules())
        {
            orig = module.LanguageGetHook(key, sheetTitle, orig);
        }
        return orig;
    }
    public void OnLoadGlobal(Settings settings) => settings_ = settings;
    public Settings OnSaveGlobal() => settings_;
    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? menu)
    {
        List<IMenuMod.MenuEntry> menus = new();
        menus.Add(
            new()
            {
                Name = "Nailgod",
                Values = new string[]
                {
                    Language.Language.Get("MOH_ON", "MainMenu"),
                    Language.Language.Get("MOH_OFF", "MainMenu"),
                },
                Saver = i => settings_.on = i == 0,
                Loader = () => settings_.on ? 0 : 1,
            }
        );
        menus.Add(
            new()
            {
                Name = "Debug Log",
                Values = new string[]
                {
                    "On",
                    "Off",
                },
                Saver = i => settings_.debugLog = i == 0,
                Loader = () => settings_.debugLog ? 0 : 1,
            }
        );
        return menus;
    }
}
