using Microsoft.DirectX.AudioVideoPlayback;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF
{
    public class Player
    {
        private readonly YandexSpeech _yandexSpeech;

        public Player(YandexSpeech yandexSpeech)
        {
            _yandexSpeech = yandexSpeech;
        }

        public async void Play(IList<Models.Card> cards)
        {
            if (cards == null) return;

            // Получаем кэшированый список с аудиофайлами и воспроизводим его
            IList<string> cachedAudio = await Cache(cards);

            PlayAudio(cachedAudio, 0);

            var test = 0;
        }

        private async Task<IList<string>> Cache(IList<Models.Card> cards)
        {
            var tempList = new List<string>();

            // Синтезируем аудио для карточек без озвучки и кэшируем
            foreach (var card in cards)
            {
                if (card == null) continue;

                if (card.Audio == null)
                {
                    // У карточки нет озвучки, синтезируем голос из текста карточки и кэшируем аудио
                    var path = await _yandexSpeech.GetAudio(card.Title);

                    // TODO: Что-то произошло, аудиофайла нет, нужно записать в лог
                    if (path == null) continue;

                    tempList.Add(path);
                } else
                {
                    // У карточки есть озвучка, вставляем путь к аудио в список воспроизведения
                    tempList.Add(card.Audio);
                }
            }

            return tempList;
        }

        // Воспроизведение
        private void PlayAudio(IList<string> pathList, int index)
        {
            if (index >= pathList.Count) return;

            var path = pathList[index];

            if (File.Exists(path) == false)
            {
                index++;
                PlayAudio(pathList, index);
                return;
            }

            var audio = new Audio(path);
            audio.Ending += new EventHandler((obj, evnt) => {
                PlayAudio(pathList, index);
                Task.Run(() => { (obj as Audio).Dispose(); });
            });
            audio.Play();
            index++;
        }
    }
}
