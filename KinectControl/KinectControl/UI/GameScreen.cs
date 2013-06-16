using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using KinectControl.Common;
using System.Collections.Generic;
using System;

namespace KinectControl.UI
{
    #region ScreenState
    /// <summary>
    /// Represents the screen states.
    /// </summary>
    public enum ScreenState
    {
        Active,
        Frozen,
        Hidden
    }
    #endregion

    /// <summary>
    /// This class represents a screen.
    /// </summary>
    public abstract class GameScreen
    {
        private ContentManager content;
        private Song[] songsarray;
        private int playQueue;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private int frameNumber;
        public int FrameNumber
        {
            get
            {
                return frameNumber;
            }
            set
            {
            }
        }
        private UserAvatar userAvatar;
        private List<Song> songs;
        public UserAvatar UserAvatar
        {
            get { return userAvatar; }
            set { userAvatar = value; }
        }
        private VoiceCommands voiceCommands;

        public bool IsFrozen
        {
            get
            {
                return screenState == ScreenState.Frozen;
            }
        }

        private ScreenState screenState;
        public ScreenState ScreenState
        {
            get { return screenState; }
            set { screenState = value; }
        }
        private ScreenManager screenManager;
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            set { screenManager = value; }
        }

        public bool IsActive
        {
            get
            {
                return screenState == ScreenState.Active;
            }
        }
        public bool showAvatar = true;
        /// <summary>
        /// LoadContent will be called only once before drawing and it's the place to load
        /// all of your content.
        /// </summary>
        public virtual void LoadContent()
        {
            frameNumber = Kinect.FramesCount;
            content = ScreenManager.Game.Content;
            spriteBatch = ScreenManager.SpriteBatch;
            font = content.Load<SpriteFont>("SpriteFont1");
            songs = MyExtension.LoadListContent<Song>(content, "Audio\\");
            songsarray = songs.ToArray();
            Random random = new Random();
            playQueue = random.Next(songs.Count);
            voiceCommands = ScreenManager.Kinect.voiceCommands;
            if (showAvatar)
            {
                userAvatar = new UserAvatar(ScreenManager.Kinect, content, ScreenManager.GraphicsDevice, spriteBatch);
                userAvatar.LoadContent();
            }
        }
        /// <summary>
        /// Initializes the GameScreen.
        /// </summary
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Unloads the content of GameScreen.
        /// </summary>
        public virtual void UnloadContent() { }
        /// <summary>
        /// Allows the game screen to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (showAvatar)
            {
                userAvatar.Update(gameTime);
            }
            if (frameNumber % 360 == 0)
            {
                voiceCommands.HeardString = "";
            }

            frameNumber++;
            if (voiceCommands != null)
            {
                if (voiceCommands.GetHeard("stop"))
                {
                    if (MediaPlayer.State.Equals(MediaState.Playing))
                        MediaPlayer.Pause();
                }
                else if (voiceCommands.GetHeard("play"))
                {
                    if (MediaPlayer.State.Equals(MediaState.Paused))
                        MediaPlayer.Resume();
                    else if (MediaPlayer.State.Equals(MediaState.Stopped))
                    {
                        Random random = new Random();
                        playQueue = random.Next(songs.Count);
                        MediaPlayer.Play(songsarray[playQueue]);
                    }
                }
                else if (voiceCommands.GetHeard("next"))
                {
                    playQueue++;
                    if (playQueue < songsarray.Length)
                    {
                        MediaPlayer.MoveNext();
                        MediaPlayer.Play(songsarray[playQueue]);
                    }
                    else MediaPlayer.Stop();
                }
                else if (voiceCommands.GetHeard("previous"))
                {
                    playQueue--;
                    if (playQueue >= 0)
                    {
                        MediaPlayer.MovePrevious();
                        MediaPlayer.Play(songsarray[playQueue]);
                    }
                    else MediaPlayer.Stop();
                }
                else if (voiceCommands.GetHeard("mute"))
                {
                    MediaPlayer.IsMuted = true;
                }
                else if (voiceCommands.GetHeard("unmute"))
                {
                    MediaPlayer.IsMuted = false;
                }
            }
        }
             

        /// <summary>
        /// Removes the current screen.
        /// </summary>
        public virtual void Remove()
        {
            screenManager.RemoveScreen(this);
        }

        /// <summary>
        /// This is called when the game screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            if (showAvatar)
                userAvatar.Draw(gameTime);
            spriteBatch.Begin();
            if(voiceCommands!=null && !voiceCommands.HeardString.Equals(""))
            spriteBatch.DrawString(font,"voice recognized :" + voiceCommands.HeardString, new Vector2(300,300), Color.Orange);
            spriteBatch.End();
        }

        public void FreezeScreen()
        {
            screenState = ScreenState.Frozen;
        }

        public void UnfreezeScreen()
        {
            screenState = ScreenState.Active;
        }


    }
}