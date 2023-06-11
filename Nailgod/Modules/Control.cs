namespace Nailgod;
public partial class Control : Module
{
    private partial class ControlStateMachine : StateMachine
    {
        ControlStateMachine() : base()
        {
            AddSetInitialState(new Move(gameObject));
            AddState(new AirDashSlashAntic(gameObject));
            AddState(new AirDashSlashJump(gameObject));
            AddState(new AirDashSlashAim1(gameObject));
            AddState(new AirDashSlashSlash1(gameObject));
            AddState(new AirDashSlashAim2(gameObject));
            AddState(new AirDashSlashSlash2(gameObject));
            AddState(new AirDashSlashAim3(gameObject));
            AddState(new AirDashSlashSlash3(gameObject));
            AddState(new AirDashSlashEnd(gameObject));
            AddState(new BlockSlashCharged(gameObject));
            AddState(new BlockSlashBlock(gameObject));
            AddState(new BlockSlashCharged(gameObject));
            AddState(new BlockSlashSlash1(gameObject));
            AddState(new BlockSlashSlash2(gameObject));
            AddState(new BlockSlashSlash3(gameObject));
            AddState(new BlockSlashSlash4(gameObject));
            AddState(new BunnyHopSlashAntic(gameObject));
            AddState(new BunnyHopSlashHop(gameObject));
            AddState(new BunnyHopSlashSlashAntic(gameObject));
            AddState(new BunnyHopSlashSlashS1(gameObject));
            AddState(new BunnyHopSlashSlashS2(gameObject));
            AddState(new BunnyHopSlashSlashS3(gameObject));
            AddState(new BunnyHopSlashBSlashAntic(gameObject));
            AddState(new BunnyHopSlashBSlashDash(gameObject));
            AddState(new BunnyHopSlashBSlashS1(gameObject));
            AddState(new BunnyHopSlashBSlashS2(gameObject));
            AddState(new BunnyHopSlashBSlashS3(gameObject));
            AddState(new BunnyHopSlashCharge(gameObject));
            AddState(new BunnyHopSlashWait(gameObject));
            AddState(new BunnyHopSlashSlash1(gameObject));
            AddState(new BunnyHopSlashSlash2(gameObject));
            AddState(new BunnyHopSlashSlash3(gameObject));
            AddState(new BunnyHopSlashBackSlash1(gameObject));
            AddState(new BunnyHopSlashBackSlash2(gameObject));
            AddState(new BunnyHopSlashBackSlash3(gameObject));
            AddState(new BunnyHopSlashFall(gameObject));
            AddState(new JumpShootJump(gameObject));
            AddState(new JumpShootShoot(gameObject));
            AddState(new JumpShootFall(gameObject));
            AddState(new JumpShootLand(gameObject));
            AddState(new JumpShootS1(gameObject));
            AddState(new JumpShootS2(gameObject));
            AddState(new JumpShootS3(gameObject));
            AddState(new JumpLaserJump(gameObject));
            AddState(new JumpLaserShoot(gameObject));
            AddState(new JumpLaserScan(gameObject));
            AddState(new JumpLaserFall(gameObject));
            AddState(new JumpLaserLand(gameObject));
            AddState(new StompAntic(gameObject));
            AddState(new StompJump(gameObject));
            AddState(new StompStompAntic(gameObject));
            AddState(new StompFall(gameObject));
            AddState(new StompLand(gameObject));
            AddState(new StompSlash(gameObject));
            AddState(new StompTransform(gameObject));
            AddState(new StompShoot(gameObject));
            AddState(new StompBackSlash1(gameObject));
            AddState(new StompBackSlash2(gameObject));
            AddState(new StompBackSlash3(gameObject));
            AddState(new StompBackSlash4(gameObject));
            AddState(new StompBackSlash5(gameObject));
            AddState(new ShadowLaserDash(gameObject));
            AddState(new ShadowLaserLaser(gameObject));
            AddState(new ShadowLaserIdle(gameObject));
            AddState(new CycloneSlashJump(gameObject));
            AddState(new CycloneSlashStart(gameObject));
            AddState(new CycloneSlashFall(gameObject));
            AddState(new CycloneSlashIdle(gameObject));
            AddState(new ExDashSlashAntic(gameObject));
            AddState(new ExDashSlashJump(gameObject));
            AddState(new ExDashSlashRoar(gameObject));
            AddState(new ExDashSlashJumpAgain(gameObject));
            AddState(new ExDashSlashAttack(gameObject));
            AddState(new ExDashSlashFall(gameObject));
            AddState(new ShadowDashDash(gameObject));
            AddState(new ShadowDashIdle(gameObject));
            SetParent(gameObject.LocateMyFSM("Control"), "Control");
        }
    }
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)>
        {
            ("GG_Soul_Master", "Mage Lord"),
            ("GG_Sly", "Battle Scene"),
            ("GG_Radiance", "Boss Control"),
            ("GG_Workshop", "GG_Statue_Grimm"),
        };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        var mageLord = preloadedObjects["GG_Soul_Master"]["Mage Lord"];
        shockwaveTemplate = mageLord.LocateMyFSM("Mage Lord").GetAction<SpawnObjectFromGlobalPool>("Quake Waves", 0).gameObject.Value;
        blockHitboxTemplate = preloadedObjects["GG_Sly"]["Battle Scene"].transform.Find("Sly Boss").Find("S4").gameObject;
        airDashSlashHitboxTemplate = preloadedObjects["GG_Sly"]["Battle Scene"].transform.Find("Sly Boss").Find("S3").gameObject;
        airDashSlashTexture = Helper.LoadTexture("Nailgod.Resources.Textures.Dash Slash.png");
        var radiance = preloadedObjects["GG_Radiance"]["Boss Control"].transform.Find("Absolute Radiance").gameObject;
        var burst = radiance.transform.Find("Eye Beam Glow").gameObject.transform.Find("Burst 1").gameObject;
        beamPrefab = burst.transform.Find("Radiant Beam").gameObject;
        radiancePrefab = radiance;
        var ggStatueGrimm = preloadedObjects["GG_Workshop"]["GG_Statue_Grimm"];
        var Base = ggStatueGrimm.transform.Find("Base").gameObject;
        var Plaque = Base.transform.Find("Plaque").gameObject;
        var Plaque_Trophy_Right = Plaque.transform.Find("Plaque_Trophy_Right").gameObject;
        var Defeated_3 = Plaque_Trophy_Right.transform.Find("Defeated_3").gameObject;
        glowPrefab = Defeated_3;
        var sly = preloadedObjects["GG_Sly"]["Battle Scene"].transform.Find("Sly Boss").gameObject;
        var cycloneTink = sly.transform.Find("S1").gameObject;
        cycloneTinkPrefab = cycloneTink;
    }
    void saveSharp()
    {
        var sharpFlash = SlyBoss().transform.Find("Sharp Flash").gameObject;
        var tk2dSprite = sharpFlash.GetComponent<tk2dSprite>();
        airDashSlashTextureOld = tk2dSprite.CurrentSprite.material.mainTexture as Texture2D;
    }
    void setSharp()
    {
        var sharpFlash = SlyBoss().transform.Find("Sharp Flash").gameObject;
        var tk2dSprite = sharpFlash.GetComponent<tk2dSprite>();
        tk2dSprite.CurrentSprite.material.mainTexture = airDashSlashTexture;
        sharpFlash.GetComponent<tk2dSpriteAnimator>().DefaultClip.fps = 15;
        var sdsharp = HeroController.instance.transform.Find("Effects").transform.Find("SD Sharp Flash").gameObject;
        sdsharp.GetComponent<MeshRenderer>().enabled = false;
    }
    void resetSharp()
    {
        var sharpFlash = HeroController.instance.transform.Find("Effects").transform.Find("SD Sharp Flash").gameObject;
        var tk2dSprite = sharpFlash.GetComponent<tk2dSprite>();
        if (airDashSlashTextureOld != null)
            tk2dSprite.CurrentSprite.material.mainTexture = airDashSlashTextureOld;
        var sdsharp = HeroController.instance.transform.Find("Effects").transform.Find("SD Sharp Flash").gameObject;
        sdsharp.GetComponent<MeshRenderer>().enabled = true;
    }
    public override void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        if (InArena())
        {
            saveSharp();
            setSharp();
            SlyBoss().GetComponent<HealthManager>().hp = 3000;
            SlyBoss().RefreshHPBar();
            shadowdashParticlesPrefab = UnityEngine.Object.Instantiate(HeroController.instance.shadowdashParticlesPrefab, SlyBoss().transform);
            shadowdashParticlesPrefab.transform.localPosition = new Vector3(0, 0, 0.0001f);
            var fsmControl = SlyBoss().LocateMyFSM("Control");
            fsmControl.AddState("Control");
            fsmControl.AddCustomAction("Battle Start", () =>
            {
                LogWarn("HP before: " + SlyBoss().GetComponent<HealthManager>().hp.ToString());
                SlyBoss().GetComponent<HealthManager>().hp = 3000;
                SlyBoss().RefreshHPBar();
                LogWarn("HP after: " + SlyBoss().GetComponent<HealthManager>().hp.ToString());
            });
            fsmControl.ChangeTransition("Idle", "FINISHED", "Control");
            fsmControl.RemoveTransition("Idle", "TOOK DAMAGE");
            fsmControl.RemoveTransition("Idle", "NAIL ART");
            var resetCustomObjects = () =>
            {
                SlyBoss().transform.Find("NA Charge").gameObject.SetActive(false);
                SlyBoss().transform.Find("NA Charged").gameObject.SetActive(false);
                SlyBoss().transform.Find("GSlash Effect").gameObject.SetActive(false);
                SlyBoss().transform.Find("cycloneTink").gameObject.SetActive(false);
                SlyBoss().transform.Find("cycloneTink2").gameObject.SetActive(false);
                SlyBoss().transform.Find("cycloneEffect").gameObject.SetActive(false);
                SlyBoss().transform.Find("halo").gameObject.SetActive(false);
                SlyBoss().transform.rotation = Quaternion.identity;
                LogWarn("Reset custom objects and trans.");
            };
            fsmControl.AddCustomAction("Stun Reset", resetCustomObjects);
            fsmControl.AddCustomAction("Death Reset", resetCustomObjects);
            fsmControl.ChangeTransition("Death Type", "RAGE", "Explosion");
            SlyBoss().AddComponent<ControlStateMachine>();
            var fsmStunControl = SlyBoss().LocateMyFSM("Stun Control");
            fsmStunControl.FsmVariables.GetFsmInt("Stun Combo").Value = 22;
            fsmStunControl.FsmVariables.GetFsmInt("Stun Hit Max").Value = 28;
            var blockHitboxLeft = UnityEngine.Object.Instantiate(blockHitboxTemplate, SlyBoss().transform);
            blockHitboxLeft.SetActive(false);
            blockHitboxLeft.name = "Block Hitbox Left";
            blockHitboxLeft.transform.localPosition = new Vector3(-1.57f, 3.7f, 0);
            blockHitboxLeft.transform.rotation = Quaternion.Euler(0, 0, 283);
            blockHitboxLeft.transform.localScale = new Vector3(1.05f, 0.15f, 1);
            var blockHitboxLeftFSM = blockHitboxLeft.GetComponent<PlayMakerFSM>();
            blockHitboxLeftFSM.RemoveAction("Blocked Hit", 0);
            blockHitboxLeftFSM.RemoveAction("Blocked Hit", 0);
            var blockHitboxRight = UnityEngine.Object.Instantiate(blockHitboxTemplate, SlyBoss().transform);
            blockHitboxRight.SetActive(false);
            blockHitboxRight.name = "Block Hitbox Right";
            blockHitboxRight.transform.localPosition = new Vector3(-1.73f, 3.65f, 0);
            blockHitboxRight.transform.rotation = Quaternion.Euler(0, 0, 102.8f);
            blockHitboxRight.transform.localScale = new Vector3(-1.05f, 0.15f, 1);
            var blockHitboxRightFSM = blockHitboxRight.GetComponent<PlayMakerFSM>();
            blockHitboxRightFSM.RemoveAction("Blocked Hit", 0);
            blockHitboxRightFSM.RemoveAction("Blocked Hit", 0);
            blockHitboxLeftFSM.InsertCustomAction("Blocked Hit", () =>
            {
                if (blockHitboxRightFSM.ActiveStateName != "Detecting")
                {
                    blockHitboxLeftFSM.SetState("Detecting");
                }
            }, 0);
            blockHitboxRightFSM.InsertCustomAction("Blocked Hit", () =>
            {
                if (blockHitboxLeftFSM.ActiveStateName != "Detecting")
                {
                    blockHitboxRightFSM.SetState("Detecting");
                }
            }, 0);
            var airDashSlashHitbox = UnityEngine.Object.Instantiate(airDashSlashHitboxTemplate, SlyBoss().transform);
            airDashSlashHitbox.name = "Air Dash Slash Hitbox";
            airDashSlashHitbox.SetActive(false);
            airDashSlashHitbox.transform.localPosition = new Vector3(-5.9f, -0.35f, 0);
            airDashSlashHitbox.transform.rotation = Quaternion.Euler(0, 0, 1);
            airDashSlashHitbox.transform.localScale = new Vector3(-0.8f, 0.1f, 1);
            beams.Clear();
            for (int i = 0; i < 7; i++)
            {
                var beam = beamPrefab;
                beam = UnityEngine.Object.Instantiate(beam, SlyBoss().transform);
                beam.SetActive(true);
                beam.SetActiveChildren(true);
                beam.transform.localPosition = new Vector3(-3.4f, 0.37f, 0);
                beam.transform.rotation = Quaternion.Euler(0, 0, 180 - i * 20 + 3 * 20);
                beam.LocateMyFSM("Control").AddTransition("Fire", "FINISHED", "End");
                var radiance = radiancePrefab;
                var action = radiance.LocateMyFSM("Attack Commands").GetAction<AudioPlayerOneShotSingle>("Aim", 3);
                action.spawnPoint = SlyBoss();
                action.delay = 0;
                beam.LocateMyFSM("Control").AddAction("Fire", action);
                if (i == 0)
                {
                    var glow = glowPrefab;
                    glow = UnityEngine.Object.Instantiate(glow, SlyBoss().transform);
                    glow.name = "glow";
                    glow.transform.localScale *= 1.5f;
                    glow.SetActive(false);
                    glow.transform.localPosition = new Vector3(-3.35f, 0.45f, -0.05f);
                    glow.GetAddComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.75f);
                    var halo = radiance.transform.Find("Halo").gameObject;
                    halo = UnityEngine.Object.Instantiate(halo, SlyBoss().transform);
                    halo.name = "halo";
                    halo.transform.localScale *= 0.2f;
                    halo.SetActive(false);
                    halo.transform.localPosition = new Vector3(-0.05f, 0.2f, 0.002f);
                }
                beams.Add(beam);
            }
            {
                var beam = beamPrefab;
                beam = UnityEngine.Object.Instantiate(beam, SlyBoss().transform);
                beam.SetActive(true);
                beam.SetActiveChildren(true);
                beam.transform.localPosition = new Vector3(-3.4f, 0.37f, 0);
                beam.transform.rotation = Quaternion.Euler(0, 0, 174.5f);
                beam.LocateMyFSM("Control").AddTransition("Fire", "FINISHED", "End");
                var radiance = radiancePrefab;
                var action = radiance.LocateMyFSM("Attack Commands").GetAction<AudioPlayerOneShotSingle>("Aim", 3);
                action.spawnPoint = SlyBoss();
                action.delay = 0;
                beam.LocateMyFSM("Control").AddAction("Fire", action);
                beam.name = "Special Beam";
                specialLaser = beam;
                var glow = glowPrefab;
                glow = UnityEngine.Object.Instantiate(glow, SlyBoss().transform);
                glow.name = "sglow";
                glow.transform.localScale *= 1.5f;
                glow.SetActive(false);
                glow.transform.localPosition = new Vector3(-3.35f, 0.45f, -0.05f);
                glow.GetAddComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.75f);
            }
            var sly = SlyBoss();
            var cycloneTink = UnityEngine.Object.Instantiate(cycloneTinkPrefab as GameObject, sly.transform);
            cycloneTink.name = "cycloneTink";
            cycloneTink.SetActive(false);
            cycloneTink.transform.localPosition = new Vector3(-8, -0.8f, 0);
            cycloneTink.transform.localScale = new Vector3(3, 1.15f, 1);
            cycloneTink.transform.localRotation = Quaternion.Euler(0, 0, 335);
            cycloneTink = UnityEngine.Object.Instantiate(cycloneTinkPrefab as GameObject, sly.transform);
            cycloneTink.name = "cycloneTink2";
            cycloneTink.SetActive(false);
            cycloneTink.transform.localPosition = new Vector3(8, -0.8f, 0);
            cycloneTink.transform.localScale = new Vector3(-3, 1.15f, 1);
            cycloneTink.transform.localRotation = Quaternion.Euler(0, 0, 25);
            var cycloneEffect = HeroController.instance.gameObject.transform.Find("Attacks").gameObject.transform.Find("Cyclone Slash").gameObject;
            cycloneEffect = UnityEngine.Object.Instantiate(cycloneEffect as GameObject, sly.transform);
            cycloneEffect.name = "cycloneEffect";
            cycloneEffect.SetActive(false);
            cycloneEffect.transform.localPosition = new Vector3(0, 0, -0.0013f);
            cycloneEffect.transform.localScale = new Vector3(2.5f, 3, 1.3863f);
            var hits = cycloneEffect.transform.Find("Hits").gameObject;
            hits.transform.Find("Hit L").gameObject.RemoveComponent<PolygonCollider2D>();
            hits.transform.Find("Hit R").gameObject.RemoveComponent<PolygonCollider2D>();
        }
        else
        {
            LogWarn(to.name);
            if (to.name != "Menu_Title")
                resetSharp();
        }
    }
}
