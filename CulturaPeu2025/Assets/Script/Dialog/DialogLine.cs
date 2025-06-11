using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DialogSystem
{
    public class DialogLine : DialogBaseClass
    {
        private TMP_Text textHolder;

        [Header("Text")]
        [SerializeField] private string input;
        [SerializeField] private Color textColor;
        [SerializeField] private TMP_FontAsset textFont;

        [Header("Time Variables")]
        [SerializeField] private float delay;
        [SerializeField] private float delayBeetweenLines;

        [Header("Sound Variables")]
        [SerializeField] private AudioInfoSO currentAudioInfo;

        [Header("Character Image")]
        [SerializeField] private Sprite charSprite;
        [SerializeField] private Image imageHolder;

        private bool canClick = false;
        private IEnumerator LineApear;

        public bool finished { get; protected set; }
        public bool finishedPlayingLines { get; protected set; }

        private void Awake()
        {
            imageHolder.sprite = charSprite;
            imageHolder.preserveAspect = true;
        }

        private void OnEnable()
        {
            ResetLine();
            LineApear = WriteText(input, textHolder, textColor, textFont, delay, delayBeetweenLines);
            StartCoroutine(LineApear);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && canClick)
            {
                if (textHolder.text != input)
                {
                    StopCoroutine(LineApear);
                    textHolder.text = input;
                    textHolder.maxVisibleCharacters = input.Length;
                }
                else
                {
                    finished = true;
                    finishedPlayingLines = true;
                }
            }
        }

        private void ResetLine()
        {
            textHolder = GetComponent<TMP_Text>();
            textHolder.text = "";
            finished = false;
            finishedPlayingLines = false;
        }

        // Start the typewriter dialog
        public void StartDialog(string input, TMP_Text textHolder, Color textColor, TMP_FontAsset textFont, float delay, float delayBetweenLines)
        {
            StartCoroutine(WriteText(input, textHolder, textColor, textFont, delay, delayBetweenLines));
        }

        protected IEnumerator WriteText(string input, TMP_Text textHolder, Color textColor, TMP_FontAsset textFont, float delay, float delayBetweenLines)
        {
            AudioClip[] sounds = currentAudioInfo.sounds;
            int soundPerCharFrequency = currentAudioInfo.soundPerCharFrequency;
            float minPitch = currentAudioInfo.minPitch;
            float maxPitch = currentAudioInfo.maxPitch;
            bool makePredictable = currentAudioInfo.makePredictable;

            textHolder.text = "";
            textHolder.color = textColor;
            textHolder.font = textFont;
            textHolder.maxVisibleCharacters = 0;

            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];

                if (textHolder.maxVisibleCharacters % soundPerCharFrequency == 0)
                {
                    SoundDialogManager.instance.StopSounds();
                    AudioClip soundClip = null;

                    if (sounds != null)
                    {
                        if (makePredictable)
                        {
                            char currentCharacter = textHolder.text[textHolder.maxVisibleCharacters];
                            int hashCode = currentCharacter.GetHashCode();
                            int predictableIndex = hashCode % sounds.Length;
                            soundClip = sounds[predictableIndex];

                            int minPitchInt = (int)(minPitch * 100);
                            int maxPitchInt = (int)(maxPitch * 100);
                            int pitchRangeInt = maxPitchInt - minPitchInt;

                            float predictablePitch = pitchRangeInt != 0 ?
                                ((hashCode % pitchRangeInt) + minPitchInt) / 100f :
                                minPitch;

                            SoundDialogManager.instance.ChangePitch(predictablePitch);
                        }
                        else
                        {
                            SoundDialogManager.instance.ChangePitch(Random.Range(minPitch, maxPitch));
                            int randomIndex = Random.Range(0, sounds.Length);
                            soundClip = sounds[randomIndex];
                        }

                        SoundDialogManager.instance.PlaySound(soundClip);
                    }
                }

                textHolder.maxVisibleCharacters++;
                yield return new WaitForSeconds(delay);
            }

            finishedPlayingLines = true;
            canClick = true;

            // Wait for player to press 'E' to continue
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E) && finishedPlayingLines);
            finished = true;
        }
    }
}
