using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace miyaluas.droplet
{
    public class GameController : MonoBehaviour
    {
        enum GameState
        {
            Logos,
            Intro,
            Intro2,
            Home,
            Level,
            LevelResult,
            Ending,
            Credits
        };

        [Header("Animations")]
        [Tooltip("After loading app. Shows logos.")]
        [SerializeField]
        PlayableDirector logosPlayable;
        [Tooltip("After tap on logos. Goes from logos to 'Home'")]
        [SerializeField]
        PlayableDirector introPlayable;
        [Tooltip("After tap on credits. Goes from credits to 'Home'")]
        [SerializeField]
        PlayableDirector creditsPlayable;

        [Header("Main game objects")]
        public CameraController cam;
        public UIController uiController;
        public AudioListener listener;
        public BallController[] playerObjects;

        [Header("Other options")]
        [SerializeField]
        bool soundTapEnabled = true;

        public BallController player => currentPlayer_;
        public SensorKind goal => currentGoal_;

        SensorKind currentGoal_;
        GameState currentState_;
        PlayableDirector currentPlayable_;
        BallController currentPlayer_;

        List<SensorKind> pendingGoals = new List<SensorKind>();
        int pendingIndex;
        float animationTime;

        private void Awake()
        {
            SetState(GameState.Logos);
        }

        private void Update()
        {
            switch(currentState_)
            {
                case GameState.Logos:
                    if (PlayableFinished() && ClickDetect())
                        SetState(GameState.Intro);
                    break;

                case GameState.LevelResult:
                case GameState.Intro:
                case GameState.Intro2:
                    if (PlayableFinished())
                        SetState(GameState.Home);
                    break;

                case GameState.Home:
                    if (PlayableFinished())
                        SetState(GameState.Level);
                    break;

                case GameState.Level:
                    if (soundTapEnabled && ClickDetect())
                        currentPlayer_?.HintSound();
                    break;

                case GameState.Ending:
                    if (PlayableFinished())
                        SetState(GameState.Credits);
                    break;

                case GameState.Credits:
                    if (ClickDetect())
                        SetState(GameState.Intro2);
                    break;
            }
        }

        private void SetState(GameState newState)
        {
            currentState_ = newState;
            switch(newState)
            {
                case GameState.Logos:
                    uiController.ShowLogos();
                    Play(logosPlayable);
                    break;

                case GameState.Intro:
                    cam.SetState(1);
                    Play(introPlayable);
                    FillGoals();
                    break;

                case GameState.Home:
                    cam.SetState(2);
                    uiController.GoToLevel();
                    CalcNextGoal();
                    PlayNewGoalPlayable();
                    break;

                case GameState.Level:
                    LevelStart();
                    break;

                case GameState.Intro2:
                    cam.SetState(1);
                    Play(creditsPlayable);
                    FillGoals();
                    break;

                case GameState.LevelResult:
                    uiController.SetLevelProgress(4 - pendingGoals.Count);
                    uiController.ShowHome();
                    cam.SetState(1);
                    break;

                case GameState.Ending:
                    uiController.SetLevelProgress(5);
                    uiController.ShowHome();
                    cam.SetState(1);
                    break;

                case GameState.Credits:
                    cam.SetState(0);
                    listener.enabled = false;
                    cam.GetComponent<AudioListener>().enabled = true;
                    break;
            }
        }

        private void FillGoals()
        {
            pendingGoals.Clear();
            pendingGoals.Add(SensorKind.ObjetivoCanalon);
            pendingGoals.Add(SensorKind.ObjetivoCasa);
            pendingGoals.Add(SensorKind.ObjetivoElectrico);
            pendingGoals.Add(SensorKind.ObjetivoPajaro);
            pendingIndex = pendingGoals.Count;
        }

        private void ConsumeGoal()
        {
            pendingGoals.Remove(currentGoal_);
            pendingIndex = pendingGoals.Count;
        }

        private void CalcNextGoal()
        {
            if (pendingGoals.Count > 0)
            {
                ++pendingIndex;
                if (pendingIndex >= pendingGoals.Count)
                {
                    pendingGoals.Shuffle();
                    pendingIndex = 0;
                }
                currentGoal_ = pendingGoals[pendingIndex];
            }
            else currentGoal_ = SensorKind.ObjetivoFinal;

            // Select player object
            listener.transform.SetParent(transform);
            listener.transform.localPosition = Vector3.zero;
            currentPlayer_?.Hide();
            currentPlayer_ = null;
            foreach (BallController c in playerObjects)
                if (c.Goal == currentGoal_)
                    currentPlayer_ = c;

            cam.GetComponent<AudioListener>().enabled = false;
            listener.enabled = true;
        }

        private void PlayNewGoalPlayable()
        {
            Play(currentPlayer_.PlayHomeAnimation(this));
        }

        private void LevelStart()
        {
            if (currentPlayer_)
            {
                listener.transform.SetParent(currentPlayer_.transform);
                listener.transform.localPosition = Vector3.zero;

                currentPlayer_.SetGameController(this);
                currentPlayer_.enabled = true;
            }
            cam.SetGameController(this);
        }

        private void Play(PlayableDirector anim)
        {
            currentPlayable_ = anim;
            if(anim) anim.Play();
            animationTime = 0f;
        }

        private bool PlayableFinished()
        {
            animationTime += Time.deltaTime;
            if (currentPlayable_) return currentPlayable_.state != PlayState.Playing;
            return uiController.AnimationFinished() && cam.AnimationFinished();
        }

        private bool ClickDetect()
        {
            return Input.anyKeyDown || Input.GetMouseButtonDown(0);
        }

        internal void GoalReached(GenericSensor sensor)
        {
            // Ask sensor for animation and play it
            currentPlayer_.enabled = false;
            Play(sensor.HitAnimation);
            if (currentGoal_ == SensorKind.ObjetivoFinal)
            {
                SetState(GameState.Ending);
            }
            else
            {
                ConsumeGoal();
                SetState(GameState.LevelResult);
            }
        }

        internal void EnemyHit(GenericSensor sensor)
        {
            // Ask sensor for animation and play it
            Play(sensor.HitAnimation);
            SetState(GameState.LevelResult);
        }
    }
}
