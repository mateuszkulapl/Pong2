using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Pong2
{
    public class Ball
    {

        Texture2D texture;
        Vector2 position;
        Vector2 speed;
        Viewport graphicViewport;
        float r;
        Point sheetSize;
        Point frameSize;
        Point currentFrame;
        Vector2 ballMargin;

        public Ball(Texture2D texture, Viewport graphicViewport, Point sheetSize)
        {
            this.texture = texture;
            this.speed = new Vector2(0, 0);
            this.graphicViewport = graphicViewport;

            this.position = new Vector2((int)graphicViewport.Width / 2, (int)graphicViewport.Height / 2);
            this.r = 20;
            this.sheetSize = sheetSize;
            this.frameSize = new Point(texture.Width / sheetSize.X, texture.Height / sheetSize.Y);
            this.currentFrame = new Point(0, 0);
            this.ballMargin = new Vector2(frameSize.X / 5, frameSize.Y / 5);
        }

        public void nextFrame()
        {
            currentFrame.X++;
            if (currentFrame.X > sheetSize.X - 1)
            {
                currentFrame.X = 0;
                currentFrame.Y++;
            }
            else
            if (currentFrame.Y == 4 && currentFrame.X == 6)
            {
                currentFrame.Y = 0;
                currentFrame.X = 0;
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Rectangle screenPosition = new Rectangle((int)(position.X - r), (int)(position.Y - r), (int)(2 * r), (int)(2 * r));
            Rectangle sourceRectangle = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            spriteBatch.Draw(texture, screenPosition, sourceRectangle, Color.White);
        }

        internal void Start(int xSpeed = 5)
        {
            Random rnd = new Random();
            int directionX = 1;
            if (rnd.Next(0, 2) == 1)
                directionX = -1;
            int directionY = 1;
            if (rnd.Next(0, 2) == 1)
                directionY = -1;
            this.speed = new Vector2((xSpeed * directionX), (xSpeed / 2 * directionY));
            this.position = new Vector2((int)graphicViewport.Width / 2, (int)graphicViewport.Height / 2);
        }
        /// <summary>
        /// Stop ball
        /// </summary>
        internal void End()
        {
            this.speed = new Vector2(0, 0);
        }
        /// <summary>
        /// Move ball by currect speed vector, then check screen and paddles collission
        /// </summary>
        /// <param name="l">left Paddle</param>
        /// <param name="r">right Paddle</param>
        internal void Move(Paddle l, Paddle r)
        {
            Vector2 oldPosition = position;
            position += speed;
            CheckCollision(l, r, oldPosition);

        }
        /// <summary>
        /// Check screen and paddles collision
        /// </summary>
        /// <param name="l">left Paddle</param>
        /// <param name="r">right Paddle</param>
        /// <param name="oldPosition">last frame ball position</param>
        private void CheckCollision(Paddle l, Paddle r, Vector2 oldPosition)
        {
            checkScreenCollision();
            checkPaddleCollision(l, oldPosition);
            checkPaddleCollision(r, oldPosition);
        }

        private void checkScreenCollision()
        {
            Vector2 downPosition = position + new Vector2(0, r) - ballMargin;
            if (downPosition.Y >= graphicViewport.Height)
            {
                speed.Y *= -1;
                this.position.Y = this.position.Y - 2 * (downPosition.Y - graphicViewport.Height);
            }
            Vector2 upPosition = position + new Vector2(0, -r) - ballMargin;
            if (upPosition.Y <= 0)
            {
                speed.Y *= -1;
                this.position.Y = this.position.Y - 2 * (upPosition.Y);
            }
        }

        /// <summary>
        /// Check if paddle bounced ball. If so increase ball speed
        /// </summary>
        /// <param name="paddle"></param>
        /// <param name="oldPosition"></param>
        private void checkPaddleCollision(Paddle paddle, Vector2 oldPosition)
        {
            if (CheckPaddleBounce(oldPosition, paddle))
            {
                speed.X *= -1.05f;
                Console.WriteLine("speed: " + speed);
            }
        }

        /// <summary>
        /// Check if paddle missed ball and add points
        /// </summary>
        /// <param name="lPad">left Paddle</param>
        /// <param name="rPad">right Paddle </param>
        /// <returns></returns>
        public bool IsEnd(Paddle lPad, Paddle rPad)
        {
            if (position.X > rPad.GetScreenPosition().X)
            {
                lPad.AddPoints();
                return true;
            }
            else
            {
                if (position.X < lPad.GetScreenPosition().X + rPad.GetScreenPosition().Width)
                {
                    rPad.AddPoints();
                    return true;
                }
            }
            return false;
        }



        public bool CheckPaddleBounce(Vector2 oldPosition, Paddle paddle)
        {
            Vector2 leftPos;
            Vector2 rightPos;
            if (oldPosition.X < position.Y)
            {
                leftPos = oldPosition;
                rightPos = position;
            }
            else
            {
                leftPos = position;
                rightPos = oldPosition;
            }

            float yOffset = rightPos.Y - leftPos.Y;
            float xOffset = rightPos.X - leftPos.X;


            if (paddle.GetSide() == Side.Right)
            {
                if (position.X + r - ballMargin.X >= paddle.GetScreenPosition().X)
                {
                    float ballPaddleDistance = paddle.GetScreenPosition().X - oldPosition.X;
                    float onPaddleY = oldPosition.Y + yOffset * (ballPaddleDistance / xOffset);
                    Rectangle paddleScreenPositionWithOffset = paddle.GetScreenPosition();
                    paddleScreenPositionWithOffset.Inflate(0, r);
                    if (paddleScreenPositionWithOffset.Contains(paddle.GetScreenPosition().X, onPaddleY))
                    {
                        position.X = paddle.GetScreenPosition().X - r;
                        position.Y = onPaddleY;
                        return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (paddle.GetSide() == Side.Left)
            {
                if (position.X - r + ballMargin.X <= paddle.GetScreenPosition().X + paddle.GetScreenPosition().Width)
                {
                    float ballPaddleDistance = paddle.GetScreenPosition().X + paddle.GetScreenPosition().Width - oldPosition.X;
                    float onPaddleY = oldPosition.Y + yOffset * (ballPaddleDistance / (xOffset));
                    Rectangle paddleScreenPositionWithOffset = paddle.GetScreenPosition();
                    paddleScreenPositionWithOffset.Inflate(0, r);
                    if (paddleScreenPositionWithOffset.Contains(paddle.GetScreenPosition().X + paddle.GetScreenPosition().Width - 1, onPaddleY))
                    {
                        position.X = paddle.GetScreenPosition().X + paddle.GetScreenPosition().Width + r;
                        position.Y = onPaddleY;
                        return true;
                    }
                    return false;
                }
                else
                    return false;
            }
            return false;

        }

    }
}
