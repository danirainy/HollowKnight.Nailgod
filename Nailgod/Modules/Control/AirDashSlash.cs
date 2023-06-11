namespace Nailgod;
public partial class Control : Module
{
    private GameObject airDashSlashHitboxTemplate;
    public static Texture2D airDashSlashTexture;
    public static Texture2D airDashSlashTextureOld = null;
    private static float airDashSlashLastDir;
    private partial class ControlStateMachine : StateMachine
    {
        private class AirDashSlashAntic : State
        {
            public AirDashSlashAntic(GameObject gameObject) : base(gameObject)
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
                return "AirDashSlashJump";
            }
        }
        private class AirDashSlashJump : State
        {
            private float timer;
            private bool wait;
            public AirDashSlashJump(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 0));
                AddAction(fsm.GetAction("Jump", 1));
                AddAction(fsm.GetAction("Jump", 2));
                AddAction(fsm.GetAction("Jump", 3));
                AddAction(fsm.GetAction("Dash Charge", 2));
                AddAction(fsm.GetAction("Dash Charge", 3));
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var from = gameObject.transform.position;
                var to = HeroController.instance.transform.position;
                to.x = Mathf.Max(to.x, 37.7f);
                to.x = Mathf.Min(to.x, 55.2f);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(2f * (to.x - from.x), 75);
                rigidBody2D.gravityScale = 0.5f;
                wait = false;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                if (wait)
                {
                    timer += Time.deltaTime;
                    return timer < 0.5;
                }
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                if (rigidBody2D.velocity.y <= 0)
                {
                    timer = 0;
                    wait = true;
                    rigidBody2D.velocity = Vector2.zero;
                    rigidBody2D.gravityScale = 0;
                    gameObject.transform.SetPositionX(0);
                    gameObject.transform.SetPositionY(0);
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = 3;
                float dis = 10;
                float x;
                float y;
                while (true)
                {
                    var angle = (float)random.NextDouble() * Mathf.PI / 2 + Mathf.PI / 4;
                    x = dis * Mathf.Cos(angle);
                    y = dis * Mathf.Sin(angle);
                    airDashSlashLastDir = Mathf.Sign(x);
                    x += HeroController.instance.transform.position.x;
                    y += HeroController.instance.transform.position.y;
                    if (x > 33.7 && x < 59.2)
                    {
                        break;
                    }
                }
                if (y > 16) y = 16;
                gameObject.transform.SetPositionX(x);
                gameObject.transform.SetPositionY(y);
                return "AirDashSlashAim1";
            }
        }
        private class AirDashSlashAim1 : State
        {
            float originalGravityScale;
            public AirDashSlashAim1(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.4f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dash Charged", 0));
                AddAction(fsm.GetAction("Dash Charged", 1));
                AddAction(fsm.GetAction("Dash Charged", 2));
                AddAction(fsm.GetAction("Dash Charged", 3));
                AddAction(fsm.GetAction("Dash Forward", 0));
                AddAction(fsm.GetAction("GSlash 1", 2));
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                rigidBody2D.velocity = Vector2.zero;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Helper.AngleToPlayer(gameObject, 0, 90, 0));
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "AirDashSlashSlash1";
            }
        }
        private class AirDashSlashSlash1 : State
        {
            float originalGravityScale;
            public AirDashSlashSlash1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dash Forward", 1));
                AddAction(fsm.GetAction("Dash Forward", 3));
                AddAction(fsm.GetAction("Dash Forward", 4));
                AddAction(fsm.GetAction("Dashing L", 2));
                AddAction(fsm.GetAction("D Slash S1", 1));
                AddAction(fsm.GetAction("D Slash S1", 4));
                AddAction(fsm.GetAction("D Slash S1", 7));
                AddAction(fsm.GetAction("DSlash S2", 1));
                ExitAfterSeconds(0.5f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Helper.AngleToPlayer(gameObject, 0, 90, 0));
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                var angle = gameObject.transform.rotation.eulerAngles.z / 180 * Mathf.PI;
                if (gameObject.transform.localScale.x < 0)
                {
                    angle += Mathf.PI;
                }
                var velocity = -70;
                rigidBody2D.velocity = new Vector2(Mathf.Cos(angle) * velocity, Mathf.Sin(angle) * velocity);
                var scale = gameObject.transform.localScale;
                gameObject.transform.localScale = scale;
                gameObject.transform.Find("Air Dash Slash Hitbox").gameObject.SetActive(true);
                gameObject.transform.Find("Sharp Flash").gameObject.SetActive(true);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                gameObject.transform.rotation = Quaternion.identity;
                var DSlash_Effect = gameObject.transform.Find("DSlash Effect").gameObject;
                DSlash_Effect.SetActive(false);
                var scale = gameObject.transform.localScale;
                scale.y = 1;
                gameObject.transform.localScale = scale;
                gameObject.transform.Find("Air Dash Slash Hitbox").gameObject.SetActive(false);
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                return "AirDashSlashAim2";
            }
        }
        private class AirDashSlashAim2 : State
        {
            float originalGravityScale;
            public AirDashSlashAim2(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.4f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dash Charged", 0));
                AddAction(fsm.GetAction("Dash Charged", 1));
                AddAction(fsm.GetAction("Dash Charged", 2));
                AddAction(fsm.GetAction("Dash Charged", 3));
                AddAction(fsm.GetAction("Dash Forward", 0));
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                rigidBody2D.velocity = Vector2.zero;
                float dis = 10;
                float x = 0;
                float y = 0;
                bool found = false;
                for (int i = 0; i < 16; ++i)
                {
                    var angle = (float)random.NextDouble() * Mathf.PI / 2 + Mathf.PI / 4;
                    x = dis * Mathf.Cos(angle);
                    y = dis * Mathf.Sin(angle);
                    float thisdir = Mathf.Sign(x);
                    x += HeroController.instance.transform.position.x;
                    y += HeroController.instance.transform.position.y;
                    if (x > 33.7 && x < 59.2 && thisdir * airDashSlashLastDir < 0)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    while (true)
                    {
                        var angle = (float)random.NextDouble() * Mathf.PI / 2 + Mathf.PI / 4;
                        x = dis * Mathf.Cos(angle);
                        y = dis * Mathf.Sin(angle);
                        x += HeroController.instance.transform.position.x;
                        y += HeroController.instance.transform.position.y;
                        if (x > 33.7 && x < 59.2)
                        {
                            break;
                        }
                    }
                }
                if (y > 16) y = 16;
                gameObject.transform.SetPositionX(x);
                gameObject.transform.SetPositionY(y);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Helper.AngleToPlayer(gameObject, 0, 90, 0));
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "AirDashSlashSlash2";
            }
        }
        private class AirDashSlashSlash2 : State
        {
            float originalGravityScale;
            public AirDashSlashSlash2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dash Forward", 1));
                AddAction(fsm.GetAction("Dash Forward", 3));
                AddAction(fsm.GetAction("Dash Forward", 4));
                AddAction(fsm.GetAction("Dashing L", 2));
                AddAction(fsm.GetAction("D Slash S1", 1));
                AddAction(fsm.GetAction("D Slash S1", 4));
                AddAction(fsm.GetAction("D Slash S1", 5));
                AddAction(fsm.GetAction("D Slash S1", 7));
                AddAction(fsm.GetAction("DSlash S2", 1));
                ExitAfterSeconds(0.5f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Helper.AngleToPlayer(gameObject, 0, 90, 0));
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                var angle = gameObject.transform.rotation.eulerAngles.z / 180 * Mathf.PI;
                if (gameObject.transform.localScale.x < 0)
                {
                    angle += Mathf.PI;
                }
                var velocity = -70;
                rigidBody2D.velocity = new Vector2(Mathf.Cos(angle) * velocity, Mathf.Sin(angle) * velocity);
                var scale = gameObject.transform.localScale;
                gameObject.transform.localScale = scale;
                gameObject.transform.Find("Air Dash Slash Hitbox").gameObject.SetActive(true);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                gameObject.transform.rotation = Quaternion.identity;
                var DSlash_Effect = gameObject.transform.Find("DSlash Effect").gameObject;
                DSlash_Effect.SetActive(false);
                var scale = gameObject.transform.localScale;
                scale.y = 1;
                gameObject.transform.localScale = scale;
                gameObject.transform.Find("Air Dash Slash Hitbox").gameObject.SetActive(false);
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                return "AirDashSlashAim3";
            }
        }
        private class AirDashSlashAim3 : State
        {
            float originalGravityScale;
            public AirDashSlashAim3(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.4f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dash Charged", 0));
                AddAction(fsm.GetAction("Dash Charged", 1));
                AddAction(fsm.GetAction("Dash Charged", 2));
                AddAction(fsm.GetAction("Dash Charged", 3));
                AddAction(fsm.GetAction("Dash Forward", 0));
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                rigidBody2D.velocity = Vector2.zero;
                float dis = 10;
                float x;
                float y;
                while (true)
                {
                    float angle;
                    if (random.NextDouble() < 0.5)
                    {
                        angle = 0;
                    }
                    else
                    {
                        angle = Mathf.PI;
                    }
                    x = dis * Mathf.Cos(angle);
                    y = dis * Mathf.Sin(angle);
                    x += HeroController.instance.transform.position.x;
                    y += 5.5f;
                    if (x > 33.7 && x < 59.2)
                    {
                        break;
                    }
                }
                gameObject.transform.SetPositionX(x);
                gameObject.transform.SetPositionY(y);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "AirDashSlashSlash3";
            }
        }
        private class AirDashSlashSlash3 : State
        {
            float originalGravityScale;
            public AirDashSlashSlash3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dash Forward", 1));
                AddAction(fsm.GetAction("Dash Forward", 3));
                AddAction(fsm.GetAction("Dash Forward", 4));
                AddAction(fsm.GetAction("Dashing L", 2));
                AddAction(fsm.GetAction("D Slash S1", 1));
                AddAction(fsm.GetAction("D Slash S1", 4));
                AddAction(fsm.GetAction("D Slash S1", 5));
                AddAction(fsm.GetAction("D Slash S1", 7));
                AddAction(fsm.GetAction("DSlash S2", 1));
                ExitAfterSeconds(0.5f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                var velocity = -70;
                if (gameObject.transform.localScale.x < 0)
                {
                    velocity *= -1;
                }
                rigidBody2D.velocity = new Vector2(velocity, 0);
                var scale = gameObject.transform.localScale;
                gameObject.transform.localScale = scale;
                gameObject.transform.Find("Air Dash Slash Hitbox").gameObject.SetActive(true);
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var pos = gameObject.transform.position;
                return pos.x > 31.7 && pos.x < 61.2;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                gameObject.transform.rotation = Quaternion.identity;
                var DSlash_Effect = gameObject.transform.Find("DSlash Effect").gameObject;
                DSlash_Effect.SetActive(false);
                var scale = gameObject.transform.localScale;
                scale.y = 1;
                gameObject.transform.localScale = scale;
                gameObject.transform.Find("Air Dash Slash Hitbox").gameObject.SetActive(false);
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                return "AirDashSlashEnd";
            }
        }
        private class AirDashSlashEnd : State
        {
            public AirDashSlashEnd(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                var actions = fsm.GetState("GSlash 4").Actions;
                for (int i = 0; i < actions.Length; ++i)
                {
                    AddAction(actions[i]);
                }
                ExitAfterSeconds(0.5f);
                AddAction(fsm.GetAction("Land", 0));
            }
            public override void OnEnter(GameObject gameObject)
            {
                var S4 = gameObject.transform.Find("S4").gameObject;
                S4.SetActive(false);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var x1 = gameObject.transform.position.x;
                var x2 = HeroController.instance.transform.position.x;
                if (Mathf.Abs(x1 - x2) < 5)
                {
                    var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity *= 0.2f;
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "Move";
            }
        }
    }
}
