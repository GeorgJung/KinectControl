using Microsoft.Xna.Framework;

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
        private int frameNumber;
        public UserAvatar userAvatar;
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

        /// <summary>
        /// LoadContent will be called only once before drawing and it's the place to load
        /// all of your content.
        /// </summary>
        public virtual void LoadContent()
        {
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