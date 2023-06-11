namespace Nailgod;
public class Helper
{
    public static void LogWarn(string message)
    {
        Nailgod.instance.LogWarn(message);
    }
    public static void LogError(string message)
    {
        Nailgod.instance.LogError(message);
    }
    public static bool OnGround(GameObject gameObject)
    {
        var origins = new List<Vector2>();
        var collider2D = gameObject.GetComponent<Collider2D>();
        origins.Add(new Vector2(collider2D.bounds.max.x, collider2D.bounds.min.y));
        origins.Add(new Vector2(collider2D.bounds.center.x, collider2D.bounds.min.y));
        origins.Add(collider2D.bounds.min);
        foreach (var origin in origins)
        {
            if (Physics2D.Raycast(origin, -Vector2.up, 0.08f, 256).collider != null)
            {
                return true;
            }
        }
        return false;
    }
    public static bool OnLeftWall(GameObject gameObject)
    {
        var origins = new List<Vector2>();
        var collider2D = gameObject.GetComponent<Collider2D>();
        origins.Add(collider2D.bounds.min);
        origins.Add(new Vector2(collider2D.bounds.min.x, collider2D.bounds.center.y));
        origins.Add(new Vector2(collider2D.bounds.min.x, collider2D.bounds.max.y));
        foreach (var origin in origins)
        {
            if (Physics2D.Raycast(origin, -Vector2.right, 0.08f, 256).collider != null)
            {
                return true;
            }
        }
        return false;
    }
    public static bool OnRightWall(GameObject gameObject)
    {
        var origins = new List<Vector2>();
        var collider2D = gameObject.GetComponent<Collider2D>();
        origins.Add(collider2D.bounds.max);
        origins.Add(new Vector2(collider2D.bounds.max.x, collider2D.bounds.center.y));
        origins.Add(new Vector2(collider2D.bounds.max.x, collider2D.bounds.min.y));
        foreach (var origin in origins)
        {
            if (Physics2D.Raycast(origin, Vector2.right, 0.08f, 256).collider != null)
            {
                return true;
            }
        }
        return false;
    }
    public static void FacePlayer(GameObject gameObject)
    {
        var localScale = gameObject.transform.localScale;
        var sign = gameObject.transform.position.x < HeroController.instance.transform.position.x ? -1 : 1;
        gameObject.transform.SetScaleX(sign * Mathf.Abs(localScale.x));
    }
    public static float AngleToPlayer(GameObject gameObject, float min = -90, float max = 90, float delta = 45)
    {
        var from = HeroController.instance.gameObject.transform.position;
        var to = gameObject.transform.position;
        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var angle = Mathf.Atan2(dy, dx) / Mathf.PI * 180;
        if (gameObject.transform.localScale.x < 0)
        {
            angle = 180 - angle;
        }
        angle += delta;
        angle = Mathf.Min(angle, max);
        angle = Mathf.Max(angle, min);
        if (gameObject.transform.localScale.x < 0)
        {
            angle = -angle;
        }
        return angle;
    }
    public static AudioClip LoadAudioClip(string path)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        var wavData = new WavData();
        wavData.Parse(stream, LogWarn);
        stream.Close();
        var samples = wavData.GetSamples();
        var clip = AudioClip.Create("Final Battle", samples.Length / wavData.FormatChunk.NumChannels, wavData.FormatChunk.NumChannels, (int)wavData.FormatChunk.SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
    public static void DumpTexture(GameObject gameObject)
    {
        var tk2dSprite = gameObject.GetComponent<tk2dSprite>();
        var mainTexture = tk2dSprite.CurrentSprite.material.mainTexture;
        TextureUtils.WriteTextureToFile(mainTexture, "mainTexture.png");
        LogWarn(Directory.GetCurrentDirectory());
    }
    public static Texture2D LoadTexture(string path)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        MemoryStream memoryStream = new((int)stream.Length);
        stream.CopyTo(memoryStream);
        stream.Close();
        var bytes = memoryStream.ToArray();
        memoryStream.Close();
        Texture2D texture2D = new(0, 0);
        texture2D.LoadImage(bytes, true);
        return texture2D;
    }
    public static void SetTexture(GameObject gameObject, Texture2D texture2D)
    {
        var tk2dSprite = gameObject.GetComponent<tk2dSprite>();
        tk2dSprite.CurrentSprite.material.mainTexture = texture2D;
    }
}
