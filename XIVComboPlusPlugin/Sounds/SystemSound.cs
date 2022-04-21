//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Speech.Synthesis;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace XIVComboPlus
//{
//    internal class SystemSound
//    {
//        private readonly SpeechSynthesizer speechSynthesizer;

//        private readonly Thread soundThread;

//        public SystemSound()
//        {
//            //speechSynthesizer = new SpeechSynthesizer();
//            this.soundThread = new Thread(PlaySoundLoop);
//            this.soundThread.Start();
//        }

//        private void PlaySoundLoop()
//        {
//            //while (active)
//            //{
//            //    this.currentItem = TryDequeue();
//            //    if (this.currentItem == null)
//            //    {
//            //        Thread.Sleep(100);
//            //        continue;
//            //    }
//            //    OnSoundLoop(this.currentItem);
//            //    this.currentItem.Dispose();
//            //    this.currentItem = null;
//            //}
//        }

//        //protected void OnSoundLoop(SystemSoundQueueItem nextItem)
//        //{

//        //    var ssml = this.lexiconManager.MakeSsml(nextItem.Text, this.speechSynthesizer.Voice.Culture.IetfLanguageTag);
//        //    PluginLog.Log(ssml);

//        //    this.speechSynthesizer.SpeakSsmlAsync(ssml);

//        //    // Waits for the AutoResetEvent lock in the callback to fire.
//        //    this.speechCompleted.WaitOne();
//        //}
//    }
//}
