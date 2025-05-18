using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Race2;


public class PlayerCar
{
    public Texture2D Texture { get; set; }
    public Position Position { get; set; } 
    private float Acceleration;// Ускорение
    private float MaxSpeed;// Максимальная скорость
    private float RotationSpeed;// Скорость поворота
    private float currentSpeed;// Текущая скорость
    private float accumulatedRotation;// Полный угол поворота
    private float targetRotation;// Целевой угол
    private Vector2 origin;
    public Color[] TextureData { get; private set; }

    public PlayerCar(Texture2D texture, Position startPosition, float speed, float rotationSpeed, float acceleration)
    {
        Texture = texture;
        TextureData = GetTextureData(texture);
        Position = startPosition;
        RotationSpeed = rotationSpeed;
        Acceleration = acceleration;
        origin = new Vector2(texture.Width / 2, texture.Height / 2);
        currentSpeed = 0f;
        MaxSpeed = speed;
        accumulatedRotation = -MathHelper.PiOver2; // -90°
        targetRotation = -MathHelper.PiOver2;      // -90°
    }
    private Color[] GetTextureData(Texture2D texture)
    {
        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData(data);
        return data;
    }
    public void Update(GameTime gameTime, int[,] trackMap, int cellSize)
    {
        var keyboardState = Keyboard.GetState();
        Vector2 movementDirection = Vector2.Zero;

        //  Управление газом/тормозом
        if (keyboardState.IsKeyDown(Keys.W))
            currentSpeed += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboardState.IsKeyDown(Keys.S))
            currentSpeed -= Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Ограничение скорости 
        currentSpeed = MathHelper.Clamp(currentSpeed, -MaxSpeed, MaxSpeed);

        // . Поворот в любую сторону 
        float turnInput = 0f;
        if (keyboardState.IsKeyDown(Keys.A)) turnInput = -1;
        if (keyboardState.IsKeyDown(Keys.D)) turnInput = 1;
        
        if (keyboardState.IsKeyDown(Keys.R))
            Position = new Position(Game1.FindSpawnPosition(trackMap, cellSize, Texture).X, Game1.FindSpawnPosition(trackMap, cellSize, Texture).Y);
        if (turnInput != 0)
        {
            // Расчет изменения угла на основе ввода 
            float deltaRotation = turnInput * RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            targetRotation += deltaRotation;
        }

        // Интерполяция угла для плавного поворота 
        accumulatedRotation = MathHelper.Lerp(
            accumulatedRotation,
            targetRotation,
            (float)gameTime.ElapsedGameTime.TotalSeconds * RotationSpeed
        );

        //  Расчет направления движения
        var direction = new Vector2(
            (float)Math.Cos(accumulatedRotation),
            (float)Math.Sin(accumulatedRotation)
        );

        // Применение скорости и проверка коллизий 
        Position tempPosition = new Position(Position.X, Position.Y);
        tempPosition.Move(direction, currentSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

        if (!tempPosition.CheckTrackCollision(trackMap, cellSize, Texture.Width, Texture.Height))
        {
            Position = tempPosition;
        }

        //  Постепенное замедление при отпускании клавиш 
        if (!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.S))
        {
            currentSpeed *= 0.99f; // Трение
            if (Math.Abs(currentSpeed) < 1f) currentSpeed = 0f;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
    {
        Position finalPosition = Position + cameraOffset; 
        spriteBatch.Draw(
            Texture,
            new Vector2(finalPosition.X, finalPosition.Y),
            null,
            Color.White,
            accumulatedRotation,
            origin,
            1f,
            SpriteEffects.None,
            0f
        );
    }
}
