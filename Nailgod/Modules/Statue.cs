namespace Nailgod;
public class Statue : Module
{
    GameObject dreamParticlesTemplate;
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)>
        {
            ("GG_Workshop", "GG_Statue_Grimm"),
        };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        dreamParticlesTemplate = preloadedObjects["GG_Workshop"]["GG_Statue_Grimm"].transform.Find("dream_version_switch").gameObject;
        dreamParticlesTemplate.transform.Find("GG_statue_plinth_dream").gameObject.SetActive(false);
        dreamParticlesTemplate.transform.Find("Statue Pt").gameObject.GetComponent<ParticleSystem>().startColor = new Color(.5f, .4f, .3f, 1);
    }
    public override void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        if (to.name == "GG_Workshop")
        {
            var GG_Statue_Sly = GameObject.Find("GG_Statue_Sly");
            var dreamParticles=UnityEngine.Object.Instantiate(dreamParticlesTemplate, GG_Statue_Sly.transform);
            dreamParticles.transform.Translate(0, -1, 0);
        }
    }
    public override string LanguageGetHook(string key, string sheetTitle, string orig)
    {
        if (key == "NAME_SLY" && sheetTitle == "CP3")
        {
            if (Language.Language.CurrentLanguage() == Language.LanguageCode.ZH)
            {
                orig = "斯莱";
            }
            else
            {
                orig = "Sly";
            }
        }
        else if (key == "GG_S_SLY" && sheetTitle == "CP3")
        {
            if (Language.Language.CurrentLanguage() == Language.LanguageCode.ZH)
            {
                orig = "骨钉之神";
            }
            else
            {
                orig = "God of Nails";
            }
        }
        else if (key == "SLY_BOSS_SUPER" && sheetTitle == "Titles")
        {
            if (Language.Language.CurrentLanguage() == Language.LanguageCode.ZH)
            {
                orig = "骨钉之神";
            }
            else
            {
                orig = "God of Nails";
            }
        }
        return orig;
    }
}
