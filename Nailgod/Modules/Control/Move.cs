namespace Nailgod;
public partial class Control : Module
{
    private partial class ControlStateMachine : StateMachine
    {
        private class Move : State
        {
            public bool exdashslash = false;
            public bool exslaser = false;
            private Vector2 velocity;
            private float timer;
            private List<string> history = new List<string>();
            public Move(GameObject gameObject) : base(gameObject)
            {
                var fsm = gameObject.LocateMyFSM("Control");
                AddAction(fsm.GetAction("Chase", 0));
                AddAction(fsm.GetAction("Chase", 1));
                AddAction(fsm.GetAction("Chase", 8));
                AddAction(fsm.GetAction("GSlash 4", 2));
            }
            private int Direction(GameObject gameObject)
            {
                int d;
                var idealDistance = 12;
                var myPos = gameObject.transform.position.x;
                var playerPos = HeroController.instance.transform.position.x;
                var left = playerPos - idealDistance;
                var right = playerPos + idealDistance;
                if (myPos < left)
                {
                    d = 1;
                }
                else if (myPos > right)
                {
                    d = -1;
                }
                else if (myPos - left < right - myPos)
                {
                    d = -1;
                }
                else
                {
                    d = 1;
                }
                return d;
            }
            public override void OnEnter(GameObject gameObject)
            {
                var scale = gameObject.transform.localScale;
                var d = Direction(gameObject);
                scale.x = -d * Mathf.Abs(scale.x);
                gameObject.transform.localScale = scale;
                velocity = new Vector2(d * 10, 0);
                var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                rigidBody2D.velocity = velocity;
                timer = (float)(random.NextDouble() * 0.1);
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                timer += Time.deltaTime;
                var x = gameObject.transform.position.x;
                if (timer < 0.3 && x > 30.8 && x < 62.1 && (history.Count == 0 || history[history.Count - 1] != "ShadowDashDash"))
                {
                    return true;
                }
                else
                {
                    var rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity = Vector2.zero;
                    return false;
                }
            }
            private (string, float, int) WeightedSelect(List<(string, float, int)> candidates)
            {
                float tot = 0;
                foreach (var candidate in candidates)
                {
                    tot += candidate.Item2;
                }
                float sel = (float)random.NextDouble() * tot;
                foreach (var candidate in candidates)
                {
                    sel -= candidate.Item2;
                    if (sel < 0)
                    {
                        return candidate;
                    }
                }
                return candidates[candidates.Count - 1];
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                if (gameObject.GetComponent<HealthManager>().hp <= 2000 && !exdashslash)
                {
                    exdashslash = true;
                    gameObject.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
                    LogWarn("Stun stopped.");
                    return "ExDashSlashAntic";
                }
                if (gameObject.GetComponent<HealthManager>().hp <= 1000 && !exslaser)
                {
                    exslaser = true;
                    gameObject.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
                    LogWarn("Stun stopped.");
                    return "JumpLaserJump";
                }
                List<(string, float, int)> candidates = new List<(string, float, int)>()
                {
                    ("BunnyHopSlashAntic",1,2),
                    ("JumpShootJump", 1, 2),
                    ("BlockSlashBlock", 1, 2),
                    ("AirDashSlashAntic", 0.5f, 1),
                    ("StompAntic", 1, 2),
                    ("ShadowLaserDash", 0.5f, 1),
                    ("CycloneSlashJump", 1, 2),
                    ("ShadowDashDash", 1f, 2),
                };
                int gapLimit = (int)(candidates.Count * 1.5);
                (string, float, int) selection;
                bool fail;
                var i = 0;
                do
                {
                    var shortCandidates = new List<(string, float, int)>();
                    if (history.Count >= gapLimit)
                    {
                        var found = new Dictionary<string, bool>();
                        for (int j = 0; j < gapLimit; ++j)
                        {
                            found[history[history.Count - 1 - j]] = true;
                        }
                        foreach (var candidate in candidates)
                        {
                            if (!found.ContainsKey(candidate.Item1))
                            {
                                shortCandidates.Add(candidate);
                            }
                        }
                    }
                    if (shortCandidates.Count > 0)
                    {
                        selection = WeightedSelect(shortCandidates);
                    }
                    else
                    {
                        selection = WeightedSelect(candidates);
                    }
                    fail = false;
                    if (selection.Item3 == 1)
                    {
                        if (history.Count > 0 && history[history.Count - 1] == selection.Item1)
                        {
                            fail = true;
                        }
                    }
                    else if (selection.Item3 == 2)
                    {
                        if (history.Count > 1 && history[history.Count - 2] == selection.Item1 && history[history.Count - 1] == selection.Item1)
                        {
                            fail = true;
                        }
                    }
                    else
                    {
                        LogError("Any move can only happen twice in a row.");
                    }
                    i += 1;
                } while (fail && i < 32);
                history.Add(selection.Item1);
                if (history.Count > gapLimit)
                {
                    history.RemoveAt(0);
                }
                return selection.Item1;
            }
        }
    }
}
