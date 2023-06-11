namespace Nailgod;
public partial class Control : Module
{
    public static GameObject shockwaveTemplate;
    private partial class ControlStateMachine : StateMachine
    {
        private class JumpShootJump : State
        {
            public JumpShootJump(GameObject gameObject) : base(gameObject)
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
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play(new List<(string, float)> {
                    ("02.Transform", 0.04f),
                    ("03.Transform", 0.04f),
                    ("04.Transform", 0.04f),
                }, Animation.Animator.Mode.Single);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                float vX;
                var idealDistance = 12;
                var myPos = gameObject.transform.position.x;
                var playerPos = HeroController.instance.transform.position.x;
                var left = playerPos - idealDistance;
                var right = playerPos + idealDistance;
                if (myPos < left)
                {
                    vX = left - myPos;
                }
                else if (myPos > right)
                {
                    vX = right - myPos;
                }
                else if (myPos - left < right - myPos)
                {
                    vX = left - myPos;
                }
                else
                {
                    vX = right - myPos;
                }
                vX *= 5;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(vX, 40);
                var pos = gameObject.transform.position;
                pos.y = Mathf.Min(pos.y, 8.408124f);
                gameObject.transform.position = pos;
                var animator = gameObject.GetComponent<Animation.Animator>();
                return animator.status == Animation.Animator.Status.Playing;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                if (interrputed)
                {
                    var animator = gameObject.GetComponent<Animation.Animator>();
                    animator.Disable();
                }
                return "JumpShootShoot";
            }
        }
        private class JumpShootShoot : State
        {
            float originalGravityScale;
            float currentAngle;
            float timer;
            int bType;
            int ta = -1;
            int tb = -1;
            public JumpShootShoot(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(1);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play("13.Shoot", 0.1f, Animation.Animator.Mode.Repeat);
                currentAngle = 0;
                timer = float.MaxValue;
                bType = random.NextDouble() < 0.5 ? 0 : 1;
                if (bType == ta && bType == tb)
                {
                    bType = 1 - bType;
                }
                ta = tb;
                tb = bType;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                var angle = Helper.AngleToPlayer(gameObject, -135, 135);
                if (currentAngle < angle)
                {
                    currentAngle += 120 * Time.deltaTime;
                }
                else
                {
                    currentAngle -= 120 * Time.deltaTime;
                }
                gameObject.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                timer += Time.deltaTime;
                if (timer > 0.075)
                {
                    timer = 0;
                    Bullet.Shoot(gameObject, bType, true);
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = originalGravityScale;
                gameObject.transform.rotation = Quaternion.identity;
                if (interrputed)
                {
                    var animator = gameObject.GetComponent<Animation.Animator>();
                    animator.Disable();
                }
                return "JumpShootFall";
            }
        }
        private class JumpShootFall : State
        {
            public JumpShootFall(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 1));
            }
            public override void OnEnter(GameObject gameObject)
            {
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play("05.Fall", 0.05f, Animation.Animator.Mode.Single);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Disable();
                return "JumpShootLand";
            }
        }
        private class JumpShootLand : State
        {
            public JumpShootLand(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dstab Land", 2));
                AddAction(fsm.GetAction("Dstab Land", 6));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return false;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "JumpShootS1";
            }
        }
        private class JumpShootS1 : State
        {
            public JumpShootS1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S1", 0));
                AddAction(fsm.GetAction("Slash S1", 1));
                AddAction(fsm.GetAction("Slash S1", 2));
                AddAction(fsm.GetAction("Slash S1", 6));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                var v = 50;
                if (gameObject.transform.localScale.x > 0)
                {
                    rigidBody2D.velocity = new Vector2(-v, 0);
                }
                else
                {
                    rigidBody2D.velocity = new Vector2(v, 0);
                }
                var x1 = gameObject.transform.position.x;
                var x2 = HeroController.instance.transform.position.x;
                if (Mathf.Abs(x1 - x2) < 1)
                {
                    rigidBody2D.velocity = Vector2.zero;
                    return false;
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "JumpShootS2";
            }
        }
        private class JumpShootS2 : State
        {
            public JumpShootS2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S2", 0));
                AddAction(fsm.GetAction("Slash S2", 1));
                AddAction(fsm.GetAction("Slash S2", 2));
                ExitAfterSeconds(0.05f);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "JumpShootS3";
            }
        }
        private class JumpShootS3 : State
        {
            public JumpShootS3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S3", 0));
                AddAction(fsm.GetAction("Slash S3", 1));
                ExitAfterSeconds(0.5f);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return "Move";
            }
        }
    }
}
