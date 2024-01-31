# My story about FFXIV and Rotation Solver

Hi, I am ArchiTed. I would like to share my story about FFXIV and Rotation Solver in this video.

## Opportunity

I was a student in China in the autumn of 2021 and I would have graduated in less than a year. I was under pressure from my family to take the postgraduate entrance exam and to get a job at that time. At that point, I had a better idea of where I was in life and became concerned about the fact that my situation didn't match my ambitions. I had hoped that by being in this situation, I would have an outlet to ease my stress.

One day, my roommate asked me if I was interested in online games and recommended several of them, he suggested quite a few games including Final Fantasy 14. I used to play strategy and economic simulation games before, but I didn’t have the ability to react quickly to play other types. Of course, I’m always curious about new things, so I tried this new one out, an MMORPG, with him.

## CN Server

I created an account most easily, in CN server. It was my first time playing an online game with other people. As I started playing, the story and the game’s world made me very excited and attracted me quickly. As a new black mage player, I did really bad DPS and I didn't know much about it. In December 2021, I was in a Dungeon at about lvl.60 – 70 and the healer, who was a mentor said something along the lines that my DPS was really low, and that if you are a new player and don’t know how to play that you shouldn’t play a Black Mage. That really hurt me, so I researched a lot on the Internet and practiced quite a bit on dummies. That helped me figure out the way to get acceptable DPS. However, after a lot of practice, I still forgot to keep the Enochian buff on. Maybe I needed more practice to be better at playing BLM, but I thought that I knew how it worked. I didn't think that hundreds of hours of practice were worth being put into practicing just one class to be decent at playing it. So, was there a way to make sure that I didn't make a mistake?

## Dalamud
In order to find a way to improve my performance in combat, I found a free, open-source plugin for FFXIV, Dalamud, you know. And the first plugin I tried was XIV Combo. For me, it was game-changing, it really helped me a lot. But I still always forgot to switch to the ice phase to regain mana. Several hours after finding it, I still couldn't find a plugin that could accommodate what I wanted from it. As a plugin developer for professional software in C#, why shouldn't I create a plugin on my own?

## AutoAttack

After finding out that I failed the postgraduate entrance examination and my roommate stopped playing FFXIV, I turned my sorrow into strength, forked XIV Combo Expandedest from tssailzz8, and created the prototype of Rotation Solver that was called Auto Attack. The first idea of it was similar to Sloth Combo which I didn't know at that time, that is the one button for the whole rotation and to choose the best target automatically. From March to June 2022, I rarely played duties but played mostly at the dummy in Falcon's Nest. At that time, the progress of my MSQ was lower than level 70. I was afraid of losing the sprout icon and making more mistakes in the duties. 

In June 2022, I finished the development of most jobs’ rotations, and I was very proud of myself. At that time, I didn't even max the level of all of the jobs, so I posted a download link to the GitHub page of this repo. Back then, everyone who could find this repo could get this plugin.

I did not create any channels for feedback from other users between June and September 2022. And because I haven't been able to get feedback, the plugin that I created was only according to the specifications I was thinking of. It also turns the game into a strategy game. I received a lot of commendations, which made me very enjoyable. I had a lot of fun from the fact that I was making my party experience a better one, although I was still a sprout after playing the game for nearly 1 year with no experience in doing high-end duties.

## The environment of CN Server

About one year ago, in late September 2022, I created a QQ group to receive feedback from all users in the CN Server because someone suggested it to me. At that time, I was encountering a lot of people at once. This is also my first time to experience what an MMORPG is. It was then that I learned about the difference between CN Server and the international server. And I also knew about a lot of plug-ins, including some other similar things to Rotation Solver like Sloth Combo, Reborn Buddy, MMO Minion, and so on.

This suddenly made me a little confused. It seemed that I had made a plug-in that someone else had already made, but I didn't know it before. And I basically developed it alone, while most other plug-ins are developed by a team. Based on the way I played this game, I made a guideline for this plug-in, which focuses on the rotation experience of regular duties, giving users a better experience in stories and scripts. That seems to make this plugin unique.

