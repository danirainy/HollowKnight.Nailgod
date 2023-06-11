namespace Nailgod;
public partial class Control : Module
{
    private partial class ControlStateMachine : StateMachine
    {
        private class StompAntic : State
        {
            public StompAntic(GameObject gameObject) : base(gameObject)
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
                return "StompJump";
            }
        }
        private class StompJump : State
        {
            public StompJump(GameObject gameObject) : base(gameObject)
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
                rigidBody2D.velocity = new Vector2(2f * (to.x - from.x), 60);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                return rigidBody2D.velocity.y > 0;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompStompAntic";
            }
        }
        private class StompStompAntic : State
        {
            public StompStompAntic(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.1f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Stomp Antic", 3));
                AddAction(fsm.GetAction("Stomp Antic", 4));
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(0, 1);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompFall";
            }
        }
        private class StompFall : State
        {
            public StompFall(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Stomp", 0));
                AddAction(fsm.GetAction("Stomp", 1));
                AddAction(fsm.GetAction("Stomp", 2));
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(0, -10);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompLand";
            }
        }
        private class StompLand : State
        {
            public StompLand(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dstab Land", 0));
                AddAction(fsm.GetAction("Dstab Land", 2));
                AddAction(fsm.GetAction("Dstab Land", 3));
                AddAction(fsm.GetAction("Dstab Land", 4));
                AddAction(fsm.GetAction("Dstab Land", 5));
                AddAction(fsm.GetAction("Dstab Land", 6));
                ExitAfterSeconds(0.05f);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompSlash";
            }
        }
        private class StompSlash : State
        {
            public StompSlash(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Dstab Slash", 0));
                AddAction(fsm.GetAction("Dstab Slash", 3));
                ExitAfterSeconds(0.05f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                GameObject shockwave;
                if (gameObject.transform.localScale.x < 0)
                {
                    shockwave = Instantiate(shockwaveTemplate);
                    shockwave.transform.position = gameObject.transform.position;
                    shockwave.transform.Translate(0, -0.9f, 0);
                    shockwave.LocateMyFSM("shockwave").FsmVariables.GetFsmBool("Facing Right").Value = true;
                    shockwave.LocateMyFSM("shockwave").FsmVariables.GetFsmFloat("Speed").Value = 50;
                    shockwave.LocateMyFSM("shockwave").RemoveTransition("Move", "WALL");
                    shockwave.transform.SetScaleX(2);
                }
                if (gameObject.transform.localScale.x > 0)
                {
                    shockwave = Instantiate(shockwaveTemplate);
                    shockwave.transform.position = gameObject.transform.position;
                    shockwave.transform.Translate(0, -0.9f, 0);
                    shockwave.LocateMyFSM("shockwave").FsmVariables.GetFsmBool("Facing Right").Value = false;
                    shockwave.LocateMyFSM("shockwave").FsmVariables.GetFsmFloat("Speed").Value = 50;
                    shockwave.LocateMyFSM("shockwave").RemoveTransition("Move", "WALL");
                    shockwave.transform.SetScaleX(2);
                }
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompTransform";
            }
        }
        private class StompTransform : State
        {
            public StompTransform(GameObject gameObject) : base(gameObject)
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
                var animator = gameObject.GetComponent<Animation.Animator>();
                return animator.status == Animation.Animator.Status.Playing;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (interrputed)
                {
                    var animator = gameObject.GetComponent<Animation.Animator>();
                    animator.Disable();
                }
                return "StompShoot";
            }
        }
        private class StompShoot : State
        {
            float timer;
            bool close;
            int btype;
            public StompShoot(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.5f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play("08.Shoot", 0.1f, Animation.Animator.Mode.Repeat);
                timer = float.MaxValue;
                close = false;
                btype = random.NextDouble() < 0.5 ? 0 : 1;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer += Time.deltaTime;
                if (timer > 0.075)
                {
                    timer = 0;
                    Bullet.Shoot(gameObject, btype);
                }
                var from = gameObject.transform.position;
                var to = HeroController.instance.transform.position;
                if (Mathf.Abs(from.x - to.x) < 15 && Mathf.Abs(from.y - to.y) < 7.5)
                {
                    close = true;
                }
                return !close || timer < 0.05;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Disable();
                return close ? "StompBackSlash1" : "Move";
            }
        }
        private class StompBackSlash1 : State
        {
            public StompBackSlash1(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.2f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash Antic 2", 0));
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompBackSlash2";
            }
        }
        private class StompBackSlash2 : State
        {
            public StompBackSlash2(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.05f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash Dash 2", 0));
                AddAction(fsm.GetAction("BSlash Dash 2", 1));
                AddAction(fsm.GetAction("BSlash Dash 2", 2));
                AddAction(fsm.GetAction("BSlash Dash 2", 3));
                AddAction(fsm.GetAction("BSlash Dash 2", 4));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var from = gameObject.transform.position;
                var to = HeroController.instance.transform.position;
                if (Mathf.Abs(from.x - to.x) < 5)
                {
                    var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity = Vector2.zero;
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompBackSlash3";
            }
        }
        private class StompBackSlash3 : State
        {
            public StompBackSlash3(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.05f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S1 2", 0));
                AddAction(fsm.GetAction("BSlash S1 2", 1));
                AddAction(fsm.GetAction("BSlash S1 2", 2));
                AddAction(fsm.GetAction("Slash S1", 1));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var from = gameObject.transform.position;
                var to = HeroController.instance.transform.position;
                if (Mathf.Abs(from.x - to.x) < 5)
                {
                    var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity = Vector2.zero;
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompBackSlash4";
            }
        }
        private class StompBackSlash4 : State
        {
            public StompBackSlash4(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.1f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S2 2", 0));
                AddAction(fsm.GetAction("BSlash S2 2", 1));
                AddAction(fsm.GetAction("BSlash S2 2", 2));
                AddAction(fsm.GetAction("BSlash S2 2", 3));
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "StompBackSlash5";
            }
        }
        private class StompBackSlash5 : State
        {
            public StompBackSlash5(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.2f);
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S3 2", 0));
                AddAction(fsm.GetAction("BSlash S3 2", 1));
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "Move";
            }
        }
    }
}
