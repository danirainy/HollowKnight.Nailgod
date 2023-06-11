namespace Nailgod;
public class Bullet : Module
{
    public class MyMegaJellyZap : MonoBehaviour
    {
        // Token: 0x06001331 RID: 4913 RVA: 0x0005744E File Offset: 0x0005564E
        private void Awake()
        {
            this.col = base.GetComponent<CircleCollider2D>();
            this.fade = base.GetComponentInChildren<ColorFader>();
            if (this.anim)
            {
                this.animMesh = this.anim.GetComponent<MeshRenderer>();
            }
        }

        // Token: 0x06001332 RID: 4914 RVA: 0x00057486 File Offset: 0x00055686
        private void OnEnable()
        {
            this.routine = base.StartCoroutine((this.type == MegaJellyZap.Type.Zap) ? this.ZapSequence() : this.MultiZapSequence());
        }

        // Token: 0x06001333 RID: 4915 RVA: 0x000574AA File Offset: 0x000556AA
        private void OnDisable()
        {
            if (this.routine != null)
            {
                base.StopCoroutine(this.routine);
            }
        }

        // Token: 0x06001334 RID: 4916 RVA: 0x000574C0 File Offset: 0x000556C0
        private IEnumerator ZapSequence()
        {
            this.col.enabled = false;
            this.ptAttack.Stop();
            this.ptAntic.Play();
            if (this.fade)
            {
                this.fade.Fade(true);
            }
            yield return new WaitForSeconds(0.25f);
            this.zapBugPt1.SpawnAndPlayOneShot(this.audioSourcePrefab, this.transform.position);
            this.col.enabled = true;
            this.ptAttack.Play();
            yield return new WaitForSeconds(0.25f);
            this.zapBugPt2.SpawnAndPlayOneShot(this.audioSourcePrefab, this.transform.position);
            if (this.fade)
            {
                this.fade.Fade(false);
            }
            this.ptAttack.Stop();
            this.ptAntic.Stop();
            this.col.enabled = false;
            yield return new WaitForSeconds(1f);
            this.gameObject.Recycle();
            yield break;
        }