Due to my lack of experience in high-end duties, I would say I have very little understanding of any job. However, some users have a very comprehensive understanding of rotations. So, I strongly encouraged everyone to modify the rotations I wrote. I also added the name of the author to each rotation. Anyone who modifies any part of the rotation can be the rotation author.  And I wrote a document in Chinese about how to create other rotations. For me, I can take the stress out of development and let the responsibility shift to individual rotation authors. What I should do is maintain the platform for rotation development.

Besides, I found that almost every plugin is forked from the International Server. On very rare occasions the plugins are created by the Chinese. The CN server is very different from any other data center. So, I really want to create a team that can create the plugins by themselves. I have to say that at that time I was very nationalistic.

But it backfired. Some of the rotation authors didn't take responsibility and just modified it and changed the rotation author to themselves. In fact, it has increased my development burden a lot. They really don't want to create plugins but they want to ask me to create them. So, I feel like I'm a failure. I failed to make the plugin better and also failed to improve the development level and enthusiasm of Chinese authors. The gradually increasing demands are gradually consuming my enthusiasm, and many demands are inconsistent with the plug-in guidelines. I often have to spend a lot of time explaining it to them.

This period of time can be said to be my dark time. I feel like I was developing plug-ins at work. But I originally developed the plugin for fun. And I had almost 0 time to play the game. The only good news for me was I found a job that suits me very well and I like it very much.

In December 2022, The feeling of not being able to freely change my plugin made me unwilling to continue doing it, so I quit playing on the CN Server.

## JP Server

Although I quit the game, I still got a lot of information about FFXIV. Every time I got a video about FFXIV, I was thinking why I failed to make a great plugin in FFXIV. This plug-in did help me, who always makes mistakes no matter how I play rotations, to complete all the MSQ as well. I am sure that it is valuable.

I thought the main problem was that my expectations for the other people who were saying they wanted to help were too high, and that may also be a problem for the Chinese gaming environment. I have been writing this plugin for over 9 months, and I knew that there were many things worth improving. There is no doubt that I learned a lot of things about .Net Core while developing this plugin. I wanted to have an interesting project to improve my level of programming although I am not majoring in software development. So, I got the motivation to rewrite the whole plugin!

The final reason that made me play the game one more time is the design of the new housing wall, Sukiya-zukuri Cottage Walls. So I got an account and joined the new adventure in the Elemental server!

## Rotation Solver

For a better understanding of the English description, I renamed the plugin to Rotation Solver. It took about one week to rewrite almost all the codes in this plugin in January 2023. And I tested it on my new character.

It was very hard for me to play on the JP server, because there were very few Chinese players here, and I went through the whole MSQ in Chinese, I didn't even know their names in the game! Everything was so similar yet so different.

At the time that I first tested the plugin, I received an invite from a free company. They were very kind and showed me their FC house and invited me to play the game with them. It was my first time playing in the JP Server, and I got the game experience that I hadn't had in the CN server for more than 1 year. This encounter told me that the environment here is completely different from the CN server, and it is very likely that I will get a different outcome doing the same activities. 

## The environment 

From January to October 2023, I steadily updated and developed the plugin. I created a lot of features that the users asked for. I can clearly feel the difference compared to the Chinese game environment. For the feature request as an example, the users in the CN server would give me the feeling that I have to do this, or they can't play the game anymore, and the users in the International server would give me the feeling that just like a suggestion, I can choose would I do it or not. And I felt respected, which was exactly what I wanted in the open-source platform.

For the development of the Rotation Solver, I also created some templates and a Nuget package for rotation development. I made a website to explain to other people how to create the rotations. I redesigned the UI for the users. And so on. I almost rewrote the whole thing.

And now, there are several rotations developers who share their own work. Thanks a lot, to them!

## End

As you can see, the theme of this plugin is non-professional. A player that can't do combat well designed this plugin. A man that not graduated in software development wrote this plugin. That may make this plugin unique. And for me, FFXIV is not my type of game. What interested me is Dalamud and the story of it, instead of the core gameplay, combat. 

For now, I finished the main story and almost all sub-stories. The Rotation Solver was almost done except for the rotation development. So the things that interested me are kind of finished. Well, sadly, FFXIV no longer entices me unless a new story is released or a new version of Dalamud like API 9!

So, I would prefer to play the strategy and economic simulation games, back! 

Thank you so much for watching! This is my first English video. Thank Const Mar for modifying my article. Thank all users who used the Rotation Solver and were nice enough to help support it! I'll see you in version 7.0! peace!

