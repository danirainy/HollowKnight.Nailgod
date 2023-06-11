namespace Nailgod;
public partial class Control : Module
{
    static public GameObject specialLaser;
    private partial class ControlStateMachine : StateMachine
    {
        private class JumpLaserJump : State
        {
            public JumpLaserJump(GameObject gameObject) : base(gameObject)
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
                to.x = (37.7f + 55.2f) / 2;
                to.y = 0f;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(2f * (to.x - from.x), 70);
                Recoil component = gameObject.GetComponent<Recoil>();
                if (component != null)
                {
                    component.SetRecoilSpeed(0);
                }
                var old = GameObject.Find("Nail");
                old.transform.SetPositionX(45.4032f);
                old.transform.SetPositionY(5.0343f);
                var oldr = old.GetComponent<Rigidbody2D>();
                oldr.velocity = new Vector2(0, 160);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                return rigidBody2D.velocity.y > 0;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "JumpLaserShoot";
            }
        }
        static public int rainNumber = 8;
        static public float rainRadius = 4;
        static public float rainTime = 0.4f;
        static public float rainSpeed = 15;
        static public float rainProb = 0.5f;
        static List<GameObject> nails = new List<GameObject>();
        private class JumpLaserShoot : State
        {
            float originalGravityScale;
            float currentAngle;
            float timer;
            int bType;
            int ta = -1;
            int tb = -1;
            int status;
            int cnt;
            float timer2;

