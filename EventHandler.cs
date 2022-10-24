using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;

namespace Better_207
{
    public class EventHandler
    {

        #region Properties & Variables
        
        private Config _config;

        private List<CoroutineHandle> _coroutineHandles;
        private List<Player> _noDommage207Effect;

        #endregion

        #region Constructor & Destructor
        internal EventHandler(Config config)
        {
            _config = config;
            _coroutineHandles = new List<CoroutineHandle>();
            _noDommage207Effect = new List<Player>();
            AttachEvent();
        }
        #endregion

        #region Methods
        public void AttachEvent()
        {
            Exiled.Events.Handlers.Player.UsedItem += OnUseItemd;
            Exiled.Events.Handlers.Player.Hurting += OnHurt;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        public void DetachEvent()
        {
            Exiled.Events.Handlers.Player.UsedItem -= OnUseItemd;
            Exiled.Events.Handlers.Player.Hurting -= OnHurt;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
        }

        private IEnumerator<float> DecreasesEffect207Coroutine(Player player)
        {
            yield return _config.EffectDuration;
            var effect207    = player.GetEffect(EffectType.Scp207);
            var newIntensity = (byte)(effect207.Intensity - 1);
            player.ChangeEffectIntensity(EffectType.Scp207, newIntensity);
            if (newIntensity == 0)
                _noDommage207Effect.Remove(player);
            yield break;
        }

        #endregion

        #region Events

        private void OnHurt(HurtingEventArgs ev)
        {
            if (ev.Handler.Type == DamageType.Scp207 && _noDommage207Effect.Contains(ev.Target))
                ev.IsAllowed = false;
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Timing.KillCoroutines(_coroutineHandles.ToArray());
            _coroutineHandles.Clear();
            _noDommage207Effect.Clear();
        }

        private void OnUseItemd(UsedItemEventArgs ev)
        {
            //check if it was a real SCP 207
            if (ev.Item.Type == ItemType.SCP207 && !CustomItem.TryGet(ev.Item, out _)) 
            {

                if (_config.Dommage != 0)
                {

                    if (UnityEngine.Random.Range(1, 100) <= _config.DommageChance)
                    {
                        var dommageHandler = new PlayerStatsSystem.UniversalDamageHandler(_config.Dommage, PlayerStatsSystem.DeathTranslations.Scp207);
                        ev.Player.Hurt(dommageHandler);
                    }
                }

                if (_config.EffectDuration != 0)
                {
                    var coroutine = Timing.RunCoroutine(DecreasesEffect207Coroutine(ev.Player));
                    _coroutineHandles.Add(coroutine);
                    if (_config.NoDammage)
                        _noDommage207Effect.Add(ev.Player);
                }
            }
        }
        #endregion
    }
}