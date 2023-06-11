namespace Nailgod;
public partial class Control : Module
{
    public static float mmm = 5;
    private partial class ControlStateMachine : StateMachine
    {
        private class ExDashSlashAntic : State
        {
            public ExDashSlashAntic(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump Antic", 0));
                AddAction(fsm.GetAction("Jump Antic", 1));
                AddAction(fsm.GetAction("Jump Antic", 4));
                AddAction(fsm.GetAction("Jump Antic", 5));
                ExitAfterSeconds(0.1f);
            }
            public override void OnEnter(GameObject gameObject)
            {
                Recoil component = gameObject.GetComponent<Recoil>();
                if (component != null)
                {
                    component.SetRecoilSpeed(0);
                }
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "ExDashSlashJump";
            }
        }
        private class ExDashSlashJump : State
        {
            public ExDashSlashJump(GameObject gameObject) : base(gameObject)
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
                rigidBody2D.velocity = new Vector2(2f * (to.x - from.x), 55);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                return rigidBody2D.velocity.y > 0;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "ExDashSlashRoar";
            }
        }
        private class ExDashSlashRoar : State
        {
            List<Vector2> targets = new List<Vector2>();
            public ExDashSlashRoar(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Air Roar", 0));
                AddAction(fsm.GetAction("Air Roar", 1));
                AddAction(fsm.GetAction("Air Roar", 4));
                AddAction(fsm.GetAction("Air Roar", 5));
                AddAction(fsm.GetAction("Air Roar", 6));
                ExitAfterSeconds(2);
            }
            public override void OnEnter(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = Vector2.zero;
                rigidBody2D.gravityScale = 0;
                HeroController.instance.gameObject.LocateMyFSM("Roar Lock").FsmVariables.GetFsmGameObject("Roar Object").Value = gameObject;
                HeroController.instance.gameObject.LocateMyFSM("Roar Lock").SendEvent("ROAR ENTER");
                targets.Clear();
                int n = Nail.dashSlashNails.Count;
                float x = gameObject.transform.position.x;
                float y = gameObject.transform.position.y;
                float r = 4;
                float g = 30;
                float delta = (180 - 2 * g) / (n - 1);
                for (int i = 0; i < n; i++)
                {
                    float thisangle = 180 + g + delta * i;
                    thisangle = thisangle / Mathf.PI * 180 * mmm;
                    float dx = Mathf.Cos(thisangle) * r;
                    float dy = Mathf.Sin(thisangle) * r;
                    targets.Add(new Vector2(x + dx, y + dy));
                }
                foreach (var nail in Nail.dashSlashNails)
                {
                    nail.transform.Find("Hitbox Left").gameObject.SetActive(false);
                    nail.transform.Find("Hitbox Right").gameObject.SetActive(false);
                    nail.transform.position = new Vector3(45.4032f, 5.0343f, 0.01f);
                    nail.transform.rotation = Quaternion.Euler(0, 0, 353.4902f);
                }
                var old = GameObject.Find("Nail");
                old.transform.SetPositionX(0);
                old.transform.SetPositionY(0);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                int n = Nail.dashSlashNails.Count;
                for (int i = 0; i < n; i++)
                {
                    var nail = Nail.dashSlashNails[i];
                    var dir = new Vector2(targets[i].x - nail.transform.position.x, targets[i].y - nail.transform.position.y);
                    dir.Normalize();
                    var vel = 20;
                    dir *= vel;
                    nail.transform.Translate(dir.x * Time.deltaTime, dir.y * Time.deltaTime, 0);
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                foreach (var go in gameObject.scene.GetRootGameObjects())
                {
                    if (go.name == "Roar Wave Emitter(Clone)")
                    {
                        go.LocateMyFSM("emitter").SetState("End");
                        LogWarn("Disabling shaking!");
                    }
                }
                foreach (var nail in Nail.dashSlashNails)
                {
                    var rgb = nail.GetComponent<Rigidbody2D>();
                    float x;
                    if (nail.transform.position.x < gameObject.transform.position.x)
                    {
                        x = -80;
                    }
                    else
                    {
                        x = 80;
                    }
                    rgb.velocity = new Vector2(x, 80);
                }
                HeroController.instance.gameObject.LocateMyFSM("Roar Lock").SendEvent("ROAR EXIT");
                return "ExDashSlashJumpAgain";
            }
        }
        private class ExDashSlashJumpAgain : State
        {
            public ExDashSlashJumpAgain(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 1));
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(0, 70);
                return gameObject.transform.position.y < 16;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return "ExDashSlashAttack";
            }
        }
        private class ExDashSlashAttack : State
        {
            int cnt;
            float timer;
            int status;
            List<float> wox = new List<float>();
            List<float> woy = new List<float>();
            public ExDashSlashAttack(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                gameObject.transform.SetPositionX(0);
                gameObject.transform.SetPositionY(0);
                status = 0;
                timer = 0;
                cnt = 0;
            }
            float getmul()
            {
                float delta;
                if (cnt <= 3)
                {
                    delta = 1f;
                }
                else if (cnt <= 6)
                {
                    delta = Mathf.Sqrt(2);
                }
                else
                {
                    delta = 2f;
                }
                return delta;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer += Time.deltaTime;
                if (status == 0)
                {
                    if (timer > (cnt == 0 ? 0.5 : -1))
                    {
                        status = 1;
                        timer = 0;
                        cnt += 1;
                        var l = 30.5f;
                        var r = 62.5f;
                        var n = Nail.dashSlashNails.Count;
                        wox.Clear();
                        var delta = (r - l) / (n + 1);
                        for (int i = 0; i < n; i++)
                        {
                            wox.Add(l + (i + 1) * delta);
                        }
                        int besti = 0;
                        for (int i = 0; i < n; i++)
                        {
                            if (Mathf.Abs(HeroController.instance.transform.position.x - wox[i]) < Mathf.Abs(HeroController.instance.transform.position.x - wox[besti]))
                            {
                                besti = i;
                            }
                        }
                        wox[besti] = HeroController.instance.transform.position.x;
                        int ii = 0;
                        foreach (var nail in Nail.dashSlashNails)
                        {
                            nail.transform.SetPositionX(wox[ii]);
                            nail.transform.SetPositionY(16f);
                            ++ii;
                            nail.transform.Find("Hitbox Left").gameObject.SetActive(true);
                            nail.transform.Find("Hitbox Right").gameObject.SetActive(true);
                        }
                        woy.Clear();
                        for (int i = 0; i < n; i++) woy.Add(wox[i]);
                        for (int i = 0; i < 100; ++i)
                        {
                            var fail = false;
                            woy = woy.OrderBy(x => random.Next()).ToList();
                            for (int j = 0; j < n; j++)
                            {
                                if (wox[j] == woy[j])
                                {
                                    fail = true;
                                }
                            }
                            if (!fail) break;
                        }
                        for (int i = 0; i < n; i++)
                        {
                            float X1 = wox[i];
                            float Y1 = 16;
                            float X2 = woy[i];
                            float Y2 = 4.3f;
                            if (Math.Abs(i - besti) <= 1)
                            {
                                X2 = HeroController.instance.gameObject.transform.position.x;
                                Y2 = HeroController.instance.gameObject.transform.position.y;
                            }
                            var nail = Nail.dashSlashNails[i];
                            var rgb = nail.GetComponent<Rigidbody2D>();
                            var dir = new Vector2(X2 - X1, Y2 - Y1);
                            rgb.velocity = Vector2.zero;
                            var angle = Mathf.Atan2(dir.y, dir.x) / Mathf.PI * 180;
                            angle -= 270;
                            nail.transform.rotation = Quaternion.Euler(0, 0, angle + 6);
                        }
                    }
                }
                else if (status == 1)
                {
                    if (timer > 0.4f / getmul())
                    {
                        var fsm = gameObject.LocateMyFSM("Control");
                        var a1 = fsm.GetAction<AudioPlayerOneShotSingle>("D Slash S1", 1);
                        if (a1 == null)
                        {
                            LogWarn("no music");
                        }
                        else
                        {
                            LogWarn("music good");
                        }
                        {
                            LogWarn("music begin");
                            GameObject go = a1.audioPlayer.Value.Spawn(HeroController.instance.transform.position, Quaternion.identity);
                            var audio = go.GetComponent<AudioSource>();
                            AudioClip audioClip = a1.audioClip.Value as AudioClip;
                            audio.pitch = 1;
                            audio.volume = 1;
                            audio.PlayOneShot(audioClip);
                            LogWarn("Music end");
                        }
                        var a2 = fsm.GetAction("D Slash S1", 7);
                        a2.OnEnter();
                        var n = Nail.dashSlashNails.Count;
                        int besti = 0;
                        for (int i = 0; i < n; i++)
                        {
                            if (Mathf.Abs(HeroController.instance.transform.position.x - wox[i]) < Mathf.Abs(HeroController.instance.transform.position.x - wox[besti]))
                            {
                                besti = i;
                            }
                        }
                        for (int i = 0; i < n; i++)
                        {
                            float X1 = wox[i];
                            float Y1 = 16;
                            float X2 = woy[i];
                            float Y2 = 4.3f;
                            if (Math.Abs(i - besti) <= 1)
                            {
                                X2 = HeroController.instance.gameObject.transform.position.x;
                                Y2 = HeroController.instance.gameObject.transform.position.y;
                            }
                            var nail = Nail.dashSlashNails[i];
                            var rgb = nail.GetComponent<Rigidbody2D>();
                            var dir = new Vector2(X2 - X1, Y2 - Y1);
                            rgb.velocity = dir * 10 * getmul();
                            var angle = Mathf.Atan2(dir.y, dir.x) / Mathf.PI * 180;
                            angle -= 270;
                            nail.transform.rotation = Quaternion.Euler(0, 0, angle + 6);
                            nail.transform.Find("Sharp Flash(Clone)").gameObject.SetActive(true);
                            nail.transform.Find("Hitbox Left").GetComponent<PlayMakerFSM>().enabled = true;
                            nail.transform.Find("Hitbox Right").GetComponent<PlayMakerFSM>().enabled = true;
                        }
                        status = 2;
                        timer = 0;
                    }
                }
                else if (status == 2)
                {
                    if (timer > 0.4 / getmul())
                    {
                        foreach (var nail in Nail.dashSlashNails)
                        {
                            nail.transform.SetPositionX(0);
                            nail.transform.SetPositionY(0);
                            var rgb = nail.GetComponent<Rigidbody2D>();
                            rgb.velocity = Vector2.zero;
                        }
                        if (cnt >= 9)
                        {
                            return false;
                        }
                        else
                        {
                            status = 0;
                            timer = 0;
                        }
                    }
                }
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                var p = gameObject.transform.position;
                p.x = (37.7f + 55.2f) / 2;
                p.y = 16f;
                while (Mathf.Abs(p.x - HeroController.instance.transform.position.x) < 5)
                {
                    var l = 37.7f;
                    var r = 55.2f;
                    p.x = l + (float)random.NextDouble() * (r - l);
                }
                gameObject.transform.position = p;
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = new Vector2(0, -15);
                rigidBody2D.gravityScale = 3;
                return "ExDashSlashFall";
            }
        }
        private class ExDashSlashFall : State
        {
            public ExDashSlashFall(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Jump", 1));
            }
            public override void OnEnter(GameObject gameObject)
            {
                var nail = GameObject.Find("Nail");
                nail.SetActive(true);
                nail.transform.position = new Vector3(45.4032f, 16f, 0.01f);
                nail.transform.rotation = Quaternion.Euler(0, 0, 353.4902f);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                var nail = GameObject.Find("Nail");
                if (nail.transform.position.y > 5.0343f)
                {
                    nail.transform.Translate(0, -80 * Time.deltaTime, 0);
                }
                return !Helper.OnGround(gameObject);
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                Recoil component = gameObject.GetComponent<Recoil>();
                if (component != null)
                {
                    component.SetRecoilSpeed(15);
                }
                gameObject.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL START");
                LogWarn("Stun started.");
                return "Move";
            }
        }
    }
}
