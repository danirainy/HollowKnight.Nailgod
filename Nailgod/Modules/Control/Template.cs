namespace Nailgod;
public partial class Control : Module
{
    private partial class ControlStateMachine : StateMachine
    {
        private class Template : State
        {
            public Template(GameObject gameObject) : base(gameObject)
            {
            }
            public override void OnEnter(GameObject gameObject)
            {
            }
            public override bool OnFixedUpdate(GameObject gameObject)
            {
                return true;
            }
            public override void OnUpdate(GameObject gameObject)
            {
            }
            public override string OnExit(GameObject gameObject, bool interrputed)
            {
                return null;
            }
        }
    }
}
