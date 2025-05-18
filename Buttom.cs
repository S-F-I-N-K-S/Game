using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Race2;

public class Button
{
    private Texture2D _texture;
    private Texture2D _hoverTexture;
    public Rectangle Rectangle { get; set; }
    private bool _isHovered;
    private Action _onClick;

    public Button(Texture2D texture, Texture2D hoverTexture, Rectangle rectangle, Action onClick)
    {
        _texture = texture;
        _hoverTexture = hoverTexture;
        Rectangle = rectangle;
        _onClick = onClick;
    }

    public void Update(MouseState mouseState)
    {
        _isHovered = Rectangle.Contains(mouseState.Position);
        if (_isHovered && mouseState.LeftButton == ButtonState.Pressed)
            _onClick?.Invoke();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_isHovered ? _hoverTexture : _texture, Rectangle, Color.White);
    }
}