# Disclaimer: Action Culling

There's been a lot of actions in here that I've been sort of iffy about ever since I added them; whether they were too cheaty, made things too automatic, or such, since I was mostly following recommendations from friends.
Having talked with moderators on the Dalamud/XIVLauncher discord, I've come to the conclusion that I'll be removing actions that are *too* automated. I know this probably won't go over well with everyone, but ultimately I want to make sure people can use this plugin guilt-free, as it's mostly meant to be what I want my own version of XIVCombo to be.
I apologize for any upset this might cause; it was ultimately something I felt like I had to do, to keep things clean.

# XIVComboPlugin Expandedest
This is an expanded version of Daemitus's XIVCombo Expanded for personal use, that I am modifying for personal use by adding features that me and friends would like to use. These can be used in any fork you like, no credit needed. None of these features will be added to the original XIVCombo, so please do not bug the creator of that about it. Thank you Daemitus, attickdoor, goaaats, and everyone else who contributed to making this plugin possible. Additional thanks to ALymphocyte for coding help!

If you would like to use this plugin, you can get it from this repo: `https://github.com/grammernatzi/MyDalamudPlugins/raw/master/pluginmaster.json`

## Additions added (all optional!)

* MCH/PLD/SMN/etc won't have action upgrade traits break due to unfinished job quests conflicting with XIVCombo features anymore.
* Respective raises on RDM/SMN/SCH/WHM/AST get replaced with Swiftcast when it is off cooldown (and Dualcast isn't up).
* Cure 2 becomes Cure when synced below level 30
* Cure 2 and Medica become Afflatus Solace and Rapture respectively when lilies are available
* Benefic 2 becomes Benefic when synced below level 26
* Minor Arcana becomes Sleeve Draw when a card is not drawn.
* Single-target and AoE combos become Holy Spirit/Circle when Requiescat is up
* Dragon Kick becomes Bootshine while a form is up and Leaden Fist is up.
* AoE Overcap prevention on GNB has also been added to WAR/DRK.
* Burst Strike will now turn into Bloodfest if gauge is empty.
* No Mercy will turn into Bow Shock, and then Sonic Break, while buff is active.*
* Single-target combo and AoE combo on WAR will be replaced by Fell Cleave/Decimate during Inner Release.
* Nascent Flash will turn into Raw Intuition when you are below level 76.
* Meikyo will be replaced by Jinpu/Shifu while its buff is up.
* Shoha will replace Iaijutsu/Tsubame when Meditation gauge is full.
* Tsubame becomes Iaijutsu when Sen gauge isn't empty.
* Fire 1 becomes Fire 3 outside of Astral Fire and when Firestarter proc is up. Also replaces Enochian with Fire 1/3 before you get Fire 4 when in AF (if Enochian is up or you don't have it yet).
* Blizzard 1 becomes Blizzard 3 when outside of Umbral Ice. Freeze now becomes Blizzard 2 when level-synced, as well.
* All Bahamut/Phoenix/Dreadwyrm-related actions on Summoner are now on one button.
* Verstone/Verfire will turn into Veraero/Verthunder when Dualcast/Swiftcast is up, with an additional feature that makes Verfire Verthunder outside of combat for openers.
* When Suiton (or Hidden) is up, Trick Attack will replace Kassatsu.*
* In the same vein, Ten Chi Jin (the move) will turn into Meisui while Suiton is up*.
* Chi is replaced with Jin while Kassatsu is up if you have Enhanced Kassatsu to help with muscle memory.
* Hide becomes Mug while in combat.
* Your combos can become Ninjutsu while you are using Mudra (so you don't have to bind Ninjutsu separately, it doesn't actually do the Mudras for you).
* Bard DoTs alternate between each other and also turn into Iron Jaws when both are up.
* Burst Shot/Quick Nock will turn into Apex Arrow when gauge is full.
* Cascade, Flourish, and both Fan Dances change into dance steps while dancing. This helps ensure you can still dance with combos on, without using auto dance. You can additionally configure which actions you want to be replaced by dance steps, if you so desire.

For actions marked with a *, you may want Remind Me or a similar plugin to keep track of cooldowns while they are invisible.

## About
XIVCombo is a plugin to allow for "one-button" combo chains, as well as implementing various other mutually-exclusive button consolidation and quality of life replacements. Some examples of the functionality it provides:
* Most weaponskill combos are put onto a single button (unfortunately not including MNK, though MNK gets a few good features too!).
* Enochian changes into Fire 4 and Blizzard 4 depending on your stacks, a la PvP Enochian.
* Hypercharge turns into Heat Blast while Overheated.
* Jump/High Jump turns into Mirage Dive while Dive Ready.
* And many, many more!

For some jobs, this frees a massive amount of hotbar space (looking at you, DRG). For most, it removes a lot of mindless tedium associated with having to press various buttons that have little logical reason to be separate.

## Installation
* Add the repo as a custom repo using /xlsettings in-game. Then, type `/xlplugins` in-game to access the plugin installer and updater. 

## In-game usage
* Type `/pcombo` to pull up a GUI for editing active combo replacements.
* Drag the named ability from your ability list onto your hotbar to use.
  * For example, to use DRK's Souleater combo, first check the box, then place Souleater on your bar. It should automatically turn into Hard Slash.
  * The description associated with each combo should be enough to tell you which ability needs to be placed on the hotbar.
  * Make sure you press "Save and close". Don't just X out.
### Examples
![](https://github.com/attickdoor/xivcomboplugin/raw/master/res/souleater_combo.gif)
![](https://github.com/attickdoor/xivcomboplugin/raw/master/res/hypercharge_heat_blast.gif)
![](https://github.com/attickdoor/xivcomboplugin/raw/master/res/eno_swap.gif)