        // Token: 0x06001335 RID: 4917 RVA: 0x000574CF File Offset: 0x000556CF
        private IEnumerator MultiZapSequence()
        {
            this.animMesh.enabled = false;
            this.col.enabled = false;
            this.ptAttack.Stop();
            this.transform.SetScaleX((float)((UnityEngine.Random.Range(0, 2) == 0) ? 1 : -1));
            this.transform.SetRotation2D(UnityEngine.Random.Range(0f, 360f));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.5f));
            this.anim.Play("Zap Antic");
            this.animMesh.enabled = true;
            yield return new WaitForSeconds(0.8f);
            this.col.enabled = true;
            this.ptAttack.Play();
            this.anim.Play("Zap");
            yield return new WaitForSeconds(1f);
            this.ptAttack.Stop();
            this.col.enabled = false;
            yield return this.StartCoroutine(this.anim.PlayAnimWait("Zap End"));
            this.animMesh.enabled = false;
            yield return new WaitForSeconds(0.5f);
            this.gameObject.SetActive(false);
            yield break;
        }

        // Token: 0x04001263 RID: 4707
        public MegaJellyZap.Type type;

        // Token: 0x04001264 RID: 4708
        public ParticleSystem ptAttack;

        // Token: 0x04001265 RID: 4709
        public ParticleSystem ptAntic;

        // Token: 0x04001266 RID: 4710
        public AudioSource audioSourcePrefab;

        // Token: 0x04001267 RID: 4711
        public AudioEvent zapBugPt1;

        // Token: 0x04001268 RID: 4712
        public AudioEvent zapBugPt2;

        // Token: 0x04001269 RID: 4713
        public tk2dSpriteAnimator anim;

        // Token: 0x0400126A RID: 4714
        private MeshRenderer animMesh;

        // Token: 0x0400126B RID: 4715
        private CircleCollider2D col;

        // Token: 0x0400126C RID: 4716
        private ColorFader fade;

        // Token: 0x0400126D RID: 4717
        private Coroutine routine;

        // Token: 0x0200034D RID: 845
        public enum Type
        {
            // Token: 0x0400126F RID: 4719
            Zap,
            // Token: 0x04001270 RID: 4720
            MultiZap
        }
    }
    private class Follow : MonoBehaviour
    {
        public int bulletType;
        float timer = 0;
        private void FixedUpdate()
        {
            timer += Time.deltaTime;
            if (timer > 2)
            {
                Destroy(gameObject);
                return;
            }
            var p = gameObject.transform.position;
            if (p.x < 30.5 || p.x > 62.5 || p.y < 4.3)
            {
                if (bulletType == 0)
                {
                    var explosionTemplate = GameObject.Find("_GameManager").transform.Find("GlobalPool").transform.Find("Gas Explosion Recycle M(Clone)").gameObject;
                    explosionTemplate.LocateMyFSM("damages_enemy").enabled = false;
                    explosionTemplate.GetComponent<DamageHero>().damageDealt = 2;
                    var explosion = Instantiate(explosionTemplate);
                    explosion.SetActive(true);
                    explosion.transform.position = gameObject.transform.position;
                }
                else
                {
                    var zap = Instantiate(zapTemplate);
                    zap.SetActive(true);
                    zap.transform.position = gameObject.transform.position;
                    if (p.y < 4.3)
                    {
                        zap.transform.localScale = new Vector3(1f, 2f, 1);
                    }
                    else
                    {
                        zap.transform.localScale = new Vector3(2f, 1f, 1);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
    public static GameObject bulletTemplate;
    public static AudioClip gunShot;
    public static GameObject hitboxTemplate;
    public static GameObject iceTemplate;
    public static GameObject zapTemplate;
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)>
        {
            ("GG_Blue_Room", "abyss_0006_blue_root_06"),
            ("GG_Sly","Battle Scene"),
            ("GG_Uumuu","Mega Jellyfish GG"),
        };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        bulletTemplate = preloadedObjects["GG_Blue_Room"]["abyss_0006_blue_root_06"];
        var rigidbody2D = bulletTemplate.AddComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        bulletTemplate.AddComponent<Follow>();
        gunShot = Helper.LoadAudioClip("Nailgod.Resources.Gun Shot.wav");
        hitboxTemplate = preloadedObjects["GG_Sly"]["Battle Scene"].transform.Find("Sly Boss").Find("S4").gameObject;
        hitboxTemplate.GetComponent<DamageHero>().damageDealt = 2;
        zapTemplate = preloadedObjects["GG_Uumuu"]["Mega Jellyfish GG"].LocateMyFSM("Mega Jellyfish").GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value;
        var oldZap = zapTemplate.GetComponent<MegaJellyZap>();
        var newZap = zapTemplate.AddComponent<MyMegaJellyZap>();
        newZap.type = oldZap.type;
        newZap.ptAttack = oldZap.ptAttack;
        newZap.ptAntic = oldZap.ptAntic;
        newZap.audioSourcePrefab = oldZap.audioSourcePrefab;
        newZap.zapBugPt1 = oldZap.zapBugPt1;
        newZap.zapBugPt2 = oldZap.zapBugPt2;
        newZap.anim = oldZap.anim;
        zapTemplate.RemoveComponent<MegaJellyZap>();
    }
    public static void Shoot(GameObject gameObject, int bulletType = 0, bool airShoot = false)
    {
        var fsm = gameObject.LocateMyFSM("Control");
        var action = fsm.GetAction<AudioPlayerOneShotSingle>("Jump", 2);
        var originalAudioClip = action.audioClip;
        action.audioClip = gunShot;
        action.volume = 4;
        action.OnEnter();
        action.audioClip = originalAudioClip;
        action.volume = 1;
        var bullet = GameObject.Instantiate(bulletTemplate, gameObject.transform);
        bullet.name = "Bullet";
        bullet.SetActive(true);
        if (!airShoot)
        {
            bullet.transform.localPosition = new Vector3(-3.5f, 1.37f, 0);
            bullet.transform.localRotation = Quaternion.Euler(0, 0, 343);
            bullet.transform.localScale = new Vector3(-0.75f, 0.6f, 1);
        }
        else
        {
            bullet.transform.localPosition = new Vector3(-3.7f, 0.83f, 0);
            bullet.transform.localRotation = Quaternion.Euler(0, 0, 350);
            bullet.transform.localScale = new Vector3(-0.75f, 0.6f, 1);
        }
        bullet.transform.parent = null;
        if (bullet.transform.localScale.x < 0)
        {
            var scale = bullet.transform.localScale;
            scale.x *= -1;
            bullet.transform.localScale = scale;
            bullet.transform.Rotate(0, 0, 180);
        }
        if (bulletType == 0)
        {
            var spriteRenderer = bullet.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Animation.clips["10.Bullet"].sprites[0];
        }
        else
        {
            var spriteRenderer = bullet.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Animation.clips["12.Bullet"].sprites[0];
        }
        var bulletRigidBody2D = bullet.GetComponent<Rigidbody2D>();
        var velocity = 50;
        var angle = bullet.transform.rotation.eulerAngles.z / 180 * Mathf.PI;
        bulletRigidBody2D.velocity = new Vector2(Mathf.Cos(angle) * velocity, Mathf.Sin(angle) * velocity);
        var hitbox = UnityEngine.Object.Instantiate(hitboxTemplate, bullet.transform);
        hitbox.SetActive(true);
        hitbox.transform.localPosition = new Vector3(0.23f, -0.05f, 0);
        hitbox.transform.localRotation = Quaternion.Euler(0, 0, 160);
        hitbox.transform.localScale = new Vector3(0.15f, 0.12f, 1);
        bullet.GetComponent<Follow>().bulletType = bulletType;
    }
}
