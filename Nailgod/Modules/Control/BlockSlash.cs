namespace Nailgod;
public partial class Control : Module
{
    private GameObject blockHitboxTemplate;
    private partial class ControlStateMachine : StateMachine
    {
        private class BlockSlashBlock : State
        {
            public BlockSlashBlock(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("GSlash Charge", 2));
                AddAction(fsm.GetAction("GSlash Charge", 3));
                AddAction(fsm.GetAction("GSlash 1", 2));
                ExitAfterSeconds(0.25f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                gameObject.transform.Find("Block Hitbox Left").gameObject.SetActive(true);
                gameObject.transform.Find("Block Hitbox Right").gameObject.SetActive(true);
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play("11.Block", 0, Animation.Animator.Mode.Single);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var blockHitboxLeft = gameObject.transform.Find("Block Hitbox Left").gameObject;
                var blockHitboxLeftFSM = blockHitboxLeft.GetComponent<PlayMakerFSM>();
                var blockHitboxRight = gameObject.transform.Find("Block Hitbox Right").gameObject;
                var blockHitboxRightFSM = blockHitboxRight.GetComponent<PlayMakerFSM>();
                return blockHitboxLeftFSM.ActiveStateName == "Detecting" && blockHitboxRightFSM.ActiveStateName == "Detecting";
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                gameObject.transform.Find("Block Hitbox Left").gameObject.SetActive(false);
                gameObject.transform.Find("Block Hitbox Right").gameObject.SetActive(false);
                if (interrputed)
                {
                    var animator = gameObject.GetComponent<Animation.Animator>();
                    animator.Disable();
                }
                return "BlockSlashCharged";
            }
        }
        private class BlockSlashCharged : State
        {
            public BlockSlashCharged(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                var actions = fsm.GetState("GSlash Charged").Actions;
                for (int i = 0; i < actions.Length; ++i)
                {
                    AddAction(actions[i]);
                }
                ExitAfterSeconds(0.3f);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Disable();
                return "BlockSlashSlash1";
            }
        }
        private class BlockSlashSlash1 : State
        {
            private float timer;
            public BlockSlashSlash1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                var actions = fsm.GetState("GSlash 1").Actions;
                for (int i = 0; i < actions.Length - 1; ++i)
                {
                    if (i == 3 || i == 5 || i == 8)
                    {
                        if (!(actions[i] is ActivateGameObject))
                        {
                            LogError("Wrong action was removed.");
                        }
                        continue;
                    }
                    if (i == 11)
                    {
                        if (!(actions[i] is SetVelocity2d))
                        {
                            LogError("Wrong action was removed.");
                        }
                        continue;
                    }
                    AddAction(actions[i]);
                }
            }
            public override void OnEnter(GameObject gameObject)
            {
                timer = 0;
                var v = 32;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                if (gameObject.transform.localScale.x > 0)
                {
                    rigidBody2D.velocity = new Vector2(-v, 0);
                }
                else
                {
                    rigidBody2D.velocity = new Vector2(v, 0);
                }
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer += Time.deltaTime;
                var GS1 = gameObject.transform.Find("GS1").gameObject;
                if (timer < 0.2)
                {
                    GS1.SetActive(true);
                    GS1.transform.localPosition = new Vector3(-0.5f, -3.5f, 0);
                    GS1.transform.localRotation = Quaternion.Euler(0, 0, 340);
                }
                else
                {
                    GS1.SetActive(false);
                }
                var x1 = gameObject.transform.position.x;
                var x2 = HeroController.instance.transform.position.x;
                if (Mathf.Abs(x1 - x2) < 3)
                {
                    var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity *= 0.2f;
                }
                if (timer < 0.4)
                {
                    return true;
                }
                else
                {
                    GS1.SetActive(false);
                    return false;
                }
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BlockSlashSlash2";
            }
        }
        private class BlockSlashSlash2 : State
        {
            float timer;
            public BlockSlashSlash2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                var actions = fsm.GetState("GSlash 2").Actions;
                for (int i = 0; i < actions.Length - 1; ++i)
                {
                    if (i == 1)
                    {
                        if (!(actions[i] is ActivateGameObject))
                        {
                            LogError("Wrong action was removed.");
                        }
                        continue;
                    }
                    AddAction(actions[i]);
                }
            }
            public override void OnEnter(GameObject gameObject)
            {
                timer = 0;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer += Time.deltaTime;
                var GS1 = gameObject.transform.Find("GS1").gameObject;
                if (timer < 0.2)
                {
                    GS1.SetActive(true);
                    GS1.transform.localPosition = new Vector3(-0.5f, -3.5f, 0);
                    GS1.transform.localRotation = Quaternion.Euler(0, 0, 340);
                }
                else
                {
                    GS1.SetActive(false);
                }
                var x1 = gameObject.transform.position.x;
                var x2 = HeroController.instance.transform.position.x;
                if (Mathf.Abs(x1 - x2) < 5)
                {
                    var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity *= 0.2f;
                }
                return timer < 0.2;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BlockSlashSlash3";
            }
        }
        private class BlockSlashSlash3 : State
        {
            float timer;
            public BlockSlashSlash3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                var actions = fsm.GetState("GSlash 3").Actions;
                for (int i = 0; i < actions.Length - 1; ++i)
                {
                    if (i == 1)
                    {
                        if (!(actions[i] is ActivateGameObject))
                        {
                            LogError("Wrong action was removed.");
                        }
                        continue;
                    }
                    AddAction(actions[i]);
                }
                ExitAfterSeconds(0.1f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                var S3 = gameObject.transform.Find("S3").gameObject;
                S3.SetActive(false);
                var S4 = gameObject.transform.Find("S4").gameObject;
                S4.SetActive(false);
                timer = 0;
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
                timer += Time.deltaTime;
                if (timer > 0.05f)
                {
                    var S4 = gameObject.transform.Find("S4").gameObject;
                    S4.SetActive(false);
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BlockSlashSlash4";
            }
        }
        private class BlockSlashSlash4 : State
        {
            public BlockSlashSlash4(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                var actions = fsm.GetState("GSlash 4").Actions;
                for (int i = 0; i < actions.Length; ++i)
                {
                    AddAction(actions[i]);
                }
                ExitAfterSeconds(0.5f);
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
