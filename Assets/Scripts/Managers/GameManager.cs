using System;
using Gameplay;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Managers
{
	public class GameManager : Singleton<GameManager>
	{
		public bool IsPlaying => state == State.GamePlaying;

		[SerializeField] private float waitingToStartTimer = 1;
		[SerializeField] private float countdownToStartTimer = 3;
		[SerializeField] private float gamePlayingTimerMax = 10;
		private float gamePlayingTimer;

		public enum State
		{
			WaitingToStart,
			CountdownToStart,
			GamePlaying,
			GameOver
		}

		private State state;
		private bool isGamePaused;

		public event UnityAction<State> OnStateChanged;
		public event UnityAction OnGamePaused;
		public event UnityAction OnGameUnpaused;

		private void Awake()
		{
			state = State.WaitingToStart;
		}

		private void Start()
		{
			OnStateChanged?.Invoke(state);
			GameInput.OnPauseAction += OnPause;
		}

		private void Update()
		{
			switch (state)
			{
				case State.WaitingToStart:
					waitingToStartTimer -= Time.deltaTime;
					if (waitingToStartTimer < 0f)
					{
						state = State.CountdownToStart;
						OnStateChanged?.Invoke(state);
					}

					break;
				case State.CountdownToStart:
					countdownToStartTimer -= Time.deltaTime;
					if (countdownToStartTimer < 0f)
					{
						state = State.GamePlaying;
						gamePlayingTimer = gamePlayingTimerMax;
						OnStateChanged?.Invoke(state);
					}

					break;
				case State.GamePlaying:
					gamePlayingTimer -= Time.deltaTime;
					if (gamePlayingTimer < 0f)
					{
						state = State.GameOver;
						OnStateChanged?.Invoke(state);
					}

					break;
				case State.GameOver:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public float GetCountdownToStartTimer()
		{
			return countdownToStartTimer;
		}

		public float GetGamePlayingTimerNormalized()
		{
			return 1 - (gamePlayingTimer / gamePlayingTimerMax);
		}

		private void OnPause(object sender, EventArgs e)
		{
			TogglePauseGame();
		}

		public void TogglePauseGame()
		{
			isGamePaused = !isGamePaused;
			if (isGamePaused)
			{
				OnGamePaused?.Invoke();
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = 1;
				OnGameUnpaused?.Invoke();
			}
		}
	}
}