using Microsoft.Xna.Framework;
using System.Threading;
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
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private int frameNumber;
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
        private string commands;

        /// <summary>
        /// LoadContent will be called only once before drawing and it's the place to load
        /// all of your content.
        /// </summary>
        public virtual void LoadContent()
        {
            content = ScreenManager.Game.Content;
            spriteBatch = ScreenManager.SpriteBatch;
            font = content.Load<SpriteFont>("SpriteFont1");
            songs = MyExtension.LoadListContent<Song>(content, "Audio\\");
            MediaPlayer.IsRepeating = false;
            if (showAvatar)
            {
                userAvatar = new UserAvatar(ScreenManager.Kinect, content, ScreenManager.GraphicsDevice, spriteBatch);
                userAvatar.LoadContent();
            }
            voiceCommands = new VoiceCommands(ScreenManager.Kinect.nui, commands);
            var voiceThread = new Thread(voiceCommands.StartAudioStream);
            voiceThread.Start();
        }
        /// <summary>
        /// Initializes the GameScreen.
        /// </summary
        public virtual void Initialize()
        {
            commands = "red,go,yellow,next,previous,stop,shuffle,mute,unmute";
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

            frameNumber++;

            if (voiceCommands.GetHeard("red"))
            {
                if (MediaPlayer.State.Equals(MediaState.Playing))
                    MediaPlayer.Pause();
            }
            else if (voiceCommands.GetHeard("go"))
            {
                if (MediaPlayer.State.Equals(MediaState.Paused))
                    MediaPlayer.Resume();
                else if (MediaPlayer.State.Equals(MediaState.Stopped))
                {
                    var array = songs.ToArray();
                    Random random = new Random();
                    var x = random.Next(songs.Count);
                    MediaPlayer.Play(array[x]);
                }
            }
            else if (voiceCommands.GetHeard("next"))
            {
                    MediaPlayer.MoveNext();
            }
            else if (voiceCommands.GetHeard("previous"))
            {
                    MediaPlayer.MovePrevious();
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
            spriteBatch.DrawString(font, voiceCommands.HeardString, new Vector2(300,300), Color.Orange);
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