#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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

    class ComboSpells:Program
    {
        public static void CastComboSpells(Obj_AI_Hero target) // Combo 발사
        {
            spell_ordernum = 0;
            if (target == null || target.IsDead)
                return;

            foreach(var spell in c_Spells)
            {
                if(target.Distance(Player.Position)<=spell.Range &&
                    spell.Slot == spell_orderlist[spell_ordernum])
                {
                    if (Player.Spellbook.CanUseSpell(spell_orderlist[spell_ordernum]) == SpellState.Ready)
                    {
                        if (!spell.IsSkillshot)
                        {
                            if (spell.IsChargedSpell)
                            {
                                CastChargedSpell(spell, target);
                                return;
                            }
                            else
                            {
                                spell.CastOnUnit(target);
                                return;
                            }
                        }
                        else if (!baseMenu.Item("IgnorePrediction").GetValue<bool>())
                        {
                            if (spell.GetPrediction(target).Hitchance >= h_chance)
                            {
                                var pos = spell.GetPrediction(target);
                                spell.Cast(pos.CastPosition);
                            }
                        }                        
                        else
                        {
                            spell.Cast(target.Position);
                            return;
                        }
                    }

                    if (spell_ordernum + 1 >= spell_orderlist.Count)
                        spell_ordernum = 0;
                    else
                        spell_ordernum++;
                }
            }
        }


        public static void getComboSpells()
        {
            c_Spells.Clear();
            foreach(var t in Misc.Combo)
            {
                if(t=="Q")
                {
                    Spell temp = new Spell(Q.slot,Q.Range);

                    temp.IsSkillshot = Q.IsMissile;
                    temp.IsChargedSpell = Q.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(Q.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            Q.IsBlockable, GetSStypeByByte(Q.MissileType));
                    }
                    if(temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)Q.ChargingTime);
                    }

                    c_Spells.Add(temp);
                }
                if (t == "W")
                {
                    Spell temp = new Spell(W.slot, W.Range);
                    temp.IsSkillshot = W.IsMissile;
                    temp.IsChargedSpell = W.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(W.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            W.IsBlockable, GetSStypeByByte(W.MissileType));
                    }
                    if (temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)W.ChargingTime);
                    }
                    c_Spells.Add(temp);
                }
                if (t == "E")
                {
                    Spell temp = new Spell(E.slot, E.Range);
                    temp.IsSkillshot = E.IsMissile;
                    temp.IsChargedSpell = E.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(E.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            E.IsBlockable, GetSStypeByByte(E.MissileType));
                    }
                    if (temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)E.ChargingTime);
                    }

                    c_Spells.Add(temp);
                }
                if (t == "R")
                {
                    Spell temp = new Spell(R.slot, R.Range);

                    temp.IsSkillshot = R.IsMissile;
                    temp.IsChargedSpell = R.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(R.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            R.IsBlockable, GetSStypeByByte(R.MissileType));
                    }
                    if (temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)R.ChargingTime);
                    }

                    c_Spells.Add(temp);
                }                
                if (t == "Q2")
                {
                    Spell temp = new Spell(Q2.slot, Q2.Range);
                    temp.IsSkillshot = Q2.IsMissile;
                    temp.IsChargedSpell = Q2.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(Q2.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            Q2.IsBlockable, GetSStypeByByte(Q2.MissileType));
                    }
                    if (temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)Q2.ChargingTime);
                    }

                    c_Spells.Add(temp);
                }
                if (t == "W2")
                {
                    Spell temp = new Spell(W2.slot, W2.Range);
                    temp.IsSkillshot = W2.IsMissile;
                    temp.IsChargedSpell = W2.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(W2.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            W2.IsBlockable, GetSStypeByByte(W2.MissileType));
                    }
                    if (temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)W2.ChargingTime);
                    }

                    c_Spells.Add(temp);
                }
                if (t == "E2")
                {
                    Spell temp = new Spell(E2.slot, E2.Range);
                    temp.IsSkillshot = E2.IsMissile;
                    temp.IsChargedSpell = E2.IsCharging;

                    if (temp.IsSkillshot)
                    {
                        temp.SetSkillshot(E2.MissileDelay,
                            Player.Spellbook.GetSpell(temp.Slot).SData.LineWidth,
                            Player.Spellbook.GetSpell(temp.Slot).SData.MissileSpeed,
                            E2.IsBlockable, GetSStypeByByte(E2.MissileType));
                    }
                    if (temp.IsChargedSpell)
                    {
                        temp.SetCharged(Player.Spellbook.GetSpell(temp.Slot).SData.Name, Player.Spellbook.GetSpell(temp.Slot).SData.Name,
                            (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRange[0], (int)Player.Spellbook.GetSpell(temp.Slot).SData.CastRadius[0], (float)E2.ChargingTime);
                    }

                    c_Spells.Add(temp);
                }
            }
        }

        public static void getComboSpellList()
        {
            spell_orderlist.Clear();
            foreach (var spell in c_Spells)
            {
                spell_orderlist.Add(spell.Slot);
            }
        }

        public static SpellSlot GetSlotByByte(byte temp)
        {
            switch(temp)
            {
                case 1:
                    return SpellSlot.Q;
                case 2:
                    return SpellSlot.W;
                case 3:
                    return SpellSlot.E;
                case 4:
                    return SpellSlot.R;

                default:
                    return SpellSlot.Unknown;

            }
        }
        public static SkillshotType GetSStypeByByte(byte temp)
        {
            switch (temp)
            {
                case 1:
                    return SkillshotType.SkillshotCircle;
                case 2:
                    return SkillshotType.SkillshotCone;
                case 3:
                    return SkillshotType.SkillshotLine;

                default:
                    return SkillshotType.SkillshotCircle;

            }
        }    
    
        public static void CastChargedSpell(Spell spell,Obj_AI_Hero target)
        {

            if (spell.IsCharging)
            {
                //spell.Cast(target);
            }
            else
            {
                spell.StartCharging();
                Game.PrintChat("Start");
            }
        }
    }
}
