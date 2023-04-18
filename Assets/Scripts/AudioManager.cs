using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Clip[] _clips;
	[SerializeField] private AudioSource _firstMusicSource;
	[SerializeField] private AudioSource _secondMusicSource;
	private float _musicVolume = 1f;
	public static AudioManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}

    [System.Serializable]
	public struct Clip
	{
		public AudioClip clip;
		public Clips clipId;
	}

	public void PlaySound(Clips clipId, AudioSource audoSource, float delay = 0f, float volume = 1f)
    {
		if (audoSource == null || !PlayerData.Instance.isAllSoundOn) return;
		audoSource.volume = volume;
		audoSource.clip = _clips[(int)clipId].clip;
		audoSource.PlayDelayed(delay);
    }

	public void StopMusic() => _firstMusicSource.Stop();

	public void PlayMusic(Clips clipId)
    {
		SetVolume();
		_firstMusicSource.volume = _musicVolume;
		_firstMusicSource.clip = _clips[(int)clipId].clip;
		_firstMusicSource.Play();
	}

	public void SwitchMusic(Clips clipId)
    {
		if (_firstMusicSource.isPlaying)
        {
			_secondMusicSource.clip = _clips[(int)clipId].clip;
			_secondMusicSource.Play();
			StartCoroutine(SmoothAudio(_firstMusicSource, false, 1));
			StartCoroutine(SmoothAudio(_secondMusicSource, true, 1));
		}
		else
        {
			_firstMusicSource.clip = _clips[(int)clipId].clip;
			_firstMusicSource.Play();
			StartCoroutine(SmoothAudio(_secondMusicSource, false, 1));
			StartCoroutine(SmoothAudio(_firstMusicSource, true, 1));
		}
    }

	public void SetVolume()
    {
		if (PlayerData.Instance.isAllSoundOn && PlayerData.Instance.isSoundOn) 
			_musicVolume = 1;
		else 
			_musicVolume = 0;		
		_firstMusicSource.volume = _musicVolume;
		_secondMusicSource.volume = _musicVolume;
	}

	private IEnumerator SmoothAudio(AudioSource audioSource, bool ascending, float fadeTime)
    {
		float currentTime = 0;
		if (ascending)
			while (audioSource.volume < 1)
			{
				currentTime += Time.deltaTime;
				audioSource.volume = Mathf.Lerp(0, 1, currentTime / fadeTime);
				yield return null;
			}
		else
        {
			while (audioSource.volume > 0)
			{
				currentTime += Time.deltaTime;
				audioSource.volume = Mathf.Lerp(1, 0, currentTime / fadeTime);
				yield return null;
			}
			audioSource.Stop();
		}
    }
}