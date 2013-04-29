using Microsoft.Xna.Framework;
using Mechanect.Common;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

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
        public UserAvatar userAvatar;
        public VoiceCommands voiceCommands;
        //Array of Background songs.
        public Song[] songs;

        //Index of current song.
        public int playQueue;

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

        public bool enablePause = false;
        public bool showAvatar = true;
        public bool screenPaused;
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
            songs[0] = content.Load<Song>("Audio\\song");
            songs[1] = content.Load<Song>("Audio\\song2");
            //songs[2] = Content.Load<Song>("Directory\\songtitle");
            MediaPlayer.IsRepeating = false;
            if (showAvatar)
            {
                userAvatar = new UserAvatar(ScreenManager.Kinect, ScreenManager.Game.Content, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
                userAvatar.LoadContent();
            }

        }
        /// <summary>
        /// Initializes the GameScreen.
        /// </summary
        public virtual void Initialize()
        {
            commands = "red,yellow,go,play,pause,next,previous,mute";
            voiceCommands = new VoiceCommands(ScreenManager.Kinect.nui, commands);
            var voiceThread = new Thread(voiceCommands.StartAudioStream);
            voiceThread.Start();
            songs = new Song[2];
            playQueue = 1;
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

           /* if(voiceCommands.GetHeard("pause"))
            {
                if(MediaPlayer.State.Equals(MediaState.Playing))
                MediaPlayer.Pause();
            }
            if(voiceCommands.GetHeard("play"))
            {
                if(MediaPlayer.State.Equals(MediaState.Paused))
                MediaPlayer.Resume();
            }
            if(voiceCommands.GetHeard("next"))
            {
                playQueue++;
            }
            if(voiceCommands.GetHeard("previous"))
            {
                playQueue--;
            }
            if(voiceCommands.GetHeard("mute"))
            {
                MediaPlayer.IsMuted=true;
            }
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                switch (playQueue)
                {
                    case 1:
                        {
                            MediaPlayer.Play(songs[0]);
                            playQueue = 2;
                            break;
                        }
                    case 2:
                        {
                            MediaPlayer.Play(songs[1]);
                            playQueue = 3;
                            break;
                        }
                    case 3:
                        {
                            playQueue = 1;
                            break;
                        }
                    default: break;
                }
            }
            */
            if (!IsFrozen)
                if (enablePause)
                {
                    if (userAvatar.Avatar == userAvatar.AllAvatars[0])
                    {
                        //Freeze Screen, Show pause Screen\
                     //   screenPaused = true;
                       // ScreenManager.AddScreen(new PauseScreen());
                   //     this.FreezeScreen();
                    }
                    else if (userAvatar.Avatar.Equals(userAvatar.AllAvatars[2]) && screenPaused == true)
                    {
                        //exit pause screen, unfreeze screen
                        this.UnfreezeScreen();
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
            spriteBatch.DrawString(font, voiceCommands.heardString, new Vector2(300,300), Color.Orange);
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