#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod;
using Monocle;

namespace Celeste {
    class patch_Parallax : Parallax {

        [MonoModIgnore]
        private float fadeIn = 1f;

        public patch_Parallax(MTexture texture)
            : base(texture) {
            // no-op. MonoMod ignores this - we only need this to make the compiler shut up.
        }

        public extern void orig_ctor(MTexture texture);
        [MonoModConstructor]
        public void ctor(MTexture texture) {
            orig_ctor(texture);
            UseSpritebatch = !(LoopX || LoopY);
        }

        public extern void orig_Update(Scene scene);
        public override void Update(Scene scene) {
            orig_Update(scene);
            UseSpritebatch = !(LoopX || LoopY);
        }

        public extern void orig_Render(Scene scene);
        public override void Render(Scene scene) {
            Matrix matrix = (scene as Level)?.Background?.Matrix ?? Matrix.Identity;

            Vector2 cam_pos = ((scene as Level).Camera.Position + CameraOffset).Floor();
            Vector2 position = (Position - cam_pos * Scroll).Floor();
            float alpha = fadeIn * Alpha * FadeAlphaMultiplier;

            if (FadeX != null) {
                alpha *= FadeX.Value(cam_pos.X + 160f);
            }
            if (FadeY != null) {
                alpha *= FadeY.Value(cam_pos.Y + 90f);
            }

            Color color = Color;
            if (alpha < 1f) {
                color *= alpha;
            }
            if (color.A <= 1) {
                return;
            }

            Rectangle rect = new Rectangle((int) position.X, (int) position.Y, Texture.Width, Texture.Height);

            if (LoopX) {
                rect.X = 0;
                rect.Width = 320;

                float newX = (position.X % 320) - 320;
                float deltaX = newX - position.X;
                position.X = newX;
                rect.X = (int) newX;
                rect.Width += (int) Math.Ceiling(deltaX / 320f);
            }
            if (LoopY) {
                rect.Y = 0;
                rect.Height = 180;

                float newY = (position.Y % 180) - 180;
                float deltaY = position.Y - newY;
                position.Y = newY;
                rect.Y = (int) newY;
                rect.Height += (int) Math.Ceiling(deltaY / 180f);
            }

            if (!UseSpritebatch) {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, matrix);
            }

            SpriteEffects flip = SpriteEffects.None;
            if (FlipX) { flip |= SpriteEffects.FlipHorizontally; }
            if (FlipY) { flip |= SpriteEffects.FlipVertically; }

            float scaleFix = ((patch_MTexture) Texture).ScaleFix;

            Draw.SpriteBatch.Draw(Texture.Texture.Texture, rect, Texture.ClipRect, color, 0f, (-Texture.DrawOffset) / scaleFix, flip, 0.5f);

            if (!UseSpritebatch) {
                Draw.SpriteBatch.End();
            }
        }

    }
}
