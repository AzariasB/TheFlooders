using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroductionScreen : MonoBehaviour
{
	/// <summary>
	/// Le VideoPlayer qui joue l'introduction. Apparemment, jouer le son avec ne fonctionne
	/// pas (?), donc on a un lecteur audio à part qu'il faut synchroniser avec.
	/// </summary>
	[SerializeField]
	private VideoPlayer introVideoPlayer;

	/// <summary>
	/// Le lecteur audio qui joue la bande sonore de la vidéo d'intro.
	/// </summary>
	[SerializeField]
	private AudioSource introMusicPlayer;

	/// <summary>
	/// Vrai si la vidéo d'introduction a été lancée avec succès.
	/// </summary>
	private bool startedPlayingVideo;

    void Start()
	{
		if (introVideoPlayer == null)
			introVideoPlayer = GetComponent<VideoPlayer> ();
		if (introMusicPlayer == null)
			introMusicPlayer = GetComponent<AudioSource> ();

		if (introVideoPlayer != null) {
			PrepareVideoPlayer ();
		}
    }
	
    // Change de scène une fois que la vidéo est terminée.
    protected virtual void Update()
    {
		if (introVideoPlayer != null && !Input.GetKeyDown(KeyCode.Escape))
		{
			// Si la vidéo a démarré et s'est terminée, passer à la scène suivante
			if (startedPlayingVideo && !introVideoPlayer.isPlaying)
				SceneManager.LoadScene(1);
		}
		else
			// Pas de vidéo ou Escape appuyée => Passage à la scène suivante.
            SceneManager.LoadScene(1);
    }

	/// <summary>
	/// Lance le préchargement de la vidéo d'intro.
	/// </summary>
	private void PrepareVideoPlayer() {
		introVideoPlayer.prepareCompleted += OnVideoPlayerPrepareCompleted;
		introVideoPlayer.errorReceived += OnVideoPlayerErrorReceived;
		introVideoPlayer.Prepare();
	}

	private void OnVideoPlayerPrepareCompleted(VideoPlayer source) {
		RemoveVideoPlayerObservers();
		startedPlayingVideo = true;
		introVideoPlayer.Play();
		if(introMusicPlayer != null)
			introMusicPlayer.Play();
	}

	private void OnVideoPlayerErrorReceived(VideoPlayer source, string message) {
		RemoveVideoPlayerObservers();
		Debug.LogError("Echec du chargement de la vidéo d'introduction : " + (message ?? "<null>"));
	}

	private void RemoveVideoPlayerObservers() {
		introVideoPlayer.prepareCompleted -= OnVideoPlayerPrepareCompleted;
		introVideoPlayer.errorReceived -= OnVideoPlayerErrorReceived;
	}
}
