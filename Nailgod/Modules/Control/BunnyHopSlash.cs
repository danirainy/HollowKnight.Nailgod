namespace Nailgod;
public partial class Control : Module
{
    private partial class ControlStateMachine : StateMachine
    {
        private class BunnyHopSlashAntic : State
        {
            public BunnyHopSlashAntic(GameObject gameObject) : base(gameObject)
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
                return "BunnyHopSlashHop";
            }
        }
        private class BunnyHopSlashHop : State
        {
            public BunnyHopSlashHop(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 2));
            }
            public override void OnEnter(GameObject gameObject)
            {
                int sign = Mathf.Abs(gameObject.transform.position.x - HeroController.instance.transform.position.x) < 7.5f ? -1 : -1;
                sign *= gameObject.transform.position.x < HeroController.instance.transform.position.x ? 1 : -1;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(sign * 20, 15 * Mathf.Abs(sign));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashSlashAntic";
            }
        }
        private class BunnyHopSlashSlashAntic : State
        {
            public BunnyHopSlashSlashAntic(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash Antic", 0));
                ExitAfterSeconds(0.1f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashSlashS1";
            }
        }
        private class BunnyHopSlashSlashS1 : State
        {
            public BunnyHopSlashSlashS1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S1", 0));
                AddAction(fsm.GetAction("Slash S1", 1));
                AddAction(fsm.GetAction("Slash S1", 2));
                AddAction(fsm.GetAction("Slash S1", 6));
                ExitAfterSeconds(0.02f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                var v = 25f;
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
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashSlashS2";
            }
        }
        private class BunnyHopSlashSlashS2 : State
        {
            public BunnyHopSlashSlashS2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S2", 0));
                AddAction(fsm.GetAction("Slash S2", 1));
                AddAction(fsm.GetAction("Slash S2", 2));
                ExitAfterSeconds(0.02f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashSlashS3";
            }
        }
        private class BunnyHopSlashSlashS3 : State
        {
            public BunnyHopSlashSlashS3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S3", 0));
                AddAction(fsm.GetAction("Slash S3", 1));
                ExitAfterSeconds(0.02f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashBSlashAntic";
            }
        }
        private class BunnyHopSlashBSlashAntic : State
        {
            public BunnyHopSlashBSlashAntic(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash Antic", 0));
                AddAction(fsm.GetAction("BSlash Antic", 1));
                ExitAfterSeconds(0.1f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashBSlashDash";
            }
        }
        private class BunnyHopSlashBSlashDash : State
        {
            public BunnyHopSlashBSlashDash(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash Dash", 0));
                ExitAfterSeconds(0.02f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                var v = 25f;
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
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashBSlashS1";
            }
        }
        private class BunnyHopSlashBSlashS1 : State
        {
            public BunnyHopSlashBSlashS1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S1", 0));
                AddAction(fsm.GetAction("BSlash S1", 1));
                AddAction(fsm.GetAction("BSlash S1", 2));
                ExitAfterSeconds(0.02f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashBSlashS2";
            }
        }
        private class BunnyHopSlashBSlashS2 : State
        {
            public BunnyHopSlashBSlashS2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S2", 0));
                AddAction(fsm.GetAction("BSlash S2", 1));
                AddAction(fsm.GetAction("BSlash S2", 2));
                ExitAfterSeconds(0.02f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashBSlashS3";
            }
        }
        private class BunnyHopSlashBSlashS3 : State
        {
            public BunnyHopSlashBSlashS3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S3", 0));
                AddAction(fsm.GetAction("BSlash S3", 1));
                ExitAfterSeconds(0.02f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity *= 0.85f;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashCharge";
            }
        }
        private class BunnyHopSlashCharge : State
        {
            public BunnyHopSlashCharge(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 0));
                AddAction(fsm.GetAction("Jump", 1));
                AddAction(fsm.GetAction("Jump", 3));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                Vector2 target = new Vector2();
                target.y = HeroController.instance.transform.position.y + 6;
                if (HeroController.instance.transform.position.x < gameObject.transform.position.x)
                {
                    target.x = HeroController.instance.transform.position.x + 3;
                }
                else
                {
                    target.x = HeroController.instance.transform.position.x - 3;
                }
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                var newV = target - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
                if (Mathf.Abs(newV.x) < 1f&&gameObject.transform.position.y>5.408124f+5.5f)
                {
                    rigidBody2D.velocity = Vector2.zero;
                    return false;
                }
                newV.x *= 10;
                if (Mathf.Abs(newV.x) < 40)
                {
                    newV.x = Mathf.Sign(newV.x) * 40;
                }
                newV.y *= 20;
                if (Mathf.Abs(newV.y) < 40)
                {
                    newV.y = Mathf.Sign(newV.y) * 40;
                }
                rigidBody2D.velocity = newV;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashWait";
            }
        }
        private class BunnyHopSlashWait : State
        {
            public BunnyHopSlashWait(GameObject gameObject) : base(gameObject)
            {
                ExitAfterSeconds(0.02f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "BunnyHopSlashSlash1";
            }
        }
        private class BunnyHopSlashSlash1 : State
        {
            public BunnyHopSlashSlash1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S1", 0));
                AddAction(fsm.GetAction("Slash S1", 1));
                AddAction(fsm.GetAction("Slash S1", 2));
                AddAction(fsm.GetAction("Slash S1", 6));
                ExitAfterSeconds(0.02f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Helper.AngleToPlayer(gameObject, -30, 30));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "BunnyHopSlashSlash2";
            }
        }
        private class BunnyHopSlashSlash2 : State
        {
            public BunnyHopSlashSlash2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S2", 0));
                AddAction(fsm.GetAction("Slash S2", 1));
                AddAction(fsm.GetAction("Slash S2", 2));
                ExitAfterSeconds(0.1f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(0, -2);
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "BunnyHopSlashSlash3";
            }
        }
        private class BunnyHopSlashSlash3 : State
        {
            public BunnyHopSlashSlash3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Slash S3", 0));
                AddAction(fsm.GetAction("Slash S3", 1));
                ExitAfterSeconds(0.02f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                var position = gameObject.transform.position;
                GameObject grubberFlyBeam;
                if (gameObject.transform.GetScaleX() < 0)
                {
                    grubberFlyBeam = Instantiate(HeroController.instance.grubberFlyBeamPrefabR);
                }
                else
                {
                    grubberFlyBeam = Instantiate(HeroController.instance.grubberFlyBeamPrefabL);
                }
                grubberFlyBeam.LocateMyFSM("Control").GetAction<Wait>("Active", 0).time = 1.8f;
                grubberFlyBeam.LocateMyFSM("damages_enemy").enabled = false;
                grubberFlyBeam.AddComponent<DamageHero>();
                var fsm = grubberFlyBeam.LocateMyFSM("Control");
                fsm.RemoveTransition("Active", "TERRAIN HIT");
                grubberFlyBeam.transform.position = new Vector3(position.x, position.y + 0.7f, position.z);
                var localScale = grubberFlyBeam.transform.localScale;
                localScale.x *= 2;
                localScale.y *= 4;
                grubberFlyBeam.transform.localScale = localScale;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "BunnyHopSlashBackSlash1";
            }
        }
        private class BunnyHopSlashBackSlash1 : State
        {
            public BunnyHopSlashBackSlash1(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S1", 0));
                AddAction(fsm.GetAction("BSlash S1", 1));
                AddAction(fsm.GetAction("BSlash S1", 2));
                ExitAfterSeconds(0.05f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Helper.AngleToPlayer(gameObject));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "BunnyHopSlashBackSlash2";
            }
        }
        private class BunnyHopSlashBackSlash2 : State
        {
            public BunnyHopSlashBackSlash2(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S2", 0));
                AddAction(fsm.GetAction("BSlash S2", 1));
                AddAction(fsm.GetAction("BSlash S2", 2));
                ExitAfterSeconds(0.05f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (interrputed)
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
                return "BunnyHopSlashBackSlash3";
            }
        }
        private class BunnyHopSlashBackSlash3 : State
        {
            public BunnyHopSlashBackSlash3(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("BSlash S3", 0));
                AddAction(fsm.GetAction("BSlash S3", 1));
                ExitAfterSeconds(0.02f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                var position = gameObject.transform.position;
                GameObject grubberFlyBeam;
                if (gameObject.transform.GetScaleX() < 0)
                {
                    grubberFlyBeam = Instantiate(HeroController.instance.grubberFlyBeamPrefabR);
                }
                else
                {
                    grubberFlyBeam = Instantiate(HeroController.instance.grubberFlyBeamPrefabL);
                }
                grubberFlyBeam.LocateMyFSM("Control").GetAction<Wait>("Active", 0).time = 1.8f;
                grubberFlyBeam.LocateMyFSM("damages_enemy").enabled = false;
                grubberFlyBeam.AddComponent<DamageHero>();
                var fsm = grubberFlyBeam.LocateMyFSM("Control");
                fsm.RemoveTransition("Active", "TERRAIN HIT");
                grubberFlyBeam.transform.position = new Vector3(position.x, position.y + 0.7f, position.z);
                var localScale = grubberFlyBeam.transform.localScale;
                localScale.x *= 4;
                localScale.y *= 8;
                grubberFlyBeam.transform.localScale = localScale;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                gameObject.transform.rotation = Quaternion.identity;
                return "BunnyHopSlashFall";
            }
        }
        private class BunnyHopSlashFall : State
        {
            public BunnyHopSlashFall(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 1));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "Move";
            }
        }
    }
}