            public JumpLaserShoot(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play("15.Shoot", 0.1f, Animation.Animator.Mode.Single);
                currentAngle = 0;
                timer = float.MaxValue;
                bType = random.NextDouble() < 0.5 ? 0 : 1;
                if (bType == ta && bType == tb)
                {
                    bType = 1 - bType;
                }
                ta = tb;
                tb = bType;
                status = 0;
                cnt = 0;
                timer = 0;
                var b = specialLaser;
                b.LocateMyFSM("Control").GetAction<Wait>("Fire", 2).time = 0.4f;
                nails.Clear();
                timer2 = 9999;
                var old = GameObject.Find("Nail");
                old.transform.SetPositionX(0);
                old.transform.SetPositionY(0);
                var oldr = old.GetComponent<Rigidbody2D>();
                oldr.velocity = new Vector2(0, 0);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer2 += Time.deltaTime;
                if (timer2 > rainTime)
                {
                    timer2 = 0;
                    var l = 30.5f - 5;
                    var r = 62.5f + 5;
                    var n = rainNumber;
                    var delta = (r - l) / (n + 1);
                    for (int i = 0; i < n; i++)
                    {
                        if (random.NextDouble() < 1 - rainProb) continue;
                        var x = l + (i + 1) * delta + (float)(rainRadius / 2.0 - rainRadius * random.NextDouble());
                        var y = 16 + 2 + rainRadius + (float)(rainRadius / 2.0 - rainRadius * random.NextDouble());
                        var nail = Nail.MakeFallNail();
                        nail.transform.SetPosition2D(x, y);
                        var rgb = nail.GetComponent<Rigidbody2D>();
                        rgb.velocity = new Vector2(0, -10);
                        nails.Add(nail);
                    }
                    List<GameObject> newnail = new List<GameObject>();
                    foreach (var nail in nails)
                    {
                        if (nail != null)
                        {
                            if (nail.transform.position.y < 0)
                            {
                                GameObject.Destroy(nail);
                            }
                            else
                            {
                                newnail.Add(nail);
                                var rgb = nail.GetComponent<Rigidbody2D>();
                                rgb.velocity = new Vector2(0, -rainSpeed);
                            }
                        }
                    }
                    nails = newnail;
                }
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                timer += Time.deltaTime;
                if (status == 0)
                {
                    if (timer > 1)
                    {
                        var b = specialLaser;
                        b.LocateMyFSM("Control").SetState("Fire");
                        status = 1;
                        timer = 0;
                        gameObject.transform.Find("sglow").gameObject.SetActive(true);
                    }
                    else
                    {
                        if (HeroController.instance.transform.position.x < gameObject.transform.position.x && gameObject.transform.localScale.x < 0)
                        {
                            gameObject.transform.SetScaleX(1);
                            currentAngle *= -1;
                        }
                        if (HeroController.instance.transform.position.x > gameObject.transform.position.x && gameObject.transform.localScale.x > 0)
                        {
                            gameObject.transform.SetScaleX(-1);
                            currentAngle *= -1;
                        }
                        if (timer > 0.5f)
                        {
                            var b = specialLaser;
                            b.SetActive(true);
                            b.LocateMyFSM("Control").SetState("Antic");
                        }
                        var x1 = HeroController.instance.transform.position.x;
                        var y1 = HeroController.instance.transform.position.y;
                        var x2 = gameObject.transform.position.x;
                        var y2 = gameObject.transform.position.y;
                        var dx = x1 - x2;
                        var dy = y1 - y2;
                        var angle = Mathf.Atan2(dy, dx) / Math.PI * 180;
                        if (gameObject.transform.localScale.x > 0)
                        {
                            if (angle < 0)
                            {
                                angle += 180;
                            }
                            else
                            {
                                angle -= 180;
                            }
                            angle += 5.5;
                        }
                        else
                        {
                            angle -= 5.5;
                        }
                        if (Mathf.Abs(currentAngle - 360 - (float)angle) < Mathf.Abs(currentAngle - (float)angle))
                        {
                            currentAngle -= 360;
                        }
                        if (Mathf.Abs(currentAngle + 360 - (float)angle) < Mathf.Abs(currentAngle - (float)angle))
                        {
                            currentAngle += 360;
                        }
                        var v = Mathf.Abs(currentAngle - (float)angle) * 2.5f;
                        if (currentAngle < angle)
                        {
                            currentAngle += v * Time.deltaTime;
                        }
                        else
                        {
                            currentAngle -= v * Time.deltaTime;
                        }
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                    }
                }
                else
                {
                    if (timer > 0.1)
                    {
                        cnt += 1;
                        var b = specialLaser;
                        b.SetActive(false);
                        gameObject.transform.Find("sglow").gameObject.SetActive(false);
                        status = 0;
                        timer = 0;
                        var animator = gameObject.GetComponent<Animation.Animator>();
                        animator.Play("15.Shoot", 0.1f, Animation.Animator.Mode.Single);
                        if (cnt >= 10)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = 0;
                gameObject.transform.rotation = Quaternion.identity;
                if (interrputed)
                {
                    var animator = gameObject.GetComponent<Animation.Animator>();
                    animator.Disable();
                }
                var b = specialLaser;
                b.SetActive(false);
                gameObject.transform.Find("sglow").gameObject.SetActive(false);
                return "JumpLaserScan";
            }
        }
        private class JumpLaserScan : State
        {
            float originalGravityScale;
            float currentAngle;
            float timer;
            int bType;
            int ta = -1;
            int tb = -1;
            int status;
            int cnt;
            float timer2 = 0;
            float timer3 = 0;
            public JumpLaserScan(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                Helper.FacePlayer(gameObject);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                originalGravityScale = rigidBody2D.gravityScale;
                rigidBody2D.gravityScale = 0;
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Play("15.Shoot", 0.01f, Animation.Animator.Mode.Single);
                currentAngle = 0;
                timer = float.MaxValue;
                bType = random.NextDouble() < 0.5 ? 0 : 1;
                if (bType == ta && bType == tb)
                {
                    bType = 1 - bType;
                }
                ta = tb;
                tb = bType;
                status = 0;
                cnt = 0;
                timer = 0;
                currentAngle = 0;
                var b = specialLaser;
                b.SetActive(true);
                b.LocateMyFSM("Control").SetState("Antic");
                b.LocateMyFSM("Control").GetAction<Wait>("Fire", 2).time = 999f;
                timer2 = 0;
                timer3 = 0;
                foreach (var nail in nails)
                {
                    var rgb = nail.GetComponent<Rigidbody2D>();
                    rgb.velocity = new Vector2(0, 0);
                }
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                if (timer2 < 0.5)
                {
                    timer2 += Time.deltaTime;
                    return true;
                }
                else if (timer2 < 100)
                {
                    var b = specialLaser;
                    b.LocateMyFSM("Control").SetState("Fire");
                    gameObject.transform.Find("sglow").gameObject.SetActive(true);
                    timer2 = 200;
                }
                timer3 += Time.deltaTime;
                if (timer3 > 0.1)
                {
                    timer3 = 0;
                    var b = specialLaser;
                    var x = b.transform.position.x;
                    var y = b.transform.position.y;
                    var a = b.transform.rotation.eulerAngles.z;
                    if (gameObject.transform.localScale.x > 0)
                    {
                        a -= 180;
                    }
                    a = a / 180 * Mathf.PI;
                    var dx = -Mathf.Cos(a);
                    var dy = -Mathf.Sin(a);
                    var t = 9999f;
                    if (Mathf.Abs(dx) > 0.001f)
                    {
                        var t1 = (30.5f - x) / dx;
                        if (t1 > 0 && t1 < t) t = t1;
                        var t2 = (62.5f - x) / dx;
                        if (t2 > 0 && t2 < t) t = t2;
                    }
                    if (dy < -0.001f)
                    {
                        var t1 = (4.3f - y) / dy;
                        if (t1 > 0 && t1 < t) t = t1;
                    }
                    x += dx * t;
                    y += dy * t;
                    var explosionTemplate = GameObject.Find("_GameManager").transform.Find("GlobalPool").transform.Find("Gas Explosion Recycle M(Clone)").gameObject;
                    explosionTemplate.LocateMyFSM("damages_enemy").enabled = false;
                    explosionTemplate.GetComponent<DamageHero>().damageDealt = 2;
                    var explosion = Instantiate(explosionTemplate);
                    explosion.SetActive(true);
                    explosion.transform.SetPosition2D(x, y);
                }
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                if (gameObject.transform.GetScaleX() > 0)
                    currentAngle += Time.deltaTime * 90;
                else
                    currentAngle -= Time.deltaTime * 90;
                if (currentAngle > 180 || currentAngle < -180) return false;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, currentAngle + 5.5f);
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.gravityScale = 3;
                gameObject.transform.rotation = Quaternion.identity;
                var animator = gameObject.GetComponent<Animation.Animator>();
                animator.Disable();
                var b = specialLaser;
                b.SetActive(false);
                gameObject.transform.Find("sglow").gameObject.SetActive(false);
                foreach (var nail in nails)
                {
                    var rgb = nail.GetComponent<Rigidbody2D>();
                    rgb.velocity = new Vector2(0, 80);
                    var hl = nail.transform.Find("Hitbox Left").gameObject;
                    GameObject.Destroy(hl);
                    var hr = nail.transform.Find("Hitbox Right").gameObject;
                    GameObject.Destroy(hr);
                }
                return "JumpLaserFall";
            }
        }
        private class JumpLaserFall : State
        {
            public JumpLaserFall(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 1));
            }
            public override void OnEnter(GameObject gameObject)
            {
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "JumpLaserLand";
            }
        }
        private class JumpLaserLand : State
        {
            public JumpLaserLand(GameObject gameObject) : base(gameObject)
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
                gameObject.transform.Find("halo").gameObject.SetActive(false);
                Recoil component = gameObject.GetComponent<Recoil>();
                if (component != null)
                {
                    component.SetRecoilSpeed(15);
                }
                gameObject.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL START");
                LogWarn("Stun started.");
                foreach (var nail in nails)
                {
                    if (nail != null)
                    {
                        GameObject.Destroy(nail);
                    }
                }
                nails.Clear();
                return "Move";
            }
        }
    }
}
