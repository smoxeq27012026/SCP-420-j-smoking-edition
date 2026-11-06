using System.Collections;
using System.Runtime.CompilerServices;
using Akequ.Base;
using UnityEngine;

namespace AdminAudit.Items
{
    public class SCP420J : Item
    {
        private const int   MAX_CIGARETTES      = 5;
        private const short SMOKE_DAMAGE        = 5;
        private const float SPEED_BOOST_DURATION = 12f;
        private const float SPEED_BOOST_MULT    = 1.15f;
        private const string CANCER_REASON      = "рак лёгких";

        private class CigState
        {
            public int Count;
            public float[] BaseSpeeds;
            public Coroutine BoostCoroutine;
        }

        private static readonly ConditionalWeakTable<Player, CigState> _states = new ConditionalWeakTable<Player, CigState>();

        public override void OnUse()
        {
            player.effects.Add("Rainbow", 20, 1, true, true);
            player.maxHealth = player.maxHealth + 15;
            player.UpdateHealth();
            if (player.isServer)
            {
                var state = _states.GetOrCreateValue(player);
                state.Count++;

                ApplySpeedBoost(player, state);

                player.ChangeHealth(new DamageHandler
                {
                    damage       = SMOKE_DAMAGE,
                    deathReason  = CANCER_REASON,
                    killID       = "SCP420J_smoke",
                    ignoreResist = true
                });

                if (state.Count >= MAX_CIGARETTES && player.health > 0)
                {
                    player.ChangeHealth(new DamageHandler
                    {
                        damage       = 1,
                        deathReason  = CANCER_REASON,
                        killID       = "SCP420J_cancer",
                        ignoreResist = true,
                        instant      = true
                    });
                }

                player.RemoveItemOnServer(this);
                player.PlaySCPSound("420J");
            }
        }

        private static void ApplySpeedBoost(Player player, CigState state)
        {
            if (state.BaseSpeeds == null)
            {
                state.BaseSpeeds = new float[]
                {
                    player.GetWalkSpeed(),
                    player.GetRunSpeed(),
                    player.GetCrouchSpeed()
                };

                var b = state.BaseSpeeds;
                player.SetSpeed(
                    b[0] * SPEED_BOOST_MULT,
                    b[1] * SPEED_BOOST_MULT,
                    b[2] * SPEED_BOOST_MULT
                );
            }
            if (state.BoostCoroutine != null)
            {
                try { player.StopCoroutine(state.BoostCoroutine); } catch { }
            }
            state.BoostCoroutine = player.StartCoroutine(RestoreSpeed(player, state, SPEED_BOOST_DURATION));
        }

        private static IEnumerator RestoreSpeed(Player player, CigState state, float duration)
        {
            yield return new WaitForSeconds(duration);

            state.BoostCoroutine = null;

            if (player == null || player.health == 0)
            {
                state.BaseSpeeds = null;
                yield break;
            }

            if (state.BaseSpeeds != null)
            {
                try
                {
                    var b = state.BaseSpeeds;
                    player.SetSpeed(b[0], b[1], b[2]);
                }
                catch { }
                state.BaseSpeeds = null;
            }
        }

        public override GameObject GetWorldPrefab()
        {
            return GameObject.Instantiate(ResourcesManager.GetObject("item_w_scp420j") as GameObject);
        }

        public override Sprite GetImage() => ResourcesManager.GetSprite("inv_item_scp420j");
        public override string GetName() => "SCP-420-J";

        public override string OnRefine(int type)
        {
            if (type == 2)
            {
                return "Coin";
            }
            else if (type >= 3)
            {
                int chance = Random.Range(0, 100);
                if (chance < 20)
                {
                    return "";
                }
                else if (chance < 30)
                {
                    return "Cup";
                }
                else
                {
                    return "SCP500";
                }
            }
            return "";
        }
    }
}