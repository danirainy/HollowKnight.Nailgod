namespace Nailgod;
public class Nail : Module
{
    private class Ground : MonoBehaviour
    {
        void Update()
        {
            var anim = gameObject.GetComponent<tk2dSpriteAnimator>();
            if (anim.CurrentClip.name != "Nail Ground")
            {
                gameObject.GetComponent<tk2dSpriteAnimator>().Play("Nail Ground");
            }
        }
    }
    private class NailStateMachine : StateMachine
    {
        private class Stand : State
        {
            public Stand(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                gameObject.GetComponent<tk2dSpriteAnimator>().Play("Nail Ground");
            }
        }
        private class Follow : State
        {
            private float rotation;
            public Follow(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
                rotation = gameObject.transform.rotation.eulerAngles.z - 95;
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                gameObject.GetComponent<tk2dSpriteAnimator>().Play("Nail Ground");
                var heroPos = HeroController.instance.transform.position;
                var myPos = gameObject.transform.position;
                var rigidBody = gameObject.GetComponent<Rigidbody2D>();
                var vX = 0f;
                var vY = 0f;
                var dX = heroPos.x - myPos.x;
                var dY = heroPos.y - myPos.y;
                if (dX < -3)
                {
                    vX = -5;
                }
                else if (dX > 3)
                {
                    vX = 5;
                }
                if (dY < -4)
                {
                    vY = -5;
                }
                else if (dY > -3)
                {
                    vY = 5;
                }
                rigidBody.velocity = new Vector2(vX, vY);
                var angle = Mathf.Atan2(dY, dX) / Mathf.PI * 180;
                var angularVelocity = 180;
                if (rotation < angle - 10)
                {
                    rotation += Time.deltaTime * angularVelocity;
                }
                else if (rotation > angle + 10)
                {
                    rotation -= Time.deltaTime * angularVelocity;
                }
                gameObject.transform.rotation = Quaternion.Euler(0, 0, rotation + 95);
                return true;
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return null;
            }
        }
        NailStateMachine() : base()
        {
            AddSetInitialState(new Stand(gameObject));
            AddState(new Follow(gameObject));
        }
    }
    static private GameObject nailTemplate;
    static private GameObject hitboxTemplate;
    public static List<GameObject> dashSlashNails = new List<GameObject>();
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)>
        {
            ("GG_Sly","Battle Scene"),
        };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        nailTemplate = preloadedObjects["GG_Sly"]["Battle Scene"].transform.Find("Stun Nail").gameObject;
        nailTemplate.RemoveComponent<PlayMakerFSM>();
        nailTemplate.RemoveComponent<KeepRotation>();
        nailTemplate.GetComponent<Rigidbody2D>().drag = 0;
        nailTemplate.GetComponent<Rigidbody2D>().angularDrag = 0;
        nailTemplate.GetComponent<Rigidbody2D>().gravityScale = 0;
        hitboxTemplate = preloadedObjects["GG_Sly"]["Battle Scene"].transform.Find("Sly Boss").Find("S4").gameObject;
    }
    private GameObject MakeNail()
    {
        var nail = UnityEngine.Object.Instantiate(nailTemplate);
        nail.name = "Nail Dash Slash";
        nail.SetActive(true);
        nail.transform.position = new Vector3(0, 0, 0.01f);
        var hitboxLeft = UnityEngine.Object.Instantiate(hitboxTemplate, nail.transform);
        hitboxLeft.GetComponent<DamageHero>().damageDealt = 1;
        hitboxLeft.SetActive(true);
        hitboxLeft.name = "Hitbox Left";
        hitboxLeft.transform.localPosition = new Vector3(-0.05f, -1.6f, 0);
        hitboxLeft.transform.rotation = Quaternion.Euler(0, 0, 84.5f);
        hitboxLeft.transform.localScale = new Vector3(1.2f, 0.15f, 1);
        var hitboxLeftFSM = hitboxLeft.GetComponent<PlayMakerFSM>();
        // hitboxLeftFSM.RemoveAction("Blocked Hit", 0);
        // hitboxLeftFSM.RemoveAction("Blocked Hit", 0);
        var hitboxRight = UnityEngine.Object.Instantiate(hitboxTemplate, nail.transform);
        hitboxRight.GetComponent<DamageHero>().damageDealt = 1;
        hitboxRight.SetActive(true);
        hitboxRight.name = "Hitbox Right";
        hitboxRight.transform.localPosition = new Vector3(0.17f, -1.6f, 0);
        hitboxRight.transform.rotation = Quaternion.Euler(0, 0, 264.5f);
        hitboxRight.transform.localScale = new Vector3(-1.2f, 0.15f, 1);
        var hitboxRightFSM = hitboxRight.GetComponent<PlayMakerFSM>();
        // hitboxRightFSM.RemoveAction("Blocked Hit", 0);
        // hitboxRightFSM.RemoveAction("Blocked Hit", 0);
        hitboxLeftFSM.InsertCustomAction("Blocked Hit", () =>
        {
            if (hitboxRightFSM.ActiveStateName != "Detecting")
            {
                hitboxLeftFSM.SetState("Detecting");
            }
        }, 0);
        hitboxRightFSM.InsertCustomAction("Blocked Hit", () =>
        {
            if (hitboxLeftFSM.ActiveStateName != "Detecting")
            {
                hitboxRightFSM.SetState("Detecting");
            }
        }, 0);
        nail.transform.rotation = Quaternion.Euler(0, 0, 6);
        nail.RemoveComponent<BoxCollider2D>();
        nail.AddComponent<Ground>();
        return nail;
    }
    static public GameObject MakeFallNail()
    {
        var nail = UnityEngine.Object.Instantiate(nailTemplate);
        nail.name = "Nail Dash Fall";
        nail.SetActive(true);
        nail.transform.position = new Vector3(0, 0, 0.01f);
        var hitboxLeft = UnityEngine.Object.Instantiate(hitboxTemplate, nail.transform);
        hitboxLeft.GetComponent<DamageHero>().damageDealt = 1;
        hitboxLeft.SetActive(true);
        hitboxLeft.name = "Hitbox Left";
        hitboxLeft.transform.localPosition = new Vector3(-0.05f, -1.6f, 0);
        hitboxLeft.transform.rotation = Quaternion.Euler(0, 0, 84.5f);
        hitboxLeft.transform.localScale = new Vector3(1.2f, 0.15f, 1);
        var hitboxLeftFSM = hitboxLeft.GetComponent<PlayMakerFSM>();
        hitboxLeftFSM.RemoveAction("Blocked Hit", 0);
        hitboxLeftFSM.RemoveAction("Blocked Hit", 0);
        var hitboxRight = UnityEngine.Object.Instantiate(hitboxTemplate, nail.transform);
        hitboxRight.GetComponent<DamageHero>().damageDealt = 1;
        hitboxRight.SetActive(true);
        hitboxRight.name = "Hitbox Right";
        hitboxRight.transform.localPosition = new Vector3(0.17f, -1.6f, 0);
        hitboxRight.transform.rotation = Quaternion.Euler(0, 0, 264.5f);
        hitboxRight.transform.localScale = new Vector3(-1.2f, 0.15f, 1);
        var hitboxRightFSM = hitboxRight.GetComponent<PlayMakerFSM>();
        hitboxRightFSM.RemoveAction("Blocked Hit", 0);
        hitboxRightFSM.RemoveAction("Blocked Hit", 0);
        hitboxLeftFSM.InsertCustomAction("Blocked Hit", () =>
        {
            if (hitboxRightFSM.ActiveStateName != "Detecting")
            {
                hitboxLeftFSM.SetState("Detecting");
            }
        }, 0);
        hitboxRightFSM.InsertCustomAction("Blocked Hit", () =>
        {
            if (hitboxLeftFSM.ActiveStateName != "Detecting")
            {
                hitboxRightFSM.SetState("Detecting");
            }
        }, 0);
        nail.transform.rotation = Quaternion.Euler(0, 0, 6);
        nail.RemoveComponent<BoxCollider2D>();
        nail.AddComponent<Ground>();
        return nail;
    }
    public override void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        if (InArena())
        {
            var nail = UnityEngine.Object.Instantiate(nailTemplate);
            nail.RemoveComponent<BoxCollider2D>();
            nail.name = "Nail";
            nail.SetActive(true);
            nail.transform.position = new Vector3(45.4032f, 5.0343f, 0.01f);
            nail.transform.rotation = Quaternion.Euler(0, 0, 353.4902f);
            var hitboxLeft = UnityEngine.Object.Instantiate(hitboxTemplate, nail.transform);
            hitboxLeft.SetActive(false);
            hitboxLeft.name = "Hitbox Left";
            hitboxLeft.transform.localPosition = new Vector3(-0.05f, -1.6f, 0);
            hitboxLeft.transform.rotation = Quaternion.Euler(0, 0, 84.5f);
            hitboxLeft.transform.localScale = new Vector3(1.2f, 0.15f, 1);
            var hitboxLeftFSM = hitboxLeft.GetComponent<PlayMakerFSM>();
            hitboxLeftFSM.RemoveAction("Blocked Hit", 0);
            hitboxLeftFSM.RemoveAction("Blocked Hit", 0);
            var hitboxRight = UnityEngine.Object.Instantiate(hitboxTemplate, nail.transform);
            hitboxRight.SetActive(false);
            hitboxRight.name = "Hitbox Right";
            hitboxRight.transform.localPosition = new Vector3(0.17f, -1.6f, 0);
            hitboxRight.transform.rotation = Quaternion.Euler(0, 0, 264.5f);
            hitboxRight.transform.localScale = new Vector3(-1.2f, 0.15f, 1);
            var hitboxRightFSM = hitboxRight.GetComponent<PlayMakerFSM>();
            hitboxRightFSM.RemoveAction("Blocked Hit", 0);
            hitboxRightFSM.RemoveAction("Blocked Hit", 0);
            hitboxLeftFSM.InsertCustomAction("Blocked Hit", () =>
            {
                if (hitboxRightFSM.ActiveStateName != "Detecting")
                {
                    hitboxLeftFSM.SetState("Detecting");
                }
            }, 0);
            hitboxRightFSM.InsertCustomAction("Blocked Hit", () =>
            {
                if (hitboxLeftFSM.ActiveStateName != "Detecting")
                {
                    hitboxRightFSM.SetState("Detecting");
                }
            }, 0);
            nail.AddComponent<Ground>();
            dashSlashNails.Clear();
            int n = 8;
            for (int i = 0; i < n; ++i)
            {
                dashSlashNails.Add(MakeNail());
                var sf = SlyBoss().transform.Find("Sharp Flash").gameObject;
                var tmp = UnityEngine.Object.Instantiate(sf, dashSlashNails[i].transform);
                tmp.transform.localPosition = new Vector3(-0.25f, 0, -0.001f);
                tmp.transform.localRotation = Quaternion.Euler(0, 0, 84);
                dashSlashNails[i].name += " " + i.ToString();
            }
            List<GameObject> allhitboxes = new List<GameObject>();
            for (int i = 0; i < n; ++i)
            {
                allhitboxes.Add(dashSlashNails[i].transform.Find("Hitbox Left").gameObject);
                allhitboxes.Add(dashSlashNails[i].transform.Find("Hitbox Right").gameObject);
            }
            for (int i = 0; i < allhitboxes.Count; ++i)
            {
                var hitboxRightFSM2 = allhitboxes[i].GetComponent<PlayMakerFSM>();
                hitboxRightFSM2.InsertCustomAction("Blocked Hit", () =>
                {
                    LogWarn("wochao" + hitboxRightFSM2.gameObject.name);
                    for (int j = 0; j < allhitboxes.Count; ++j)
                    {
                        var hitboxLeftFSM2 = allhitboxes[j].GetComponent<PlayMakerFSM>();
                        if (GameObject.ReferenceEquals(hitboxLeftFSM2, hitboxRightFSM2))
                        {
                            continue;
                        }
                        LogWarn("--" + hitboxLeftFSM2.gameObject.name);
                        hitboxLeftFSM2.enabled = false;
                    }
                }, 0);
            }
            LogWarn("Nails setup done.");
        }
    }
}
