﻿using Eolnyss.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Eolnyss.Prefabs
{
    public class Player : PhysicsObject, IBox
    {
        private readonly Vector2 start;

        private const float MaxJumpTime = 0.3f;
        private const float JumpLaunchVelocity = -2040.0f;
        private const float Gravity = 21000.0f;
        private const float MaxFallSpeed = 2000.0f;
        private const float JumpControlPower = 0.14f;

        private bool isJumping;
        private float jumpTime;

        private bool isOnGround;

        private Vector2 movement;

        public Player(IWorld world, float x, float y, float width, float height, Vector2 start)
            : base(world, x, y, width, height)
        {
            this.start = start;
            OnCollision += Collision;

            Reset(start);
        }

        public bool IsOnGround => this.isOnGround;

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Assets.PlayerTexture, Bounds, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput();

            isOnGround = false;

            movement.X = 390;

            if (isJumping)
            {
                jumpTime += elapsed;

                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                    movement.Y = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                else
                    isJumping = false;
            }
            else
            {
                //fallTime += elapsed * 3;
                movement.Y = MathHelper.Clamp((movement.Y + Gravity * elapsed), -MaxFallSpeed, MaxFallSpeed);
                jumpTime = 0.0f;
            }

            movement *= elapsed;

            Push(movement);

            Debug.WriteLine(Position);
        }

        public void LoadContent(ContentManager content)
        {

        }

        public override void Collision(object sender, CollisionArgs collisionArgs)
        {
            var side = collisionArgs.Side;

            if (side.HasFlag(Side.Top) ||
                side.HasFlag(Side.Bottom))
            {
                movement.Y = 0.0f;
                isJumping = false;
            }

            if (side.HasFlag(Side.Bottom))
            {
                isOnGround = true;
            }

            if (side.HasFlag(Side.Left) ||
                side.HasFlag(Side.Right))
            {
                movement.X = 0.0f;
            }

            if (side.HasFlag(Side.Right) ||
                side.HasFlag(Side.Left) ||
                side.HasFlag(Side.Top))
            {
                Reset(start);
            }

            var box = collisionArgs.Box;
        }

        public void HandleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && IsOnGround)
                isJumping = true;
        }

        public void Reset(Vector2 start)
        {
            Move(start.X, start.Y);
        }
    }
}