using System;

namespace XIVComboExpandedestPlugin
{
    public enum CustomComboPreset
    {
        // Last enum used: 98
        // Unused enums: 73, 76, 77, 83, 86, 92
        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("跳跃 - 幻象冲", "处于幻象冲预备状态下，替换跳跃为幻象冲", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 44,

        [CustomComboInfo("苍天龙血 - 坠星冲", "当处于红莲龙血状态下,替换苍天龙血为坠星冲", DRG.JobID, DRG.BloodOfTheDragon)]
        DragoonBOTDFeature = 46,

        [CustomComboInfo("山境酷刑连击", "替换山境酷刑为相应的连击", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 0,

        [CustomComboInfo("樱花怒放连击", "替换樱花怒放为相应的连击", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 1,

        [CustomComboInfo("直刺连击", "替换直刺为相应的连击", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 2,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("噬魂斩连击", "替换噬魂斩为相应的连击", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 3,

        [CustomComboInfo("刚魂连击", "替换刚魂为相应的连击", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 4,

        [CustomComboInfo("血乱状态", "当开启前面连击你又处于血乱状态时，替换血乱或血贱", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DeliriumFeature = 57,
        
        [CustomComboInfo("嗜血状态", "当你开启前面连击时候，嗜血大于70自动帮你替换血乱或血贱./n可以自定义替换数值，当输入的值小于50时候为50", DRK.JobID, DRK.StalwartSoul)]
        DRKOvercapFeature = 71,

        [CustomComboInfo("暗黑buff", "当你魔量大于8000时替换暗影峰或者暗影波动", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DRKMPOvercapFeature = 73,

        [CustomComboInfo("弗雷状态", "弗雷CD好时候替换在相应连击上", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DRKFoLeiFeature = 107,
        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("沥血剑连击", "替换沥血剑为相应的连击", PLD.JobID, PLD.GoringBlade)]
        PaladinGoringBladeCombo = 5,

        [CustomComboInfo("王权剑连击", "替换 王权剑/战女神之怒为相应的连击", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone)]
        PaladinRoyalAuthorityCombo = 6,

        [CustomComboInfo("赎罪剑状态", "当前面开启王权剑连击时处于赎罪剑状态时候，赎罪剑替换王权剑连击", PLD.JobID, PLD.RoyalAuthority)]
        PaladinAtonementFeature = 59,

        [CustomComboInfo("日珥斩连击", "替换日珥斩为相应的连击", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 7,

        [CustomComboInfo("安魂祈祷 - 悔罪", "当处于安魂祈祷状态下,替换安魂祈祷为悔罪", PLD.JobID, PLD.Requiescat)]
        PaladinRequiescatCombo = 55,

        [CustomComboInfo("安魂状态", "当开启前面安魂连时，处于安魂状态下圣灵替换相应连击.", PLD.JobID, PLD.HolyCircle,PLD.HolySpirit,PLD.RoyalAuthority, PLD.GoringBlade, PLD.Prominence)]
        PaladinRequiescatFeature = 63,

        [CustomComboInfo("赎罪状态", "当你魔量低于4000时候，自动替换赎罪.", PLD.JobID, PLD.HolySpirit, PLD.HolyCircle)]
        PaladinConfiteorFeature = 68,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("暴风斩连击", "替换暴风斩为相应的连击", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 8,

        [CustomComboInfo("暴风碎连击", "替换暴风碎为相应的连击", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 9,

        [CustomComboInfo("秘银暴风连击", "替换秘银暴风为相应的连击", WAR.JobID, WAR.MythrilTempest)]
        WarriorMythrilTempestCombo = 10,

        [CustomComboInfo ("兽魂量普状态", "兽魂量普快满时暴风战连自动替换相应技能", WAR.JobID, WAR.MythrilTempest, WAR.StormsEye, WAR.StormsPath)]
        WarriorGaugeOvercapFeature = 67,

        [CustomComboInfo("战壕量普替换", "兽魂量普快满时战壕自动替换相应技能", WAR.JobID,WAR.Infuriate)]
        WarriorInfuriateOvercapFeature = 104,

        [CustomComboInfo("暴风buff", "如果暴风时间小于15s并且开启前面连击最后下替换成暴风碎", WAR.JobID, WAR.StormsPath)]
        WARBuffpFeature = 105,

        [CustomComboInfo("原初的解放状态", "原初解放状态时候用裂石飞环/地毁人亡替换相应连击", WAR.JobID, WAR.MythrilTempest, WAR.StormsPath)]
        WarriorInnerReleaseFeature = 69,

        [CustomComboInfo("原初的勇猛状态", "同步到76以下时候原初的直觉替换原初的勇猛", WAR.JobID, WAR.NascentFlash)]
        WarriorNascentFlashFeature = 96,

        [CustomComboInfo("动乱状态", "在开启前面连击情况下动乱CD好了替换相应连击", WAR.JobID, WAR.StormsPath,WAR.StormsEye)]
        WarriorDongLuanFeature = 109,
        #endregion
        // ====================================================================================
        #region SAMURAI

        [CustomComboInfo("雪风连击", "替换雪风为相应的连击", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 11,

        [CustomComboInfo("月光连击", "替换月光为相应的连击", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 12,

        [CustomComboInfo("花车连击", "替换花车为相应的连击", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 13,

        [CustomComboInfo("满月连击", "替换满月为相应的连击", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 14,

        [CustomComboInfo("樱花连击", "替换樱花为相应的连击", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 15,

        [CustomComboInfo("星眼 - 心眼", "当没有触发时，替换星眼为心眼", SAM.JobID, SAM.Seigan)]
        SamuraiThirdEyeFeature = 51,

        [CustomComboInfo("阵风/士风状态", "当正风/士风哪个需要时替换明镜止水.", SAM.JobID, SAM.MeikyoShisui)]
        SamuraiJinpuShifuFeature = 72,

        [CustomComboInfo("照破状态", "当剑压满时照破替换居合术/燕回反.", SAM.JobID, SAM.Iaijutsu, SAM.Tsubame)]
        SamuraiShohaFeature = 74,

        [CustomComboInfo("燕回反状态", "当闪在提高时用燕回反替换居合术.", SAM.JobID, SAM.Tsubame)]
        SamuraiTsubameFeature = 91,

        #endregion
        // ====================================================================================
        #region NINJA

        [CustomComboInfo("强甲破点突连击", "替换强甲破点突为相应的连击", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushCombo = 17,

        [CustomComboInfo("旋风刃连击", "替换旋风刃为相应的连击", NIN.JobID, NIN.AeolianEdge)]
        NinjaAeolianEdgeCombo = 18,

        [CustomComboInfo("八卦无刃杀连击", "替换八卦无刃杀为相应的连击", NIN.JobID, NIN.HakkeMujinsatsu)]
        NinjaHakkeMujinsatsuCombo = 19,

        [CustomComboInfo("梦幻三段 - 断绝", "当处于断绝预备状态下，替换梦幻三段为断绝", NIN.JobID, NIN.DreamWithinADream)]
        NinjaAssassinateFeature = 45,

        [CustomComboInfo("生杀予夺背刺替换", "背刺时替换生杀予夺.", NIN.JobID, NIN.Kassatsu)]
        NinjaKassatsuTrickFeature = 81,

        [CustomComboInfo("天地人替换命水", "天地人完后命水替换天地人.", NIN.JobID, NIN.TenChiJin)]
        NinjaTCJMeisuiFeature = 82,

        [CustomComboInfo("生杀时天地印", "当你有生杀予夺buff时候把地之印替换成人之印.", NIN.JobID, NIN.Chi)]
        NinjaKassatsuChiJinFeature = 89,

        [CustomComboInfo("夺取替换隐遁", "当你战斗时夺取替换隐遁.", NIN.JobID, NIN.Hide)]
        NinjaHideMugFeature = 90,

        [CustomComboInfo("AOE连状态", "如果在结印忍术替换AOE连击.", NIN.JobID, NIN.AeolianEdge)]
        NinjaNinjutsuFeature = 97,

        [CustomComboInfo("GCD连状态", "如果在结印忍术替换GCD连击", NIN.JobID, NIN.AeolianEdge, NIN.ArmorCrush, NIN.HakkeMujinsatsu)]
        NinjaGCDNinjutsuFeature = 98,

        [CustomComboInfo("分身", "分身不在CD时候替换六道", NIN.JobID, NIN.LiuDao)]
        NinjaFenShenFeature = 106,

        [CustomComboInfo("忍气状态", "如果忍气大于40六道替换夺取和大于50六道替换命水", NIN.JobID, NIN.Mug,NIN.Meisui)]
        NinjaLiangPuFeature = 108,

        [CustomComboInfo("自动忍术", "使用天之印来自动忍术", NIN.JobID, NIN.Tian)]
        NinjaRenShuFeature = 117,
        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("迅连斩连击", "替换迅连斩为相应的连击", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 20,

        [CustomComboInfo("凶禽爪连击", "替换凶禽爪为相应的连击", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCombo = 21,

        [CustomComboInfo("凶禽爪 - 续剑", "除了凶禽爪连击, 替换凶禽爪为续剑", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCont = 52,

        [CustomComboInfo("恶魔杀连击", "替换恶魔杀为相应的连击", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 22,

        [CustomComboInfo("晶壤状态", "当子弹快满时候自动替换在单体或AOE连上", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerGaugeOvercapFeature = 30,

        [CustomComboInfo("血壤状态", "当你晶壤溢出时用爆发击替换血壤.", GNB.JobID, GNB.BurstStrike)]
        GunbreakerBloodfestOvercapFeature = 70,

        [CustomComboInfo("无情状态", "用弓形冲波替换无情, 然后用音速破替换, 当无情被激活时.", GNB.JobID, GNB.NoMercy)]
        GunbreakerNoMercyFeature = 66,

        [CustomComboInfo("一键连击", "枪刃一键gcd连.", GNB.JobID, GNB.NoMercy)]
        GunbreakerZiDongeature = 114,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("狙击弹连击", "替换狙击弹为相应的连击", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistMainCombo = 23,

        [CustomComboInfo("散射(过热)", "在过热状态下，替换散射为自动弩", MCH.JobID, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 24,

        [CustomComboInfo("热冲击(过热)", "在过热状态下，替换超荷为热冲击", MCH.JobID, MCH.HeatBlast, MCH.AutoCrossbow)]
        MachinistOverheatFeature = 47,

        [CustomComboInfo("机器人状态", "当机器人被激活时候用冲击替换车式浮空炮塔和后式自走人偶", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverdriveFeature = 58,

        [CustomComboInfo("虹吸弹-弹射状态", "如果弹射有多余充能弹射替换虹吸弹.", MCH.JobID, MCH.GaussRound)]
        MachinistGaussRicochetFeature = 95,

        [CustomComboInfo("机工一键", "开启狙击弹连击后的一键gcd连击", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistZiDongFeature = 113,
        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [CustomComboInfo("天语-冰澈/炽炎", "根据相应状态，替换天语为冰澈/炽炎", BLM.JobID, BLM.Enochian)]
        BlackEnochianFeature = 25,

        [CustomComboInfo("灵极魂/星灵移位", "当灵极魂可用时，替换星灵移位为灵极魂", BLM.JobID, BLM.Transpose)]
        BlackManaFeature = 26,

        [CustomComboInfo("魔纹步/黑魔纹", "当黑魔纹激活时，替换黑魔纹为魔纹步", BLM.JobID, BLM.LeyLines)]
        BlackLeyLines = 56,

        [CustomComboInfo("绝望状态", "当低低于2400MP时绝望火替换火4.\n天语状态必须被激活.", BLM.JobID, BLM.Enochian)]
        BlackDespairFeature = 77,

        [CustomComboInfo("雷状态", "当雷云buff准备就绪和雷buff或者雷1/3快消失时候，雷1/3替换天语/火4/冰4, 假设不会中断计时器.\n天语状态必须被激活.", BLM.JobID, BLM.Enochian)]
        BlackThunderFeature = 86,

        [CustomComboInfo("火1/3状态", "如果星极火剩6秒就是火1，如果是3秒就是火3.\n天语状态必须被激活.", BLM.JobID, BLM.Enochian)]
        BlackFireFeature = 76,

        [CustomComboInfo("火3-火1状态", "挡在星极火时候火苗buff准备好时火1替换火3.\n当然在你在星极火时火4前火1/3替换了天语 (如果天语你没有丢失).", BLM.JobID, BLM.Enochian, BLM.Fire)]
        BlackFire3Feature = 87,

        [CustomComboInfo("玄极冰状态", "当灵极冰时候冰3替换冰1, 同样对于冰2.", BLM.JobID, BLM.Blizzard, BLM.Freeze)]
        BlackBlizzardFeature = 88,

        #endregion
        // ====================================================================================
        #region ASTROLOGIAN

        [CustomComboInfo("抽卡/出卡", "没有卡被抽出时，替换出卡为抽卡", AST.JobID, AST.Play)]
        AstrologianCardsOnDrawFeature = 27,

        [CustomComboInfo("等级同步吉星替换福星", "等级同步时吉星替换福星.", AST.JobID, AST.Benefic2)]
        AstrologianBeneficFeature = 62,

        [CustomComboInfo("小奥秘卡 - 袖内抽卡", "当没有抽卡时袖内抽卡替换小奥秘卡.", AST.JobID, AST.MinorArcana)]
        AstrologianSleeveDrawFeature = 75,

        [CustomComboInfo("dot状态", "当目标dot时间小于3秒时，dot替换煞星/n不能煞星状态与状态一起使用", AST.JobID,AST.凶星,AST.灾星,AST.煞星,AST.祸星 )]
        ASTdotFeature = 103,

        [CustomComboInfo("煞星状态", "当目标身上dot大于3s时候dot替换111/n不能与dot状态一起使用", AST.JobID, AST.烧灼, AST.炽灼, AST.焚灼)]
        AST111Feature = 110,

        [CustomComboInfo("学派状态", "当自己是什么学派时候替换成另一个学派,在非战斗状态中间流派替换占卜", AST.JobID, AST.白昼学派,AST.黑夜学派,AST.占卜)]
        ASTSectFeature = 111,

        [CustomComboInfo("阳星状态", "当身上阳星相应dot大于5s时候替换成阳星", AST.JobID, AST.阳星相位_白)]
        ASTYangXingFeature = 119,
        #endregion
        // ====================================================================================
        #region SUMMONER

        [CustomComboInfo("亚灵神召唤整合", "整合龙神附体, 龙神召唤, 不死鸟附体为一个按键.\n整合死星核爆, 龙神迸发, 不死鸟迸发为一个按键", SMN.JobID, SMN.DreadwyrmTrance, SMN.Deathflare)]
        SummonerDemiCombo = 28,

        [CustomComboInfo("灵泉连击", "在处于灵泉状态下，替换灵泉之炎为炼狱之炎", SMN.JobID, SMN.Ruin1, SMN.Ruin3)]
        SummonerBoPCombo = 38,

        [CustomComboInfo("能量吸收-溃烂爆发", "太超流未被消耗完时，替换溃烂爆发为能量吸收", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 39,

        [CustomComboInfo("能量抽取-痛苦核爆", "以太超流未被消耗完时，替换痛苦核爆为能量抽取", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 40,

        [CustomComboInfo("灵攻1/2 -毁绝4状态", "当毁绝4状态满时毁绝4替换灵攻1/2.", SMN.JobID, SMN.EgiAssault, SMN.EgiAssault2)]
        SummonerRuinIVFeature = 92,

        [CustomComboInfo("亚灵神召唤整合加强", "龙神附体, 龙神召唤, 不死鸟附体, 死星核爆, 龙神迸发, 不死鸟迸发为一个按键.\n需要前面整合开启.", SMN.JobID, SMN.DreadwyrmTrance)]
        SummonerDemiComboUltra = 93,

        [CustomComboInfo("灵功状态", "替换灵攻1使灵攻1和灵攻2CD差不多.", SMN.JobID, SMN.EgiAssault)]
        SummonerLingGongFeature = 121,

        #endregion
        // ====================================================================================
        #region SCHOLAR

        [CustomComboInfo("异想的祥光/慰藉", "当炽天使被召唤时，替换异想的祥光为慰藉", SCH.JobID, SCH.FeyBless)]
        ScholarSeraphConsolationFeature = 29,

        [CustomComboInfo("能量吸收 - 以太超流", "零档以太超流时,替换能量吸收为以太超流", SCH.JobID, SCH.EnergyDrain)]
        ScholarEnergyDrainFeature = 37,

        [CustomComboInfo("dot状态", "当目标dot时间小于3秒时，dot替换炎法", SCH.JobID, SCH.毁灭,SCH.气炎法,SCH.魔炎法)]
        SCHDotFeature = 101,

        [CustomComboInfo("应急状态", "当身上有应急或者在CD时候替换成群盾", SCH.JobID, SCH.应急战术)]
        SCHYingJitFeature = 120,

        #endregion
        // ====================================================================================
        #region DANCER

        [CustomComboInfo("扇舞连击", "当扇舞·急预备时，替换扇舞·序和扇舞·破为扇舞·急", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 33,

        [CustomComboInfo("跳舞连击", "自动跳舞", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepCombo = 31,

        [CustomComboInfo("百花争艳进程保存", "在使用后所有触发技能替换成百花争艳成", DNC.JobID, DNC.Flourish)]
        DancerFlourishFeature = 34,

        [CustomComboInfo("单体连击", "单体连击", DNC.JobID, DNC.Cascade)]
        DancerSingleTargetMultibutton = 43,

        [CustomComboInfo("AOE连击", "AOE连击", DNC.JobID, DNC.Windmill)]
        DancerAoeMultibutton = 50,

        [CustomComboInfo("舞步状态", "当跳舞时候把技能替换成舞步." +
            "\n这有助于确保你仍然可以使用跳舞连招，而不使用自动舞蹈." +
            "\n您可以通过为每个舞步输入下面的技能id来更改相应的舞步." +
            "\n默认的是倾泻，百花争艳，扇舞·序，扇舞·破，如果设置为0，会重置成这样." +
            "\n可以在Garland Tools(http://www.garlandtools.org/)通过搜索点击相应齿轮相应技能id.", DNC.JobID, DNC.Cascade, DNC.Flourish, DNC.FanDance1, DNC.FanDance2)]
        DancerDanceComboCompatibility = 64,

        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [CustomComboInfo("安慰/苦难之心", "当苦难之心可以使用时，替换安慰之心为苦难之心", WHM.JobID, WHM.AfflatusSolace)]
        WhiteMageSolaceMiseryFeature = 35,

        [CustomComboInfo("狂喜/苦难之心", "当苦难之心可以使用时，替换狂喜之心为苦难之心", WHM.JobID, WHM.AfflatusRapture)]
        WhiteMageRaptureMiseryFeature = 36,

        [CustomComboInfo("等级同步时救疗 -治疗", "等级同步时治疗替换成救疗.", WHM.JobID, WHM.Cure2)]
        WhiteMageCureFeature = 60,

        [CustomComboInfo("百合状态", "当拜火满时安慰之心替换救疗，狂喜替换医治", WHM.JobID, WHM.Cure2, WHM.Medica)]
        WhiteMageAfflatusFeature = 61,

        [CustomComboInfo("dot状态", "当目标dot时间小于3秒时，dot替换闪耀", WHM.JobID, WHM.Stone,WHM.Glare,WHM.StoneFour,WHM.StoneThree,WHM.StoneTwo)]
        WhiteStoneFeature = 100,


        [CustomComboInfo("医济状态", "当身上医济dot时间大于5秒时，医治替换医济", WHM.JobID, WHM.医济)]
        WhiteYiJiFeature = 118,


        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("放浪神 - 完美音调", "当处于放浪神的小步舞曲状态下，替换放浪神的小步舞曲为完美音调", BRD.JobID, BRD.WanderersMinuet)]
        BardWandererPPFeature = 41,

        [CustomComboInfo("强力射击 - 直线射击", "当触发时，替换强力射击/爆发射击为直线射击/辉煌箭", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 42,

        [CustomComboInfo("单人dot", "当2个毒没消失的时候烈毒咬箭/狂风蚀箭替换伶牙俐齿。/n适用于同步情况，如果伶牙俐齿不能使用请两个版本交替试.", BRD.JobID, BRD.IronJaws)]
        BardIronJawsFeature = 84,

        [CustomComboInfo("爆发射击/连珠箭 - 绝峰箭", "当量普满时绝峰箭替换爆发射击/连珠箭.", BRD.JobID, BRD.BurstShot, BRD.QuickNock)]
        BardApexFeature = 85,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("AoE连击", "替换地烈劲为相应的AoE连击,当震脚可用时，替换为地烈劲", MNK.JobID, MNK.Rockbreaker)]
        MnkAoECombo = 54,

        [CustomComboInfo("武僧连接状态", "连击替换双龙脚如果有连击buff.", MNK.JobID, MNK.DragonKick)]
        MnkBootshineFeature = 65,

        [CustomComboInfo("武僧破碎拳状态", "破碎拳替换崩拳如果破碎拳被激活并且剩余持续时间超过6s.", MNK.JobID, MNK.SnapPunch)]
        MnkDemolishFeature = 83,

        [CustomComboInfo("金刚极意状态", "真北替换金刚极意使二者CD差不多.", MNK.JobID, MNK.金刚极意)]
        MnkShenWeiFeature =115,

        [CustomComboInfo("震脚状态", "在震脚时候打双龙-连击，没有buff补buff.", MNK.JobID, MNK.DragonKick,MNK.Bootshine,MNK.双掌,MNK.正拳,MNK.SnapPunch,MNK.Demolish)]
        MnkZhenJiaoFeature = 116,

        #endregion
        // ====================================================================================
        #region RED MAGE

        [CustomComboInfo("AoE连击", "当连续咏唱/即刻咏唱可用时，替换赤烈风/赤震雷为冲击", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 48,

        [CustomComboInfo("魔连攻连击", "替换魔连攻为相应的连击", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeCombo = 49,

        [CustomComboInfo("魔连攻Plus", "增加赤神圣/赤核爆/焦热在魔连攻连击，取决于进程和量普. \n要求魔连攻连击", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeComboPlus = 78,

        [CustomComboInfo("赤火/石 - 震荡", "没有触发时，替换赤飞石/赤火炎为震荡/焦热.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 53,

        [CustomComboInfo("赤火/石 - 震荡 Plus", "另外如果连续咏唱/即刻发动赤飞石/赤火炎替换赤疾风/赤闪雷\n要求赤火/石 - 震荡.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocComboPlus= 79,

        [CustomComboInfo("赤火/石 - 震荡 Plus 开启功能", "当退出战斗让赤疾风替换赤火焰.\n要求赤火/石 - 震荡 Plus.", RDM.JobID, RDM.Verfire)]
        RedMageVerprocOpenerFeature = 80,

        [CustomComboInfo("震荡状态", "在相应buff下使用相应技能使魔元差不多.", RDM.JobID, RDM.Jolt,RDM.Jolt2)]
        RedMageZiDONGFeature = 112,

        #endregion
        // ====================================================================================
        #region DISCIPLE OF MAGIC

        [CustomComboInfo("当有即刻BUFF", "当即刻没冷却时候(并且没有连续咏唱buff)分别替换赤魔/召唤/学者/白魔/占星的复活.", DoM.JobID, WHM.Raise, ACN.Resurrection, AST.Ascend, RDM.Verraise)]
        DoMSwiftcastFeature = 94,

        #endregion
        // ====================================================================================
    }

    internal class SecretCustomComboAttribute : Attribute { }

    internal class CustomComboInfoAttribute : Attribute
    {
        internal CustomComboInfoAttribute(string fancyName, string description, byte jobID, params uint[] abilities)
        {
            FancyName = fancyName;
            Description = description;
            JobID = jobID;
            Abilities = abilities;
        }

        public string FancyName { get; }
        public string Description { get; }
        public byte JobID { get; }
        public string JobName => JobIDToName(JobID);
        public uint[] Abilities { get; }

        private static string JobIDToName(byte key)
        {
            return key switch
            {
                1 => "剑术师",
                2 => "格斗家",
                3 => "斧术师",
                4 => "枪术师",
                5 => "弓箭手",
                6 => "幻术师",
                7 => "咒术师",
                8 => "刻木匠",
                9 => "锻铁匠",
                10 => "铸甲匠",
                11 => "雕金匠",
                12 => "制革匠",
                13 => "裁衣匠",
                14 => "炼金术士",
                15 => "烹调师",
                16 => "采矿工",
                17 => "园艺工",
                18 => "捕鱼人",
                19 => "骑士",
                20 => "武僧",
                21 => "战士",
                22 => "龙骑士",
                23 => "诗人",
                24 => "白魔法师",
                25 => "黑魔法师",
                26 => "秘术师",
                27 => "召唤师",
                28 => "学者",
                29 => "双剑师",
                30 => "忍者",
                31 => "机工士",
                32 => "暗黑骑士",
                33 => "占星术士",
                34 => "武士",
                35 => "赤魔法师",
                36 => "青魔法师",
                37 => "绝枪战士",
                38 => "舞者",
                99 => "魔法导师",
                _ => "Unknown",
            };
        }
    }
}
