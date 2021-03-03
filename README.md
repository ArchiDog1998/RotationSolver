# XIVComboPlugin Expandedest
This is an expanded version of Daemitus's XIVCombo Expanded for personal use, that I am modifying for personal use by adding features that me and friends would like to use. These can be used in any fork you like, no credit needed. None of these features will be added to the original XIVCombo, so please do not bug the creator of that about it. Thank you Daemitus, attickdoor, goaaats, and everyone else who contributed to making this plugin possible.

If you would like to use this plugin, you can get it from this repo: `https://github.com/grammernatzi/MyDalamudPlugins/raw/master/pluginmaster.json`

## Additions added (all optional!)

* MCH/PLD/SMN/etc won't have action upgrade traits break due to unfinished job quests conflicting with XIVCombo features anymore.
* Cure 2 becomes Cure when synced below level 30
* Cure 2 and Medica become Afflatus Solace and Rapture respectively when lilies are available
* Benefic 2 becomes Benefic when synced below level 26
* Minor Arcana becomes Sleeve Draw when a card is not drawn.
* Single-target and AoE combos become Holy Spirit/Circle when Requiescat is up
* Holy Spirit/Circle become Confiteor when MP is lower than 4000 and Requiescat is up
* Dragon Kick becomes Bootshine while a form is up and Leaden Fist is up.
* Demolish will turn into snap punch when Demolish's DoT is above 6s.
* Four-point Fury is selected in MNK's AoE combo if Perfect Balance or Form Shift is up and Twin Snakes is below 4s. 
* Twin Snakes will be selected in MNK's AoE combo instead of Four-Point Fury if the buff is not up.
* Overcap prevention on GNB now extends to single-target combo and has also been added to WAR/DRK.
* A second overcap prevention has been added for Infuriate/Bloodfest.
* An MP overcap prevention that involves weaving a CD before syphon strike/stalwart soul has been added.
* No Mercy will turn into Bow Shock, and then Sonic Break, while buff is active.*
* Single-target combo and AoE combo on WAR will be replaced by Fell Cleave/Decimate during Inner Release to prevent wasting GCDs.
* Meikyo will be replaced by Jinpu/Shifu while its buff is up.
* Shoha will replace Iaijutsu/Tsubame when Meditation gauge is full.
* Tsubame becomes Iaijutsu when Sen gauge isn't empty. Also, this feature deletes Kaeshi: Higanbana when active since it is literally the most useless action in the game, so you cannot accidentally use it.
* Enochian/Fire 4 will be replaced with either Fire 1 and 3 depending on remaining time and firestarter proc.
* Despair will replace Enochian/Fire 4 when MP is below 2400.
* Thunder (3) will replace Enochian/Fire 4/Blizzard 4 when Thunderbutt is up and either Thunderbutt or Thunder (3) is about to run out, assuming it will not interrupt UI/AF upkeep.
* Fire 3 becomes Fire 1 when in Astral Fire when no Firestarter proc is up. Also replaces Enochian with Fire 3/1 before you get Fire 4 when in AF (if Enochian is up or you don't have it yet).
* Blizzard 3 becomes Blizzard 1 when synced below requirement level, same for Freeze and Blizzard 2.
* Egi Assaults become Ruin IV when capped on Further Ruin if Ifrit-Egi is out and level applies.
* All Bahamut/Phoenix/Dreadwyrm-related actions on Summoner are now on one button. As it should be.
* Red Mage Verholy/Verflare/Scorch now part of redoublement combo (separate option), based on gauge and procs.
* Verstone/Verfire will turn into Veraero/Verthunder when Dualcast/Swiftcast is up, with an additional feature that makes Verfire Verthunder outside of combat for openers.
* When Suiton (or Hidden) is up, Trick Attack will replace Kassatsu.*
* In the same vein, Ten Chi Jin (the move) will turn into Meisui while Suiton is up*.
* Chi is replaced with Jin while Kassatsu is up if you have Enhanced Kassatsu to help with muscle memory.
* Hide becomes Mug while in combat.
* One button DoT on Bard! Now all your DoTs are on the Iron Jaws button, and when both are applied, it becomes Iron Jaws. Best of all, this is per your active target, so you can DoT multiple targets just fine!
* Burst Shot/Quick Nock will turn into Apex Arrow when gauge is full.

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
