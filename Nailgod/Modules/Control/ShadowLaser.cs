namespace Nailgod;
public partial class Control : Module
{
    public static GameObject shadowdashParticlesPrefab;
    public static List<GameObject> beams = new List<GameObject>();
    public static GameObject beamPrefab;
    public static GameObject radiancePrefab;
    public static GameObject glowPrefab;
    private partial class ControlStateMachine : StateMachine
    {
        private class ShadowLaserDash : State
        {
            int lastSign;
            public ShadowLaserDash(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S1", 1));
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                lastSign = gameObject.transform.position.x < HeroController.instance.transform.position.x ? 1 : -1;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(lastSign * 60, 0);
                var d = gameObject.GetComponent<BoxCollider2D>();
                d.enabled = false;
                rigidBody2D.gravityScale = 0;
                if (gameObject.transform.localScale.x > 0f)
                {
                    var dashEffect = HeroController.instance.shadowdashBurstPrefab.Spawn(new Vector3(gameObject.transform.position.x + 5.21f, gameObject.transform.position.y - 0.58f, gameObject.transform.position.z + 0.00101f));
                    dashEffect.transform.localScale = new Vector3(1.919591f, dashEffect.transform.localScale.y, dashEffect.transform.localScale.z);
                }
                else
                {
                    var dashEffect = HeroController.instance.shadowdashBurstPrefab.Spawn(new Vector3(gameObject.transform.position.x - 5.21f, gameObject.transform.position.y - 0.58f, gameObject.transform.position.z + 0.00101f));
                    dashEffect.transform.localScale = new Vector3(-1.919591f, dashEffect.transform.localScale.y, dashEffect.transform.localScale.z);
                }
                shadowdashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = true;
                VibrationManager.PlayVibrationClipOneShot(HeroController.instance.shadowDashVibration, null, false, "");
                HeroController.instance.shadowRingPrefab.Spawn(gameObject.transform.position);
                ReflectionHelper.GetField<HeroController, HeroAudioController>(HeroController.instance, "audioCtrl").PlaySound(GlobalEnums.HeroSounds.DASH);
                ReflectionHelper.GetField<HeroController, AudioSource>(HeroController.instance, "audioSource").PlayOneShot(HeroController.instance.sharpShadowClip, 1f);
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play(new List<(string, float)> {
                    ("14.Shadow", 0.04f),
                }, Animation.Animator.Mode.Repeat);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var curSign = gameObject.transform.position.x < HeroController.instance.transform.position.x ? 1 : -1;
                var x = gameObject.transform.position.x;
                if (x < 30.7 || x > 62.2)
                {
                    return false;
                }
                if (curSign * lastSign < 0 && Mathf.Abs(gameObject.transform.position.x - HeroController.instance.transform.position.x) > 7.5)
                {
                    return false;
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var d = gameObject.GetComponent<BoxCollider2D>();
                d.enabled = true;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = 3;
                rigidBody2D.velocity = Vector2.zero;
                shadowdashParticlesPrefab.GetComponent<ParticleSystem>().enableEmission = false;
                if (interrputed)
                {
                    var animator = gameObject.GetComponent<Animation.Animator>();
                    animator.Disable();
                }
                return "ShadowLaserLaser";
            }
        }
        private class ShadowLaserLaser : State
        {
            float timer;
            bool fired;
            public ShadowLaserLaser(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play(new List<(string, float)> {
                    ("15.Shoot", 0.1f),
                }, Animation.Animator.Mode.Single);
                timer = 0;
                foreach (var b in beams)
                {
                    b.SetActive(true);
                    b.LocateMyFSM("Control").SetState("Antic");
                }
                fired = false;
                gameObject.transform.Find("halo").gameObject.SetActive(true);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer += Time.deltaTime;
                if (timer > 0.6 && !fired)
                {
                    foreach (var b in beams)
                    {
                        b.LocateMyFSM("Control").SetState("Fire");
                    }
                    fired = true;
                    gameObject.transform.Find("glow").gameObject.SetActive(true);
                }
                return timer < 0.9;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                gameObject.transform.Find("glow").gameObject.SetActive(false);
                foreach (var b in beams)
                {
                    b.SetActive(false);
                }
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Disable();
                gameObject.transform.Find("halo").gameObject.SetActive(false);
                return "ShadowLaserIdle";
            }
        }
        private class ShadowLaserIdle : State
        {
            public ShadowLaserIdle(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Idle", 2));
                ExitAfterSeconds(0.3f);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "Move";
            }
        }
    }
}
