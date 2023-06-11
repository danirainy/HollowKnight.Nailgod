namespace Nailgod;
public class Animation : Module
{
    public class Clip
    {
        public List<Sprite> sprites = new List<Sprite>();
        public Vector3 position;
        public Vector3 scale;
        public Clip(Vector3 position_, Vector3 scale_)
        {
            position = position_;
            scale = scale_;
        }
    }
    public class SpriteFlasher : MonoBehaviour
    {
        public float flashDuration = 0.5f;
        public float flashAlpha = 0.5f;

        private SpriteRenderer spriteRenderer;
        private Shader shaderGUItext;
        private Shader shaderSpritesDefault;

        float timer = 0;
        public bool flashing;

        private void Awake()
        {
            var flash = gameObject.transform.Find("StageFlash").gameObject;
            spriteRenderer = flash.GetComponent<SpriteRenderer>();
            shaderGUItext = Shader.Find("GUI/Text Shader");
            shaderSpritesDefault = Shader.Find("Sprites/Default");
            flashing = false;
        }

        public void StartFlash()
        {
            flashing = true;
            timer = 0;
        }
        private void FixedUpdate()
        {
            if (flashing)
            {
                timer += Time.deltaTime;
                if (timer > flashDuration)
                {
                    flashing = false;
                }
            }
            if (flashing)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.material.shader = shaderGUItext;
                spriteRenderer.color = Color.white;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, timer / flashDuration);
            }
            else
            {
                spriteRenderer.enabled = false;
                spriteRenderer.material.shader = shaderSpritesDefault;
                spriteRenderer.color = Color.white;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }
    public class Animator : MonoBehaviour
    {
        public enum Status
        {
            Disabled,
            Playing,
            Played,
        }
        public enum Mode
        {
            Single,
            Repeat,
        }
        public GameObject stage;
        private GameObject stageFlash;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer s2;
        public Dictionary<string, Clip> clips;
        public Status status = Status.Disabled;
        private Mode currentMode;
        private List<(string, float)> currentClipPackages;
        private int currentClipIndex;
        private string currentClipName;
        private int currentFrameIndex;
        private float currentFrameDuration;
        private float timer;
        public Animator() : base()
        {
            stage = new GameObject("Stage");
            stage.transform.parent = gameObject.transform;
            spriteRenderer = stage.AddComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            stageFlash = new GameObject("StageFlash");
            stageFlash.transform.position = stage.transform.position;
            stageFlash.transform.SetParent(stage.transform);
            s2 = stageFlash.AddComponent<SpriteRenderer>();
            s2.enabled = false;
            stage.AddComponent<SpriteFlasher>();
        }
        public void Disable()
        {
            status = Status.Disabled;
        }
        private void PlayClip(string clipName, float frameDuration, Mode mode)
        {
            if (clips == null)
            {
                Helper.LogError("Clips are not loaded.");
                return;
            }
            if (!clips.ContainsKey(clipName))
            {
                Helper.LogError("Clip name is invalid.");
                return;
            }
            if (frameDuration < 0)
            {
                Helper.LogError("Frame duration is invalid.");
                return;
            }
            status = Status.Playing;
            currentMode = mode;
            currentClipName = clipName;
            currentFrameIndex = 0;
            currentFrameDuration = frameDuration;
            timer = 0;
            var clip = clips[currentClipName];
            stage.transform.localPosition = clip.position;
            stage.transform.localScale = clip.scale;
            spriteRenderer.sprite = clip.sprites[0];
            s2.sprite = spriteRenderer.sprite;
        }
        public void Play(List<(string, float)> clipPackages, Mode mode)
        {
            if (clipPackages == null || clipPackages.Count == 0)
            {
                Helper.LogError("Clip packages are invalid.");
                return;
            }
            currentClipPackages = clipPackages;
            currentClipIndex = 0;
            var package = currentClipPackages[0];
            PlayClip(package.Item1, package.Item2, mode);
        }
        public void Play(string clipName, float frameDuration, Mode mode)
        {
            Play(new List<(string, float)> { (clipName, frameDuration) }, mode);
        }
        private void FixedUpdate()
        {
            var disable = status == Status.Disabled;
            GetComponent<tk2dSprite>().color = disable ? Vector4.one : Vector4.zero;
            spriteRenderer.enabled = !disable;
            if (disable)
            {
                stage.GetComponent<SpriteFlasher>().flashing = false;
            }
            if (status == Status.Playing)
            {
                timer += Time.deltaTime;
                if (timer > currentFrameDuration)
                {
                    timer = 0;
                    ++currentFrameIndex;
                    if (currentFrameIndex == clips[currentClipName].sprites.Count)
                    {
                        ++currentClipIndex;
                        if (currentClipIndex == currentClipPackages.Count)
                        {
                            if (currentMode == Mode.Single)
                            {
                                status = Status.Played;
                                return;
                            }
                            else
                            {
                                currentClipIndex = 0;
                            }
                        }
                        var package = currentClipPackages[currentClipIndex];
                        PlayClip(package.Item1, package.Item2, currentMode);
                        return;
                    }
                    spriteRenderer.sprite = clips[currentClipName].sprites[currentFrameIndex];
                    s2.sprite = spriteRenderer.sprite;
                }
            }
        }
    }
    public static Dictionary<string, Clip> clips = new Dictionary<string, Clip>();
    private Sprite LoadSprite(string name)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        MemoryStream memoryStream = new((int)stream.Length);
        stream.CopyTo(memoryStream);
        stream.Close();
        var bytes = memoryStream.ToArray();
        memoryStream.Close();
        var texture2D = new Texture2D(0, 0);
        texture2D.LoadImage(bytes, true);
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), Vector2.one / 2, 100.0f);
    }
    private void LoadClip(string name, int length, Vector3 position, Vector3 scale)
    {
        var clip = new Clip(position, scale);
        for (int i = 0; i < length; ++i)
        {
            clip.sprites.Add(LoadSprite("Nailgod.Resources.Animations._" + name + "." + i.ToString("D2") + ".png"));
        }
        clips[name] = clip;
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        LoadClip("00.Evade", 8, new Vector3(1.4f, 0.6f, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("01.Jump", 5, new Vector3(1.7f, 0.3f, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("02.Transform", 2, new Vector3(1.5f, 0.3f, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("03.Transform", 2, new Vector3(-1.6f, 0.9f, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("04.Transform", 6, new Vector3(-1.6f, 0.9f, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("05.Fall", 5, new Vector3(-1.6f, 0.9f, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("08.Shoot", 3, new Vector3(-1.1f, 1, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("10.Bullet", 1, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        LoadClip("11.Block", 1, new Vector3(-0.5f, 1, 0), new Vector3(-1.6f, 1.6f, 1));
        LoadClip("12.Bullet", 1, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        LoadClip("13.Shoot", 3, new Vector3(-1.1f, 1, 0), new Vector3(-1.5f, 1.5f, 1));
        LoadClip("14.Shadow", 1, new Vector3(0.5f, 0.2f, 0.004f - 0.0062f), new Vector3(2, 2, 1));
        LoadClip("15.Shoot", 6, new Vector3(-1.1f, 1, 0), new Vector3(-1.5f, 1.5f, 1));
    }
    public override void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        if (InArena())
        {
            var an = SlyBoss().AddComponent<Animator>();
            an.clips = clips;
            var fsm = SlyBoss().LocateMyFSM("Stun Control");
            fsm.InsertCustomAction("In Combo", () =>
            {
                var sf = an.stage.GetComponent<SpriteFlasher>();
                if (sf != null)
                {
                    sf.StartFlash();
                    LogWarn("Start flash");
                }
            }, 0);
            fsm.InsertCustomAction("Unstun Increment", () =>
            {
                var sf = an.stage.GetComponent<SpriteFlasher>();
                if (sf != null)
                {
                    sf.StartFlash();
                    LogWarn("Start flash");
                }
            }, 0);
        }
    }
}
