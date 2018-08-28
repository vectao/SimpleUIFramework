using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
//using System.Diagnostics;

[RequireComponent(typeof(Image))]
public class UGUISpriteAnimation : MonoBehaviour
{
    private Image ImageSource;
    private int mCurFrame = 0;
    private float mDelta = 0;
    private bool IsPlaying = false;

    [Range(1, 30)]
    public int FPS = 5;
    public List<FrameData> SpriteFrames;
    public bool Foward = true;
    [Tooltip("用于OnStart播放,仅用于GameObject激活第一次的时候自动播放")]
    public bool AutoPlayOnStart = false;
    [Tooltip("用于OnEnable播放,每次GameObject激活的时候都会自动播放")]
    public bool AutoPlayOnEnable = false;
    public bool Loop = false;

    public int FrameCount
    {
        get
        {
            return SpriteFrames.Count;
        }
    }

    void Awake()
    {
        ImageSource = GetComponent<Image>();
    }

//    Stopwatch stopWatch;

    void Start()
    {
        if (AutoPlayOnStart)
        {
            Play();
        }
    }

    private void OnEnable()
    {
        if (AutoPlayOnEnable)
        {
            Rewind();
        }
    }

	private float SetSprite(int idx) {
		FrameData frame = SpriteFrames[idx];
		if (frame != null) {
			ImageSource.sprite = frame.sprite;
			ImageSource.SetNativeSize();
			return frame.duration <= 0f ? 1f / FPS : frame.duration;
		}
		return 0f;
	}

    public void Play()
    {
        IsPlaying = true;
        Foward = true;
    }

    public void PlayReverse()
    {
        IsPlaying = true;
        Foward = false;
    }

    void Update()
    {
        if (!IsPlaying || 0 == FrameCount)
        {
            return;
        }

        mDelta -= Time.deltaTime;

        while (mDelta <= 0f)
        {
            /*
            if (stopWatch == null)
            {
                stopWatch = new Stopwatch();
                stopWatch.Start();
                UnityEngine.Debug.LogError("启动时间(毫秒) --> " + stopWatch.ElapsedMilliseconds);
            }
            UnityEngine.Debug.LogErrorFormat("第{0}帧时间(毫秒) --> " + stopWatch.ElapsedMilliseconds, mCurFrame);
            */
            mDelta += SetSprite(mCurFrame);

			if (Foward)
                mCurFrame++;
            else
                mCurFrame--;

            if (mCurFrame >= FrameCount)
            {
                if (Loop)
                {
                    mCurFrame = 0;
                }
                else
                {
                    mCurFrame--;
                    IsPlaying = false;
                    return;
                }
            }
            else if (mCurFrame < 0)
            {
                if (Loop)
                {
                    mCurFrame = FrameCount - 1;
                }
                else
                {
                    IsPlaying = false;
                    return;
                }
            }
        }
    }

    public void Pause()
    {
        IsPlaying = false;
    }

    public void Resume()
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
        }
    }

    public void Stop()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        IsPlaying = false;
    }

    public void Rewind()
    {
        mCurFrame = 0;
		mDelta = SetSprite(mCurFrame);
        Play();
    }

	[Serializable]
	public class FrameData {
		public Sprite sprite;
		public float duration;
	}

}