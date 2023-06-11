namespace Nailgod;
public partial class Control : Module
{
    public static GameObject cycloneTinkPrefab;
    private partial class ControlStateMachine : StateMachine
    {
        private class CycloneSlashAntic : State
        {
            public CycloneSlashAntic(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump Antic", 0));
                AddAction(fsm.GetAction("Jump Antic", 1));
                AddAction(fsm.GetAction("Jump Antic", 4));
                AddAction(fsm.GetAction("Jump Antic", 5));
                ExitAfterSeconds(0.1f);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "CycloneSlashJump";
            }
        }
        private class CycloneSlashJump : State
        {
            float sign;
            public CycloneSlashJump(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 0));
                AddAction(fsm.GetAction("Jump", 1));
                AddAction(fsm.GetAction("Jump", 2));
                AddAction(fsm.GetAction("Jump", 3));
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var from = gameObject.transform.position;
                var to = HeroController.instance.transform.position;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(4f * (to.x - from.x), 60);
                gameObject.transform.Find("NA Charge").gameObject.SetActive(true);
                sign = Mathf.Sign(gameObject.transform.position.x - HeroController.instance.transform.position.x);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                LogWarn(gameObject.transform.position.x.ToString() + " " + HeroController.instance.transform.position.x.ToString());
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                return rigidBody2D.velocity.y > 0 && Mathf.Sign(gameObject.transform.position.x - HeroController.instance.transform.position.x) * sign > 0;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                gameObject.transform.Find("NA Charge").gameObject.SetActive(false);
                return "CycloneSlashStart";
            }
        }
        private class CycloneSlashStart : State
        {
            public CycloneSlashStart(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.25f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Cyclone Start", 2));
                AddAction(fsm.GetAction("Cyclone Start", 9));
                AddAction(fsm.GetAction("Cyclone Start", 11));
                AddAction(fsm.GetAction("Cyclone Start", 12));
                AddAction(fsm.GetAction("Cyc Charged", 2));
                AddAction(fsm.GetAction("Cyc Charged", 4));
            }
            public override void OnEnter(GameObject gameObject)
            {
                gameObject.transform.Find("NA Charged").gameObject.SetActive(true);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                gameObject.transform.Find("cycloneTink").gameObject.SetActive(true);
                gameObject.transform.Find("cycloneTink2").gameObject.SetActive(true);
                gameObject.transform.Find("cycloneEffect").gameObject.SetActive(true);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {

                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                var v = rigidBody2D.velocity;
                v.x = 0;
                v.y = 0;
                rigidBody2D.velocity = v;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                gameObject.transform.Find("NA Charged").gameObject.SetActive(false);
                return "CycloneSlashFall";
            }
        }
        private class CycloneSlashFall : State
        {
            public CycloneSlashFall(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                var v = rigidBody2D.velocity;
                v.x = 0;
                rigidBody2D.velocity = v;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "CycloneSlashIdle";
            }
        }
        private class CycloneSlashIdle : State
        {
            public CycloneSlashIdle(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Idle", 2));
                AddAction(fsm.GetAction("Cyclone End", 1));
                ExitAfterSeconds(0.5f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var fsm = gameObject.LocateMyFSM("Control");
                var a = fsm.GetAction<AudioPlayerOneShotSingle>("Cyclone Start", 11);
                if (a.storePlayer.Value != null)
                {
                    var b = a.storePlayer.Value;
                    var audioSrc=b.GetComponent<AudioSource>();
                    if (audioSrc != null)
                    {
                        audioSrc.Stop();
                        LogWarn("Stopped audio.");
                    }
                }
                gameObject.transform.Find("cycloneTink").gameObject.SetActive(false);
                gameObject.transform.Find("cycloneTink2").gameObject.SetActive(false);
                gameObject.transform.Find("cycloneEffect").gameObject.SetActive(false);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "Move";
            }
        }
    }
}
