﻿using Exiled.API.Features;
using System;

namespace Better_207
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "BlackOut";
        public override string Name => "BlackOut";
        public override string Author => "Rysik5318, fix by VT";
        public override Version Version { get; } = new Version(2, 0, 0);


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