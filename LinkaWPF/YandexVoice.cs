using System;

namespace LinkaWPF
{
   public class YandexVoice
    {
        public static readonly YandexVoice[] VOICES= new YandexVoice[]
        {
            new YandexVoice("Оксана", "oksana"),
new YandexVoice("Джейн", "jane"),
new YandexVoice("Ома", "omazh"),
new YandexVoice("Захар", "zahar"),
new YandexVoice("Емиль", "ermil"),
new YandexVoice("Филипп", "filipp"),
new YandexVoice("Алена", "alena"),
        };
        string title;
        string id;
        YandexVoice(string title, string id)
        {
            this.title = title;
            this.id = id;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }
        public string Title
        {
            get
            {
                return this.title;
            }
        }

        internal static YandexVoice FindById(string voiceId)
        {
            foreach (var item in VOICES)
            {
                if (item.Id == voiceId) return item;
            }
            return null;
        }

        internal static int FindIndexById(string voiceId)
        {
            for (int i = 0; i < VOICES.Length; i++)
            {
                if (VOICES[i].id == voiceId) return i;
            }
            return 0;
        }
    }
}