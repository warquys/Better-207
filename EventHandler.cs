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

        private Dictionary<Player, (CoroutineHandle, short)> _coroutineHandles;

        #endregion

        #region Constructor & Destructor
        internal EventHandler(Config config)
        {
            _config = config;
            _coroutineHandles = new Dictionary<Player, (CoroutineHandle, short)>();
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

        private IEnumerator<float> DecreasesEffect207Coroutine(Player player, short intensityToDecreases = 1)
        {
            yield return Timing.WaitForSeconds(_config.EffectDuration);
            yield return Timing.WaitForOneFrame; //Other yield bc (Timing.WaitForOneFrame = -inifinity) so idk who EMC reacte if it is added

            var effect207    = player.GetEffect(EffectType.Scp207);
            var newIntensity = (byte)Math.Max(effect207.Intensity - intensityToDecreases, 0);
            player.ChangeEffectIntensity(EffectType.Scp207, newIntensity);

            if (_coroutineHandles.ContainsKey(player))
                _coroutineHandles.Remove(player);
            yield break;
        }

        #endregion

        #region Events

        private void OnHurt(HurtingEventArgs ev)
        {
            if (!_config.NoDammage || ev.Handler.Type != DamageType.Scp207) return;

            if (_config.EffectDuration == 0 || _coroutineHandles.ContainsKey(ev.Target))
                ev.IsAllowed = false;
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var coroutine in _coroutineHandles.Values)
                Timing.KillCoroutines(coroutine.Item1);
            _coroutineHandles.Clear();
        }

        private void OnUseItemd(UsedItemEventArgs ev)
        {
            //check if it was a real SCP 207
            if (ev.Item.Type != ItemType.SCP207 || CustomItem.TryGet(ev.Item, out _)) return;

            if (_config.DommageAmount != 0)
            {
                if (UnityEngine.Random.Range(1, 100) <= _config.DommageChance)
                {
                    var dommageHandler = new UniversalDamageHandler(_config.DommageAmount, DeathTranslations.Scp207);
                    ev.Player.Hurt(dommageHandler);
                }
            }

            if (_config.EffectDuration != 0)
            {
                if (_coroutineHandles.ContainsKey(ev.Player))
                {
                    var coroutineIntensity = _coroutineHandles[ev.Player];
                    Timing.KillCoroutines(_coroutineHandles[ev.Player].Item1);
                    var coroutine = Timing.RunCoroutine(DecreasesEffect207Coroutine(ev.Player, coroutineIntensity.Item2));
                    _coroutineHandles[ev.Player] = (coroutine, 1);
                }
                else
                {
                    var coroutine = Timing.RunCoroutine(DecreasesEffect207Coroutine(ev.Player));
                    _coroutineHandles.Add(ev.Player, (coroutine, 1));
                }
            }
        }
        #endregion
    }
}