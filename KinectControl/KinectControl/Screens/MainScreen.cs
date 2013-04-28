using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using KinectControl.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using KinectControl.Common;
using Microsoft.Kinect;

namespace KinectControl.Screens
{
    class MainScreen : GameScreen
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Kinect kinect;
        private SkeletonAnalyzer analyzer;
        private string text;
        private string gesture;
        double angle;
        private GraphicsDevice graphics;
        private int screenWidth,screenHeight;
        private Button button;
        private HandCursor hand;
        private ContentManager content;
        private Texture2D gradientTexture;

        public MainScreen()
        {
            
        }

        public override void Initialize()
        {
            text = "0";
            angle = 0;
            showAvatar = true;
            button = new Button();
            hand = new HandCursor();
            hand.Initialize(ScreenManager.Kinect);
            analyzer = new SkeletonAnalyzer();
            if (!(ScreenManager.Kinect.trackedSkeleton == null))
                analyzer.SetBodySegments(ScreenManager.Kinect.trackedSkeleton.Joints[JointType.HandRight], ScreenManager.Kinect.trackedSkeleton.Joints[JointType.ElbowRight], ScreenManager.Kinect.trackedSkeleton.Joints[JointType.ShoulderRight]);
            button.Initialize("Buttons/OK", this.ScreenManager.Kinect, new Vector2(820, 100));
            button.Clicked += new Button.ClickedEventHandler(button_Clicked);
            base.Initialize();
        }
        void button_Clicked(object sender, System.EventArgs a)
        {
            this.Remove();
           // ScreenManager.AddScreen(playScreen);
        }
        public override void LoadContent()
        {
            kinect = ScreenManager.Kinect;
            gesture = kinect.Gesture;
            content = ScreenManager.Game.Content;
            graphics = ScreenManager.GraphicsDevice;
            spriteBatch = ScreenManager.SpriteBatch;
            screenHeight = graphics.Viewport.Height;
            screenWidth = graphics.Viewport.Width;
            gradientTexture = content.Load<Texture2D>("Textures/gradientTexture");
            font = content.Load<SpriteFont>("SpriteFont1");
            hand.LoadContent(content);
            button.LoadContent(content);
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            hand.Update(gameTime);
            button.Update(gameTime);
            gesture = kinect.Gesture;
            if (!(ScreenManager.Kinect.trackedSkeleton == null))
            {
                angle = analyzer.GetBodySegmentAngle(ScreenManager.Kinect.trackedSkeleton.Joints);
                text += angle;
            }
            FreezeScreen();
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
        //    spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,null,null,null,null,cam.Transform);
            spriteBatch.Begin();
            spriteBatch.Draw(gradientTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.DrawString(font, text, Vector2.Zero, Color.Orange);
            spriteBatch.DrawString(font, gesture, new Vector2(500,500), Color.Orange);
            button.Draw(spriteBatch);
            hand.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
