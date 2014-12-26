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

    class ComboSpells:Program
    {
        public static void CastComboSpells(Obj_AI_Hero target) // Combo 발사
        {
            if(!target.IsDead && target.IsEnemy && target.IsVisible)
            {
                foreach(var s in c_Spells)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, target); // 일단 움직임

                    if(Vector3.Distance(target.Position, Player.Position) <= s.Range)//사정거리 안에 있음 발동
                        s.CastIfHitchanceEquals(target, h_chance);
                }
            }
        }



        public static void getComboSpells()
        {
            foreach (var combospell in Misc.Combo) // Misc.Combo : Byte형으로 1,2,3,4 등의 숫자를 담고 있음
            {
                Spell tempSpell = new Spell(GetSlotByByte(combospell));
                #region setspells
                if(!IsChangeable)
                if (tempSpell.Slot == SpellSlot.Q && Q.Range >= 100)
                {
                    tempSpell.Range = Q.Range;
                    if(Q.IsCharging)
                    tempSpell.SetCharged(Q.name[0], null, Q.ChargingMin, Q.ChargingMax, (float)Q.ChargingTime);
                    if(Q.IsMissile)
                    tempSpell.SetSkillshot(Q.MissileDelay,Q.MissileWidth,Q.MissileSpeed,Q.IsBlockable,
                        GetSStypeByByte(Q.MissileType));
                }
                if (tempSpell.Slot == SpellSlot.W && W.Range >= 100)
                {
                    tempSpell.Range = W.Range;
                    if (W.IsCharging)
                        tempSpell.SetCharged(W.name[0], null, W.ChargingMin, W.ChargingMax, (float)W.ChargingTime);
                    if (W.IsMissile)
                        tempSpell.SetSkillshot(W.MissileDelay, W.MissileWidth, W.MissileSpeed, W.IsBlockable,
                            GetSStypeByByte(W.MissileType));
                }

                if (tempSpell.Slot == SpellSlot.E && E.Range >= 100)
                {
                    tempSpell.Range = E.Range;
                    if (E.IsCharging)
                        tempSpell.SetCharged(E.name[0], null, E.ChargingMin, E.ChargingMax, (float)E.ChargingTime);
                    if (E.IsMissile)
                        tempSpell.SetSkillshot(E.MissileDelay, E.MissileWidth, E.MissileSpeed, E.IsBlockable,
                            GetSStypeByByte(E.MissileType));
                }
                if (tempSpell.Slot == SpellSlot.R && R.Range >= 100)
                {
                    tempSpell.Range = R.Range;
                    if (R.IsCharging)
                        tempSpell.SetCharged(R.name[0], null, R.ChargingMin, R.ChargingMax, (float)R.ChargingTime);
                    if (R.IsMissile)
                        tempSpell.SetSkillshot(R.MissileDelay, R.MissileWidth, R.MissileSpeed, R.IsBlockable,
                            GetSStypeByByte(R.MissileType));
                }
                #endregion
           
                c_Spells.Add(tempSpell); // Spell형 리스트에 tempSpell추가 Range는 위에서 추가됨
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
    
    }
}
