using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAction;
using XIVAutoAction.Data;

namespace XIVAutoAction.Helpers;

public record LevelModifier(int Main, int Sub, int Div);
internal class HealHelper
{
    private static double CalcDet(int det, ref LevelModifier lvlModifier)
    {
        var cVal = Math.Floor(140d * (det - lvlModifier.Main) / lvlModifier.Div) / 1000d;
        return cVal;
    }

    /// <summary>
    /// 输入技能威力计算当前等级下(目前只有90级准确)的治疗量
    /// </summary>
    /// <param name="strength">技能威力</param>
    /// <returns>治疗量</returns>
    public static unsafe double CalcNormalHeal(uint strength)
    {
        try
        {
            var uiState = UIState.Instance();
            //获得当前等级
            var lvl = uiState->PlayerState.CurrentLevel;
            var levelModifier = LevelTable[lvl];
            int AttackModifier = 115;

            var det = CalcDet(uiState->PlayerState.Attributes[44], ref levelModifier);

            //获得精神属性值 (int)(jobId.IsCaster() ? Attributes.AttackMagicPotency : Attributes.AttackPower)
            var ap = uiState->PlayerState.Attributes[33];
            //装备武器
            var equippedWeapon = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems)->Items[0];
            //获得当前武器
            var weaponItem = Service.DataManager.GetExcelSheet<Item>()?.GetRow(equippedWeapon.ItemID);
            //获得武器基础伤害性能
            var weaponBaseDamage = (weaponItem?.DamageMag) ?? 0;

            //如果是HQ,再做一次计算
            if (equippedWeapon.Flags.HasFlag(InventoryItem.ItemFlags.HQ))
            {
                weaponBaseDamage += (ushort)(weaponItem?.UnkData73.FirstOrDefault(d => d.BaseParamSpecial == 12)?.BaseParamValueSpecial ?? 0);
            }

            //计算武器伤害值
            var weaponDamage = Math.Floor(levelModifier.Main * AttackModifier / 1000.0 + weaponBaseDamage) / 100.0;
            //计算治疗威力
            var healPot = Math.Floor(569.0 * (ap - levelModifier.Main) / 1522.0 + 100) / 100.0;
            //计算获得正常治疗量
            var normalHeal = Math.Floor(Math.Floor(Math.Floor(Math.Floor(strength * healPot * weaponDamage) * (1 + det)) * TraitModifiers(lvl)));

            return normalHeal;
        }
        catch (Exception e)
        {
            PluginLog.Warning(e, "Failed to calculate raw damage");
            return 0;
        }

    }

    /// <summary>
    /// 根据目标血量判断是否要释放技能(单体技能)
    /// </summary>
    /// <param name="chara">队友</param>
    /// <param name="strength">威力</param>
    /// <param name="isTank">坦克的血量比例</param>
    /// <param name="notTank">非坦克的血量比例</param>
    /// <returns></returns>
    public static bool SingleHeal(BattleChara chara, uint strength, double isTank, double notTank)
    {
        var shouldHeal = chara.MaxHp * 0.97 - chara.CurrentHp;
        var healthRatio = chara.GetHealthRatio();

        if (Service.ClientState.LocalPlayer!.Level == 90 && shouldHeal >= CalcNormalHeal(strength)) return true;
        if (healthRatio < isTank && new BattleChara[] { chara }.GetJobCategory(JobRole.Tank).Count() == 1) return true;
        if (healthRatio < notTank) return true;

        return false;
    }

    /*        public static bool AreaHeal(uint strength, double memberHP, double healRatio)
            {

                var shouldHeal = TargetHelper.PartyMembersAverHP ;
                var healthRatio = ;

                if (Service.ClientState.LocalPlayer!.Level == 90 && shouldHeal >= CalcNormalHeal(strength)) return true;
                if (healthRatio < isTank && TargetFilter.GetJobCategory(new BattleChara[] { chara }, Role.防护).Length == 1) return true;
                if (healthRatio < notTank) return true;

                return false;
            }*/

    private static double TraitModifiers(int level)
    {
        return level switch
        {
            >= 40 => 1.3,
            >= 20 => 1.1,
            _ => 1
        };
    }

    internal static readonly SortedList<int, LevelModifier> LevelTable = new() {
        { 1, new(Main: 20, Sub: 56, Div: 56) },
        { 2, new(Main: 21, Sub: 57, Div: 57) },
        { 3, new(Main: 22, Sub: 60, Div: 60) },
        { 4, new(Main: 24, Sub: 62, Div: 62) },
        { 5, new(Main: 26, Sub: 65, Div: 65) },
        { 6, new(Main: 27, Sub: 68, Div: 68) },
        { 7, new(Main: 29, Sub: 70, Div: 70) },
        { 8, new(Main: 31, Sub: 73, Div: 73) },
        { 9, new(Main: 33, Sub: 76, Div: 76) },
        { 10, new(Main: 35, Sub: 78, Div: 78) },
        { 11, new(Main: 36, Sub: 82, Div: 82) },
        { 12, new(Main: 38, Sub: 85, Div: 85) },
        { 13, new(Main: 41, Sub: 89, Div: 89) },
        { 14, new(Main: 44, Sub: 93, Div: 93) },
        { 15, new(Main: 46, Sub: 96, Div: 96) },
        { 16, new(Main: 49, Sub: 100, Div: 100) },
        { 17, new(Main: 52, Sub: 104, Div: 104) },
        { 18, new(Main: 54, Sub: 109, Div: 109) },
        { 19, new(Main: 57, Sub: 113, Div: 113) },
        { 20, new(Main: 60, Sub: 116, Div: 116) },
        { 21, new(Main: 63, Sub: 122, Div: 122) },
        { 22, new(Main: 67, Sub: 127, Div: 127) },
        { 23, new(Main: 71, Sub: 133, Div: 133) },
        { 24, new(Main: 74, Sub: 138, Div: 138) },
        { 25, new(Main: 78, Sub: 144, Div: 144) },
        { 26, new(Main: 81, Sub: 150, Div: 150) },
        { 27, new(Main: 85, Sub: 155, Div: 155) },
        { 28, new(Main: 89, Sub: 162, Div: 162) },
        { 29, new(Main: 92, Sub: 168, Div: 168) },
        { 30, new(Main: 97, Sub: 173, Div: 173) },
        { 31, new(Main: 101, Sub: 181, Div: 181) },
        { 32, new(Main: 106, Sub: 188, Div: 188) },
        { 33, new(Main: 110, Sub: 194, Div: 194) },
        { 34, new(Main: 115, Sub: 202, Div: 202) },
        { 35, new(Main: 119, Sub: 209, Div: 209) },
        { 36, new(Main: 124, Sub: 215, Div: 215) },
        { 37, new(Main: 128, Sub: 223, Div: 223) },
        { 38, new(Main: 134, Sub: 229, Div: 229) },
        { 39, new(Main: 139, Sub: 236, Div: 236) },
        { 40, new(Main: 144, Sub: 244, Div: 244) },
        { 41, new(Main: 150, Sub: 253, Div: 253) },
        { 42, new(Main: 155, Sub: 263, Div: 263) },
        { 43, new(Main: 161, Sub: 272, Div: 272) },
        { 44, new(Main: 166, Sub: 283, Div: 283) },
        { 45, new(Main: 171, Sub: 292, Div: 292) },
        { 46, new(Main: 177, Sub: 302, Div: 302) },
        { 47, new(Main: 183, Sub: 311, Div: 311) },
        { 48, new(Main: 189, Sub: 322, Div: 322) },
        { 49, new(Main: 196, Sub: 331, Div: 331) },
        { 50, new(Main: 202, Sub: 341, Div: 341) },
        { 51, new(Main: 204, Sub: 342, Div: 366) },
        { 52, new(Main: 205, Sub: 344, Div: 392) },
        { 53, new(Main: 207, Sub: 345, Div: 418) },
        { 54, new(Main: 209, Sub: 346, Div: 444) },
        { 55, new(Main: 210, Sub: 347, Div: 470) },
        { 56, new(Main: 212, Sub: 349, Div: 496) },
        { 57, new(Main: 214, Sub: 350, Div: 522) },
        { 58, new(Main: 215, Sub: 351, Div: 548) },
        { 59, new(Main: 217, Sub: 352, Div: 574) },
        { 60, new(Main: 218, Sub: 354, Div: 600) },
        { 61, new(Main: 224, Sub: 355, Div: 630) },
        { 62, new(Main: 228, Sub: 356, Div: 660) },
        { 63, new(Main: 236, Sub: 357, Div: 690) },
        { 64, new(Main: 244, Sub: 358, Div: 720) },
        { 65, new(Main: 252, Sub: 359, Div: 750) },
        { 66, new(Main: 260, Sub: 360, Div: 780) },
        { 67, new(Main: 268, Sub: 361, Div: 810) },
        { 68, new(Main: 276, Sub: 362, Div: 840) },
        { 69, new(Main: 284, Sub: 363, Div: 870) },
        { 70, new(Main: 292, Sub: 364, Div: 900) },
        { 71, new(Main: 296, Sub: 365, Div: 940) },
        { 72, new(Main: 300, Sub: 366, Div: 980) },
        { 73, new(Main: 305, Sub: 367, Div: 1020) },
        { 74, new(Main: 310, Sub: 368, Div: 1060) },
        { 75, new(Main: 315, Sub: 370, Div: 1100) },
        { 76, new(Main: 320, Sub: 372, Div: 1140) },
        { 77, new(Main: 325, Sub: 374, Div: 1180) },
        { 78, new(Main: 330, Sub: 376, Div: 1220) },
        { 79, new(Main: 335, Sub: 378, Div: 1260) },
        { 80, new(Main: 340, Sub: 380, Div: 1300) },
        { 81, new(Main: 345, Sub: 382, Div: 1360) },
        { 82, new(Main: 350, Sub: 384, Div: 1420) },
        { 83, new(Main: 355, Sub: 386, Div: 1480) },
        { 84, new(Main: 360, Sub: 388, Div: 1540) },
        { 85, new(Main: 365, Sub: 390, Div: 1600) },
        { 86, new(Main: 370, Sub: 392, Div: 1660) },
        { 87, new(Main: 375, Sub: 394, Div: 1720) },
        { 88, new(Main: 380, Sub: 396, Div: 1780) },
        { 89, new(Main: 385, Sub: 398, Div: 1840) },
        { 90, new(Main: 390, Sub: 400, Div: 1900) }
    };
}
