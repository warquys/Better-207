using Exiled.API.Features;

namespace Better_207
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "Better207";
        public override string Name => "Better 207";
        public override string Author => "VT";
        public override Version Version { get; } = new Version(1, 0, 0);

        public EventHandler EventHandler { get; private set; }

        public override void OnEnabled()
        {
            base.OnEnabled();
            if (EventHandler == null)
                EventHandler = new EventHandler(Config);
            else
                EventHandler.AttachEvent();
        }

        public override void OnDisabled()
        {
            EventHandler.DetachEvent();
            base.OnDisabled();
        }

    }
}