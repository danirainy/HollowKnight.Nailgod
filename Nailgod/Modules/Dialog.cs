namespace Nailgod;
public class Dialog : Module
{
    public override void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        if (InArena())
        {
            SlyBoss().GetComponent<EnemyDreamnailReaction>().enabled = false;
            var fsm = SlyBoss().LocateMyFSM("Control");
            fsm.AddCustomAction("Bow", () =>
            {
                PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
                playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 3;
                playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "SLY";
                playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
            });
        }
    }
    public override string LanguageGetHook(string key, string sheetTitle, string orig)
    {
        if (key.StartsWith("SLY") && sheetTitle == "Enemy Dreams")
        {
            var bossLevel = BossSceneController.Instance.BossLevel;
            if (Language.Language.CurrentLanguage() == Language.LanguageCode.ZH)
            {
                if (bossLevel == 2)
                {
                    orig = "bd.";
                }
                else
                {
                    orig = "......";
                }
            }
            else
            {
                if (bossLevel == 2)
                {
                    orig = "bd.";
                }
                else
                {
                    orig = "......";
                }
            }
        }
        return orig;
    }
}
