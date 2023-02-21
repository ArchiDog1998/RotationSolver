
# [![](docs/RotationSolverIcon_128.png)](https://archidog1998.github.io/RotationSolver/#/) **Rotation Solver**

![Github Latest Releases](https://img.shields.io/github/downloads/ArchiDog1998/RotationSolver/latest/total.svg?style=for-the-badge)
![Github All Releases](https://img.shields.io/github/downloads/ArchiDog1998/RotationSolver/total.svg?style=for-the-badge)
![Github Lines](https://img.shields.io/tokei/lines/github/ArchiDog1998/RotationSolver?style=for-the-badge)
![Github License](https://img.shields.io/github/license/ArchiDog1998/RotationSolver.svg?label=License&style=for-the-badge)
![Github Commits](https://img.shields.io/github/commits-since/ArchiDog1998/RotationSolver/latest/main?style=for-the-badge)

Download it at this url:

`https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/pluginmaster.json`

## Brief

> Based on the pve combat information in one frame, find the best action.

The `information` is almost all the information available in one frame in combat, including the status of the all players in party, the status of the hostile targets, action cooling, the number of action stack, the MP and HP of characters, the location of characters, casting action of the hostile target, combo ID, combat duration, player level, etc. In this case, opener is also a kind of information that a lot of abilities are not cooling down.

Then, it will highlight the best action one the hot bar, or help you to click on it.

It is designed for `general combat`, not for savage or ultimate. So use it carefully.

## Concept

I have to admit, I'm not a good arpg player. But I enjoy the experience of FFXIV. So I want to design a plugin that can improve my gaming experience without affecting other player's gaming experience. So there goes to `Rotation Solver`. 

I have to admit, it does have an automatic component, which might not good for some player. But it can NOT affect any other player's game experience. pvp is absolutely NOT allowed in this plugin.

## Compatibility

literally, `Rotation Solver` helps you to choose the target and then click the action. So any plugin who changes these will affect its decision. 

- [XIVCombo](https://github.com/daemitus/XIVComboPlugin)
- [ReAction](https://github.com/UnknownX7/ReAction)
- etc...

NOTICE: It can't use with [`Block Targeting Treasure Hunt Enemies`](https://github.com/Caraxi/SimpleTweaksPlugin/blob/7e94915afa17ea873d48be2c469ebdaddd2e5200/Tweaks/TreasureHuntTargets.cs) in [Simple Tweaks](https://github.com/Caraxi/SimpleTweaksPlugin). 

I don't know why. I just used the [GetIsTargetable](https://github.com/aers/FFXIVClientStructs/blob/c554a586c4649a472433734b45c59a4bc4979ead/FFXIVClientStructs/FFXIV/Client/Game/Object/GameObject.cs#L71) Method in [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs). If anybody knows why, please tell me.

## Links

If you have any questions about usage, please check the [Wiki](https://archidog1998.github.io/RotationSolver/#/). Wiki is NOT ready yet...

[![Discord](https://discordapp.com/api/guilds/1064448004498653245/embed.png?style=banner2)](https://discord.gg/4fECHunam9)

[![Crowdin](https://badges.crowdin.net/badge/light/crowdin-on-dark.png)](https://crowdin.com/project/rotationsolver)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/H2H5IW7GS)

## 对于国服用户

> 这个库已经不再维护了。
>
> 本插件不得以任何形式在国服中使用。

### 声明
本仓库使用的是[GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)协议，任何人均享有自由分发软件的自由。详看[原文](https://github.com/ArchiDog1998/XIVAutoAction/blob/main/LICENSE#L23-L27)。请所有使用本仓库的作者遵循此协议。

> 不能限制任何人自由分发此软件，不论是否收费！如已有限制，请删除！

### 插件简要历程

| 时间 | 事件 |
| --- | --- |
|2022.03|创立仓库，并完成测试版[黑魔循环](https://www.bilibili.com/video/BV1Ea41147eM/)|
|2022.04|完成基本功能并上传[视频](https://www.bilibili.com/video/BV1ot4y1x7dL/)，原视频被举报删除，这个是后来传的|
|2022.04|插件迎来了第一个用户|
|2022.06|根据B站上的职业教学视频完成所有职业基础循环|
|2022.07|第一次拥有了Dalamud的url，可以自由下载|
|2022.10|受到用户的热情影响，创立QQ交流群|
|2022.11|收到国际服玩家[要求](https://github.com/ArchiDog1998/XIVAutoAction/issues/14)，计划进入国际服并创立Discord交流群|
|2022.12|不堪重负，已无游戏体验，不再维护|

### 添加url的原因

- 期望有足够多人能够一起维护这个插件
- 让我这个手残玩家能用自己写的插件完整的体验一下极神副本。

2022.10时有了某位玩家的热情让我觉得这两个曾经的梦想有可能实现，所以创立了QQ群。

很遗憾，直到我的心力用尽，两个梦想`一个都没实现`。

### 不再维护原因

- 没有足够的人手愿意维持插件，而我精力不够。
- 插件不再是我的插件，不论加入什么新功能都已不再能按照我的游戏体验发展进行，已没有游戏体验。

### 删除Discord和Crowdin的原因

没有任何一个团队或个人有能力和意愿更新和维护本插件，让插件进一步发展。只有个`维持现状`或会`后退`的。

因此，也没有任何必要留给他们我一手创建的资源，也枉费了我想要提供插件`技术支持`的想法，太心寒了。

### 整体原因

- 未能正确理解到大部分用户都是为了玩游戏，不会愿意有责任去维护一个过于复杂的插件。
- 没有认真了解开发者们都想要什么，而不是只是我想要什么。
- 对于插件要求`严苛`，虽然让本插件有了很高的质量，但同时也带来了巨大的维护成本。
- 虽然这点不能完全确认，但是从我的交流体验上来说是这样的，而且也确实影响到了我的维护心态：国人玩家素质普遍还不足。

### 经验教训
- 明确合作开发者的期望、能力、素质与意愿，以更好地协调矛盾并组织完成复杂工程。
- 明确被服务人员的素质，来判断是否有必要为其服务。并非所有判断均需要通过调研获得，如游戏可以根据游戏的性质大致猜测出玩家整体的素质水平。
- 对于产品的严苛要求是必须的，应当考虑好项目的具体分工及其实现方法、可持续发展模式等，这些需要结合合作开发者的期望、能力、素质与意愿。
- 对于未达到合格标准的开发者，不应也不能给予较高的期望。

### 属于AutoAction的价值观

- 插件设计`不能`也`绝不允许`影响到`任何一类`玩家的`任何类型`的游戏体验。
- 提供足够多的`可选项`，满足不同玩家在不同硬件条件下对自动循环的需求。
- 打造生态，降低写循环的难度，让更多的玩家可以加入进来。
- 优秀的开源插件的最大敌人从来都不是贩卖，而是`不负责任`或`能力不足`的开发者。

### 总结

我以满腔的热血与丰满的理想创作本插件，得来的却是无尽且孤独的维护、冷漠且贪婪的用户及自私且慵懒的同僚。
他们一步步蚕食我对最终幻想14的热爱，对AutoAction的期待。我愤怒，我失望，我无力，但我不后悔。

上述内容没有说所有人也没说具体哪个人，请不要对号入座。我只描述了事实，就是这些情况的存在，让我逐步失去了兴致。
别心虚，大概率说的不是你。
