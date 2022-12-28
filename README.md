
# AutoAction

[![Github Latest Releases](https://img.shields.io/github/downloads/moewcorp/AutoAction/latest/total.svg?label=最新版本下载量&style=for-the-badge)]()
[![Github All Releases](https://img.shields.io/github/downloads/moewcorp/AutoAction/total.svg?label=总下载量&style=for-the-badge)]()
[![Github Lines](https://img.shields.io/tokei/lines/github/moewcorp/AutoAction?label=总行数&style=for-the-badge)]()
[![Github License](https://img.shields.io/github/license/moewcorp/AutoAction.svg?label=开源协议&style=for-the-badge)]()

本仓库为 [XIVAutoAction](https://github.com/ArchiDog1998/XIVAutoAction) 的一个Fork，通过社区的方式进行继续维护与开发。



Talk about it on [Discord](https://discord.gg/nfzmJ6ujDP) For Player not in Chinese Server.

Download this plugin on this url.

`https://raw.githubusercontent.com/moewcorp/AutoAction/master/pluginmaster.json`

Translation is on [Crowdin](https://crowdin.com/project/xivautoattack)

中文玩家可以加入[Discord](https://discord.gg/awuCKbxR6q)参与讨论。

如果你喜欢这个插件，可以在这个目录中下载它: 

`https://raw.githubusercontent.com/moewcorp/AutoAction/master/pluginmaster_CN.json`

QQ交流群：`913297282`，注意，入群问题中的下载量并非上方标签的下载量，请看Dalamud中显示的数值

## 插件概况

本插件提供全职业的PVE的自动攻击，可以自动找最优目标，并提供循环教育模式。

**不**包含未来也**不**会提供任何PVP功能。

![案例](gifs/ExampleDNC.gif)

## 设计原则

> 不降低任何玩家群体的游戏体验。
>
> 不能让服务器看到任何异常。

为达到上述目标最需要保证会因为自动循环而降低游戏体验的群体：`手打玩家`。

手打玩家中会有一部分重视自己在游戏中的各方面表现，如果自动循环能够非常轻易的超过手打玩家的游戏表现，那么就会让一部分纯手打玩家失去手打的乐趣，这不是本插件所期望达到的。 

其次，本插件能做到的功能手动操作`均可复现`。

### 设计宗旨

为此，需要保证所做的插件`不能超过`手打的表现，只能保证游戏表现的`兜底`。

所以对于本插件的循环的要求，仅仅有以下指标：

- 基本满足[警察网](https://xivanalysis.com/)对于循环的要求
- 能打爆除了当前版本以外的`所有等级同步木桩`
- 任何等级下，没有必要能力技`空转`
- 会灵活`切换`AOE和单体攻击适应各种战斗环境
- 能够一定程度上的`自动奶`和`自动上盾`，但不保证不溢出
- 可以随时`调整战术`以及`屏蔽`自动循环

为保证游戏表现不能过高，必须做到以下限制：

- 不能为`任何副本`单独做轴

- 不能精确到每个技能的单独控制

  需要一定的模糊度以区分出自动的整体表现不如手动

本插件几乎所有循环都是按照[NGA-职业攻略](https://nga.178.com/thread.php?fid=-362960)种的循环设计的，能基本做到输出无压力。如果还觉得输出有困难，那多半是在玩`零式`或`绝本`。那么作者非常建议手打，总需要有副本给予FF14`原本的体验`。

### 适用人群
- 不想自己打循环，但是想要`体验副本机制`的玩家。
  - 日常刷日随、幻化的玩家
  - 每周清个CD摸摸鱼的玩家
- 想要如何`学习循环`的玩家
  - 刚刚接手一个职业不会玩，想了解怎么打
  - 练习循环看攻略麻烦，想要哪儿亮了点哪里的玩家



## 循环开发

如果您对本插件的循环不是很满意或者想要写出自己的循环，可以参考[Wiki](https://github.com/moewcorp/AutoAction/wiki)学习如何开发。

