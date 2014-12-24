#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonComboScriptor
{
    class dmgCalculator:Program
    {
        public static void GetSpeedDamage(SpellStatus Spell)
        {
            GetSpellDamage(Spell,TargetSelector.GetSelectedTarget());
        }

        public static double GetSpellDamage(SpellStatus Spell,Obj_AI_Hero target)
        {
            #region get info
            float Player_baseAD = Player.BaseAttackDamage;
            float Player_addAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_baseAD + Player_addAD;
            float Player_totalAP = Player.FlatMagicDamageMod;
            #endregion


            double[] spell_basedamage = {   0,
                                            Spell.DmgLv1,
                                            Spell.DmgLv1 + Spell.DmgPer * 1,
                                            Spell.DmgLv1 + Spell.DmgPer * 2,
                                            Spell.DmgLv1 + Spell.DmgPer * 3,
                                            Spell.DmgLv1 + Spell.DmgPer * 4
                                        };

            if (Spell.IsNeedCalculate)
            {
                if (Player.BaseSkinName == "Kalista" && Spell.slot == SpellSlot.E) // 칼리스타E - Twitch E
                {
                    double eDmg = 0;
                    foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
                    {
                        double[] spell_perdamage = { 0, 0.25, 0.30, 0.35, 0.40, 0.45 };
                        int count = venoms.Count - 1; // Only Kalista
                        eDmg = Player_totalAD * 0.60 + spell_basedamage[Spell.level];
                        eDmg = eDmg + count * (eDmg * spell_perdamage[Spell.level]);
                    }

                    return Player.CalcDamage(target, Damage.DamageType.Physical, eDmg);
                }

                else if (Player.BaseSkinName == "Twitch" && Spell.slot == SpellSlot.E) // 트위치E - Twitch E
                {
                    double eDmg = 0;
                    foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "TwitchDeadlyVenom"))
                    {
                        double[] spell_stackdamage = { 0, 15, 20, 25, 30, 35 };
                        eDmg = spell_basedamage[Spell.level] + venoms.Count * (spell_stackdamage[Spell.level] + (Player_totalAP * 0.2) + (Player_addAD * 0.25));
                    }
                    return Player.CalcDamage(target, Damage.DamageType.Physical, eDmg);
                }

                else if (Player.BaseSkinName == "Darius" && Spell.slot == SpellSlot.R) // Darius R
                {
                    double eDmg = 0;
                    foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "DariusHemorrhage"))
                    {
                        eDmg = spell_basedamage[Spell.level] + venoms.Count * (spell_basedamage[Spell.level] * 0.2);
                    }
                    return Player.CalcDamage(target, Damage.DamageType.True, eDmg);
                }
                return 0;
            }
            
            else // Default Calculate
            {
                double Dmg=0;
            
                Dmg = spell_basedamage[Spell.level];

                if (Spell.addAD > 0.00)
                    Dmg += Player_addAD * Spell.addAD;

                if (Spell.totalAD > 0.00)
                    Dmg += Player_totalAD * Spell.totalAD;

                if (Spell.totalAP > 0.00)
                    Dmg += Player_totalAP * Spell.totalAP;

                if (Spell.EnemyAP > 0.00)
                    Dmg += target.FlatMagicDamageMod * Spell.EnemyAP;

                if (Spell.EnemyCurHP > 0.00)
                    Dmg += target.Health * Spell.EnemyCurHP;

                if (Spell.EnemyMaxHP > 0.00)
                    Dmg += target.MaxHealth * Spell.EnemyMaxHP;

                if (Spell.EnemyMissHP > 0.00)
                    Dmg += (target.MaxHealth-target.Health) * Spell.EnemyMissHP;

                if (Spell.Per100AP > 0.00) // it's for ZacW
                    Dmg += target.MaxHealth*(Player_totalAP * Spell.Per100AP/100f);

                if (Spell.MaxMana > 0.00)
                    Dmg += Player.MaxMana * Spell.MaxMana;


                
                //----------------------return---------------------------//
                if (Spell.Damagetype == 1)
                    return Player.CalcDamage(target, Damage.DamageType.Physical, Dmg);
                if (Spell.Damagetype == 2)
                    return Player.CalcDamage(target, Damage.DamageType.Magical, Dmg);
                if (Spell.Damagetype == 3)
                    return Player.CalcDamage(target, Damage.DamageType.True, Dmg);
                return 0;
            }
        }
    }
}
